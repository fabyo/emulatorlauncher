﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using EmulatorLauncher.Common.FileFormats;
using EmulatorLauncher.Common;

namespace EmulatorLauncher
{
    partial class Pcsx2Generator : Generator
    {
        public Pcsx2Generator()
        {
            DependsOnDesktopResolution = true;
        }

        private SaveStatesWatcher _saveStatesWatcher;
        private string _path;
        private BezelFiles _bezelFileInfo;
        private ScreenResolution _resolution;
        private bool _isPcsxqt;
        private bool _fullscreen;

        public override void Cleanup()
        {
            if (_saveStatesWatcher != null)
            {
                _saveStatesWatcher.Dispose();
                _saveStatesWatcher = null;
            }

            base.Cleanup();
        }

        public override System.Diagnostics.ProcessStartInfo Generate(string system, string emulator, string core, string rom, string playersControllers, ScreenResolution resolution)
        {
            SimpleLogger.Instance.Info("[Generator] Getting " + emulator + " path and executable name.");

            string path = AppConfig.GetFullPath(emulator);
            _path = path;

            string exe = Path.Combine(_path, "pcsx2-qt.exe");
            if (File.Exists(exe))
                _isPcsxqt = true;

            // v1.6 filename
            else if (!File.Exists(exe))
                exe = Path.Combine(_path, "pcsx2.exe");

            if (!File.Exists(exe))
                return null;

            _fullscreen = !IsEmulationStationWindowed() || SystemConfig.getOptBoolean("forcefullscreen");

            if (!_fullscreen)
                SystemConfig["bezel"] = "none";

            // Manage .m3u files
            if (Path.GetExtension(rom).ToLowerInvariant() == ".m3u")
            {
                string[] lines = File.ReadAllLines(rom);
                string targetRom;
                if (lines.Length > 0)
                {
                    targetRom = Path.Combine(Path.GetDirectoryName(rom), lines[0]);

                    if (File.Exists(targetRom))
                        rom = targetRom;

                    else
                        SimpleLogger.Instance.Error("PCSX2: M3U file target not found: " + targetRom);
                }
                else
                    SimpleLogger.Instance.Error("PCSX2: M3U file is empty: " + rom);
            }

            // Configuration files
            if (_isPcsxqt)
                SetupConfigurationQT(path, rom, system, _fullscreen);

            else
            {
                SetupPaths(_fullscreen);
                SetupVM();
                SetupLilyPad();
                SetupGSDx(resolution);
            }

            File.WriteAllText(Path.Combine(_path, "portable.ini"), "RunWizard=0");

            //Applying bezels
            if (!SystemConfig.isOptSet("ratio") || SystemConfig["ratio"] == "4:3")
                _bezelFileInfo = BezelFiles.GetBezelFiles(system, rom, resolution);

            _resolution = resolution;

            //setting up command line parameters
            var commandArray = new List<string>();

            if (_isPcsxqt)
            {
                commandArray.Add("-batch");
                commandArray.Add("-nogui");

                if (SystemConfig.isOptSet("pcsx2_startbios") && SystemConfig.getOptBoolean("pcsx2_startbios"))
                {
                    commandArray.Add("-bios");
                    string argsBios = string.Join(" ", commandArray);
                    return new ProcessStartInfo()
                    {
                        FileName = exe,
                        WorkingDirectory = _path,
                        Arguments = argsBios,
                    };
                }

                if (SystemConfig.isOptSet("bigpicture") && SystemConfig.getOptBoolean("bigpicture"))
                {
                    if (!SystemConfig.getOptBoolean("disable_fullscreen") || SystemConfig.getOptBoolean("forcefullscreen"))
                        commandArray.Add("-fullscreen");
                    commandArray.Add("-bigpicture");
                }

                if (SystemConfig.isOptSet("fullboot") && SystemConfig.getOptBoolean("fullboot"))
                    commandArray.Add("-slowboot");
            }
            else 
            {
                commandArray.Add("--portable");

                if (_fullscreen)
                    commandArray.Add("--fullscreen");

                commandArray.Add("--nogui");

                if (SystemConfig.isOptSet("fullboot") && SystemConfig.getOptBoolean("fullboot") && SystemConfig["pcsx2_forcebios"] != "ps3_ps2_emu_bios.bin")
                    commandArray.Add("--fullboot");
            }

            commandArray.Add("\"" + rom + "\"");

            if (File.Exists(SystemConfig["state_file"]))
            {
                commandArray.Add("-statefile");
                commandArray.Add("\"" + Path.GetFullPath(SystemConfig["state_file"]) + "\"");
            }

            string args = string.Join(" ", commandArray);

            //start emulator
            return new ProcessStartInfo()
            {
                FileName = exe,
                WorkingDirectory = _path,
                Arguments = args,
            };
        }

        #region 1.6 version
        private void SetupPaths(bool fullscreen)
        {
            if (SystemConfig.getOptBoolean("disableautoconfig"))
                return;

            var biosList = new string[] { 
                            "SCPH30004R.bin", "SCPH30004R.MEC", "scph39001.bin", "scph39001.MEC", 
                            "SCPH-39004_BIOS_V7_EUR_160.BIN", "SCPH-39001_BIOS_V7_USA_160.BIN", "SCPH-70000_BIOS_V12_JAP_200.BIN" };

            string iniFile = Path.Combine(_path, "inis", "PCSX2_ui.ini");

            try
            {
                using (var ini = new IniFile(iniFile))
                {
                    string biosPath = AppConfig.GetFullPath("bios");
                    string cheatsPath = AppConfig.GetFullPath("cheats");
                    if (!string.IsNullOrEmpty(biosPath))
                    {
                        ini.WriteValue("Folders", "UseDefaultBios", "disabled");

                        if (biosList.Any(b => File.Exists(Path.Combine(biosPath, "pcsx2", "bios", b))))
                            ini.WriteValue("Folders", "Bios", Path.Combine(biosPath, "pcsx2", "bios").Replace("\\", "\\\\"));
                        else
                            ini.WriteValue("Folders", "Bios", biosPath.Replace("\\", "\\\\"));

                        ini.WriteValue("Folders", "UseDefaultCheats", "disabled");
                        ini.WriteValue("Folders", "Cheats", Path.Combine(cheatsPath, "pcsx2", "cheats").Replace("\\", "\\\\"));
                        ini.WriteValue("Folders", "UseDefaultCheatsWS", "disabled");
                        ini.WriteValue("Folders", "CheatsWS", Path.Combine(cheatsPath, "pcsx2", "cheats_ws").Replace("\\", "\\\\"));
                    }

                    string savesPath = AppConfig.GetFullPath("saves");
                    if (!string.IsNullOrEmpty(savesPath))
                    {
                        savesPath = Path.Combine(savesPath, "ps2", Path.GetFileName(_path));

                        if (!Directory.Exists(savesPath))
                            try { Directory.CreateDirectory(savesPath); }
                            catch { }

                        ini.WriteValue("Folders", "UseDefaultSavestates", "disabled");
                        ini.WriteValue("Folders", "Savestates", savesPath.Replace("\\", "\\\\") + "\\\\" + "sstates");

                        ini.WriteValue("Folders", "UseDefaultMemoryCards", "disabled");
                        ini.WriteValue("Folders", "MemoryCards", savesPath.Replace("\\", "\\\\") + "\\\\" + "memcards");
                    }

                    string screenShotsPath = AppConfig.GetFullPath("screenshots");
                    if (!string.IsNullOrEmpty(screenShotsPath))
                    {

                        ini.WriteValue("Folders", "UseDefaultSnapshots", "disabled");
                        ini.WriteValue("Folders", "Snapshots", screenShotsPath.Replace("\\", "\\\\") + "\\\\" + "pcsx2");
                    }

                    if (SystemConfig.isOptSet("ratio") && !string.IsNullOrEmpty(SystemConfig["ratio"]))
                        ini.WriteValue("GSWindow", "AspectRatio", SystemConfig["ratio"]);
                    else
                        ini.WriteValue("GSWindow", "AspectRatio", "4:3");

                    if (SystemConfig.isOptSet("fmv_ratio") && !string.IsNullOrEmpty(SystemConfig["fmv_ratio"]))
                        ini.WriteValue("GSWindow", "FMVAspectRatioSwitch", SystemConfig["fmv_ratio"]);
                    else if (Features.IsSupported("fmv_ratio"))
                        ini.WriteValue("GSWindow", "FMVAspectRatioSwitch", "Off");

                    ini.WriteValue("ProgramLog", "Visible", "disabled");
                    ini.WriteValue("GSWindow", "IsFullscreen", fullscreen ? "enabled" : "disabled");

                    if (Features.IsSupported("negdivhack") && SystemConfig.isOptSet("negdivhack") && SystemConfig.getOptBoolean("negdivhack"))
                        ini.WriteValue(null, "EnablePresets", "disabled");

                    ini.WriteValue("Filenames", "PAD", "LilyPad.dll");

                    if (Features.IsSupported("gs_plugin"))
                    {
                        if (SystemConfig.isOptSet("gs_plugin") && !string.IsNullOrEmpty(SystemConfig["gs_plugin"]))
                            ini.WriteValue("Filenames", "GS", SystemConfig["gs_plugin"]);
                        else
                            ini.WriteValue("Filenames", "GS", "GSdx32-SSE2.dll");
                    }

                    foreach (var key in new string[] { "SPU2", "CDVD", "USB", "FW", "DEV9", "BIOS"})
                    {
                        string value = ini.GetValue("Filenames", key);
                        if (value == null || value == "Please Configure")
                        {
                            switch (key)
                            {
                                case "SPU2":
                                    value = "Spu2-X.dll";
                                    break;
                                case "CDVD":
                                    value = "cdvdGigaherz.dll";
                                    break;
                                case "USB":
                                    value = "USBnull.dll";
                                    break;
                                case "FW":
                                    value = "FWnull.dll";
                                    break;
                                case "DEV9":
                                    value = "DEV9null.dll";
                                    break;
                                case "BIOS":

                                    var biosFile = biosList.FirstOrDefault(b => File.Exists(Path.Combine(biosPath, "pcsx2", "bios", b)));
                                    if (!string.IsNullOrEmpty(biosFile))
                                        value = biosFile;
                                    else
                                        value = "SCPH30004R.bin";

                                    break;
                            }

                            ini.WriteValue("Filenames", key, value);
                        }
                    }
                }
            }
            catch { }
        }

        private void SetupLilyPad()
        {
            if (SystemConfig.getOptBoolean("disableautoconfig"))
                return;

            string iniFile = Path.Combine(_path, "inis", "LilyPad.ini");

            try
            {
                using (var ini = new IniFile(iniFile))
                    ini.WriteValue("General Settings", "Keyboard Mode", "1");
            }
            catch { }
        }

        //Setup PCSX2_vm.ini file
        private void SetupVM()
        {
            if (SystemConfig.getOptBoolean("disableautoconfig"))
                return;

            string iniFile = Path.Combine(_path, "inis", "PCSX2_vm.ini");

            try
            {
                using (var ini = new IniFile(iniFile))
                {
                    if (!string.IsNullOrEmpty(SystemConfig["pcsx2_vsync"]))
                        ini.WriteValue("EmuCore/GS", "VsyncEnable", SystemConfig["pcsx2_vsync"] == "false" ? "0" : "1");
                    else
                        ini.WriteValue("EmuCore/GS", "VsyncEnable", "1");

                    if (Features.IsSupported("negdivhack"))
                    {
                        string negdivhack = SystemConfig.isOptSet("negdivhack") && SystemConfig.getOptBoolean("negdivhack") ? "enabled" : "disabled";

                        ini.WriteValue("EmuCore/Speedhacks", "vuThread", negdivhack);

                        ini.WriteValue("EmuCore/CPU/Recompiler", "vuExtraOverflow", negdivhack);
                        ini.WriteValue("EmuCore/CPU/Recompiler", "vuSignOverflow", negdivhack);
                        ini.WriteValue("EmuCore/CPU/Recompiler", "fpuExtraOverflow", negdivhack);
                        ini.WriteValue("EmuCore/CPU/Recompiler", "fpuFullMode", negdivhack);

                        ini.WriteValue("EmuCore/Gamefixes", "VuClipFlagHack", negdivhack);
                        ini.WriteValue("EmuCore/Gamefixes", "FpuNegDivHack", negdivhack);
                    }
                }
            }
            catch { }
        }

        //Setup GSdx.ini file
        private void SetupGSDx(ScreenResolution resolution)
        {
            if (SystemConfig.getOptBoolean("disableautoconfig"))
                return;

            string iniFile = Path.Combine(_path, "inis", "GSdx.ini");

            try
            {
                using (var ini = new IniFile(iniFile))
                {                
                    //Activate user hacks - default activation if a hack is enabled later  
                    if ((SystemConfig.isOptSet("UserHacks") && !string.IsNullOrEmpty(SystemConfig["UserHacks"])))
                        ini.WriteValue("Settings", "UserHacks", SystemConfig["UserHacks"]);
                    else
                        ini.WriteValue("Settings", "UserHacks", "0");

                    //Resolution upscale
                    if (!string.IsNullOrEmpty(SystemConfig["internalresolution"]))
                        ini.WriteValue("Settings", "upscale_multiplier", SystemConfig["internalresolution"]);
                    else
                        ini.WriteValue("Settings", "upscale_multiplier", "1");

                    if (string.IsNullOrEmpty(SystemConfig["internalresolution"]) || SystemConfig["internalresolution"] == "0")
                    {
                        if (resolution != null)
                        {
                            ini.WriteValue("Settings", "resx", resolution.Width.ToString());
                            ini.WriteValue("Settings", "resy", (resolution.Height * 2).ToString());
                        }
                        else
                        {
                            ini.WriteValue("Settings", "resx", Screen.PrimaryScreen.Bounds.Width.ToString());
                            ini.WriteValue("Settings", "resy", (Screen.PrimaryScreen.Bounds.Height * 2).ToString());
                        }
                    }

                    //Enable External Shader
                    ini.WriteValue("Settings", "shaderfx", "1");

                    //TVShader
                    if (SystemConfig.isOptSet("TVShader") && !string.IsNullOrEmpty(SystemConfig["TVShader"]))
                        ini.WriteValue("Settings", "TVShader", SystemConfig["TVShader"]);
                    else if (Features.IsSupported("TVShader"))
                        ini.WriteValue("Settings", "TVShader", "0");

                    //Wild Arms offset
                    if (SystemConfig.isOptSet("Offset") && !string.IsNullOrEmpty(SystemConfig["Offset"]))
                    {
                        ini.WriteValue("Settings", "UserHacks_WildHack", SystemConfig["Offset"]);
                        ini.WriteValue("Settings", "UserHacks", "1");
                    }
                    else if (Features.IsSupported("Offset"))
                        ini.WriteValue("Settings", "UserHacks_WildHack", "0");

                    //Half Pixel Offset
                    if (SystemConfig.isOptSet("UserHacks_HalfPixelOffset") && !string.IsNullOrEmpty(SystemConfig["UserHacks_HalfPixelOffset"]))
                    {
                        ini.WriteValue("Settings", "UserHacks_HalfPixelOffset", SystemConfig["UserHacks_HalfPixelOffset"]);
                        ini.WriteValue("Settings", "UserHacks", "1");
                    }
                    else if (Features.IsSupported("Offset"))
                        ini.WriteValue("Settings", "UserHacks_HalfPixelOffset", "0");

                    //Half-screen fix
                    if (SystemConfig.isOptSet("UserHacks_Half_Bottom_Override") && !string.IsNullOrEmpty(SystemConfig["UserHacks_Half_Bottom_Override"]))
                    {
                        ini.WriteValue("Settings", "UserHacks_Half_Bottom_Override", SystemConfig["UserHacks_Half_Bottom_Override"]);
                        ini.WriteValue("Settings", "UserHacks", "1");
                    }
                    else if (Features.IsSupported("Offset"))
                        ini.WriteValue("Settings", "UserHacks_Half_Bottom_Override", "-1");

                    //Round sprite
                    if (SystemConfig.isOptSet("UserHacks_round_sprite_offset") && !string.IsNullOrEmpty(SystemConfig["UserHacks_round_sprite_offset"]))
                    {
                        ini.WriteValue("Settings", "UserHacks_round_sprite_offset", SystemConfig["UserHacks_round_sprite_offset"]);
                        ini.WriteValue("Settings", "UserHacks", "1");
                    }
                    else if (Features.IsSupported("Offset"))
                        ini.WriteValue("Settings", "UserHacks_round_sprite_offset", "0");

                    //Shader - Texture filtering of display
                    if (SystemConfig.isOptSet("bilinear_filtering") && !string.IsNullOrEmpty(SystemConfig["bilinear_filtering"]))
                        ini.WriteValue("Settings", "linear_present", SystemConfig["bilinear_filtering"]);
                    else if (Features.IsSupported("bilinear_filtering"))
                        ini.WriteValue("Settings", "linear_present", "0");

                    //Shader - FXAA Shader
                    if (SystemConfig.isOptSet("fxaa") && !string.IsNullOrEmpty(SystemConfig["fxaa"]))
                        ini.WriteValue("Settings", "fxaa", SystemConfig["fxaa"]);
                    else if (Features.IsSupported("fxaa"))
                        ini.WriteValue("Settings", "fxaa", "0");

                    //Renderer
                    if (SystemConfig.isOptSet("renderer") && !string.IsNullOrEmpty(SystemConfig["renderer"]))
                        ini.WriteValue("Settings", "Renderer", SystemConfig["renderer"]);
                    else if (Features.IsSupported("renderer"))
                        ini.WriteValue("Settings", "Renderer", "12");

                    //Deinterlacing : automatic or NONE options
                    if (SystemConfig.isOptSet("interlace") && !string.IsNullOrEmpty(SystemConfig["interlace"]))
                        ini.WriteValue("Settings", "interlace", SystemConfig["interlace"]);
                    else if (Features.IsSupported("interlace"))
                        ini.WriteValue("Settings", "interlace", "7");

                    //Anisotropic filtering
                    if (SystemConfig.isOptSet("anisotropic_filtering") && !string.IsNullOrEmpty(SystemConfig["anisotropic_filtering"]))
                        ini.WriteValue("Settings", "MaxAnisotropy", SystemConfig["anisotropic_filtering"]);
                    else if (Features.IsSupported("anisotropic_filtering"))
                        ini.WriteValue("Settings", "MaxAnisotropy", "0");

                    //Align sprite
                    if (SystemConfig.isOptSet("align_sprite") && !string.IsNullOrEmpty(SystemConfig["align_sprite"]))
                    {
                        ini.WriteValue("Settings", "UserHacks_align_sprite_X", SystemConfig["align_sprite"]);
                        ini.WriteValue("Settings", "UserHacks", "1");
                    }
                    else if (Features.IsSupported("align_sprite"))
                        ini.WriteValue("Settings", "UserHacks_align_sprite_X", "0");

                    //Merge sprite
                    if (SystemConfig.isOptSet("UserHacks_merge_pp_sprite") && !string.IsNullOrEmpty(SystemConfig["UserHacks_merge_pp_sprite"]))
                    {
                        ini.WriteValue("Settings", "UserHacks_merge_pp_sprite", SystemConfig["UserHacks_merge_pp_sprite"]);
                        ini.WriteValue("Settings", "UserHacks", "1");
                    }
                    else if (Features.IsSupported("align_sprite"))
                        ini.WriteValue("Settings", "UserHacks_merge_pp_sprite", "0");

                    //Disable safe features
                    if (SystemConfig.isOptSet("UserHacks_Disable_Safe_Features") && !string.IsNullOrEmpty(SystemConfig["UserHacks_Disable_Safe_Features"]))
                    {
                        ini.WriteValue("Settings", "UserHacks_Disable_Safe_Features", SystemConfig["UserHacks_Disable_Safe_Features"]);
                        ini.WriteValue("Settings", "UserHacks", "1");
                    }
                    else if (Features.IsSupported("UserHacks_Disable_Safe_Features"))
                        ini.WriteValue("Settings", "UserHacks_Disable_Safe_Features", "0");

                    //Texture Offsets
                    if (SystemConfig.isOptSet("TextureOffsets") && (SystemConfig["TextureOffsets"] == "1"))
                    {
                        ini.WriteValue("Settings", "UserHacks_TCOffsetX", "500");
                        ini.WriteValue("Settings", "UserHacks_TCOffsetY", "500");
                        ini.WriteValue("Settings", "UserHacks", "1");
                    }
                    else if (SystemConfig.isOptSet("TextureOffsets") && (SystemConfig["TextureOffsets"] == "2"))
                    {
                        ini.WriteValue("Settings", "UserHacks_TCOffsetX", "0");
                        ini.WriteValue("Settings", "UserHacks_TCOffsetY", "1000");
                        ini.WriteValue("Settings", "UserHacks", "1");
                    }
                    else if (Features.IsSupported("TextureOffsets"))
                    {
                        ini.WriteValue("Settings", "UserHacks_TCOffsetX", "0");
                        ini.WriteValue("Settings", "UserHacks_TCOffsetY", "0");
                    }

                    //Skipdraw Range
                    if (SystemConfig.isOptSet("skipdraw") && (SystemConfig["skipdraw"] == "1"))
                    {
                        ini.WriteValue("Settings", "UserHacks_SkipDraw_Offset", "1");
                        ini.WriteValue("Settings", "UserHacks_SkipDraw", "1");
                        ini.WriteValue("Settings", "UserHacks", "1");
                    }
                    else if (SystemConfig.isOptSet("skipdraw") && (SystemConfig["skipdraw"] == "2"))
                    {
                        ini.WriteValue("Settings", "UserHacks_SkipDraw_Offset", "1");
                        ini.WriteValue("Settings", "UserHacks_SkipDraw", "2");
                        ini.WriteValue("Settings", "UserHacks", "1");
                    }
                    else if (SystemConfig.isOptSet("skipdraw") && (SystemConfig["skipdraw"] == "3"))
                    {
                        ini.WriteValue("Settings", "UserHacks_SkipDraw_Offset", "1");
                        ini.WriteValue("Settings", "UserHacks_SkipDraw", "3");
                        ini.WriteValue("Settings", "UserHacks", "1");
                    }
                    else if (SystemConfig.isOptSet("skipdraw") && (SystemConfig["skipdraw"] == "4"))
                    {
                        ini.WriteValue("Settings", "UserHacks_SkipDraw_Offset", "1");
                        ini.WriteValue("Settings", "UserHacks_SkipDraw", "4");
                        ini.WriteValue("Settings", "UserHacks", "1");
                    }
                    else if (SystemConfig.isOptSet("skipdraw") && (SystemConfig["skipdraw"] == "5"))
                    {
                        ini.WriteValue("Settings", "UserHacks_SkipDraw_Offset", "1");
                        ini.WriteValue("Settings", "UserHacks_SkipDraw", "5");
                        ini.WriteValue("Settings", "UserHacks", "1");
                    }
                    else if (Features.IsSupported("skipdraw"))
                    {
                        ini.WriteValue("Settings", "UserHacks_SkipDraw_Offset", "0");
                        ini.WriteValue("Settings", "UserHacks_SkipDraw", "0");
                    }

                    //CRC Hack Level
                    if (SystemConfig.isOptSet("crc_hack_level") && !string.IsNullOrEmpty(SystemConfig["crc_hack_level"]))
                        ini.WriteValue("Settings", "crc_hack_level", SystemConfig["crc_hack_level"]);
                    else if (Features.IsSupported("crc_hack_level"))
                        ini.WriteValue("Settings", "crc_hack_level", "-1");

                    //Custom textures
                    if (SystemConfig.isOptSet("hires_textures") && SystemConfig.getOptBoolean("hires_textures"))
                    {
                        ini.WriteValue("Settings", "LoadTextureReplacements", "1");
                        ini.WriteValue("Settings", "PrecacheTextureReplacements", "1");
                    }
                    else
                    {
                        ini.WriteValue("Settings", "LoadTextureReplacements", "0");
                        ini.WriteValue("Settings", "PrecacheTextureReplacements", "0");
                    }

                    if (SystemConfig.isOptSet("DrawFramerate") && SystemConfig.getOptBoolean("DrawFramerate"))
                    {
                        ini.WriteValue("Settings", "osd_monitor_enabled", "1");
                        ini.WriteValue("Settings", "osd_indicator_enabled", "1");
                    }
                    else
                    {
                        ini.WriteValue("Settings", "osd_monitor_enabled", "0");
                        ini.WriteValue("Settings", "osd_indicator_enabled", "0");
                    }

                    if (SystemConfig.isOptSet("Notifications") && SystemConfig.getOptBoolean("Notifications"))
                        ini.WriteValue("Settings", "osd_log_enabled", "1");
                    else
                        ini.WriteValue("Settings", "osd_log_enabled", "0");
                }
            }
            catch { }
        }
        #endregion

        #region QT version
        /// <summary>
        /// Setup Configuration of PCSX2.ini file for PCSX2 QT version
        /// </summary>
        /// <param name="path"></param>
        private void SetupConfigurationQT(string path, string rom, string system, bool fullscreen)
        {
            if (SystemConfig.getOptBoolean("disableautoconfig"))
                return;

            var biosList = new string[] { "ps2-0230a-20080220.bin", "ps2-0230e-20080220.bin", "ps2-0250e-20100415.bin", "ps2-0230j-20080220.bin", "ps3_ps2_emu_bios.bin", 
                "SCPH30004R.bin", "scph39001.bin", "SCPH-39004_BIOS_V7_EUR_160.BIN", "SCPH-39001_BIOS_V7_USA_160.BIN", "SCPH-70000_BIOS_V12_JAP_200.BIN" };

            string conf = Path.Combine(_path, "inis", "PCSX2.ini");

            using (var ini = IniFile.FromFile(conf, IniOptions.UseSpaces | IniOptions.AllowDuplicateValues))
            {
                ini.WriteValue("UI", "HideMouseCursor", "true");
                CreateControllerConfiguration(ini);
                SetupGunQT(ini, path);

                if (!SystemConfig.isOptSet("pcsx2_emulatedwheel"))
                    SetupWheelQT(ini);

                // Disable auto-update
                ini.WriteValue("AutoUpdater", "CheckAtStartup", "false");

                // Enable cheevos is needed
                if (Features.IsSupported("cheevos") && SystemConfig.getOptBoolean("retroachievements"))
                {
                    ini.WriteValue("Achievements", "Enabled", "true");
                    ini.WriteValue("Achievements", "ChallengeMode", SystemConfig.getOptBoolean("retroachievements.hardcore") ? "true" : "false");
                    ini.WriteValue("Achievements", "EncoreMode", SystemConfig.getOptBoolean("retroachievements.encore") ? "true" : "false");
                    ini.WriteValue("Achievements", "UnofficialTestMode", "false");
                    ini.WriteValue("Achievements", "Notifications", "true");
                    ini.WriteValue("Achievements", "LeaderboardNotifications", SystemConfig.getOptBoolean("retroachievements.leaderboards") ? "true" : "false");
                    ini.WriteValue("Achievements", "SoundEffects", "true");
                    ini.WriteValue("Achievements", "Overlays", SystemConfig.getOptBoolean("retroachievements.challenge_indicators") ? "true" : "false");
                    
                    // Inject credentials
                    if (SystemConfig.isOptSet("retroachievements.username") && SystemConfig.isOptSet("retroachievements.token"))
                    {
                        ini.WriteValue("Achievements", "Username", SystemConfig["retroachievements.username"]);
                        ini.WriteValue("Achievements", "Token", SystemConfig["retroachievements.token"]);
                        
                        if (string.IsNullOrEmpty(ini.GetValue("Achievements", "Token")))
                            ini.WriteValue("Achievements", "LoginTimestamp", Convert.ToString((int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds));
                    }
                }
                else
                {
                    ini.WriteValue("Achievements", "Enabled", "false");
                    ini.WriteValue("Achievements", "ChallengeMode", "false");
                }

                // Define paths
                
                // Add rom path to RecursivePaths
                AddPathToRecursivePaths(Path.GetFullPath(Path.GetDirectoryName(rom)), ini);

                // BIOS path
                string biosPath = Path.Combine(AppConfig.GetFullPath("bios"), "pcsx2", "bios");
                if (!Directory.Exists(biosPath))
                    try { Directory.CreateDirectory(biosPath); }
                    catch { }

                string biosFile = "ps2-0230a-20080220.bin";                     // Default bios

                if (Directory.GetFiles(biosPath).Length == 0)                 // if no bios, do not set
                    biosPath = AppConfig.GetFullPath("bios");
                
                if (!biosList.Any(b => File.Exists(Path.Combine(biosPath, b))))
                    throw new ApplicationException("No BIOS found in bios/pcsx2/bios folder.");

                if (!File.Exists(Path.Combine(biosPath, biosFile)))             // if default does not exist, select first one that exists
                    biosFile = biosList.FirstOrDefault(b => File.Exists(Path.Combine(biosPath, b)));

                if (SystemConfig.isOptSet("pcsx2_forcebios") && !string.IsNullOrEmpty(SystemConfig["pcsx2_forcebios"]))                         // Precise bios to use through feature
                {
                    string checkBiosFile = Path.Combine(biosPath, SystemConfig["pcsx2_forcebios"]);
                    if (File.Exists(checkBiosFile))
                        biosFile = SystemConfig["pcsx2_forcebios"];
                }

                ini.WriteValue("Folders", "Bios", biosPath);

                if (!string.IsNullOrEmpty(biosFile))
                    ini.WriteValue("Filenames", "BIOS", biosFile);

                // Cheats Path
                var cheatsRootPath = AppConfig.GetFullPath("cheats");
                cheatsRootPath = string.IsNullOrEmpty(cheatsRootPath) ? path : Path.Combine(cheatsRootPath, "pcsx2");
                
                SetIniPath(ini, "Folders", "Cheats", Path.Combine(cheatsRootPath, "cheats"));
                SetIniPath(ini, "Folders", "CheatsWS", Path.Combine(cheatsRootPath, "cheats_ws"));
                SetIniPath(ini, "Folders", "CheatsNI", Path.Combine(cheatsRootPath, "cheats_ni"));
                
                // Snapshots path
                string screenShotsPath = Path.Combine(AppConfig.GetFullPath("screenshots"), "pcsx2");
                SetIniPath(ini, "Folders", "Snapshots", screenShotsPath);

                // Memory cards path
                string memcardsPath = Path.Combine(AppConfig.GetFullPath("saves"), "ps2", "pcsx2", "memcards");                
                SetIniPath(ini, "Folders", "MemoryCards", memcardsPath);

                bool newSaveStates = Program.HasEsSaveStates && Program.EsSaveStates.IsEmulatorSupported("pcsx2");

                // SaveStates path
                string savesPath = newSaveStates ?
                    Program.EsSaveStates.GetSavePath(system, "pcsx2", "pcsx2") :
                    Path.Combine(AppConfig.GetFullPath("saves"), system, "pcsx2", "sstates");

                if (!string.IsNullOrEmpty(savesPath))
                {
                    if (newSaveStates)
                    {
                        // Keep the original folder, we'll listen to it, and inject in our custom folder
                        ini.WriteValue("Folders", "Savestates ", "sstates");

                        _saveStatesWatcher = new Pcsx2SaveStatesMonitor(rom, Path.Combine(path, "sstates"), savesPath);
                        _saveStatesWatcher.PrepareEmulatorRepository();
                    }
                    else
                    {
                        FileTools.TryCreateDirectory(savesPath);
                        ini.WriteValue("Folders", "Savestates ", savesPath);
                    }
                }

                // autosave
                if (_saveStatesWatcher != null)
                    ini.WriteValue("EmuCore", "SaveStateOnShutdown", _saveStatesWatcher.IsLaunchingAutoSave() || SystemConfig.getOptBoolean("autosave") ? "true" : "false");
                else
                    ini.WriteValue("EmuCore", "SaveStateOnShutdown", "false");

                //Custom textures path
                string texturePath = Path.Combine(AppConfig.GetFullPath("bios"), "pcsx2", "textures");
                SetIniPath(ini, "Folders", "Textures", texturePath);

                // UI section
                ini.WriteValue("UI", "ConfirmShutdown", "false");

                // fullscreen management
                
                if (SystemConfig.getOptBoolean("forcefullscreen"))
                    ini.WriteValue("UI", "StartFullscreen", "true");
                else if (SystemConfig.getOptBoolean("disable_fullscreen"))
                    ini.WriteValue("UI", "StartFullscreen", "false");
                else if (fullscreen)
                    ini.WriteValue("UI", "StartFullscreen", "true");
                else
                    ini.WriteValue("UI", "StartFullscreen", "false");

                ini.Remove("UI", "MainWindowGeometry");
                ini.Remove("UI", "MainWindowState");
                ini.Remove("UI", "DisplayWindowGeometry");

                // Emucore section
                ini.WriteValue("EmuCore", "SavestateZstdCompression", "true");
                ini.WriteValue("EmuCore", "EnableFastBoot", SystemConfig.getOptBoolean("fullboot") ? "false" : "true");
                ini.WriteValue("EmuCore", "EnableFastBootFastForward", SystemConfig.getOptBoolean("pcsx2_fastboot") ? "true" : "false");

                //Enable cheats automatically on load if Retroachievements-hardcore is not set only
                if (SystemConfig.isOptSet("enable_cheats") && !SystemConfig.getOptBoolean("retroachievements.hardcore") && !string.IsNullOrEmpty(SystemConfig["enable_cheats"]))
                    ini.WriteValue("EmuCore", "EnableCheats", SystemConfig["enable_cheats"]);
                else if (Features.IsSupported("enable_cheats"))
                    ini.WriteValue("EmuCore", "EnableCheats", "false");

                BindBoolIniFeature(ini, "EmuCore", "EnableDiscordPresence", "discord", "true", "false");
                BindBoolIniFeature(ini, "EmuCore", "EnableWideScreenPatches", "widescreen_patch", "true", "false");
                BindBoolIniFeature(ini, "EmuCore", "EnableNoInterlacingPatches", "interlacing_patch", "true", "false");

                // EmuCore/GS
                BindBoolIniFeature(ini, "EmuCore/GS", "IntegerScaling", "integerscale", "true", "false");
                BindIniFeature(ini, "EmuCore/GS", "AspectRatio", "ratio", "Auto 4:3/3:2");
                BindIniFeature(ini, "EmuCore/GS", "FMVAspectRatioSwitch", "fmv_ratio", "Off");
                BindIniFeature(ini, "EmuCore/GS", "Renderer", "renderer", "-1");
                BindIniFeature(ini, "EmuCore/GS", "deinterlace_mode", "interlace", "0");
                BindIniFeature(ini, "EmuCore/GS", "VsyncEnable", "pcsx2_vsync", "true");
                BindBoolIniFeature(ini, "EmuCore/GS", "pcrtc_offsets", "pcrtc_offsets", "true", "false");
                BindIniFeature(ini, "EmuCore/GS", "pcrtc_antiblur", "pcrtc_antiblur", "true");
                BindIniFeature(ini, "EmuCore/GS", "upscale_multiplier", "internalresolution", "1");
                BindIniFeature(ini, "EmuCore/GS", "hw_mipmap", "mipmap", "true");
                BindIniFeature(ini, "EmuCore/GS", "filter", "texture_filtering", "2");
                BindIniFeature(ini, "EmuCore/GS", "TriFilter", "trilinear_filtering", "-1");
                BindIniFeature(ini, "EmuCore/GS", "MaxAnisotropy", "anisotropic_filtering", "0");
                BindIniFeature(ini, "EmuCore/GS", "dithering_ps2", "dithering", "2");
                BindIniFeature(ini, "EmuCore/GS", "accurate_blending_unit", "blending_accuracy", "3");
                BindBoolIniFeature(ini, "EmuCore/GS", "fxaa", "fxaa", "true", "false");
                BindIniFeature(ini, "EmuCore/GS", "TVShader", "TVShader", "0");

                if (SystemConfig.isOptSet("bilinear_filtering") && SystemConfig["bilinear_filtering"] == "0")
                    ini.WriteValue("EmuCore/GS", "linear_present_mode", "0");
                else if (SystemConfig.isOptSet("bilinear_filtering") && SystemConfig["bilinear_filtering"] == "2")
                    ini.WriteValue("EmuCore/GS", "linear_present_mode", "2");
                else if (Features.IsSupported("bilinear_filtering"))
                    ini.WriteValue("EmuCore/GS", "linear_present_mode", "1");

                BindIniFeature(ini, "EmuCore/GS", "texture_preloading", "texture_preloading", "2");

                // User hacks
                BindBoolIniFeature(ini, "EmuCore/GS", "UserHacks", "UserHacks", "true", "false");

                // User hacks Skipdraw range
                if (SystemConfig.isOptSet("skipdraw") && !string.IsNullOrEmpty(SystemConfig["skipdraw"]))
                {
                    Action<string, string> skipdrawWrite = (s, e) =>
                    {
                        ini.WriteValue("EmuCore/GS", "UserHacks_SkipDraw_Start", s);
                        ini.WriteValue("EmuCore/GS", "UserHacks_SkipDraw_End", e);
                        ini.WriteValue("EmuCore/GS", "UserHacks", "true");
                    };

                    switch (SystemConfig["skipdraw"])
                    {
                        case "1":
                            skipdrawWrite("1", "1");
                            break;
                        case "2":
                            skipdrawWrite("1", "2");
                            break;
                        case "3":
                            skipdrawWrite("1", "3");
                            break;
                        case "4":
                            skipdrawWrite("1", "4");
                            break;
                        case "5":
                            skipdrawWrite("1", "5");
                            break;
                        case "bully":
                            skipdrawWrite("1", "6");
                            break;
                    }
                }
                else if (Features.IsSupported("skipdraw"))
                {
                    ini.WriteValue("EmuCore/GS", "UserHacks_SkipDraw_Start", "0");
                    ini.WriteValue("EmuCore/GS", "UserHacks_SkipDraw_End", "0");
                }

                // User hack safe features
                if (SystemConfig.isOptSet("UserHacks_Disable_Safe_Features") && !string.IsNullOrEmpty(SystemConfig["UserHacks_Disable_Safe_Features"]))
                {
                    ini.WriteValue("EmuCore/GS", "UserHacks_Disable_Safe_Features", SystemConfig["UserHacks_Disable_Safe_Features"]);
                    ini.WriteValue("EmuCore/GS", "UserHacks", "true");
                }
                else if (Features.IsSupported("UserHacks_Disable_Safe_Features"))
                    ini.WriteValue("EmuCore/GS", "UserHacks_Disable_Safe_Features", "false");

                // User hacks Half Pixel Offset
                if (SystemConfig.isOptSet("UserHacks_HalfPixelOffset") && !string.IsNullOrEmpty(SystemConfig["UserHacks_HalfPixelOffset"]))
                {
                    ini.WriteValue("EmuCore/GS", "UserHacks_HalfPixelOffset", SystemConfig["UserHacks_HalfPixelOffset"]);
                    ini.WriteValue("EmuCore/GS", "UserHacks", "true");
                }
                else if (Features.IsSupported("Offset"))
                    ini.WriteValue("EmuCore/GS", "UserHacks_HalfPixelOffset", "0");

                // User hacks Round sprite
                if (SystemConfig.isOptSet("UserHacks_round_sprite_offset") && !string.IsNullOrEmpty(SystemConfig["UserHacks_round_sprite_offset"]))
                {
                    ini.WriteValue("EmuCore/GS", "UserHacks_round_sprite_offset", SystemConfig["UserHacks_round_sprite_offset"]);
                    ini.WriteValue("EmuCore/GS", "UserHacks", "true");
                }
                else if (Features.IsSupported("Offset"))
                    ini.WriteValue("EmuCore/GS", "UserHacks_round_sprite_offset", "0");

                // User hacks Align sprite
                if (SystemConfig.isOptSet("align_sprite") && !string.IsNullOrEmpty(SystemConfig["align_sprite"]))
                {
                    ini.WriteValue("EmuCore/GS", "UserHacks_align_sprite_X", SystemConfig["align_sprite"]);
                    ini.WriteValue("EmuCore/GS", "UserHacks", "true");
                }
                else if (Features.IsSupported("align_sprite"))
                    ini.WriteValue("EmuCore/GS", "UserHacks_align_sprite_X", "false");

                // User hacks Merge sprite
                if (SystemConfig.isOptSet("UserHacks_merge_pp_sprite") && !string.IsNullOrEmpty(SystemConfig["UserHacks_merge_pp_sprite"]))
                {
                    ini.WriteValue("EmuCore/GS", "UserHacks_merge_pp_sprite", SystemConfig["UserHacks_merge_pp_sprite"]);
                    ini.WriteValue("EmuCore/GS", "UserHacks", "true");
                }
                else if (Features.IsSupported("align_sprite"))
                    ini.WriteValue("EmuCore/GS", "UserHacks_merge_pp_sprite", "false");

                // User hacks Wild Arms offset
                if (SystemConfig.isOptSet("UserHacks_WildHack") && !string.IsNullOrEmpty(SystemConfig["UserHacks_WildHack"]))
                {
                    ini.WriteValue("EmuCore/GS", "UserHacks_forceEvenSpritePosition", SystemConfig["UserHacks_WildHack"]);
                    ini.WriteValue("EmuCore/GS", "UserHacks", "true");
                }
                else if (Features.IsSupported("UserHacks_WildHack"))
                    ini.WriteValue("EmuCore/GS", "UserHacks_forceEvenSpritePosition", "false");

                //texture offset
                if (SystemConfig.isOptSet("TextureOffsets") && !string.IsNullOrEmpty(SystemConfig["TextureOffsets"]))
                {
                    Action<string, string> textureOffsetsWrite = (x, y) =>
                    {
                        ini.WriteValue("EmuCore/GS", "UserHacks_TCOffsetX", x);
                        ini.WriteValue("EmuCore/GS", "UserHacks_TCOffsetY", y);
                        ini.WriteValue("EmuCore/GS", "UserHacks", "true");
                    };

                    switch (SystemConfig["TextureOffsets"])
                    {
                        case "1":
                            textureOffsetsWrite("500", "500");
                            break;
                        case "2":
                            textureOffsetsWrite("0", "1000");
                            break;
                    }
                }
                else if (Features.IsSupported("TextureOffsets"))
                {
                    ini.WriteValue("EmuCore/GS", "UserHacks_TCOffsetX", "0");
                    ini.WriteValue("EmuCore/GS", "UserHacks_TCOffsetY", "0");
                }

                // Custom textures
                if (SystemConfig.isOptSet("hires_textures") && SystemConfig["hires_textures"] == "1")
                {
                    ini.WriteValue("EmuCore/GS", "LoadTextureReplacements", "true");
                    ini.WriteValue("EmuCore/GS", "PrecacheTextureReplacements", "true");
                }
                else if (SystemConfig.isOptSet("hires_textures") && SystemConfig["hires_textures"] == "2")
                {
                    ini.WriteValue("EmuCore/GS", "LoadTextureReplacements", "true");
                    ini.WriteValue("EmuCore/GS", "PrecacheTextureReplacements", "false");
                }
                else
                {
                    ini.WriteValue("EmuCore/GS", "LoadTextureReplacements", "false");
                    ini.WriteValue("EmuCore/GS", "PrecacheTextureReplacements", "false");
                }

                // OSD information
                BindIniFeature(ini, "EmuCore/GS", "OsdShowMessages", "Notifications", "true");

                if (SystemConfig.isOptSet("DrawFramerate") && SystemConfig.getOptBoolean("DrawFramerate"))
                {
                    ini.WriteValue("EmuCore/GS", "OsdShowCPU", "true");
                    ini.WriteValue("EmuCore/GS", "OsdShowFPS", "true");
                    ini.WriteValue("EmuCore/GS", "OsdShowGPU", "true");
                    ini.WriteValue("EmuCore/GS", "OsdShowResolution", "true");
                    ini.WriteValue("EmuCore/GS", "OsdShowSpeed", "true");
                }
                else
                {
                    ini.WriteValue("EmuCore/GS", "OsdShowCPU", "false");
                    ini.WriteValue("EmuCore/GS", "OsdShowFPS", "false");
                    ini.WriteValue("EmuCore/GS", "OsdShowGPU", "false");
                    ini.WriteValue("EmuCore/GS", "OsdShowResolution", "false");
                    ini.WriteValue("EmuCore/GS", "OsdShowSpeed", "false");
                }

                // AUDIO section
                BindIniFeature(ini, "SPU2/Output", "Backend", "pcsx2_apu", "Cubeb");

                // Game fixes
                if (SystemConfig.getOptBoolean("FpuNegDivHack"))
                    ini.WriteValue("EmuCore/Gamefixes", "FpuNegDivHack", "true");
                else if (Features.IsSupported("FpuNegDivHack"))
                    ini.Remove("EmuCore/Gamefixes", "FpuNegDivHack");

                if (SystemConfig.getOptBoolean("FpuMulHack"))
                    ini.WriteValue("EmuCore/Gamefixes", "FpuMulHack", "true");
                else if (Features.IsSupported("FpuMulHack"))
                    ini.Remove("EmuCore/Gamefixes", "FpuMulHack");

                if (SystemConfig.getOptBoolean("SoftwareRendererFMVHack"))
                    ini.WriteValue("EmuCore/Gamefixes", "SoftwareRendererFMVHack", "true");
                else if (Features.IsSupported("SoftwareRendererFMVHack"))
                    ini.Remove("EmuCore/Gamefixes", "SoftwareRendererFMVHack");

                if (SystemConfig.getOptBoolean("SkipMPEGHack"))
                    ini.WriteValue("EmuCore/Gamefixes", "SkipMPEGHack", "true");
                else if (Features.IsSupported("SkipMPEGHack"))
                    ini.Remove("EmuCore/Gamefixes", "SkipMPEGHack");

                if (SystemConfig.getOptBoolean("GoemonTlbHack"))
                    ini.WriteValue("EmuCore/Gamefixes", "GoemonTlbHack", "true");
                else if (Features.IsSupported("GoemonTlbHack"))
                    ini.Remove("EmuCore/Gamefixes", "GoemonTlbHack");

                if (SystemConfig.getOptBoolean("EETimingHack"))
                    ini.WriteValue("EmuCore/Gamefixes", "EETimingHack", "true");
                else if (Features.IsSupported("EETimingHack"))
                    ini.Remove("EmuCore/Gamefixes", "EETimingHack");

                if (SystemConfig.getOptBoolean("InstantDMAHack"))
                    ini.WriteValue("EmuCore/Gamefixes", "InstantDMAHack", "true");
                else if (Features.IsSupported("InstantDMAHack"))
                    ini.Remove("EmuCore/Gamefixes", "InstantDMAHack");

                if (SystemConfig.getOptBoolean("OPHFlagHack"))
                    ini.WriteValue("EmuCore/Gamefixes", "OPHFlagHack", "true");
                else if (Features.IsSupported("OPHFlagHack"))
                    ini.Remove("EmuCore/Gamefixes", "OPHFlagHack");

                if (SystemConfig.getOptBoolean("GIFFIFOHack"))
                    ini.WriteValue("EmuCore/Gamefixes", "GIFFIFOHack", "true");
                else if (Features.IsSupported("GIFFIFOHack"))
                    ini.Remove("EmuCore/Gamefixes", "GIFFIFOHack");

                if (SystemConfig.getOptBoolean("DMABusyHack"))
                    ini.WriteValue("EmuCore/Gamefixes", "DMABusyHack", "true");
                else if (Features.IsSupported("DMABusyHack"))
                    ini.Remove("EmuCore/Gamefixes", "DMABusyHack");

                if (SystemConfig.getOptBoolean("VIF1StallHack"))
                    ini.WriteValue("EmuCore/Gamefixes", "VIF1StallHack", "true");
                else if (Features.IsSupported("VIF1StallHack"))
                    ini.Remove("EmuCore/Gamefixes", "VIF1StallHack");

                if (SystemConfig.getOptBoolean("VIFFIFOHack"))
                    ini.WriteValue("EmuCore/Gamefixes", "VIFFIFOHack", "true");
                else if (Features.IsSupported("VIFFIFOHack"))
                    ini.Remove("EmuCore/Gamefixes", "VIFFIFOHack");

                if (SystemConfig.getOptBoolean("IbitHack"))
                    ini.WriteValue("EmuCore/Gamefixes", "IbitHack", "true");
                else if (Features.IsSupported("IbitHack"))
                    ini.Remove("EmuCore/Gamefixes", "IbitHack");

                if (SystemConfig.getOptBoolean("VuAddSubHack"))
                    ini.WriteValue("EmuCore/Gamefixes", "VuAddSubHack", "true");
                else if (Features.IsSupported("VuAddSubHack"))
                    ini.Remove("EmuCore/Gamefixes", "VuAddSubHack");

                if (SystemConfig.getOptBoolean("VUOverflowHack"))
                    ini.WriteValue("EmuCore/Gamefixes", "VUOverflowHack", "true");
                else if (Features.IsSupported("VUOverflowHack"))
                    ini.Remove("EmuCore/Gamefixes", "VUOverflowHack");

                if (SystemConfig.getOptBoolean("VUSyncHack"))
                    ini.WriteValue("EmuCore/Gamefixes", "VUSyncHack", "true");
                else if (Features.IsSupported("VUSyncHack"))
                    ini.Remove("EmuCore/Gamefixes", "VUSyncHack");

                if (SystemConfig.getOptBoolean("XgKickHack"))
                    ini.WriteValue("EmuCore/Gamefixes", "XgKickHack", "true");
                else if (Features.IsSupported("XgKickHack"))
                    ini.Remove("EmuCore/Gamefixes", "XgKickHack");

                if (SystemConfig.getOptBoolean("BlitInternalFPSHack"))
                    ini.WriteValue("EmuCore/Gamefixes", "BlitInternalFPSHack", "true");
                else if (Features.IsSupported("BlitInternalFPSHack"))
                    ini.Remove("EmuCore/Gamefixes", "BlitInternalFPSHack");

                // Memory cards management
                ini.WriteValue("MemoryCards", "Slot1_Enable", "true");
                ini.WriteValue("MemoryCards", "Slot2_Enable", "true");

                if (SystemConfig.isOptSet("pcsx2_pergame_memory") && SystemConfig.getOptBoolean("pcsx2_pergame_memory"))
                {
                    ini.WriteValue("MemoryCards", "Slot1_Filename", Path.GetFileNameWithoutExtension(rom) + ".ps2");
                    ini.WriteValue("MemoryCards", "Slot2_Filename", "Mcd002.ps2");
                }
                else
                {
                    ini.WriteValue("MemoryCards", "Slot1_Filename", "Mcd001.ps2");
                    ini.WriteValue("MemoryCards", "Slot2_Filename", "Mcd002.ps2");
                }
            }
        }

        private static void AddPathToRecursivePaths(string romPath, IniFile ini)
        {
            var recursivePaths = ini.EnumerateValues("GameList")
                .Where(e => e.Key == "RecursivePaths")
                .Select(e => Path.GetFullPath(e.Value))
                .ToList();

            if (!recursivePaths.Contains(romPath))
                ini.AppendValue("GameList", "RecursivePaths", romPath);
        }
        #endregion

        public override int RunAndWait(ProcessStartInfo path)
        {
            int ret = 0;
            int monitorIndex = Math.Max(0, SystemConfig["MonitorIndex"].ToInteger() - 1);
            
            if (_bezelFileInfo != null)
            {               
                var bezel = _bezelFileInfo.ShowFakeBezel(_resolution, true, monitorIndex);
                if (bezel != null)
                {
                    RECT rc = bezel.ViewPort;

                    if (rc.bottom - rc.top == (_resolution ?? ScreenResolution.CurrentResolution).Height)
                        rc.bottom--;

                    var process = StartProcessAndMoveItsWindowTo(path, rc);
                    if (process != null)
                    {
                        while (!process.WaitForExit(50))
                            Application.DoEvents();

                        try { ret = process.ExitCode; }
                        catch { }
                    }

                    bezel?.Dispose();

                    return ret;
                }
            }

            if (monitorIndex >= 0 && Screen.AllScreens.Length > 1 && monitorIndex < Screen.AllScreens.Length)
            {
                var process = StartProcessAndMoveItsWindowTo(path, Screen.AllScreens[monitorIndex].Bounds);
                if (process != null)
                {
                    process.WaitForExit();

                    try { ret = process.ExitCode; }
                    catch { }
                }

                return ret;
            }

            return base.RunAndWait(path);
        }

        private Process StartProcessAndMoveItsWindowTo(ProcessStartInfo path, RECT rc)
        {
            int retryCount = 0;
            var process = Process.Start(path);

            while (process != null)
            {
                if (process.WaitForExit(50))
                {
                    process = null;
                    break;
                }

                retryCount++;

                // If it's longer than 10 seconds, then exit loop
                if (retryCount > 10000 / 50)
                    break;

                var hWnd = User32.FindHwnd(process.Id);
                if (hWnd == IntPtr.Zero)
                    continue;

                if (!User32.IsWindowVisible(hWnd))
                    continue;

                User32.SetWindowPos(hWnd, IntPtr.Zero, rc.left, rc.top, rc.right - rc.left, rc.bottom - rc.top, SWP.ASYNCWINDOWPOS);
                if (SystemConfig["pcsx2_crosshair"] == "disabled")
                {
                    Thread.Sleep(500);
                    User32.ShowWindow(hWnd, SW.SHOWMAXIMIZED);
                }
                break;
            }
            return process;
        }
    }
}
