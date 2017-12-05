using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cn.lds.chatcore.pcw.Views.Page;
using Microsoft.Win32;

namespace cn.lds.chatcore.pcw.Common {
public class RegistryWriter {
    public static void changeToIE10() {
        try {
            RegistryKey registrybrowser = Registry.CurrentUser.OpenSubKey
                                          (@"SOFTWARE\\Microsoft\\Internet Explorer\\Main\\FeatureControl\\FEATURE_BROWSER_EMULATION", true);
            string myProgramName = App.AppName;
            myProgramName = myProgramName + ".exe";
            //Path.GetFileName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            if (registrybrowser != null) {
                var currentValue = registrybrowser.GetValue(myProgramName);
                if (currentValue == null || (int)currentValue != 0x02711)
                    registrybrowser.SetValue(myProgramName, 0x02711, RegistryValueKind.DWord);
            }

        } catch (Exception e) {
            Log.Error(typeof(RegistryWriter), e);

        }
    }

    public static void changeChromiumFont() {
        try {
            String BorderWidth = "-15";
            String CaptionFont = "f4,ff,ff,ff,00,00,00,00,00,00,00,00,00,00,00,00,90,01,00,00,00,00,00,01,00,00,05,00,53,00,65,00,67,00,6f,00,65,00,20,00,55,00,49,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00";
            String CaptionHeight = "-315";
            String CaptionWidth = "-315";
            String IconFont = "f4,ff,ff,ff,00,00,00,00,00,00,00,00,00,00,00,00,90,01,00,00,00,00,00,01,00,00,05,00,53,00,65,00,67,00,6f,00,65,00,20,00,55,00,49,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00";
            String IconTitleWrap = "1";
            String MenuFont = "f4,ff,ff,ff,00,00,00,00,00,00,00,00,00,00,00,00,90,01,00,00,00,00,00,01,00,00,05,00,53,00,65,00,67,00,6f,00,65,00,20,00,55,00,49,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00";
            String MenuHeight = "-285";
            String MenuWidth = "-285";
            String MessageFont = "f4,ff,ff,ff,00,00,00,00,00,00,00,00,00,00,00,00,90,01,00,00,00,00,00,01,00,00,05,00,53,00,65,00,67,00,6f,00,65,00,20,00,55,00,49,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00";
            String ScrollHeight = "-255";
            String ScrollWidth = "-255";
            String SmCaptionFont = "f4,ff,ff,ff,00,00,00,00,00,00,00,00,00,00,00,00,90,01,00,00,00,00,00,01,00,00,05,00,53,00,65,00,67,00,6f,00,65,00,20,00,55,00,49,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00";
            String SmCaptionHeight = "-255";
            String SmCaptionWidth = "-255";
            String StatusFont = "f4,ff,ff,ff,00,00,00,00,00,00,00,00,00,00,00,00,90,01,00,00,00,00,00,01,00,00,05,00,53,00,65,00,67,00,6f,00,65,00,20,00,55,00,49,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00";
            String ShellIconSize = "32";
            //dword
            String AppliedDPI = "00000060";
            String PaddedBorderWidth = "-60";
            String IconSpacing = "-1125";
            String IconVerticalSpacing = "-1125";
            String MinAnimate = "1";

            RegistryKey registry = Registry.CurrentUser.OpenSubKey
                                   (@"Control Panel\\Desktop\\WindowMetrics", true);
            if (registry == null) {
                registry = Registry.CurrentUser.CreateSubKey(@"Control Panel\\Desktop\\WindowMetrics");
            }

            registry.SetValue("BorderWidth", BorderWidth);
            registry.SetValue("CaptionHeight", CaptionHeight);
            registry.SetValue("CaptionWidth", CaptionWidth);
            registry.SetValue("IconTitleWrap", IconTitleWrap);
            registry.SetValue("MenuHeight", MenuHeight);
            registry.SetValue("MenuWidth", MenuWidth);
            registry.SetValue("ScrollHeight", ScrollHeight);
            registry.SetValue("ScrollWidth", ScrollWidth);
            registry.SetValue("SmCaptionHeight", SmCaptionHeight);
            registry.SetValue("SmCaptionWidth", SmCaptionWidth);
            registry.SetValue("ShellIconSize", ShellIconSize);
            registry.SetValue("PaddedBorderWidth", PaddedBorderWidth);
            registry.SetValue("IconSpacing", IconSpacing);
            registry.SetValue("IconVerticalSpacing", IconVerticalSpacing);
            registry.SetValue("MinAnimate", MinAnimate);
            registry.SetValue("AppliedDPI", 0x00000060, RegistryValueKind.DWord);

            var CaptionFontData = CaptionFont.Split(',').Select(x => Convert.ToByte(x, 16)).ToArray();
            registry.SetValue("CaptionFont", CaptionFontData, RegistryValueKind.Binary);

            var IconFontData = IconFont.Split(',').Select(x => Convert.ToByte(x, 16)).ToArray();
            registry.SetValue("IconFont", IconFontData, RegistryValueKind.Binary);


            var MenuFontData = MenuFont.Split(',').Select(x => Convert.ToByte(x, 16)).ToArray();
            registry.SetValue("MenuFont", MenuFontData, RegistryValueKind.Binary);


            var MessageFontData = MessageFont.Split(',').Select(x => Convert.ToByte(x, 16)).ToArray();
            registry.SetValue("MessageFont", MessageFontData, RegistryValueKind.Binary);

            var SmCaptionFontData = SmCaptionFont.Split(',').Select(x => Convert.ToByte(x, 16)).ToArray();
            registry.SetValue("SmCaptionFont", SmCaptionFontData, RegistryValueKind.Binary);

            var StatusFontData = StatusFont.Split(',').Select(x => Convert.ToByte(x, 16)).ToArray();
            registry.SetValue("StatusFont", StatusFontData, RegistryValueKind.Binary);
        } catch (Exception e) {
            Log.Error(typeof(RegistryWriter), e);

        }

    }
}
}
