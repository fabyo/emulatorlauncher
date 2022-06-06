﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;

namespace emulatorLauncher.libRetro
{
    partial class LibRetroGenerator : Generator
    {
        private void ConfigureCoreOptions(ConfigFile retroarchConfig, string system, string core)
        {
            var coreSettings = ConfigFile.FromFile(Path.Combine(RetroarchPath, "retroarch-core-options.cfg"), new ConfigFileOptions() { CaseSensitive = true });

            ConfigureBlueMsx(retroarchConfig, coreSettings, system, core);
            ConfigureTheodore(retroarchConfig, coreSettings, system, core);
            ConfigureFCEumm(retroarchConfig, coreSettings, system, core);
            ConfigureNestopia(retroarchConfig, coreSettings, system, core);
            ConfigureO2em(retroarchConfig, coreSettings, system, core);
            ConfigureMame2003(retroarchConfig, coreSettings, system, core);
            ConfigureAtari800(retroarchConfig, coreSettings, system, core);
            ConfigureVirtualJaguar(retroarchConfig, coreSettings, system, core);
            ConfigureSNes9x(retroarchConfig, coreSettings, system, core);
            ConfigureMupen64(retroarchConfig, coreSettings, system, core);
            ConfigurePuae(retroarchConfig, coreSettings, system, core);
            ConfigureFlycast(retroarchConfig, coreSettings, system, core);
            ConfigureMesen(retroarchConfig, coreSettings, system, core);
            ConfigureMednafenPsxHW(retroarchConfig, coreSettings, system, core);
            ConfigureCap32(retroarchConfig, coreSettings, system, core);
            ConfigureQuasi88(retroarchConfig, coreSettings, system, core);
            ConfigureGenesisPlusGX(retroarchConfig, coreSettings, system, core);
            ConfigureGenesisPlusGXWide(retroarchConfig, coreSettings, system, core);
            ConfigurePotator(retroarchConfig, coreSettings, system, core);
            ConfigureDosboxPure(retroarchConfig, coreSettings, system, core);
            ConfigureKronos(retroarchConfig, coreSettings, system, core);
            ConfigurePicodrive(retroarchConfig, coreSettings, system, core);
            ConfigureMednafenSaturn(retroarchConfig, coreSettings, system, core);
            ConfigureCitra(retroarchConfig, coreSettings, system, core);
            ConfigureFbneo(retroarchConfig, coreSettings, system, core);
            ConfigureGambatte(retroarchConfig, coreSettings, system, core);
            ConfigurePpsspp(retroarchConfig, coreSettings, system, core);
            ConfigureMame(retroarchConfig, coreSettings, system, core);
            ConfigureFbalphaCPS1(retroarchConfig, coreSettings, system, core);
            ConfigureFbalphaCPS2(retroarchConfig, coreSettings, system, core);
            ConfigureFbalphaCPS3(retroarchConfig, coreSettings, system, core);
            ConfigureMednafenPce(retroarchConfig, coreSettings, system, core);
            ConfigureNeocd(retroarchConfig, coreSettings, system, core);

            if (coreSettings.IsDirty)
                coreSettings.Save(Path.Combine(RetroarchPath, "retroarch-core-options.cfg"), true);

            // Disable Bezel as default if a widescreen ratio is set. Can be manually set.
            if (SystemConfig.isOptSet("ratio") && !SystemConfig.isOptSet("bezel"))
            {
                int idx = ratioIndexes.IndexOf(SystemConfig["ratio"]);
                if (idx == 1 || idx == 2 || idx == 4 || idx == 6 || idx == 7 || idx == 9 || idx == 14 || idx == 16 || idx == 18 || idx == 19)
                {
                    retroarchConfig["aspect_ratio_index"] = idx.ToString();
                    retroarchConfig["video_aspect_ratio_auto"] = "false";
                    SystemConfig["bezel"] = "none";
                }
            }
            else
                SystemConfig["bezel"] = SystemConfig["bezel"];

        }
        
        private void ConfigureNeocd(ConfigFile retroarchConfig, ConfigFile coreSettings, string system, string core)
        {
            if (core != "neocd")
                return;

            coreSettings["neocd_per_content_saves"] = "On";
            coreSettings["neocd_bios"] = SystemConfig.GetValueOrDefault("neocd_bios", "uni-bioscd.rom (CDZ, Universe 3.3)");
            coreSettings["neocd_cdspeedhack"] = SystemConfig.GetValueOrDefault("neocd_cdspeedhack", "Off");
            coreSettings["neocd_loadskip"] = SystemConfig.GetValueOrDefault("neocd_loadskip", "Off");
            coreSettings["neocd_region"] = SystemConfig.GetValueOrDefault("neocd_region", "USA");
        }
        
        private void ConfigureMednafenPce(ConfigFile retroarchConfig, ConfigFile coreSettings, string system, string core)
        {
            if (core != "mednafen_pce")
                return;

            coreSettings["pce_show_advanced_input_settings"] = "enabled";
            coreSettings["pce_psgrevision"] = SystemConfig.GetValueOrDefault("pce_psgrevision", "auto");
            coreSettings["pce_resamp_quality"] = SystemConfig.GetValueOrDefault("pce_resamp_quality", "3");
            coreSettings["pce_ocmultiplier"] = SystemConfig.GetValueOrDefault("pce_ocmultiplier", "1");
            coreSettings["pce_nospritelimit"] = SystemConfig.GetValueOrDefault("pce_nospritelimit", "disabled");
            coreSettings["pce_cdimagecache"] = SystemConfig.GetValueOrDefault("pce_cdimagecache", "disabled");
            coreSettings["pce_cdbios"] = SystemConfig.GetValueOrDefault("pce_cdbios", "System Card 3");
            coreSettings["pce_cdspeed"] = SystemConfig.GetValueOrDefault("pce_cdspeed", "1");
            coreSettings["pce_palette"] = SystemConfig.GetValueOrDefault("pce_palette", "Composite");
            coreSettings["pce_scaling"] = SystemConfig.GetValueOrDefault("pce_scaling", "auto");
            coreSettings["pce_hires_blend"] = SystemConfig.GetValueOrDefault("pce_hires_blend", "disabled");
            coreSettings["pce_h_overscan"] = SystemConfig.GetValueOrDefault("pce_h_overscan", "auto");
            coreSettings["pce_adpcmextraprec"] = SystemConfig.GetValueOrDefault("pce_adpcmextraprec", "12-bit");
            coreSettings["pce_adpcmvolume"] = SystemConfig.GetValueOrDefault("pcecdvolume", "100");
            coreSettings["pce_cddavolume"] = SystemConfig.GetValueOrDefault("pcecdvolume", "100");
            coreSettings["pce_cdpsgvolume"] = SystemConfig.GetValueOrDefault("pcecdvolume", "100");
        }

        private void ConfigureFbalphaCPS3(ConfigFile retroarchConfig, ConfigFile coreSettings, string system, string core)
        {
            if (core != "fbalpha2012_cps3")
                return;

            coreSettings["fbalpha2012_cps3_frameskip"] = "0";
            coreSettings["fbalpha2012_cps3_aspect"] = "DAR";
            coreSettings["fbalpha2012_cps3_cpu_speed_adjust"] = SystemConfig.GetValueOrDefault("fbalpha2012_cps3_cpu_speed_adjust", "100");
            coreSettings["fbalpha2012_cps3_hiscores"] = SystemConfig.GetValueOrDefault("fbalpha2012_cps3_hiscores", "enabled");
            coreSettings["fbalpha2012_cps3_controls_p1"] = SystemConfig.GetValueOrDefault("fbalpha2012_cps3_controls_p1", "gamepad");
            coreSettings["fbalpha2012_cps3_controls_p2"] = SystemConfig.GetValueOrDefault("fbalpha2012_cps3_controls_p2", "gamepad");
            coreSettings["fbalpha2012_cps3_lr_controls_p1"] = SystemConfig.GetValueOrDefault("fbalpha2012_cps3_lr_controls_p1", "normal");
            coreSettings["fbalpha2012_cps3_lr_controls_p2"] = SystemConfig.GetValueOrDefault("fbalpha2012_cps3_lr_controls_p2", "normal");
        }

        private void ConfigureFbalphaCPS2(ConfigFile retroarchConfig, ConfigFile coreSettings, string system, string core)
        {
            if (core != "fbalpha2012_cps2")
                return;

            coreSettings["fba2012cps2_frameskip"] = "disabled";
            coreSettings["fba2012cps2_aspect"] = "DAR";
            coreSettings["fba2012cps2_auto_rotate"] = SystemConfig.GetValueOrDefault("fba2012cps1_auto_rotate", "enabled");
            coreSettings["fba2012cps2_cpu_speed_adjust"] = SystemConfig.GetValueOrDefault("fba2012cps1_cpu_speed_adjust", "100");
            coreSettings["fba2012cps2_hiscores"] = SystemConfig.GetValueOrDefault("fba2012cps1_hiscores", "enabled");
            coreSettings["fba2012cps2_lowpass_filter"] = SystemConfig.GetValueOrDefault("fba2012cps1_lowpass_filter", "disabled");
            coreSettings["fba2012cps2_lowpass_range"] = SystemConfig.GetValueOrDefault("fba2012cps1_lowpass_range", "50");
            coreSettings["fba2012cps2_controls"] = SystemConfig.GetValueOrDefault("fba2012cps2_controls", "gamepad");
        }

        private void ConfigureFbalphaCPS1(ConfigFile retroarchConfig, ConfigFile coreSettings, string system, string core)
        {
            if (core != "fbalpha2012_cps1")
                return;

            coreSettings["fba2012cps1_frameskip"] = "disabled";
            coreSettings["fba2012cps1_aspect"] = "DAR";
            coreSettings["fba2012cps1_auto_rotate"] = SystemConfig.GetValueOrDefault("fba2012cps1_auto_rotate", "enabled");
            coreSettings["fba2012cps1_cpu_speed_adjust"] = SystemConfig.GetValueOrDefault("fba2012cps1_cpu_speed_adjust", "100");
            coreSettings["fba2012cps1_hiscores"] = SystemConfig.GetValueOrDefault("fba2012cps1_hiscores", "enabled");
            coreSettings["fba2012cps1_lowpass_filter"] = SystemConfig.GetValueOrDefault("fba2012cps1_lowpass_filter", "disabled");
            coreSettings["fba2012cps1_lowpass_range"] = SystemConfig.GetValueOrDefault("fba2012cps1_lowpass_range", "50");
        }

        private void ConfigurePpsspp(ConfigFile retroarchConfig, ConfigFile coreSettings, string system, string core)
        {
            if (core != "ppsspp")
                return;

            coreSettings["ppsspp_cpu_core"] = "jit";
            coreSettings["ppsspp_auto_frameskip"] = "disabled";
            coreSettings["ppsspp_frameskip"] = "0";
            coreSettings["ppsspp_frameskiptype"] = "number of frames";
            coreSettings["ppsspp_rendering_mode"] = "buffered";
            coreSettings["ppsspp_locked_cpu_speed"] = "off";
            coreSettings["ppsspp_cheats"] = "enabled";
            coreSettings["ppsspp_button_preference"] = "cross";

            switch (SystemConfig["PerformanceMode"])
            {
                case "Fast":
                    coreSettings["ppsspp_block_transfer_gpu"] = "disabled";
                    coreSettings["ppsspp_spline_quality"] = "low";
                    coreSettings["ppsspp_software_skinning"] = "enabled";
                    coreSettings["ppsspp_gpu_hardware_transform"] = "enabled";
                    coreSettings["ppsspp_vertex_cache"] = "enabled";
                    coreSettings["ppsspp_fast_memory"] = "enabled";
                    coreSettings["ppsspp_lazy_texture_caching"] = "enabled";
                    coreSettings["ppsspp_retain_changed_textures"] = "enabled";
                    coreSettings["ppsspp_force_lag_sync"] = "disabled";
                    coreSettings["ppsspp_disable_slow_framebuffer_effects"] = "enabled";
                    break;
                case "Balanced":
                    coreSettings["ppsspp_block_transfer_gpu"] = "enabled";
                    coreSettings["ppsspp_spline_quality"] = "medium";
                    coreSettings["ppsspp_software_skinning"] = "disabled";
                    coreSettings["ppsspp_gpu_hardware_transform"] = "enabled";
                    coreSettings["ppsspp_vertex_cache"] = "enabled";
                    coreSettings["ppsspp_fast_memory"] = "enabled";
                    coreSettings["ppsspp_lazy_texture_caching"] = "disabled";
                    coreSettings["ppsspp_retain_changed_textures"] = "disabled";
                    coreSettings["ppsspp_force_lag_sync"] = "disabled";
                    coreSettings["ppsspp_disable_slow_framebuffer_effects"] = "disabled";
                    break;
                case "Accurate":
                    coreSettings["ppsspp_block_transfer_gpu"] = "enabled";
                    coreSettings["ppsspp_spline_quality"] = "high";
                    coreSettings["ppsspp_software_skinning"] = "disabled";
                    coreSettings["ppsspp_gpu_hardware_transform"] = "disabled";
                    coreSettings["ppsspp_vertex_cache"] = "disabled";
                    coreSettings["ppsspp_fast_memory"] = "disabled";
                    coreSettings["ppsspp_lazy_texture_caching"] = "disabled";
                    coreSettings["ppsspp_retain_changed_textures"] = "disabled";
                    coreSettings["ppsspp_force_lag_sync"] = "enabled";
                    coreSettings["ppsspp_disable_slow_framebuffer_effects"] = "disabled";
                    break;
                default:
                    coreSettings["ppsspp_block_transfer_gpu"] = "enabled";
                    coreSettings["ppsspp_spline_quality"] = "medium";
                    coreSettings["ppsspp_software_skinning"] = "disabled";
                    coreSettings["ppsspp_gpu_hardware_transform"] = "enabled";
                    coreSettings["ppsspp_vertex_cache"] = "enabled";
                    coreSettings["ppsspp_fast_memory"] = "enabled";
                    coreSettings["ppsspp_lazy_texture_caching"] = "disabled";
                    coreSettings["ppsspp_retain_changed_textures"] = "disabled";
                    coreSettings["ppsspp_force_lag_sync"] = "disabled";
                    coreSettings["ppsspp_disable_slow_framebuffer_effects"] = "disabled";
                    break;
            }

            coreSettings["ppsspp_internal_resolution"] = SystemConfig.GetValueOrDefault("ppsspp_internal_resolution", "1440x816");
            coreSettings["ppsspp_texture_anisotropic_filtering"] = SystemConfig.GetValueOrDefault("ppsspp_texture_anisotropic_filtering", "off");
            coreSettings["ppsspp_texture_filtering"] = SystemConfig.GetValueOrDefault("ppsspp_texture_filtering", "auto");
            coreSettings["ppsspp_texture_scaling_type"] = SystemConfig.GetValueOrDefault("ppsspp_texture_scaling_type", "xbrz");
            coreSettings["ppsspp_texture_scaling_level"] = SystemConfig.GetValueOrDefault("ppsspp_texture_scaling_level", "auto");
            coreSettings["ppsspp_texture_deposterize"] = SystemConfig.GetValueOrDefault("ppsspp_texture_deposterize", "disabled");
            coreSettings["ppsspp_language"] = SystemConfig.GetValueOrDefault("ppsspp_language", "automatic");
            coreSettings["ppsspp_io_timing_method"] = SystemConfig.GetValueOrDefault("ppsspp_io_timing_method", "Fast");
            coreSettings["ppsspp_ignore_bad_memory_access"] = SystemConfig.GetValueOrDefault("ppsspp_ignore_bad_memory_access", "enabled");
            coreSettings["ppsspp_texture_replacement"] = SystemConfig.GetValueOrDefault("ppsspp_texture_replacement", "disabled");
        }

        private void ConfigureGambatte(ConfigFile retroarchConfig, ConfigFile coreSettings, string system, string core)
        {
            if (core != "gambatte")
                return;

            coreSettings["gambatte_gb_bootloader"] = "enabled";
            coreSettings["gambatte_gbc_color_correction_mode"] = "accurate";
            coreSettings["gambatte_gbc_color_correction"] = "GBC only";
            coreSettings["gambatte_up_down_allowed"] = "disabled";
            coreSettings["gambatte_gb_hwmode"] = SystemConfig.GetValueOrDefault("gambatte_gb_hwmode", "Auto");
            coreSettings["gambatte_mix_frames"] = SystemConfig.GetValueOrDefault("gambatte_mix_frames", "lcd_ghosting");
            coreSettings["gambatte_gb_internal_palette"] = SystemConfig.GetValueOrDefault("gambatte_gb_internal_palette", "GB - DMG");
            coreSettings["gambatte_gb_colorization"] = SystemConfig.GetValueOrDefault("gambatte_gb_colorization", "auto");
        }

        private void ConfigureFbneo(ConfigFile retroarchConfig, ConfigFile coreSettings, string system, string core)
        {
            if (core != "fbneo")
                return;

            coreSettings["fbneo-allow-depth-32"] = "enabled";
            coreSettings["fbneo-allow-patched-romsets"] = "enabled";
            coreSettings["fbneo-memcard-mode"] = "per-game";
            coreSettings["fbneo-hiscores"] = "enabled";
            coreSettings["fbneo-load-subsystem-from-parent"] = "enabled";
            coreSettings["fbneo-fm-interpolation"] = "4-point 3rd order";
            coreSettings["fbneo-sample-interpolation"] = "4-point 3rd order";
            coreSettings["fbneo-neogeo-mode"] = SystemConfig.GetValueOrDefault("fbneo-neogeo-mode", "UNIBIOS");
            coreSettings["fbneo-vertical-mode"] = SystemConfig.GetValueOrDefault("fbneo-vertical-mode", "disabled");

            if (SystemConfig["fbneo-vertical-mode"] == "enabled")
                SystemConfig["bezel"] = "none";

            coreSettings["fbneo-lightgun-hide-crosshair"] = SystemConfig.GetValueOrDefault("fbneo-lightgun-hide-crosshair", "disabled");

            if (SystemConfig.isOptSet("use_guns") && SystemConfig.getOptBoolean("use_guns"))
            {
                retroarchConfig["input_libretro_device_p1"] = "4";
                retroarchConfig["input_player1_mouse_index"] = "0";
                retroarchConfig["input_player1_gun_trigger_mbtn"] = "1";
                retroarchConfig["input_player1_gun_aux_a_mbtn"]   = "2"; // # for all games ?
                retroarchConfig["input_player1_gun_start_mbtn"]   = "3";
                retroarchConfig["input_player1_gun_start"] = "enter";
                retroarchConfig["input_player1_gun_select"] = "escape";
            }
        }

        private void ConfigureCitra(ConfigFile retroarchConfig, ConfigFile coreSettings, string system, string core)
        {
            if (core != "citra")
                return;

            coreSettings["citra_use_libretro_save_path"] = "LibRetro Default";
            coreSettings["citra_is_new_3ds"] = "New 3DS";

            if (SystemConfig.isOptSet("citra_layout_option"))
            {
                coreSettings["citra_layout_option"] = SystemConfig["citra_layout_option"];
                if ((SystemConfig["citra_layout_option"] == "Large Screen, Small Screen") && !SystemConfig.isOptSet("ratio") && !SystemConfig.isOptSet("bezel"))
                {
                    retroarchConfig["aspect_ratio_index"] = "1";
                    SystemConfig["bezel"] = "none";
                }
                else if ((SystemConfig["citra_layout_option"] == "Single Screen Only") && !SystemConfig.isOptSet("ratio") && !SystemConfig.isOptSet("bezel"))
                {
                    retroarchConfig["aspect_ratio_index"] = "2";
                    SystemConfig["bezel"] = "none";
                }
                else
                    SystemConfig["bezel"] = SystemConfig["bezel"];
            }
            else
                coreSettings["citra_layout_option"] = "Default Top-Bottom Screen";

            coreSettings["citra_mouse_show_pointer"] = SystemConfig.GetValueOrDefault("citra_mouse_show_pointer", "enabled");
            coreSettings["citra_region_value"] = SystemConfig.GetValueOrDefault("citra_region_value", "Auto");
            coreSettings["citra_resolution_factor"] = SystemConfig.GetValueOrDefault("citra_resolution_factor", "1x (Native)");
            coreSettings["citra_swap_screen"] = SystemConfig.GetValueOrDefault("citra_swap_screen", "Top");
            coreSettings["citra_mouse_touchscreen"] = SystemConfig.GetValueOrDefault("citra_mouse_touchscreen", "enabled");
        }

        private void ConfigureMednafenSaturn(ConfigFile retroarchConfig, ConfigFile coreSettings, string system, string core)
        {
            if (core != "mednafen_saturn")
                return;

            coreSettings["beetle_saturn_autortc"] = "enabled";
            coreSettings["beetle_saturn_shared_ext"] = "enabled";
            coreSettings["beetle_saturn_shared_int"] = "enabled";
            coreSettings["beetle_saturn_autortc_lang"] = SystemConfig.GetValueOrDefault("beetle_saturn_autortc_lang", "english");
            coreSettings["beetle_saturn_cart"] = SystemConfig.GetValueOrDefault("beetle_saturn_cart", "Auto Detect");
            coreSettings["beetle_saturn_cdimagecache"] = SystemConfig.GetValueOrDefault("beetle_saturn_cdimagecache", "disabled");
            coreSettings["beetle_saturn_midsync"] = SystemConfig.GetValueOrDefault("beetle_saturn_midsync", "disabled");
            coreSettings["beetle_saturn_multitap_port1"] = SystemConfig.GetValueOrDefault("beetle_saturn_multitap_port1", "disabled");
            coreSettings["beetle_saturn_multitap_port2"] = SystemConfig.GetValueOrDefault("beetle_saturn_multitap_port2", "disabled");
            coreSettings["beetle_saturn_region"] = SystemConfig.GetValueOrDefault("beetle_saturn_region", "Auto Detect");
            coreSettings["beetle_saturn_midsync"] = SystemConfig.GetValueOrDefault("beetle_saturn_midsync", "disabled");

            coreSettings["beetle_saturn_virtuagun_crosshair"] = "cross";
            coreSettings["beetle_saturn_mouse_sensitivity"] = "100%";
            coreSettings["beetle_saturn_virtuagun_input"] = "lightgun";

            if (SystemConfig.isOptSet("use_guns") && SystemConfig.getOptBoolean("use_guns"))
            {
                retroarchConfig["input_libretro_device_p1"] = "260";                
                retroarchConfig["input_player1_mouse_index"] = "0";                
                retroarchConfig["input_player1_gun_trigger_mbtn"] = "1";
                retroarchConfig["input_player1_gun_offscreen_shot_mbtn"] = "2";
                retroarchConfig["input_player1_gun_start_mbtn"] = "3";
                retroarchConfig["input_player1_gun_start"] = "enter";
                retroarchConfig["input_player1_gun_select"] = "escape";
            }
        }

        private void ConfigurePicodrive(ConfigFile retroarchConfig, ConfigFile coreSettings, string system, string core)
        {
            if (core != "picodrive")
                return;

            coreSettings["picodrive_ramcart"] = "disabled";

            coreSettings["picodrive_overclk68k"] = SystemConfig.GetValueOrDefault("overclk68k", "disabled");
            coreSettings["picodrive_overscan"] = SystemConfig.GetValueOrDefault("overscan", "disabled");
            coreSettings["picodrive_region"] = SystemConfig.GetValueOrDefault("region", "Auto");
            coreSettings["picodrive_renderer"] = SystemConfig.GetValueOrDefault("renderer", "accurate");
            coreSettings["picodrive_audio_filter"] = SystemConfig.GetValueOrDefault("audio_filter", "disabled");
            coreSettings["picodrive_drc"] = SystemConfig.GetValueOrDefault("dynamic_recompiler", "disabled");
            coreSettings["picodrive_input1"] = SystemConfig.GetValueOrDefault("input1", "3 button pad");
            coreSettings["picodrive_input2"] = SystemConfig.GetValueOrDefault("input2", "3 button pad");
        }

        private void ConfigureKronos(ConfigFile retroarchConfig, ConfigFile coreSettings, string system, string core)
        {
            if (core != "kronos")
                return;

            coreSettings["kronos_use_beetle_saves"] = "enabled";
            coreSettings["kronos_multitap_port2"] = "disabled";
            coreSettings["kronos_sh2coretype"] = "kronos";
            coreSettings["kronos_addon_cartridge"] = SystemConfig.GetValueOrDefault("addon_cartridge", "512K_backup_ram");
            coreSettings["kronos_force_downsampling"] = SystemConfig.GetValueOrDefault("force_downsampling", "disabled");
            coreSettings["kronos_language_id"] = SystemConfig.GetValueOrDefault("language_id", "English");
            coreSettings["kronos_meshmode"] = SystemConfig.GetValueOrDefault("meshmode", "disabled");
            coreSettings["kronos_multitap_port1"] = SystemConfig.GetValueOrDefault("multitap_port1", "disabled");
            coreSettings["kronos_polygon_mode"] = SystemConfig.GetValueOrDefault("polygon_mode", "cpu_tesselation");
            coreSettings["kronos_resolution_mode"] = SystemConfig.GetValueOrDefault("resolution_mode", "original");
            coreSettings["kronos_use_cs"] = SystemConfig.GetValueOrDefault("use_cs", "disabled");
            coreSettings["kronos_videocoretype"] = SystemConfig.GetValueOrDefault("videocoretype", "opengl");
            coreSettings["kronos_videoformattype"] = SystemConfig.GetValueOrDefault("videoformattype", "auto");

            if (SystemConfig.isOptSet("use_guns") && SystemConfig.getOptBoolean("use_guns"))
            {
                retroarchConfig["input_libretro_device_p1"] = "260";
                retroarchConfig["input_player1_mouse_index"] = "0";
                retroarchConfig["input_player1_gun_trigger_mbtn"] = "1";
                retroarchConfig["input_player1_gun_offscreen_shot_mbtn"] = "2";
                retroarchConfig["input_player1_gun_start_mbtn"] = "3";
                retroarchConfig["input_player1_gun_start"] = "enter";
                retroarchConfig["input_player1_gun_select"] = "escape";
            }
        }

        private void ConfigureTheodore(ConfigFile retroarchConfig, ConfigFile coreSettings, string system, string core)
        {
            if (core != "theodore")
                return;

            coreSettings["theodore_autorun"] = "enabled";
        }

        private void ConfigureBlueMsx(ConfigFile retroarchConfig, ConfigFile coreSettings, string system, string core)
        {
            if (core != "bluemsx")
                return;

            coreSettings["bluemsx_overscan"] = "enabled";

            if (system == "spectravideo")
                coreSettings["bluemsx_msxtype"] = "SVI - Spectravideo SVI-328 MK2";
            else if (system == "colecovision")
                coreSettings["bluemsx_msxtype"] = "ColecoVision";
            else if (system == "msx1")
                coreSettings["bluemsx_msxtype"] = "MSX";
            else if (system == "msx2")
                coreSettings["bluemsx_msxtype"] = "MSX2";
            else if (system == "msx2+")
                coreSettings["bluemsx_msxtype"] = "MSX2+";
            else if (system == "msxturbor")
                coreSettings["bluemsx_msxtype"] = "MSXturboR";
            else
                coreSettings["bluemsx_msxtypec"] = "Auto";
        }

        private void ConfigureFCEumm(ConfigFile retroarchConfig, ConfigFile coreSettings, string system, string core)
        {
            if (core != "fceumm")
                return;

            if (Features.IsSupported("fceumm_cropoverscan"))
            {
                if (SystemConfig.isOptSet("fceumm_cropoverscan") && SystemConfig["fceumm_cropoverscan"] == "none")
                {
                    coreSettings["fceumm_overscan_h"] = "disabled";
                    coreSettings["fceumm_overscan_v"] = "disabled";
                }
                else if (SystemConfig.isOptSet("fceumm_cropoverscan") && SystemConfig["fceumm_cropoverscan"] == "h")
                {
                    coreSettings["fceumm_overscan_h"] = "enabled";
                    coreSettings["fceumm_overscan_v"] = "disabled";
                }
                else if (SystemConfig.isOptSet("fceumm_cropoverscan") && SystemConfig["fceumm_cropoverscan"] == "v")
                {
                    coreSettings["fceumm_overscan_h"] = "disabled";
                    coreSettings["fceumm_overscan_v"] = "enabled";
                }
                else
                {
                    coreSettings["fceumm_overscan_h"] = "enabled";
                    coreSettings["fceumm_overscan_v"] = "enabled";
                }
            }

            coreSettings["fceumm_palette"] = SystemConfig.GetValueOrDefault("fceumm_palette", "default");
            coreSettings["fceumm_ntsc_filter"] = SystemConfig.GetValueOrDefault("fceumm_ntsc_filter", "disabled");
            coreSettings["fceumm_sndquality"] = SystemConfig.GetValueOrDefault("fceumm_sndquality", "Low");
            coreSettings["fceumm_overclocking"] = SystemConfig.GetValueOrDefault("fceumm_overclocking", "disabled");
            coreSettings["fceumm_nospritelimit"] = SystemConfig.GetValueOrDefault("fceumm_nospritelimit", "enabled");
            coreSettings["fceumm_show_crosshair"] = SystemConfig.GetValueOrDefault("fceumm_show_crosshair", "enabled");

            if (SystemConfig.isOptSet("use_guns") && SystemConfig.getOptBoolean("use_guns"))
            {
                retroarchConfig["input_libretro_device_p2"] = "258";
                retroarchConfig["input_player2_mouse_index"] = "0";
                retroarchConfig["input_player2_gun_trigger_mbtn"] = "1";

                coreSettings["fceumm_zapper_mode"] = "lightgun";
            }
        }

        private void ConfigureNestopia(ConfigFile retroarchConfig, ConfigFile coreSettings, string system, string core)
        {
            if (core != "nestopia")
                return;

            if (Features.IsSupported("nestopia_cropoverscan"))
            {
                if (SystemConfig.isOptSet("nestopia_cropoverscan") && SystemConfig["nestopia_cropoverscan"] == "none")
                {
                    coreSettings["nestopia_overscan_h"] = "disabled";
                    coreSettings["nestopia_overscan_v"] = "disabled";
                }
                else if (SystemConfig.isOptSet("nestopia_cropoverscan") && SystemConfig["nestopia_cropoverscan"] == "h")
                {
                    coreSettings["nestopia_overscan_h"] = "enabled";
                    coreSettings["nestopia_overscan_v"] = "disabled";
                }
                else if (SystemConfig.isOptSet("nestopia_cropoverscan") && SystemConfig["nestopia_cropoverscan"] == "v")
                {
                    coreSettings["nestopia_overscan_h"] = "disabled";
                    coreSettings["nestopia_overscan_v"] = "enabled";
                }
                else
                {
                    coreSettings["nestopia_overscan_h"] = "enabled";
                    coreSettings["nestopia_overscan_v"] = "enabled";
                }
            }

            coreSettings["nestopia_nospritelimit"] = SystemConfig.GetValueOrDefault("nestopia_nospritelimit", "enabled");
            coreSettings["nestopia_palette"] = SystemConfig.GetValueOrDefault("nestopia_palette", "consumer");
            coreSettings["nestopia_blargg_ntsc_filter"] = SystemConfig.GetValueOrDefault("nestopia_blargg_ntsc_filter", "disabled");
            coreSettings["nestopia_overclock"] = SystemConfig.GetValueOrDefault("nestopia_overclock", "1x");
            coreSettings["nestopia_select_adapter"] = SystemConfig.GetValueOrDefault("nestopia_select_adapter", "auto");
            coreSettings["nestopia_show_crosshair"] = SystemConfig.GetValueOrDefault("nestopia_show_crosshair", "enabled");

            if (SystemConfig.isOptSet("use_guns") && SystemConfig.getOptBoolean("use_guns"))
            {
                retroarchConfig["input_libretro_device_p2"] = "262";
                retroarchConfig["input_player2_mouse_index"] = "0";
                retroarchConfig["input_player2_gun_trigger_mbtn"] = "1";

                coreSettings["nestopia_zapper_device"] = "lightgun";
            }
        }        

        private void ConfigureO2em(ConfigFile retroarchConfig, ConfigFile coreSettings, string system, string core)
        {
            if (core != "o2em")
                return;

            coreSettings["o2em_vkbd_transparency"] = "25";

            // Emulated Hardware
            if (Features.IsSupported("o2em_bios"))
            {
                if (SystemConfig.isOptSet("o2em_bios"))
                    coreSettings["o2em_bios"] = SystemConfig["o2em_bios"];
                else if (system == "videopacplus")
                    coreSettings["o2em_bios"] = "g7400.bin";
                else
                    coreSettings["o2em_bios"] = "o2rom.bin";
            }

            // Emulated Hardware
            if (Features.IsSupported("o2em_region"))
                coreSettings["o2em_region"] = SystemConfig.GetValueOrDefault("o2em_region", "auto");

            // Swap Gamepad
            if (Features.IsSupported("o2em_swap_gamepads"))
                coreSettings["o2em_swap_gamepads"] = SystemConfig.GetValueOrDefault("o2em_swap_gamepads", "disabled");

            // Crop Overscan
            if (Features.IsSupported("o2em_crop_overscan"))
                coreSettings["o2em_crop_overscan"] = SystemConfig.GetValueOrDefault("o2em_crop_overscan", "enabled");

            // Ghosting effect
            if (Features.IsSupported("o2em_mix_frames"))
                coreSettings["o2em_mix_frames"] = SystemConfig.GetValueOrDefault("o2em_mix_frames", "disabled");

            // Audio Filter
            if (Features.IsSupported("o2em_low_pass_range"))
            {
                if (SystemConfig.isOptSet("o2em_low_pass_range") && SystemConfig["o2em_low_pass_range"] != "0")
                {
                    coreSettings["o2em_low_pass_filter"] = "enabled";
                    coreSettings["o2em_low_pass_range"] = SystemConfig["o2em_low_pass_range"];
                }
                else
                {
                    coreSettings["o2em_low_pass_filter"] = "disabled";
                    coreSettings["o2em_low_pass_range"] = "0";
                }
            }
        }

        private void ConfigureMame2003(ConfigFile retroarchConfig, ConfigFile coreSettings, string system, string core)
        {
            if (core != "mame078plus" && core != "mame2003_plus")
                return;

            coreSettings["mame2003-plus_skip_disclaimer"] = "enabled";
            coreSettings["mame2003-plus_skip_warnings"] = "enabled";
            coreSettings["mame2003-plus_xy_device"] = "lightgun";           

            coreSettings["mame2003-plus_analog"] = SystemConfig.GetValueOrDefault("mame2003-plus_analog", "digital");
            coreSettings["mame2003-plus_frameskip"] = SystemConfig.GetValueOrDefault("mame2003-plus_frameskip", "0");
            coreSettings["mame2003-plus_input_interface"] = SystemConfig.GetValueOrDefault("mame2003-plus_input_interface", "retropad");
            coreSettings["mame2003-plus_neogeo_bios"] = SystemConfig.GetValueOrDefault("mame2003-plus_neogeo_bios", "unibios33");
            coreSettings["mame2003-plus_tate_mode"] = SystemConfig.GetValueOrDefault("mame2003-plus_tate_mode", "disabled");

            if (SystemConfig["mame2003-plus_tate_mode"] == "enabled")
                SystemConfig["bezel"] = "none";
        }

        private void ConfigureQuasi88(ConfigFile retroarchConfig, ConfigFile coreSettings, string system, string core)
        {
            if (core != "quasi88")
                return;

            coreSettings["q88_basic_mode"] = SystemConfig.GetValueOrDefault("q88_basic_mode", "N88 V2");
            coreSettings["q88_cpu_clock"] = SystemConfig.GetValueOrDefault("q88_cpu_clock", "4");
            coreSettings["q88_pcg-8100"] = SystemConfig.GetValueOrDefault("q88_pcg-8100", "disabled");
        }

        private void ConfigureCap32(ConfigFile retroarchConfig, ConfigFile coreSettings, string system, string core)
        {
            if (core != "cap32")
                return;

            // Virtual Keyboard by default (select+start) change to (start+Y)
            coreSettings["cap32_combokey"] = "y";

            //  Auto Select Model
            if (system == "gx4000")
                coreSettings["cap32_model"] = "6128+";
            else
                coreSettings["cap32_model"] = SystemConfig.GetValueOrDefault("cap32_model", "6128");                

            //  Ram size
            coreSettings["cap32_ram"] = SystemConfig.GetValueOrDefault("cap32_ram", "128");
        }

        private void ConfigureAtari800(ConfigFile retroarchConfig, ConfigFile coreSettings, string system, string core)
        {
            if (core != "atari800")
                return;

            bool atari800 = (system == "atari800");
            bool atariXE = !atari800 && system.IndexOf("xe", StringComparison.InvariantCultureIgnoreCase) >= 0;
         
            if (atari800)
            {
                var romExt =  Path.GetExtension(Program.SystemConfig["rom"]).ToLower();

                coreSettings["atari800_internalbasic"] = (romExt == ".bas" ? "enabled" : "disabled");
                coreSettings["atari800_cassboot"] = (romExt == ".cas" ? "enabled" : "disabled");
                coreSettings["atari800_opt1"] = "disabled"; // detect card type

                coreSettings["atari800_system"] = SystemConfig.GetValueOrDefault("atari800_system", "800XL (64K)");
                coreSettings["atari800_ntscpal"] = SystemConfig.GetValueOrDefault("atari800_ntscpal", "NTSC");
                coreSettings["atari800_sioaccel"] = SystemConfig.GetValueOrDefault("atari800_sioaccel", "enabled");
                coreSettings["atari800_artifacting"] = SystemConfig.GetValueOrDefault("atari800_artifacting", "disabled");
            }
            else if (atariXE)
            {             
                coreSettings["atari800_system"] = "130XE (128K)";
                coreSettings["atari800_internalbasic"] = "disabled";
                coreSettings["atari800_opt1"] = "enabled";
                coreSettings["atari800_cassboot"] = "disabled";

                coreSettings["atari800_ntscpal"] = SystemConfig.GetValueOrDefault("atari800_ntscpal", "NTSC");
                coreSettings["atari800_sioaccel"] = SystemConfig.GetValueOrDefault("atari800_sioaccel", "enabled");
                coreSettings["atari800_artifacting"] = SystemConfig.GetValueOrDefault("atari800_artifacting", "disabled");
            }                       
            else // Atari 5200
            {
                coreSettings["atari800_system"] = "5200";
                coreSettings["atari800_opt1"] = "enabled"; // detect card type
                coreSettings["atari800_cassboot"] = "disabled";
            }

            if (string.IsNullOrEmpty(AppConfig["bios"]))
                return;

            var atariCfg = ConfigFile.FromFile(Path.Combine(RetroarchPath, ".atari800.cfg"), new ConfigFileOptions() { CaseSensitive = true, KeepEmptyLines = true });
            if (!atariCfg.Any())
                atariCfg.AppendLine("Atari 800 Emulator, Version 3.1.0");

            string biosPath = AppConfig.GetFullPath("bios");            
            atariCfg["ROM_OS_A_PAL"] = Path.Combine(biosPath, "ATARIOSA.ROM");
            atariCfg["ROM_OS_BB01R2"] = Path.Combine(biosPath, "ATARIXL.ROM");
            atariCfg["ROM_BASIC_C"] = Path.Combine(biosPath, "ATARIBAS.ROM");
            atariCfg["ROM_400/800_CUSTOM"] = Path.Combine(biosPath, "ATARIOSB.ROM");
            atariCfg["ROM_XL/XE_CUSTOM"] = Path.Combine(biosPath, "ATARIXL.ROM");
            atariCfg["ROM_5200"] = Path.Combine(biosPath, "5200.ROM");
            atariCfg["ROM_5200_CUSTOM"] = Path.Combine(biosPath, "atari5200.ROM");

            atariCfg["OS_XL/XE_VERSION"] = "AUTO";
            atariCfg["OS_5200_VERSION"] = "AUTO";
            atariCfg["BASIC_VERSION"] = "AUTO";
            atariCfg["XEGS_GAME_VERSION"] = "AUTO";
            atariCfg["OS_400/800_VERSION"] = "AUTO";

            atariCfg["CASSETTE_FILENAME"] = null;
            atariCfg["CASSETTE_LOADED"] = "0";
            atariCfg["CARTRIDGE_FILENAME"] = null;
            atariCfg["CARTRIDGE_TYPE"] = "0";

            if (atari800)
            {
                atariCfg["MACHINE_TYPE"] = "Atari XL/XE";
                atariCfg["RAM_SIZE"] = "64";
                atariCfg["DISABLE_BASIC"] = "0";
            }
            else if (atariXE)
            {
                atariCfg["MACHINE_TYPE"] = "Atari XL/XE";
                atariCfg["RAM_SIZE"] = "128";
                atariCfg["DISABLE_BASIC"] = "1";

                var rom = Program.SystemConfig["rom"];
                if (File.Exists(rom))
                {
                    atariCfg["CARTRIDGE_FILENAME"] = rom;

                    try
                    {
                        var ln = new FileInfo(rom).Length;
                        if (ln == 131072)
                            atariCfg["CARTRIDGE_TYPE"] = "14";
                        else if (ln == 65536)
                            atariCfg["CARTRIDGE_TYPE"] = "13";
                    }
                    catch { }
                }
            }            
            else // Atari 5200
            {
                atariCfg["ROM_OS_A_PAL"] = "";
                atariCfg["ROM_OS_BB01R2"] = "";
                atariCfg["ROM_BASIC_C"] = "";
                atariCfg["ROM_400/800_CUSTOM"] = "";

                atariCfg["MACHINE_TYPE"] = "Atari 5200";
                atariCfg["RAM_SIZE"] = "16";
                atariCfg["DISABLE_BASIC"] = "1";
            }

            atariCfg.Save(Path.Combine(RetroarchPath, ".atari800.cfg"), false);
        }

        private void ConfigureVirtualJaguar(ConfigFile retroarchConfig, ConfigFile coreSettings, string system, string core)
        {
            if (core != "virtualjaguar")
                return;

            coreSettings["virtualjaguar_usefastblitter"] = "enabled";
        }

        private void ConfigureSNes9x(ConfigFile retroarchConfig, ConfigFile coreSettings, string system, string core)
        {
            if (core != "snes9x")
                return;
            
            coreSettings["snes9x_show_advanced_av_settings"] = "enabled";

            coreSettings["snes9x_blargg"] = SystemConfig.GetValueOrDefault("snes9x_blargg", "disabled"); // Emulated video signal
            coreSettings["snes9x_overscan"] = SystemConfig.GetValueOrDefault("snes9x_overscan", "auto"); // Overscan
            coreSettings["snes9x_region"] = SystemConfig.GetValueOrDefault("snes9x_region", "auto"); // Region
            coreSettings["snes9x_gfx_hires"] = SystemConfig.GetValueOrDefault("snes9x_gfx_hires", "disabled"); // Internal resolution
            coreSettings["snes9x_hires_blend"] = SystemConfig.GetValueOrDefault("snes9x_hires_blend", "disabled"); // Pixel blending
            coreSettings["snes9x_audio_interpolation"] = SystemConfig.GetValueOrDefault("snes9x_audio_interpolation", "none"); // Audio interpolation
            coreSettings["snes9x_overclock_superfx"] = SystemConfig.GetValueOrDefault("snes9x_overclock_superfx", "100%"); // SuperFX overclock
            coreSettings["snes9x_block_invalid_vram_access"] = SystemConfig.GetValueOrDefault("snes9x_block_invalid_vram_access", "enabled"); // Block invalid VRAM access
                        
            // Unsafe hacks (config must be done in Core options)
            if (SystemConfig.isOptSet("SnesUnsafeHacks") && SystemConfig["SnesUnsafeHacks"] == "config")
            {
                coreSettings["snes9x_echo_buffer_hack"] = "enabled";
                coreSettings["snes9x_overclock_cycles"] = "enabled";
                coreSettings["snes9x_randomize_memory"] = "enabled";
                coreSettings["snes9x_reduce_sprite_flicker"] = "enabled";
            }
            else
            {
                coreSettings["snes9x_echo_buffer_hack"] = "disabled";
                coreSettings["snes9x_overclock_cycles"] = "disabled";
                coreSettings["snes9x_randomize_memory"] = "disabled";
                coreSettings["snes9x_reduce_sprite_flicker"] = "disabled";
            }
            
            // Advanced video options (config must be done in Core options menu)
            if (SystemConfig.isOptSet("SnesAdvancedVideoOptions") && SystemConfig["SnesAdvancedVideoOptions"] == "config")
            {
                coreSettings["snes9x_layer_1"] = "disabled";
                coreSettings["snes9x_layer_2"] = "disabled";
                coreSettings["snes9x_layer_3"] = "disabled";
                coreSettings["snes9x_layer_4"] = "disabled";
                coreSettings["snes9x_layer_5"] = "disabled";
                coreSettings["snes9x_gfx_clip"] = "disabled";
                coreSettings["snes9x_gfx_transp"] = "disabled";
            }
            else
            {
                coreSettings["snes9x_layer_1"] = "enabled";
                coreSettings["snes9x_layer_2"] = "enabled";
                coreSettings["snes9x_layer_3"] = "enabled";
                coreSettings["snes9x_layer_4"] = "enabled";
                coreSettings["snes9x_layer_5"] = "enabled";
                coreSettings["snes9x_gfx_clip"] =  "enabled";
                coreSettings["snes9x_gfx_transp"] =  "enabled";
            }
            
            // Advanced audio options (config must be done in Core options menu)
            if (SystemConfig.isOptSet("SnesAdvancedAudioOptions") && SystemConfig["SnesAdvancedAudioOptions"] == "config")
            {
                coreSettings["snes9x_sndchan_1"] = "disabled";
                coreSettings["snes9x_sndchan_2"] = "disabled";
                coreSettings["snes9x_sndchan_3"] = "disabled";
                coreSettings["snes9x_sndchan_4"] = "disabled";
                coreSettings["snes9x_sndchan_5"] = "disabled";
                coreSettings["snes9x_sndchan_6"] = "disabled";
                coreSettings["snes9x_sndchan_7"] = "disabled";
                coreSettings["snes9x_sndchan_8"] = "disabled";
            }
            else
            {
                coreSettings["snes9x_sndchan_1"] = "enabled";
                coreSettings["snes9x_sndchan_2"] = "enabled";
                coreSettings["snes9x_sndchan_3"] = "enabled";
                coreSettings["snes9x_sndchan_4"] = "enabled";
                coreSettings["snes9x_sndchan_5"] = "enabled";
                coreSettings["snes9x_sndchan_6"] = "enabled";
                coreSettings["snes9x_sndchan_7"] = "enabled";
                coreSettings["snes9x_sndchan_8"] = "enabled";
            }

            coreSettings["snes9x_show_lightgun_settings"] = "enabled";
            coreSettings["snes9x_lightgun_mode"] = SystemConfig.GetValueOrDefault("snes9x_lightgun_mode", "Lightgun"); // Lightgun mode

            if (SystemConfig.isOptSet("use_guns") && SystemConfig.getOptBoolean("use_guns"))
            {
                retroarchConfig["input_libretro_device_p1"] = "260";
                retroarchConfig["input_player2_mouse_index"] = "0";
                retroarchConfig["input_player2_gun_trigger_mbtn"] = "1";
            }
        }

        private void ConfigureGenesisPlusGX(ConfigFile retroarchConfig, ConfigFile coreSettings, string system, string core)
        {
            if (core != "genesis_plus_gx")
                return;

            coreSettings["genesis_plus_gx_bram"] = "per game";
            coreSettings["genesis_plus_gx_ym2413"] = "auto";

            coreSettings["genesis_plus_gx_addr_error"] = SystemConfig.GetValueOrDefault("addr_error", "enabled");
            coreSettings["genesis_plus_gx_lock_on"] = SystemConfig.GetValueOrDefault("lock_on", "disabled");
            coreSettings["genesis_plus_gx_ym2612"] = SystemConfig.GetValueOrDefault("ym2612", "mame (ym2612)");
            coreSettings["genesis_plus_gx_audio_filter"] = SystemConfig.GetValueOrDefault("audio_filter", "disabled");
            coreSettings["genesis_plus_gx_blargg_ntsc_filter"] = SystemConfig.GetValueOrDefault("ntsc_filter", "disabled");
            coreSettings["genesis_plus_gx_lcd_filter"] = SystemConfig.GetValueOrDefault("lcd_filter", "lcd_filter");
            coreSettings["genesis_plus_gx_overscan"] = SystemConfig.GetValueOrDefault("overscan", "disabled");
            coreSettings["genesis_plus_gx_render"] = SystemConfig.GetValueOrDefault("render", "single field");
            coreSettings["genesis_plus_gx_force_dtack"] = SystemConfig.GetValueOrDefault("genesis_plus_gx_force_dtack", "enabled");
            coreSettings["genesis_plus_gx_overclock"] = SystemConfig.GetValueOrDefault("genesis_plus_gx_overclock", "100%");
            coreSettings["genesis_plus_gx_no_sprite_limit"] = SystemConfig.GetValueOrDefault("genesis_plus_gx_no_sprite_limit", "disabled");
            coreSettings["genesis_plus_gx_bios"] = SystemConfig.GetValueOrDefault("genesis_plus_gx_bios", "disabled");
            coreSettings["genesis_plus_gx_add_on"] = SystemConfig.GetValueOrDefault("genesis_plus_gx_add_on", "auto");

            coreSettings["genesis_plus_gx_gun_cursor"] = SystemConfig.GetValueOrDefault("gun_cursor", "enabled");
            coreSettings["genesis_plus_gx_gun_input"] = SystemConfig.GetValueOrDefault("gun_input", "lightgun");

            if (SystemConfig.isOptSet("use_guns") && SystemConfig.getOptBoolean("use_guns"))
            {
                retroarchConfig["input_libretro_device_p1"] = "260";
                retroarchConfig["input_player2_mouse_index"] = "0";
                retroarchConfig["input_player2_gun_trigger_mbtn"] = "1";
            }
        }
        
        private void ConfigureGenesisPlusGXWide(ConfigFile retroarchConfig, ConfigFile coreSettings, string system, string core)
        {
            if (core != "genesis_plus_gx_wide")
                return;
            
            if (SystemConfig.isOptSet("ratio") && !SystemConfig.isOptSet("bezel"))
            {
                int idx = ratioIndexes.IndexOf(SystemConfig["ratio"]);
                if (idx == 1 || idx == 2 || idx == 4 || idx == 6 || idx == 7 || idx == 9 || idx == 14 || idx == 16 || idx == 18 || idx == 19)
                {
                    retroarchConfig["aspect_ratio_index"] = idx.ToString();
                    retroarchConfig["video_aspect_ratio_auto"] = "false";
                    SystemConfig["bezel"] = "none";
                }
            }
            else
            {
                retroarchConfig["aspect_ratio_index"] = "1";
                retroarchConfig["video_aspect_ratio_auto"] = "false";
                SystemConfig["bezel"] = "none";
            }

            coreSettings["genesis_plus_gx_wide_bram"] = "per game";
            coreSettings["genesis_plus_gx_wide_ym2413"] = "auto";
            coreSettings["genesis_plus_gx_wide_overscan"] = "disabled";

            coreSettings["genesis_plus_gx_wide_addr_error"] = SystemConfig.GetValueOrDefault("addr_error", "enabled");
            coreSettings["genesis_plus_gx_wide_lock_on"] = SystemConfig.GetValueOrDefault("lock_on", "disabled");
            coreSettings["genesis_plus_gx_wide_ym2612"] = SystemConfig.GetValueOrDefault("ym2612", "mame (ym2612)");
            coreSettings["genesis_plus_gx_wide_audio_filter"] = SystemConfig.GetValueOrDefault("audio_filter", "disabled");
            coreSettings["genesis_plus_gx_wide_blargg_ntsc_filter"] = SystemConfig.GetValueOrDefault("ntsc_filter", "disabled");
            coreSettings["genesis_plus_gx_wide_lcd_filter"] = SystemConfig.GetValueOrDefault("lcd_filter", "disabled");
            coreSettings["genesis_plus_gx_wide_overscan"] = SystemConfig.GetValueOrDefault("overscan", "disabled");
            coreSettings["genesis_plus_gx_wide_render"] = SystemConfig.GetValueOrDefault("render", "single field");
            coreSettings["genesis_plus_gx_wide_force_dtack"] = SystemConfig.GetValueOrDefault("genesis_plus_gx_force_dtack", "enabled");
            coreSettings["genesis_plus_gx_wide_overclock"] = SystemConfig.GetValueOrDefault("genesis_plus_gx_overclock", "100%");
            coreSettings["genesis_plus_gx_wide_no_sprite_limit"] = SystemConfig.GetValueOrDefault("genesis_plus_gx_no_sprite_limit", "disabled");            
            coreSettings["genesis_plus_gx_wide_bios"] = SystemConfig.GetValueOrDefault("genesis_plus_gx_bios", "disabled");
            coreSettings["genesis_plus_gx_wide_add_on"] = SystemConfig.GetValueOrDefault("genesis_plus_gx_add_on", "auto");
            coreSettings["genesis_plus_gx_wide_h40_extra_columns"] = SystemConfig.GetValueOrDefault("h40_extra_columns", "10");

            coreSettings["genesis_plus_gx_wide_gun_cursor"] = SystemConfig.GetValueOrDefault("gun_cursor", "enabled");
            coreSettings["genesis_plus_gx_wide_gun_input"] = SystemConfig.GetValueOrDefault("gun_input", "lightgun");
            
            if (SystemConfig.isOptSet("use_guns") && SystemConfig.getOptBoolean("use_guns"))
            {
                retroarchConfig["input_libretro_device_p1"] = "260";
                retroarchConfig["input_player2_mouse_index"] = "0";
                retroarchConfig["input_player2_gun_trigger_mbtn"] = "1";
            }
        }
        
        private void ConfigureMame(ConfigFile retroarchConfig, ConfigFile coreSettings, string system, string core)
        {
            if (core != "mame")
                return;

            string softLists = "enabled";

            MessSystem messSystem = MessSystem.GetMessSystem(system);
            if (messSystem != null)
            {
                CleanupMameMessConfigFiles(messSystem);

                // If we have a know system name, disable softlists as we run with CLI
                if (!string.IsNullOrEmpty(messSystem.MachineName))
                    softLists = "disabled";
            }

            coreSettings["mame_softlists_enable"] = softLists;
            coreSettings["mame_softlists_auto_media"] = softLists; 

            coreSettings["mame_read_config"] = "enabled";
            coreSettings["mame_write_config"] = "enabled";
            coreSettings["mame_mouse_enable"] = "enabled";
            coreSettings["mame_mame_paths_enable"] = "disabled";
            coreSettings["mame_alternate_renderer"] = SystemConfig.GetValueOrDefault("alternate_renderer", "disabled");
            coreSettings["mame_altres"] = SystemConfig.GetValueOrDefault("internal_resolution", "640x480");
            coreSettings["mame_cheats_enable"] = SystemConfig.GetValueOrDefault("cheats_enable", "disabled");
            coreSettings["mame_mame_4way_enable"] = SystemConfig.GetValueOrDefault("4way_enable", "enabled");
            coreSettings["mame_lightgun_mode"] = SystemConfig.GetValueOrDefault("lightgun_mode", "lightgun");

            if (SystemConfig.isOptSet("boot_from_cli") && Features.IsSupported("boot_from_cli"))
                coreSettings["mame_boot_from_cli"] = SystemConfig["boot_from_cli"];
            else
                coreSettings["mame_boot_from_cli"] = "enabled";

            if (SystemConfig.isOptSet("boot_to_bios") && Features.IsSupported("boot_to_bios"))
                coreSettings["mame_boot_to_bios"] = SystemConfig["boot_to_bios"];
            else
                coreSettings["mame_boot_to_bios"] = "disabled";

            if (SystemConfig.isOptSet("boot_to_osd") && Features.IsSupported("boot_to_osd"))
                coreSettings["mame_boot_to_osd"] = SystemConfig["boot_to_osd"];
            else
                coreSettings["mame_boot_to_osd"] = "disabled";
        }

        private void CleanupMameMessConfigFiles(MessSystem messSystem)
        {
            try
            {
                // Remove image_directories node in cfg file
                string cfgPath = Path.Combine(AppConfig.GetFullPath("bios"), "mame", "cfg", messSystem.MachineName + ".cfg");
                if (File.Exists(cfgPath))
                {
                    XDocument xml = XDocument.Load(cfgPath);

                    var image_directories = xml.Descendants().FirstOrDefault(d => d.Name == "image_directories");
                    if (image_directories != null)
                    {
                        image_directories.Remove();
                        xml.Save(cfgPath);
                    }
                }
            }
            catch { }

            try 
            {
                // Remove medias declared in ini file
                string iniPath = Path.Combine(AppConfig.GetFullPath("bios"), "mame", "ini", messSystem.MachineName + ".ini");
                if (File.Exists(iniPath))
                {
                    var lines = File.ReadAllLines(iniPath);
                    var newLines = lines.Where(l =>
                        !l.StartsWith("cartridge") && !l.StartsWith("floppydisk") &&
                        !l.StartsWith("cassette") && !l.StartsWith("cdrom") &&
                        !l.StartsWith("romimage") && !l.StartsWith("memcard") &&
                        !l.StartsWith("quickload") && !l.StartsWith("harddisk") &&
                        !l.StartsWith("autoboot_command") && !l.StartsWith("autoboot_delay") && !l.StartsWith("autoboot_script") &&
                        !l.StartsWith("printout")
                        ).ToArray();

                    if (lines.Length != newLines.Length)
                        File.WriteAllLines(iniPath, newLines);
                }
            }
            catch { }
        }

        private void ConfigurePotator(ConfigFile retroarchConfig, ConfigFile coreSettings, string system, string core)
        {
            if (core != "potator")
                return;

            coreSettings["potator_lcd_ghosting"] = SystemConfig.GetValueOrDefault("lcd_ghosting", "0");
            coreSettings["potator_palette"] = SystemConfig.GetValueOrDefault("palette", "default");
        }

        private void ConfigureMupen64(ConfigFile retroarchConfig, ConfigFile coreSettings, string system, string core)
        {
            if (core != "mupen64plus_next" && core != "mupen64plus_next_gles3")
                return;

            //coreSettings["mupen64plus-cpucore"] = "pure_interpreter";
            coreSettings["mupen64plus-rsp-plugin"] = "hle";
            coreSettings["mupen64plus-EnableLODEmulation"] = "True";
            coreSettings["mupen64plus-EnableCopyAuxToRDRAM"] = "True";
            coreSettings["mupen64plus-EnableHWLighting"] = "True";
            coreSettings["mupen64plus-txHiresFullAlphaChannel"] = "True";
            coreSettings["mupen64plus-GLideN64IniBehaviour"] = "early";
            coreSettings["mupen64plus-parallel-rdp-native-tex-rect"] = "True";
            coreSettings["mupen64plus-parallel-rdp-synchronous"] = "True";

            coreSettings["mupen64plus-cpucore"] = SystemConfig.GetValueOrDefault("mupen64plus-cpucore", "pure_interpreter"); // CPU core
            coreSettings["mupen64plus-rdp-plugin"] = SystemConfig.GetValueOrDefault("RDP_Plugin", "gliden64"); // Plugin selection           
            
            // Set RSP plugin: HLE for Glide, LLE for Parallel
            if (SystemConfig.isOptSet("RDP_Plugin") && coreSettings["mupen64plus-rdp-plugin"] == "parallel")
                coreSettings["mupen64plus-rsp-plugin"] = "parallel";
            else
                coreSettings["mupen64plus-rsp-plugin"] = "hle";
            
            // Overscan (Glide)
            if (SystemConfig.isOptSet("CropOverscan") && SystemConfig.getOptBoolean("CropOverscan"))
            {
                coreSettings["mupen64plus-OverscanBottom"] = "0";
                coreSettings["mupen64plus-OverscanLeft"] = "0";
                coreSettings["mupen64plus-OverscanRight"] = "0";
                coreSettings["mupen64plus-OverscanTop"] = "0";
            }
            else
            {
                coreSettings["mupen64plus-OverscanBottom"] = "15";
                coreSettings["mupen64plus-OverscanLeft"] = "18";
                coreSettings["mupen64plus-OverscanRight"] = "13";
                coreSettings["mupen64plus-OverscanTop"] = "12";
            }

            // Performance presets
            if (SystemConfig.isOptSet("PerformanceMode") && SystemConfig.getOptBoolean("PerformanceMode"))
            {
                coreSettings["mupen64plus-EnableCopyColorToRDRAM"] = "Off";
                coreSettings["mupen64plus-EnableCopyDepthToRDRAM"] = "Off";
                coreSettings["mupen64plus-EnableFBEmulation"] = "False";
                coreSettings["mupen64plus-ThreadedRenderer"] = "False";
                coreSettings["mupen64plus-HybridFilter"] = "False";
                coreSettings["mupen64plus-BackgroundMode"] = "OnePiece";
                coreSettings["mupen64plus-EnableLegacyBlending"] = "True";
                coreSettings["mupen64plus-txFilterIgnoreBG"] = "True";
            }
            else
            {
                coreSettings["mupen64plus-EnableCopyColorToRDRAM"] = "TripleBuffer";
                coreSettings["mupen64plus-EnableCopyDepthToRDRAM"] = "Software";
                coreSettings["mupen64plus-EnableFBEmulation"] = "True";
                coreSettings["mupen64plus-ThreadedRenderer"] = "True";
                coreSettings["mupen64plus-HybridFilter"] = "True";
                coreSettings["mupen64plus-BackgroundMode"] = "Stripped";
                coreSettings["mupen64plus-EnableLegacyBlending"] = "False";
                coreSettings["mupen64plus-txFilterIgnoreBG"] = "False";

            }

            // Hi Res textures methods
            if (SystemConfig.isOptSet("TexturesPack"))
            {
                if (SystemConfig["TexturesPack"] == "legacy")
                {
                    coreSettings["mupen64plus-EnableTextureCache"] = "True";
                    coreSettings["mupen64plus-txHiresEnable"] = "True";
                    coreSettings["mupen64plus-txCacheCompression"] = "True";
                    coreSettings["mupen64plus-txHiresFullAlphaChannel"] = "False";
                    coreSettings["mupen64plus-EnableEnhancedTextureStorage"] = "False";
                    coreSettings["mupen64plus-EnableEnhancedHighResStorage"] = "False";
                }
                else if (SystemConfig["TexturesPack"] == "cache")
                {
                    coreSettings["mupen64plus-EnableTextureCache"] = "True";
                    coreSettings["mupen64plus-txHiresEnable"] = "True";
                    coreSettings["mupen64plus-txCacheCompression"] = "True";
                    coreSettings["mupen64plus-txHiresFullAlphaChannel"] = "True";
                    coreSettings["mupen64plus-EnableEnhancedTextureStorage"] = "True";
                    coreSettings["mupen64plus-EnableEnhancedHighResStorage"] = "True";
                }
            }
            else
            {
                coreSettings["mupen64plus-EnableTextureCache"] = "False";
                coreSettings["mupen64plus-txHiresEnable"] = "False";
                coreSettings["mupen64plus-txCacheCompression"] = "False";
                coreSettings["mupen64plus-txHiresFullAlphaChannel"] = "False";
                coreSettings["mupen64plus-EnableEnhancedTextureStorage"] = "False";
                coreSettings["mupen64plus-EnableEnhancedHighResStorage"] = "False";
            }

            // Texture Enhancement (Glide)
            if (SystemConfig.isOptSet("Texture_Enhancement"))
                coreSettings["mupen64plus-txEnhancementMode"] = SystemConfig["Texture_Enhancement"];
            else
                coreSettings["mupen64plus-txEnhancementMode"] = "As Is";

            // Widescreen (Glide)
            if (SystemConfig.isOptSet("Widescreen") && SystemConfig.getOptBoolean("Widescreen"))
            {
                coreSettings["mupen64plus-aspect"] = "16:9 adjusted";
                retroarchConfig["aspect_ratio_index"] = "1";
                SystemConfig["bezel"] = "none";
            }
            else
                coreSettings["mupen64plus-aspect"] = "4/3";

            // 4:3 resolution (Glide)
            if (SystemConfig.isOptSet("43screensize"))
                coreSettings["mupen64plus-43screensize"] = SystemConfig["43screensize"];
            else
                coreSettings["mupen64plus-43screensize"] = "640x480";

            // 16:9 resolution (Glide)
            if (SystemConfig.isOptSet("169screensize"))
                coreSettings["mupen64plus-169screensize"] = SystemConfig["169screensize"];
            else
                coreSettings["mupen64plus-169screensize"] = "960x540";

            // BilinearMode (Glide)
            if (SystemConfig.isOptSet("BilinearMode"))
                coreSettings["mupen64plus-BilinearMode"] = SystemConfig["BilinearMode"];
            else
                coreSettings["mupen64plus-BilinearMode"] = "3point";

            // Multisampling aa (Glide)
            if (SystemConfig.isOptSet("MultiSampling"))
                coreSettings["mupen64plus-MultiSampling"] = SystemConfig["MultiSampling"];
            else
                coreSettings["mupen64plus-MultiSampling"] = "0";

            // Texture filter (Glide)
            if (SystemConfig.isOptSet("Texture_filter"))
                coreSettings["mupen64plus-txFilterMode"] = SystemConfig["Texture_filter"];
            else
                coreSettings["mupen64plus-txFilterMode"] = "None";

            // Player 1 pack
            if (SystemConfig.isOptSet("mupen64plus-pak1"))
                coreSettings["mupen64plus-pak1"] = SystemConfig["mupen64plus-pak1"];
            else
                coreSettings["mupen64plus-pak1"] = "memory";

            // Player 2 pack
            if (SystemConfig.isOptSet("mupen64plus-pak2"))
                coreSettings["mupen64plus-pak2"] = SystemConfig["mupen64plus-pak2"];
            else
                coreSettings["mupen64plus-pak2"] = "none";

            // Player 3 pack
            if (SystemConfig.isOptSet("mupen64plus-pak3"))
                coreSettings["mupen64plus-pak3"] = SystemConfig["mupen64plus-pak3"];
            else
                coreSettings["mupen64plus-pak3"] = "none";

            // Player 4 pack
            if (SystemConfig.isOptSet("mupen64plus-pak4"))
                coreSettings["mupen64plus-pak4"] = SystemConfig["mupen64plus-pak4"];
            else
                coreSettings["mupen64plus-pak4"] = "none";
            
            // Deinterlacing method (Parallel)
            if (SystemConfig.isOptSet("mupen64plus-parallel-rdp-deinterlace-method"))
                coreSettings["mupen64plus-parallel-rdp-deinterlace-method"] = SystemConfig["mupen64plus-parallel-rdp-deinterlace-method"];
            else
                coreSettings["mupen64plus-parallel-rdp-deinterlace-method"] = "none";
            
            // Dithering filter (Parallel)
            if (SystemConfig.isOptSet("mupen64plus-parallel-rdp-dither-filter"))
                coreSettings["mupen64plus-parallel-rdp-dither-filter"] = SystemConfig["mupen64plus-parallel-rdp-dither-filter"];
            else
                coreSettings["mupen64plus-parallel-rdp-dither-filter"] = "True";
            
            // Divot filter (Parallel)
            if (SystemConfig.isOptSet("mupen64plus-parallel-rdp-divot-filter"))
                coreSettings["mupen64plus-parallel-rdp-divot-filter"] = SystemConfig["mupen64plus-parallel-rdp-divot-filter"];
            else
                coreSettings["mupen64plus-parallel-rdp-divot-filter"] = "True";
            
            // Downscaling (Parallel)
            if (SystemConfig.isOptSet("mupen64plus-parallel-rdp-downscaling"))
                coreSettings["mupen64plus-parallel-rdp-downscaling"] = SystemConfig["mupen64plus-parallel-rdp-downscaling"];
            else
                coreSettings["mupen64plus-parallel-rdp-downscaling"] = "disable";
            
            // Gamma dither (Parallel)
            if (SystemConfig.isOptSet("mupen64plus-parallel-rdp-gamma-dither"))
                coreSettings["mupen64plus-parallel-rdp-gamma-dither"] = SystemConfig["mupen64plus-parallel-rdp-gamma-dither"];
            else
                coreSettings["mupen64plus-parallel-rdp-gamma-dither"] = "disable";
            
            // Native texture LOD (Parallel)
            if (SystemConfig.isOptSet("mupen64plus-parallel-rdp-native-texture-lod"))
                coreSettings["mupen64plus-parallel-rdp-native-texture-lod"] = SystemConfig["mupen64plus-parallel-rdp-native-texture-lod"];
            else
                coreSettings["mupen64plus-parallel-rdp-native-texture-lod"] = "False";
            
            // Overscan (Parallel)
            if (SystemConfig.isOptSet("mupen64plus-parallel-rdp-overscan"))
                coreSettings["mupen64plus-parallel-rdp-overscan"] = SystemConfig["mupen64plus-parallel-rdp-overscan"];
            else
                coreSettings["mupen64plus-parallel-rdp-overscan"] = "16";
            
            // SSA framebuffer effects (Parallel)
            if (SystemConfig.isOptSet("mupen64plus-parallel-rdp-super-sampled-read-back"))
                coreSettings["mupen64plus-parallel-rdp-super-sampled-read-back"] = SystemConfig["mupen64plus-parallel-rdp-super-sampled-read-back"];
            else
                coreSettings["mupen64plus-parallel-rdp-super-sampled-read-back"] = "False";
            
            // Dither SSA framebuffer effects (Parallel)
            if (SystemConfig.isOptSet("mupen64plus-parallel-rdp-super-sampled-read-back-dither"))
                coreSettings["mupen64plus-parallel-rdp-super-sampled-read-back-dither"] = SystemConfig["mupen64plus-parallel-rdp-super-sampled-read-back-dither"];
            else
                coreSettings["mupen64plus-parallel-rdp-super-sampled-read-back-dither"] = "False";
            
            // Textures upscaling (Parallel)
            if (SystemConfig.isOptSet("mupen64plus-parallel-rdp-upscaling"))
                coreSettings["mupen64plus-parallel-rdp-upscaling"] = SystemConfig["mupen64plus-parallel-rdp-upscaling"];
            else
                coreSettings["mupen64plus-parallel-rdp-upscaling"] = "1x";
            
            // Anti aliasing (Parallel)
            if (SystemConfig.isOptSet("mupen64plus-parallel-rdp-vi-aa"))
                coreSettings["mupen64plus-parallel-rdp-vi-aa"] = SystemConfig["mupen64plus-parallel-rdp-vi-aa"];
            else
                coreSettings["mupen64plus-parallel-rdp-vi-aa"] = "False";
            
            // Bilinear scaling (Parallel)
            if (SystemConfig.isOptSet("mupen64plus-parallel-rdp-vi-bilinear"))
                coreSettings["mupen64plus-parallel-rdp-vi-bilinear"] = SystemConfig["mupen64plus-parallel-rdp-vi-bilinear"];
            else
                coreSettings["mupen64plus-parallel-rdp-vi-bilinear"] = "False";
            
            
        }

        private void ConfigureDosboxPure(ConfigFile retroarchConfig, ConfigFile coreSettings, string system, string core)
        {
            if (core != "dosbox_pure")
                return;

            coreSettings["dosbox_pure_advanced"] = "true";
            coreSettings["dosbox_pure_auto_mapping"] = "true";
            coreSettings["dosbox_pure_bind_unused"] = "true";
            coreSettings["dosbox_pure_savestate"] = "on";

            if (SystemConfig.isOptSet("ratio"))
                coreSettings["dosbox_pure_aspect_correction"] = SystemConfig["ratio"];
            else
                coreSettings["dosbox_pure_aspect_correction"] = "true";

            if (SystemConfig.isOptSet("cga"))
                coreSettings["dosbox_pure_cga"] = SystemConfig["cga"];
            else
                coreSettings["dosbox_pure_cga"] = "early_auto";

            if (SystemConfig.isOptSet("cpu_core"))
                coreSettings["dosbox_pure_cpu_core"] = SystemConfig["cpu_core"];
            else
                coreSettings["dosbox_pure_cpu_core"] = "auto";

            if (SystemConfig.isOptSet("cpu_type"))
                coreSettings["dosbox_pure_cpu_type"] = SystemConfig["cpu_type"];
            else
                coreSettings["dosbox_pure_cpu_type"] = "auto";

            if (SystemConfig.isOptSet("cycles"))
                coreSettings["dosbox_pure_cycles"] = SystemConfig["cycles"];
            else
                coreSettings["dosbox_pure_cycles"] = "auto";

            if (SystemConfig.isOptSet("gus"))
                coreSettings["dosbox_pure_gus"] = SystemConfig["gus"];
            else
                coreSettings["dosbox_pure_gus"] = "false";

            if (SystemConfig.isOptSet("hercules"))
                coreSettings["dosbox_pure_hercules"] = SystemConfig["hercules"];
            else
                coreSettings["dosbox_pure_hercules"] = "white";

            if (SystemConfig.isOptSet("machine"))
                coreSettings["dosbox_pure_machine"] = SystemConfig["machine"];
            else
                coreSettings["dosbox_pure_machine"] = "svga";

            if (SystemConfig.isOptSet("memory_size"))
                coreSettings["dosbox_pure_memory_size"] = SystemConfig["memory_size"];
            else
                coreSettings["dosbox_pure_memory_size"] = "16";

            if (SystemConfig.isOptSet("menu_time"))
                coreSettings["dosbox_pure_menu_time"] = SystemConfig["menu_time"];
            else
                coreSettings["dosbox_pure_menu_time"] = "5";

            if (SystemConfig.isOptSet("midi"))
                coreSettings["dosbox_pure_midi"] = SystemConfig["midi"];
            else
                coreSettings["dosbox_pure_midi"] = "scummvm/extra/Roland_SC-55.sf2";

            if (SystemConfig.isOptSet("on_screen_keyboard"))
                coreSettings["dosbox_pure_on_screen_keyboard"] = SystemConfig["on_screen_keyboard"];
            else
                coreSettings["dosbox_pure_on_screen_keyboard"] = "true";

            if (SystemConfig.isOptSet("sblaster_adlib_emu"))
                coreSettings["dosbox_pure_sblaster_adlib_emu"] = SystemConfig["sblaster_adlib_emu"];
            else
                coreSettings["dosbox_pure_sblaster_adlib_emu"] = "default";

            if (SystemConfig.isOptSet("sblaster_adlib_mode"))
                coreSettings["dosbox_pure_sblaster_adlib_mode"] = SystemConfig["sblaster_adlib_mode"];
            else
                coreSettings["dosbox_pure_sblaster_adlib_mode"] = "auto";

            if (SystemConfig.isOptSet("sblaster_conf"))
                coreSettings["dosbox_pure_sblaster_conf"] = SystemConfig["sblaster_conf"];
            else
                coreSettings["dosbox_pure_sblaster_conf"] = "A220 I7 D1 H5";

            if (SystemConfig.isOptSet("sblaster_type"))
                coreSettings["dosbox_pure_sblaster_type"] = SystemConfig["sblaster_type"];
            else
                coreSettings["dosbox_pure_sblaster_type"] = "sb16";

            if (SystemConfig.isOptSet("svga"))
                coreSettings["dosbox_pure_svga"] = SystemConfig["svga"];
            else
                coreSettings["dosbox_pure_svga"] = "vesa_nolfb";

            if (SystemConfig.isOptSet("keyboard_layout"))
                coreSettings["dosbox_pure_keyboard_layout"] = SystemConfig["keyboard_layout"];
            else
                coreSettings["dosbox_pure_keyboard_layout"] = "us";

        }

        private void ConfigurePuae(ConfigFile retroarchConfig, ConfigFile coreSettings, string system, string core)
        {
            if (core != "puae")
                return;

            if (SystemConfig.isOptSet("config_source") && SystemConfig.getOptBoolean("config_source") || (!SystemConfig.isOptSet("config_source")))
            {
                coreSettings["puae_video_options_display"] = "enabled";
                coreSettings["puae_use_whdload"] = "hdfs";

                // model
                if (SystemConfig.isOptSet("model"))
                    coreSettings["puae_model"] = SystemConfig["model"];
                else
                    coreSettings["puae_model"] = "auto";

                // cpu compatibility
                if (SystemConfig.isOptSet("cpu_compatibility"))
                    coreSettings["puae_cpu_compatibility"] = SystemConfig["cpu_compatibility"];
                else
                    coreSettings["puae_cpu_compatibility"] = "normal";

                // cpu multiplier
                if (SystemConfig.isOptSet("cpu_multiplier"))
                    coreSettings["puae_cpu_multiplier"] = SystemConfig["cpu_multiplier"];
                else
                    coreSettings["puae_cpu_multiplier"] = "default";

                // video resolution
                if (SystemConfig.isOptSet("video_resolution"))
                    coreSettings["puae_video_resolution"] = SystemConfig["video_resolution"];
                else
                    coreSettings["puae_video_resolution"] = "auto";

                // zoom_mode
                if (SystemConfig.isOptSet("zoom_mode"))
                    coreSettings["puae_zoom_mode"] = SystemConfig["zoom_mode"];
                else
                    coreSettings["puae_zoom_mode"] = "auto";

                // video_standard
                if (SystemConfig.isOptSet("video_standard"))
                    coreSettings["puae_video_standard"] = SystemConfig["video_standard"];
                else
                    coreSettings["puae_video_standard"] = "PAL auto";

                // whdload
                if (SystemConfig.isOptSet("whdload"))
                    coreSettings["puae_use_whdload_prefs"] = SystemConfig["whdload"];
                else
                    coreSettings["puae_use_whdload_prefs"] = "config";

                // Jump on B
                if (SystemConfig.isOptSet("pad_options"))
                    coreSettings["puae_retropad_options"] = SystemConfig["pad_options"];
                else
                    coreSettings["puae_retropad_options"] = "jump";

                // floppy speed
                if (SystemConfig.isOptSet("floppy_speed"))
                    coreSettings["puae_floppy_speed"] = SystemConfig["floppy_speed"];
                else
                    coreSettings["puae_floppy_speed"] = "100";

                // floppy sound
                if (SystemConfig.isOptSet("floppy_sound"))
                    coreSettings["puae_floppy_sound"] = SystemConfig["floppy_sound"];
                else
                    coreSettings["puae_floppy_sound"] = "75";
                
                // Kickstart
                if (SystemConfig.isOptSet("puae_kickstart"))
                    coreSettings["puae_kickstart"] = SystemConfig["puae_kickstart"];
                else
                    coreSettings["puae_kickstart"] = "auto";

            }
        }

        private void ConfigureFlycast(ConfigFile retroarchConfig, ConfigFile coreSettings, string system, string core)
        {
            if (core != "flycast")
                return;

            if (SystemConfig.isOptSet("config_source") && SystemConfig.getOptBoolean("config_source") || (!SystemConfig.isOptSet("config_source")))
            {
                coreSettings["reicast_threaded_rendering"] = "enabled";

                // widescreen hack
                /* if (SystemConfig.isOptSet("widescreen_hack"))
				{
					coreSettings["reicast_widescreen_hack"] = SystemConfig["widescreen_hack"];

					if (SystemConfig["widescreen_hack"] == "enabled" && !SystemConfig.isOptSet("ratio"))
					{
						int idx = ratioIndexes.IndexOf("16/9");
						if (idx >= 0)
						{
							retroarchConfig["aspect_ratio_index"] = idx.ToString();
							retroarchConfig["video_aspect_ratio_auto"] = "false";
							SystemConfig["bezel"] = "none";
							coreSettings["reicast_widescreen_cheats"] = "enabled";
						}
					}
				}
				else
				{    
					coreSettings["reicast_widescreen_cheats"] = "disabled";
					coreSettings["reicast_widescreen_hack"] = "disabled";
				}
				*/
                if (SystemConfig.isOptSet("widescreen_hack"))
                {
                    coreSettings["reicast_widescreen_hack"] = SystemConfig["widescreen_hack"];
                    if (SystemConfig["widescreen_hack"] == "enabled")
                    {
                        retroarchConfig["aspect_ratio_index"] = "1";
                        SystemConfig["bezel"] = "none";
                    }
                }
                else
                {
                    coreSettings["reicast_widescreen_hack"] = "disabled";
                    coreSettings["reicast_widescreen_cheats"] = "disabled";
                }

                // anisotropic filtering
                if (SystemConfig.isOptSet("anisotropic_filtering"))
                    coreSettings["reicast_anisotropic_filtering"] = SystemConfig["anisotropic_filtering"];
                else
                    coreSettings["reicast_anisotropic_filtering"] = "off";

                // texture upscaling (xBRZ)
                if (SystemConfig.isOptSet("texture_upscaling"))
                    coreSettings["reicast_texupscale"] = SystemConfig["texture_upscaling"];
                else
                    coreSettings["reicast_texupscale"] = "off";

                // render to texture upscaling
                if (SystemConfig.isOptSet("render_to_texture_upscaling"))
                    coreSettings["reicast_render_to_texture_upscaling"] = SystemConfig["render_to_texture_upscaling"];
                else
                    coreSettings["reicast_render_to_texture_upscaling"] = "1x";

                // force wince game compatibility
                if (SystemConfig.isOptSet("force_wince"))
                    coreSettings["reicast_force_wince"] = SystemConfig["force_wince"];
                else
                    coreSettings["reicast_force_wince"] = "disabled";

                // cable type
                if (SystemConfig.isOptSet("cable_type"))
                    coreSettings["reicast_cable_type"] = SystemConfig["cable_type"];
                else
                    coreSettings["reicast_cable_type"] = "VGA (RGB)";

                // internal resolution
                if (SystemConfig.isOptSet("internal_resolution"))
                    coreSettings["reicast_internal_resolution"] = SystemConfig["internal_resolution"];
                else
                    coreSettings["reicast_internal_resolution"] = "1280x960";
            }

            if (SystemConfig.isOptSet("use_guns") && SystemConfig.getOptBoolean("use_guns"))
            {
                retroarchConfig["input_libretro_device_p1"] = "4";
                retroarchConfig["input_player1_mouse_index"] = "0";
                retroarchConfig["input_player1_gun_trigger_mbtn"] = "1";
                retroarchConfig["input_player1_gun_offscreen_shot_mbtn"] = "2";
                retroarchConfig["input_player1_gun_start_mbtn"] = "3";
                retroarchConfig["input_player1_gun_start"] = "enter";
                retroarchConfig["input_player1_gun_select"] = "escape";
            }
        }

        private void ConfigureMesen(ConfigFile retroarchConfig, ConfigFile coreSettings, string system, string core)
        {
            if (core != "mesen")
                return;

            coreSettings["mesen_aspect_ratio"] = "Auto";

            if (SystemConfig.isOptSet("hd_packs"))
                coreSettings["mesen_hdpacks"] = SystemConfig["hd_packs"];
            else
                coreSettings["mesen_hdpacks"] = "disabled";

            if (SystemConfig.isOptSet("ntsc_filter"))
                coreSettings["mesen_ntsc_filter"] = SystemConfig["ntsc_filter"];
            else
                coreSettings["mesen_ntsc_filter"] = "Disabled";

            if (SystemConfig.isOptSet("palette"))
                coreSettings["mesen_palette"] = SystemConfig["palette"];
            else
                coreSettings["mesen_palette"] = "Default";

            if (SystemConfig.isOptSet("shift_buttons"))
                coreSettings["mesen_shift_buttons_clockwise"] = SystemConfig["shift_buttons"];
            else
                coreSettings["mesen_shift_buttons_clockwise"] = "disabled";

            if (SystemConfig.isOptSet("fake_stereo"))
                coreSettings["mesen_fake_stereo"] = SystemConfig["fake_stereo"];
            else
                coreSettings["mesen_fake_stereo"] = "disabled";

            if (SystemConfig.isOptSet("use_guns") && SystemConfig.getOptBoolean("use_guns"))
            {
                retroarchConfig["input_libretro_device_p2"] = "262";
                retroarchConfig["input_player2_mouse_index"] = "0";
                retroarchConfig["input_player2_gun_trigger_mbtn"] = "1";
            }
        }

        private void ConfigurePcsxRearmed(ConfigFile retroarchConfig, ConfigFile coreSettings, string system, string core)
        {
            if (core != "pcsx_rearmed")
                return;

            // video resolution
            if (SystemConfig.isOptSet("neon_enhancement") && SystemConfig["neon_enhancement"] != "disabled")
            {
                if (SystemConfig["neon_enhancement"] == "enabled")
                {
                    coreSettings["pcsx_rearmed_neon_enhancement_enable"] = "enabled";
                    coreSettings["pcsx_rearmed_neon_enhancement_no_main"] = "disabled";
                }
                else if (SystemConfig["neon_enhancement"] == "enabled_with_speedhack")
                {
                    coreSettings["pcsx_rearmed_neon_enhancement_enable"] = "enabled";
                    coreSettings["pcsx_rearmed_neon_enhancement_no_main"] = "enabled";
                }
            }
        }

        private void ConfigureMednafenPsxHW(ConfigFile retroarchConfig, ConfigFile coreSettings, string system, string core)
        {
            if (core != "mednafen_psx_hw")
                return;

            // coreSettings["beetle_psx_hw_skip_bios"] = "enabled";

            // video resolution
            if (SystemConfig.isOptSet("internal_resolution"))
                coreSettings["beetle_psx_hw_internal_resolution"] = SystemConfig["internal_resolution"];
            else
                coreSettings["beetle_psx_hw_internal_resolution"] = "1x(native)";

            // texture filtering
            if (SystemConfig.isOptSet("texture_filtering"))
                coreSettings["beetle_psx_hw_filter"] = SystemConfig["texture_filtering"];
            else
                coreSettings["beetle_psx_hw_filter"] = "nearest";

            // dithering pattern
            if (SystemConfig.isOptSet("dither_mode"))
                coreSettings["beetle_psx_hw_dither_mode"] = SystemConfig["dither_mode"];
            else
                coreSettings["beetle_psx_hw_dither_mode"] = "disabled";

            // anti aliasing
            if (SystemConfig.isOptSet("msaa"))
                coreSettings["beetle_psx_hw_msaa"] = SystemConfig["msaa"];
            else
                coreSettings["beetle_psx_hw_msaa"] = "1x";

            // force analog
            if (SystemConfig.isOptSet("analog_toggle"))
                coreSettings["beetle_psx_hw_analog_toggle"] = SystemConfig["analog_toggle"];
            else
                coreSettings["beetle_psx_hw_analog_toggle"] = "enabled";

            // widescreen
            if (SystemConfig.isOptSet("widescreen_hack"))
            {
                coreSettings["beetle_psx_hw_widescreen_hack"] = SystemConfig["widescreen_hack"];

                if (SystemConfig["widescreen_hack"] == "enabled")
                {
                    int idx = ratioIndexes.IndexOf(SystemConfig["ratio"]);
                    if (idx > 0)
                    {
                        retroarchConfig["aspect_ratio_index"] = idx.ToString();
                        retroarchConfig["video_aspect_ratio_auto"] = "false";
                        SystemConfig["bezel"] = "none";
                    }
                    else
                    {
                        retroarchConfig["aspect_ratio_index"] = "1";
                        retroarchConfig["video_aspect_ratio_auto"] = "false";
                        SystemConfig["bezel"] = "none";
                        coreSettings["beetle_psx_hw_widescreen_hack_aspect_ratio"] = "16:9";
                    }
                }
            }
            else
                coreSettings["beetle_psx_hw_widescreen_hack"] = "disabled";

            // widescreen aspect ratio
            if (SystemConfig.isOptSet("widescreen_hack_aspect_ratio"))
                coreSettings["beetle_psx_hw_widescreen_hack_aspect_ratio"] = SystemConfig["widescreen_hack_aspect_ratio"];
            else
                coreSettings["beetle_psx_hw_widescreen_hack_aspect_ratio"] = "16:9";

            // force NTSC timings
            if (SystemConfig.isOptSet("pal_video_timing_override"))
                coreSettings["beetle_psx_hw_pal_video_timing_override"] = SystemConfig["pal_video_timing_override"];
            else
                coreSettings["beetle_psx_hw_pal_video_timing_override"] = "disabled";

            // skip BIOS
            if (SystemConfig.isOptSet("skip_bios"))
                coreSettings["beetle_psx_hw_skip_bios"] = SystemConfig["skip_bios"];
            else
                coreSettings["beetle_psx_hw_skip_bios"] = "enabled";

            /*
            // 32BPP
            if (SystemConfig.isOptSet("32bits_color_depth") && SystemConfig.getOptBoolean("32bits_color_depth"))
            {
                coreSettings["beetle_psx_hw_depth"] = "32bpp";
                coreSettings["beetle_psx_hw_dither_mode"] = "disabled";
            }
            else
            {
                coreSettings["beetle_psx_hw_depth"] = "16bpp(native)";
                coreSettings["beetle_psx_hw_dither_mode"] = "1x(native)";
            }
			*/

            // PGXP
            if (SystemConfig.isOptSet("pgxp") && SystemConfig.getOptBoolean("pgxp"))
            {
                coreSettings["beetle_psx_hw_pgxp_mode"] = "memory only";
                coreSettings["beetle_psx_hw_pgxp_texture"] = "enabled";
                coreSettings["beetle_psx_hw_pgxp_vertex"] = "enabled";
            }
            else
            {
                coreSettings["beetle_psx_hw_pgxp_mode"] = "disabled";
                coreSettings["beetle_psx_hw_pgxp_texture"] = "disabled";
                coreSettings["beetle_psx_hw_pgxp_vertex"] = "disabled";
            }

            coreSettings["beetle_psx_hw_gun_input_mode"] = "lightgun";
            coreSettings["beetle_psx_hw_gun_cursor"] = "cross";

            if (SystemConfig.isOptSet("use_guns") && SystemConfig.getOptBoolean("use_guns"))
            {
                retroarchConfig["input_libretro_device_p1"] = "260";
                retroarchConfig["input_player1_mouse_index"] = "0";
                retroarchConfig["input_player1_gun_trigger_mbtn"] = "1";
                retroarchConfig["input_player1_gun_aux_a_mbtn"] = "2";
                retroarchConfig["input_player1_gun_start_mbtn"] = "3";
                retroarchConfig["input_player1_gun_start"] = "enter";
                retroarchConfig["input_player1_gun_select"] = "escape";
            }
        }
    }
}
