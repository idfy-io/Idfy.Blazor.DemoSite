using MatBlazor;

namespace Idfy.Blazor.DemoSite.Client.Static
{

    public static class Themes
    {
        public static MatTheme BlueIsh => new MatTheme()
        {
            Primary = "#3f51b5",
            Secondary = "#8c9eff"
        };

        public static MatTheme Success => new MatTheme()
        {
            Primary = MatThemeColors.Green._500.Value,
            Secondary = MatThemeColors.Green._500.Value
        };

        public static MatTheme Warn => new MatTheme()
        {
            Primary = MatThemeColors.Red._500.Value,
            Secondary = MatThemeColors.Red._500.Value
        };
    }
}
