using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Homework_App {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();

            Style style = new Style();
            // theme[0]: regular color
            // theme[1]: selection color
            string[] theme = Themes.BrushValues[Themes.CurrentTheme].Split(',');

            style.Setters.Add(new Setter(Border.BackgroundProperty, new BrushConverter().ConvertFrom(theme[0]) as Brush));
            style.Setters.Add(new Setter(Border.BorderBrushProperty, new BrushConverter().ConvertFrom(theme[0]) as Brush));
            Application.Current.Resources["menu-color"] = style;
        }

        private void homeworkButton_Click(object sender, RoutedEventArgs e) {
            UpdateSelection(homeworkButton);
        }

        private void classesButton_Click(object sender, RoutedEventArgs e) {
            UpdateSelection(classesButton);
        }

        private void calendarButton_Click(object sender, RoutedEventArgs e) {
            UpdateSelection(calendarButton);
        }

        private void settingsButton_Click(object sender, RoutedEventArgs e) {
            UpdateSelection(settingsButton);

            // Prepare the scene
            themeComboBox.SelectedIndex = Themes.ThemeToInt(Themes.CurrentTheme);
            settingsGrid.Visibility = Visibility.Visible;

        }

        private void UpdateSelection(Button selectedButton) {
            Button[] menuList = new Button[] { 
                homeworkButton,
                calendarButton,
                classesButton,
                settingsButton
            };

            foreach (Button button in menuList) {
                if (button == selectedButton) {
                    Themes.SetSelection(button);
                }
                else {
                    Themes.DeselectButton(button);
                }
            }
        }

        /// <summary>
        /// Changes the theme globally.
        /// </summary>
        private void themeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            string themeName = Themes.IntTotheme(themeComboBox.SelectedIndex);
            Themes.CurrentTheme = themeName;

            Style style = new Style();
            // theme[0]: regular color
            // theme[1]: selection color
            string[] theme = Themes.BrushValues[themeName].Split(',');

            style.Setters.Add(new Setter(Border.BackgroundProperty, new BrushConverter().ConvertFrom(theme[0]) as Brush));
            style.Setters.Add(new Setter(Border.BorderBrushProperty, new BrushConverter().ConvertFrom(theme[0]) as Brush));

            Application.Current.Resources["menu-color"] = style;

            // We have to update the button selection again for the new style
            UpdateSelection(settingsButton);
        }
    }
}
