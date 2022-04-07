using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Homework_App.Pages; 

public partial class Settings : Page {
    public Settings() {
        InitializeComponent();
        
        // Prepare the scene
        ThemeComboBox.SelectedIndex = Themes.ThemeToInt(Properties.Settings.Default.SelectedTheme);

        switch (Properties.Settings.Default.StartTab) {
            case "Homework":
                TabComboBox.SelectedIndex = 0;
                break;
            case "Classes":
                TabComboBox.SelectedIndex = 1;
                break;
            case "Calendar":
                TabComboBox.SelectedIndex = 2;
                break;
        }
    }
    
    /// <summary>
    /// Changes the theme globally.
    /// </summary>
    private void themeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
        var themeName = Themes.IntTotheme(ThemeComboBox.SelectedIndex);

        // Saves the new theme to properties
        Properties.Settings.Default.SelectedTheme = themeName;
        Properties.Settings.Default.Save();

        var style = new Style();
        // theme[0]: regular color
        // theme[1]: selection color
        var theme = Themes.BrushValues[themeName].Split(',');

        style.Setters.Add(new Setter(Border.BackgroundProperty, new BrushConverter().ConvertFrom(theme[0]) as Brush));
        style.Setters.Add(new Setter(Border.BorderBrushProperty, new BrushConverter().ConvertFrom(theme[0]) as Brush));

        Application.Current.Resources["menu-color"] = style;

        // We have to update the button selection again for the new style
        //UpdateSelection(SettingsButton, SettingsGrid);
    }
    
    /// <summary>
    /// Changes the user's start tab
    /// </summary>
    private void tabComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
        var tab = TabComboBox.SelectedIndex;

        switch (tab) {
            case 0:
                Properties.Settings.Default.StartTab = "Homework";
                break;
            case 1:
                Properties.Settings.Default.StartTab = "Classes";
                break;
            case 2:
                Properties.Settings.Default.StartTab = "Calendar";
                break;
        }

        Properties.Settings.Default.Save();
    }
}