﻿using System.IO;
using EmulatorLauncher.Common.FileFormats;
using System.Linq;
using EmulatorLauncher.Common.EmulationStation;
using System;

namespace EmulatorLauncher
{
    partial class PortsLauncherGenerator : Generator
    {
        private void ConfigureSonic3airControls(string configFolder, DynamicJson settings)
        {
            if (_emulator != "sonic3air")
                return;

            if (!Controllers.Any(c => !c.IsKeyboard))
                return;

            settings["PreferredGamepadPlayer1"] = string.Empty;
            settings["PreferredGamepadPlayer2"] = string.Empty;

            string inputSettingsFile = Path.Combine(configFolder, "settings_input.json");

            var inputJson = DynamicJson.Load(inputSettingsFile);

            var inputDevices = inputJson.GetOrCreateContainer("InputDevices");

            foreach (var controller in this.Controllers.Where(c => !c.IsKeyboard).OrderBy(i => i.PlayerIndex).Take(2))
            {
                string deviceName = controller.Name;
                var device = inputDevices.GetOrCreateContainer(deviceName);
                bool isXinput = controller.IsXInputDevice;

                string[] deviceNames = new string[] { deviceName };
                device.SetObject("DeviceNames", deviceNames);

                string[] up = new string[] { GetSDLInputName(controller, InputKey.up, isXinput) };
                string[] down = new string[] { GetSDLInputName(controller, InputKey.down, isXinput) };
                string[] left = new string[] { GetSDLInputName(controller, InputKey.left, isXinput) };
                string[] right = new string[] { GetSDLInputName(controller, InputKey.right, isXinput) };
                string[] a = new string[] { GetSDLInputName(controller, InputKey.a, isXinput) };
                string[] b = new string[] { GetSDLInputName(controller, InputKey.b, isXinput) };
                string[] x = new string[] { GetSDLInputName(controller, InputKey.y, isXinput) };
                string[] y = new string[] { GetSDLInputName(controller, InputKey.x, isXinput) };
                string[] start = new string[] { GetSDLInputName(controller, InputKey.start, isXinput) };
                string[] back = new string[] { GetSDLInputName(controller, InputKey.select, isXinput) };
                string[] l = new string[] { GetSDLInputName(controller, InputKey.pageup, isXinput) };
                string[] r = new string[] { GetSDLInputName(controller, InputKey.pagedown, isXinput) };
                device.SetObject("Up", up);
                device.SetObject("Down", down);
                device.SetObject("Left", left);
                device.SetObject("Right", right);
                device.SetObject("A", a);
                device.SetObject("B", b);
                device.SetObject("X", x);
                device.SetObject("Y", y);
                device.SetObject("Start", start);
                device.SetObject("Back", back);
                device.SetObject("L", l);
                device.SetObject("R", r);

                if (controller.PlayerIndex == 1)
                    settings["PreferredGamepadPlayer1"] = deviceName;
                if (controller.PlayerIndex == 2)
                    settings["PreferredGamepadPlayer2"] = deviceName;
            }
             
            inputJson.Save();
        }

        private static string GetSDLInputName(Controller c, InputKey key, bool isXinput)
        {
            Int64 pid = -1;

            bool revertAxis = false;
            key = key.GetRevertedAxis(out revertAxis);

            var input = c.Config[key];
            if (input != null)
            {
                if (input.Type == "button")
                {
                    pid = input.Id;
                    return "Button" + pid;
                }

                if (input.Type == "hat")
                {
                    pid = input.Value;
                    switch (pid)
                    {
                        case 1: return "Pov0";
                        case 2: return "Pov1";
                        case 4: return "Pov2";
                        case 8: return "Pov3";
                    }
                }

                if (input.Type == "axis")
                {
                    pid = input.Id;
                    switch (pid)
                    {
                        case 0: 
                            if (revertAxis) return "Axis1";
                            else return "Axis0";
                        case 1:
                            if (revertAxis) return "Axis3";
                            else return "Axis2";
                        case 2:
                            if (revertAxis) return "Axis5";
                            else return "Axis4";
                        case 3:
                            if (revertAxis) return "Axis7";
                            else return "Axis8";
                        case 4: return "Axis9";
                        case 5: return "Axis11";
                    }
                }
            }
            return "";
        }
    }
}
