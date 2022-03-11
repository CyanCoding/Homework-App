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

            // Generate and apply the theme
            Style style = new Style();
            // theme[0]: regular color
            // theme[1]: selection color
            string[] theme = Themes.BrushValues[Properties.Settings.Default.SelectedTheme].Split(',');

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
            UpdateSelection(homeworkButton, homeworkGrid);
        }

        private void classesButton_Click(object? sender = null, RoutedEventArgs? e = null) {
            UpdateSelection(classesButton, classesGrid);
        }

        private void calendarButton_Click(object? sender = null, RoutedEventArgs? e = null) {
            UpdateSelection(calendarButton, calendarGrid);
        }

        private void settingsButton_Click(object? sender = null, RoutedEventArgs? e = null) {
            UpdateSelection(settingsButton, settingsGrid);

            // Prepare the scene
            themeComboBox.SelectedIndex = Themes.ThemeToInt(Properties.Settings.Default.SelectedTheme);

            switch (Properties.Settings.Default.StartTab) {
                case "Homework":
                    tabComboBox.SelectedIndex = 0;
                    break;
                case "Classes":
                    tabComboBox.SelectedIndex = 1;
                    break;
                case "Calendar":
                    tabComboBox.SelectedIndex = 2;
                    break;
            }

            settingsGrid.Visibility = Visibility.Visible;

        }

        private void UpdateSelection(Button selectedButton, Grid showingGrid) {
            Button[] menuList = new Button[] { 
                homeworkButton,
                classesButton,
                calendarButton,
                settingsButton
            };

            Grid[] gridList = new Grid[] {
                homeworkGrid,
                classesGrid,
                calendarGrid,
                settingsGrid
            };

            foreach (Button button in menuList) {
                if (button == selectedButton) {
                    Themes.SetSelection(button);
                }
                else {
                    Themes.DeselectButton(button);
                }
            }

            foreach (Grid grid in gridList) {
                if (grid == showingGrid) {
                    grid.Visibility = Visibility.Visible;
                }
                else {
                    grid.Visibility = Visibility.Hidden;
                }
            }
        }

        /// <summary>
        /// Changes the theme globally.
        /// </summary>
        private void themeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            string themeName = Themes.IntTotheme(themeComboBox.SelectedIndex);
            
            // Saves the new theme to properties
            Properties.Settings.Default.SelectedTheme = themeName;
            Properties.Settings.Default.Save();

            Style style = new Style();
            // theme[0]: regular color
            // theme[1]: selection color
            string[] theme = Themes.BrushValues[themeName].Split(',');

            style.Setters.Add(new Setter(Border.BackgroundProperty, new BrushConverter().ConvertFrom(theme[0]) as Brush));
            style.Setters.Add(new Setter(Border.BorderBrushProperty, new BrushConverter().ConvertFrom(theme[0]) as Brush));

            Application.Current.Resources["menu-color"] = style;

            // We have to update the button selection again for the new style
            UpdateSelection(settingsButton, settingsGrid);
        }

        /// <summary>
        /// Changes the user's start tab
        /// </summary>
        private void tabComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            int tab = tabComboBox.SelectedIndex;

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

        private void Label_MouseEnter(object sender, MouseEventArgs e) {
            testy.Content = "✔";
        }

        private void Label_MouseLeave(object sender, MouseEventArgs e) {
            testy.Content = "";
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e) {
            newAssignmentGrid.Visibility = Visibility.Hidden;
        }

        private void newAssignmentButton_Click(object sender, RoutedEventArgs e) {
            newAssignmentGrid.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Checks if the user has filled in the assignment title AND datepicker
        /// </summary>
        /// <returns>True if both are filled in.</returns>
        private bool assignmentFilledDetails() {
            bool returnVal = true;
            // Title text box is required
            if (assignmentTitle.Text == "") {
                assignmentTitle.BorderBrush = new BrushConverter().ConvertFrom("#FFAA2929") as Brush;
                assignmentTitleRequiredLabel.Visibility = Visibility.Visible;
                returnVal = false;
            } else {
                assignmentTitle.BorderBrush = new BrushConverter().ConvertFrom("#FFABADB3") as Brush;
                assignmentTitleRequiredLabel.Visibility = Visibility.Hidden;
            }
            // Valid calendar day is required
            if (assignmentCalendar.SelectedDate == null) {
                assignmentCalendar.BorderBrush = new BrushConverter().ConvertFrom("#FFAA2929") as Brush;
                assignmentCalendarRequiredLabel.Visibility = Visibility.Visible;
                returnVal = false;
            } else {
                assignmentCalendar.BorderBrush = new BrushConverter().ConvertFrom("#FFABADB3") as Brush;
                assignmentCalendarRequiredLabel.Visibility = Visibility.Hidden;
            }

            return returnVal;
        }

        private void saveAndAddButton_Click(object sender, RoutedEventArgs e) {
            assignmentFilledDetails();
            // Add assignment and clear values
        }

        private void saveButton_Click(object sender, RoutedEventArgs e) {
            assignmentFilledDetails();
            // Add assignment and close screen and clear values
        }

        /// <summary>
        /// When the user clicks to add a file to their assignment
        /// </summary>
        private void assignmentAddFileButton_Click(object sender, RoutedEventArgs e) {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result.HasValue && result.Value) {
                // File
                string filename = dlg.FileName; // System.IO.Path.GetFileName(filename);
            }
        }
    }
}
