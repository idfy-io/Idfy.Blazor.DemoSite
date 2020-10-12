using MatBlazor;

namespace Idfy.Blazor.DemoSite.Client.Static
{

    public static class Themes
    {
        public static MatTheme GreyIsh => new MatTheme()
        {
            Primary = "#adadad",
            Secondary = "#adadad"
        };
        
        public static MatTheme Signicat => new MatTheme()
        {
            Primary = "#293440",
            Secondary = "#293440"
        };

        public static MatTheme Success => new MatTheme()
        {
            Primary = "#5BB0FF", // MatThemeColors.Green._500.Value,
            Secondary = "#5BB0FF", // MatThemeColors.Green._500.Value
        };

        public static MatTheme Warn => new MatTheme()
        {
            Primary = MatThemeColors.Red._500.Value,
            Secondary = MatThemeColors.Red._500.Value
        };
    }
}
