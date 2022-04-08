using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Homework_App {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow {
        public MainWindow() {
            InitializeComponent();

            // Generate and apply the theme
            var style = new Style();
            // theme[0]: regular color
            // theme[1]: selection color
            var theme = Themes.BrushValues[Properties.Settings.Default.SelectedTheme].Split(',');

            style.Setters.Add(new Setter(Border.BackgroundProperty, new BrushConverter().ConvertFrom(theme[0]) as Brush));
            style.Setters.Add(new Setter(Border.BorderBrushProperty, new BrushConverter().ConvertFrom(theme[0]) as Brush));
            Application.Current.Resources["menu-color"] = style;

            // Switch tabs to the user's settings
            switch (Properties.Settings.Default.StartTab) {
                case "Homework":
                    homeworkButton_Click();
                    break;
                case "Classes":
                    classesButton_Click();
                    break;
                case "Calendar":
                    calendarButton_Click();
                    break;
            }
        }

        private void homeworkButton_Click(object? sender = null, RoutedEventArgs? e = null) {
            UpdateSelection(HomeworkButton, HomeworkFrame);
        }

        private void classesButton_Click(object? sender = null, RoutedEventArgs? e = null) {
            UpdateSelection(ClassesButton, ClassesFrame);
        }

        private void calendarButton_Click(object? sender = null, RoutedEventArgs? e = null) {
            UpdateSelection(CalendarButton, CalendarFrame);
        }

        private void settingsButton_Click(object? sender = null, RoutedEventArgs? e = null) {
            UpdateSelection(SettingsButton, SettingsFrame);
        }
        
        private void UpdateSelection(Button selectedButton, Frame showingFrame) {
            var menuList = new[] {
                HomeworkButton,
                ClassesButton,
                CalendarButton,
                SettingsButton
            };

            var frameList = new[] {
                HomeworkFrame,
                ClassesFrame,
                CalendarFrame,
                SettingsFrame
            };

            foreach (var button in menuList) {
                if (button == selectedButton) {
                    Themes.SetSelection(button);
                }
                else {
                    Themes.DeselectButton(button);
                }
            }

            foreach (var frame in frameList) {
                frame.Visibility = frame == showingFrame ? Visibility.Visible : Visibility.Hidden;
            }
        }
    }
}
