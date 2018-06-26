using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ElectronicObserver.Utility
{
    public static class ThemeManager
    {
        private static Dictionary<Tuple<Theme, ThemeColors>, Color> colors;

        static ThemeManager()
        {
            colors = new Dictionary<Tuple<Theme, ThemeColors>, Color>();
            colors.Add(Tuple.Create(Theme.Light, ThemeColors.MainFontColor), SystemColors.ControlText);
            colors.Add(Tuple.Create(Theme.Light, ThemeColors.SubFontColor), Color.FromArgb(0x88, 0x88, 0x88));
            colors.Add(Tuple.Create(Theme.Light, ThemeColors.BackgroundColor), SystemColors.Control);
            colors.Add(Tuple.Create(Theme.Light, ThemeColors.RedHighlight), Color.IndianRed);
            colors.Add(Tuple.Create(Theme.Light, ThemeColors.OrangeHighlight), Color.Moccasin);
            colors.Add(Tuple.Create(Theme.Light, ThemeColors.YellowHighlight), Color.FromArgb(0xFF, 0xFF, 0xBB));
            colors.Add(Tuple.Create(Theme.Light, ThemeColors.GreenHighlight), Color.FromArgb(0xBB, 0xFF, 0xBB));
            colors.Add(Tuple.Create(Theme.Light, ThemeColors.GrayHighlight), Color.FromArgb(0xBB, 0xBB, 0xBB));
            colors.Add(Tuple.Create(Theme.Light, ThemeColors.PinkHighlight), Color.LightCoral);
            colors.Add(Tuple.Create(Theme.Light, ThemeColors.MVPHighlight), Color.Moccasin);
            colors.Add(Tuple.Create(Theme.Light, ThemeColors.RepairColor), Color.FromArgb(0, 0, 0x88));
            colors.Add(Tuple.Create(Theme.Dark, ThemeColors.MainFontColor), SystemColors.Control);
            colors.Add(Tuple.Create(Theme.Dark, ThemeColors.SubFontColor), SystemColors.ControlDark);
            colors.Add(Tuple.Create(Theme.Dark, ThemeColors.BackgroundColor), Color.FromArgb(0x22, 0x22, 0x22));
            colors.Add(Tuple.Create(Theme.Dark, ThemeColors.RedHighlight), Color.IndianRed);
            colors.Add(Tuple.Create(Theme.Dark, ThemeColors.OrangeHighlight), Color.FromArgb(0xE5, 0x68, 0));
            colors.Add(Tuple.Create(Theme.Dark, ThemeColors.YellowHighlight), Color.FromArgb(0xFF, 0xCC, 0x00));
            colors.Add(Tuple.Create(Theme.Dark, ThemeColors.GreenHighlight), Color.DarkGreen);
            colors.Add(Tuple.Create(Theme.Dark, ThemeColors.GrayHighlight), Color.Silver);
            colors.Add(Tuple.Create(Theme.Dark, ThemeColors.PinkHighlight), Color.LightCoral);
            colors.Add(Tuple.Create(Theme.Dark, ThemeColors.MVPHighlight), Color.FromArgb(0xCC, 0xB6, 0x90));
            colors.Add(Tuple.Create(Theme.Dark, ThemeColors.RepairColor), Color.LightBlue);
        }

        public static Color GetColor(Theme th, ThemeColors tr)
        {
            return colors[Tuple.Create(th, tr)];
        }
    }

    [Serializable]
    public enum Theme
    {
        Light = 0,
        Dark = 1 //,
        //Pink = 2
    }

    [Serializable]
    public enum ThemeColors
    {
        MainFontColor,
        SubFontColor,
        BackgroundColor,
        RedHighlight,
        OrangeHighlight,
        YellowHighlight,
        GreenHighlight,
        GrayHighlight,
        PinkHighlight,
        MVPHighlight,
        RepairColor
    }
}
