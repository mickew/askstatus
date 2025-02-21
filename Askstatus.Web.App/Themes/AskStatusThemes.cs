using MudBlazor;

namespace Askstatus.Web.App.Themes;

public static class AskStatusThemes
{
    /* 
!
 * Bootswatch v5.3.3 (https://bootswatch.com)
 * Theme: flatly
 * Copyright 2012-2024 Thomas Park
 * Licensed under MIT
 * Based on Bootstrap
    */
    public static readonly MudTheme Flatlytheme = new()
    {
        PaletteLight = new PaletteLight()
        {
            Black = "#000",
            White = "#fff",
            Primary = "#2c3e50",
            PrimaryContrastText = "#d5d8dc",
            Secondary = "#95a5a6",
            SecondaryContrastText = "#eaeded",
            Tertiary = "rgba(33, 37, 41, 0.5)",
            TertiaryContrastText = "#f8f9fa",
            Info = "#3498db",
            InfoContrastText = "#d6eaf8",
            Success = "#18bc9c",
            SuccessContrastText = "#d1f2eb",
            Warning = "#f39c12",
            WarningContrastText = "#fdebd0",
            Error = "rgba(244,67,54,1)",
            ErrorContrastText = "rgba(255,255,255,1)",
            Dark = "#7b8a8b",
            DarkContrastText = "#ced4da",
            TextPrimary = "#121920",
            TextSecondary = "#3c4242",
            TextDisabled = "rgba(0,0,0,0.3764705882352941)",
            ActionDefault = "rgba(0,0,0,0.5372549019607843)",
            ActionDisabled = "rgba(0,0,0,0.25882352941176473)",
            ActionDisabledBackground = "rgba(0,0,0,0.11764705882352941)",
            Background = "rgba(255,255,255,1)",
            BackgroundGray = "rgba(245,245,245,1)",
            Surface = "rgba(255,255,255,1)",
            DrawerBackground = "#eaeded",
            DrawerText = "#95a5a6",
            DrawerIcon = "#95a5a6",
            AppbarBackground = "#d5d8dc",
            AppbarText = "#2c3e50",
            LinesDefault = "rgba(0,0,0,0.11764705882352941)",
            LinesInputs = "rgba(189,189,189,1)",
            TableLines = "rgba(224,224,224,1)",
            TableStriped = "rgba(0,0,0,0.0196078431372549)",
            TableHover = "rgba(0,0,0,0.0392156862745098)",
            Divider = "rgba(224,224,224,1)",
            DividerLight = "rgba(0,0,0,0.8)",
            PrimaryDarken = "#121920",
            PrimaryLighten = "#d5d8dc",
            SecondaryDarken = "#3c4242",
            SecondaryLighten = "#eaeded",
            TertiaryDarken = "rgba(33, 37, 41, 0.5)",
            TertiaryLighten = "rgba(33, 37, 41, 0.5)",
            InfoDarken = "#153d58",
            InfoLighten = "#d6eaf8",
            SuccessDarken = "#0a4b3e",
            SuccessLighten = "#d1f2eb",
            WarningDarken = "#613e07",
            WarningLighten = "#fdebd0",
            ErrorDarken = "rgb(242,28,13)",
            ErrorLighten = "rgb(246,96,85)",
            DarkDarken = "rgb(46,46,46)",
            DarkLighten = "rgb(87,87,87)",
            HoverOpacity = 0.06,
            RippleOpacity = 0.1,
            RippleOpacitySecondary = 0.2,
            GrayDefault = "#9E9E9E",
            GrayLight = "#BDBDBD",
            GrayLighter = "#E0E0E0",
            GrayDark = "#757575",
            GrayDarker = "#616161",
            OverlayDark = "rgba(33,33,33,0.4980392156862745)",
            OverlayLight = "rgba(255,255,255,0.4980392156862745)",
        },
        PaletteDark = new PaletteDark()
        {
            Black = "#000",
            White = "#fff",
            Primary = "#2c3e50",
            PrimaryContrastText = "#090c10",
            Secondary = "#95a5a6",
            SecondaryContrastText = "#1e2121",
            Tertiary = "rgba(222, 226, 230, 0.5)",
            TertiaryContrastText = "#2b3035",
            Info = "#3498db",
            InfoContrastText = "#0a1e2c",
            Success = "#18bc9c",
            SuccessContrastText = "#05261f",
            Warning = "#f39c12",
            WarningContrastText = "#311f04",
            Error = "rgba(244,67,54,1)",
            ErrorContrastText = "rgba(255,255,255,1)",
            Dark = "#7b8a8b",
            DarkContrastText = "#1a1d20",
            TextPrimary = "#808b96",
            TextSecondary = "#bfc9ca",
            TextDisabled = "rgba(255,255,255,0.2)",
            ActionDefault = "rgba(173,173,177,1)",
            ActionDisabled = "rgba(255,255,255,0.25882352941176473)",
            ActionDisabledBackground = "rgba(255,255,255,0.11764705882352941)",
            Background = "rgba(50,51,61,1)",
            BackgroundGray = "rgba(39,39,47,1)",
            Surface = "rgba(55,55,64,1)",
            DrawerBackground = "#1e2121",
            DrawerText = "#95a5a6",
            DrawerIcon = "#95a5a6",
            AppbarBackground = "#090c10",
            AppbarText = "#2c3e50",
            LinesDefault = "rgba(255,255,255,0.11764705882352941)",
            LinesInputs = "rgba(255,255,255,0.2980392156862745)",
            TableLines = "rgba(255,255,255,0.11764705882352941)",
            TableStriped = "rgba(255,255,255,0.2)",
            Divider = "rgba(255,255,255,0.11764705882352941)",
            DividerLight = "rgba(255,255,255,0.058823529411764705)",
            PrimaryDarken = "#808b96",
            PrimaryLighten = "#090c10",
            SecondaryDarken = "#bfc9ca",
            SecondaryLighten = "#1e2121",
            TertiaryDarken = "rgba(222, 226, 230, 0.5)",
            TertiaryLighten = "rgba(222, 226, 230, 0.5)",
            InfoDarken = "#85c1e9",
            InfoLighten = "#0a1e2c",
            SuccessDarken = "#74d7c4",
            SuccessLighten = "#05261f",
            WarningDarken = "#f8c471",
            WarningLighten = "#311f04",
            ErrorDarken = "rgb(242,28,13)",
            ErrorLighten = "rgb(246,96,85)",
            DarkDarken = "rgb(23,23,28)",
            DarkLighten = "rgb(56,56,67)",
        },
        LayoutProperties = new LayoutProperties()
        {
            DefaultBorderRadius = "4px",
            DrawerMiniWidthLeft = "56px",
            DrawerMiniWidthRight = "56px",
            DrawerWidthLeft = "240px",
            DrawerWidthRight = "240px",
            AppbarHeight = "64px",
        },
        Typography = new Typography()
        {
            Default = new DefaultTypography
            {
                FontFamily = ["Roboto", "Helvetica", "Arial", "sans-serif"],
                FontWeight = "400",
                FontSize = ".875rem",
                LineHeight = "1.43",
                LetterSpacing = ".01071em",
                TextTransform = "none",
            },
            H1 = new H1Typography
            {
                FontWeight = "300",
                FontSize = "6rem",
                LineHeight = "1.167",
                LetterSpacing = "-.01562em",
                TextTransform = "none",
            },
            H2 = new H2Typography
            {
                FontWeight = "300",
                FontSize = "3.75rem",
                LineHeight = "1.2",
                LetterSpacing = "-.00833em",
                TextTransform = "none",
            },
            H3 = new H3Typography
            {
                FontWeight = "400",
                FontSize = "3rem",
                LineHeight = "1.167",
                LetterSpacing = "0",
                TextTransform = "none",
            },
            H4 = new H4Typography
            {
                FontWeight = "400",
                FontSize = "2.125rem",
                LineHeight = "1.235",
                LetterSpacing = ".00735em",
                TextTransform = "none",
            },
            H5 = new H5Typography
            {
                FontWeight = "400",
                FontSize = "1.5rem",
                LineHeight = "1.334",
                LetterSpacing = "0",
                TextTransform = "none",
            },
            H6 = new H6Typography
            {
                FontWeight = "500",
                FontSize = "1.25rem",
                LineHeight = "1.6",
                LetterSpacing = ".0075em",
                TextTransform = "none",
            },
            Subtitle1 = new Subtitle1Typography
            {
                FontWeight = "400",
                FontSize = "1rem",
                LineHeight = "1.75",
                LetterSpacing = ".00938em",
                TextTransform = "none",
            },
            Subtitle2 = new Subtitle2Typography
            {
                FontWeight = "500",
                FontSize = ".875rem",
                LineHeight = "1.57",
                LetterSpacing = ".00714em",
                TextTransform = "none",
            },
            Body1 = new Body1Typography
            {
                FontWeight = "400",
                FontSize = "1rem",
                LineHeight = "1.5",
                LetterSpacing = ".00938em",
                TextTransform = "none",
            },
            Body2 = new Body2Typography
            {
                FontWeight = "400",
                FontSize = ".875rem",
                LineHeight = "1.43",
                LetterSpacing = ".01071em",
                TextTransform = "none",
            },
            //Input = new Input
            //{
            //    FontWeight = 400,
            //    FontSize = "1rem",
            //    LineHeight = 1.1876,
            //    LetterSpacing = ".00938em",
            //    TextTransform = "none",
            //},
            Button = new ButtonTypography
            {
                FontWeight = "500",
                FontSize = ".875rem",
                LineHeight = "1.75",
                LetterSpacing = ".02857em",
                TextTransform = "uppercase",
            },
            Caption = new CaptionTypography
            {
                FontWeight = "400",
                FontSize = ".75rem",
                LineHeight = "1.66",
                LetterSpacing = ".03333em",
                TextTransform = "none",
            },
            Overline = new OverlineTypography
            {
                FontWeight = "400",
                FontSize = ".75rem",
                LineHeight = "2.66",
                LetterSpacing = ".08333em",
                TextTransform = "none",
            },
        },
        ZIndex = new ZIndex()
        {
            Drawer = 1100,
            Popover = 1200,
            AppBar = 1300,
            Dialog = 1400,
            Snackbar = 1500,
            Tooltip = 1600,
        },
    };
}
