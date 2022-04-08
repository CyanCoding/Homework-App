using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Homework_App {
    internal static class Themes {
        /// <summary>
        /// Key: The name of the theme
        /// Value: {normal theme color}, {selection color}
        /// </summary>
        internal static Dictionary<string, string> BrushValues = new Dictionary<string, string>() {
            {"Blueish", "#FFA6C5FF,#FFA6E3FF" },
            {"Purpleish", "#FFABA6FF,#FFCEA6FF" },
            {"Redish", "#FFE94B4B,#FFF27B7B"}
        };

        /// <summary>
        /// This is called from the settings page when theme is updated.
        /// </summary>
        internal static void RefreshColors() {
            var theme = BrushValues[Properties.Settings.Default.SelectedTheme].Split(',');
            MainWindow win = Application.Current.Windows[0] as MainWindow;
            
            win.SettingsButton.Background = new BrushConverter().ConvertFrom(theme[1]) as Brush;
            win.HomeworkButton.Background = new BrushConverter().ConvertFrom(theme[0]) as Brush;
            win.ClassesButton.Background = new BrushConverter().ConvertFrom(theme[0]) as Brush;
            win.CalendarButton.Background = new BrushConverter().ConvertFrom(theme[0]) as Brush;
        }

        /// <summary>
        /// Sets a button's background to the appropriate color based
        /// on the currentTheme value.
        /// </summary>
        /// <param name="button">The Button to select.</param>
        internal static void SetSelection(Button button) {
            // theme[0]: regular color
            // theme[1]: selection color
            var theme = BrushValues[Properties.Settings.Default.SelectedTheme].Split(',');
            button.Background = new BrushConverter().ConvertFrom(theme[1]) as Brush;
        }

        internal static void DeselectButton(Button button) {
            // theme[0]: regular color
            // theme[1]: selection color
            var theme = BrushValues[Properties.Settings.Default.SelectedTheme].Split(',');
            button.Background = new BrushConverter().ConvertFrom(theme[0]) as Brush;
        }

        /// <summary>
        /// Returns the themeComboBox selection index of the theme
        /// </summary>
        /// <param name="value">The theme string</param>
        /// <returns>An index of the theme in the themeComboBox</returns>
        internal static int ThemeToInt(string value) {
            switch (value) {
                case "Blueish":
                    return 0;
                case "Purpleish":
                    return 1;
                case "Redish":
                    return 2;

            }
            return 0;
        }

        /// <summary>
        /// Returns the theme name from the themeComboBox index
        /// </summary>
        /// <param name="value">The index of the theme in themeComboBox</param>
        /// <returns>The name of the theme</returns>
        internal static string IntTotheme(int value) {
            switch (value) {
                case 0:
                    return "Blueish";
                case 1:
                    return "Purpleish";
                case 2:
                    return "Redish";
            }
            return "Blueish";
        }
    }
}
