using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
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

            ChangeAssignmentDisplay1();
            LoadAssignmentsFromFile();
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
            Label box = (Label)sender;
            box.Content = "✔";
        }

        private void Label_MouseLeave(object sender, MouseEventArgs e) {
            Label box = (Label)sender;
            box.Content = "";
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

        /// <summary>
        /// Saves and clears values (but doesn't close window).
        /// </summary>
        private void saveAndAddButton_Click(object sender, RoutedEventArgs e) {
            // Make sure everything required is filled
            if (!assignmentFilledDetails()) {
                return;
            }

            // Add assignment
            Assignment.AssignmentData data;
            data.Title = assignmentTitle.Text;
            data.Type = assignmentType.Text;
            data.Class = assignmentClass.Text;
            data.Date = assignmentCalendar.Text;
            data.Time = assignmentTime.Text;
            data.Priority = assignmentPriority.Text;
            data.Repeat = assignmentRepeat.Text;
            data.Reminder = assignmentReminder.Text;
            data.Notes = assignmentNotes.Text;

            Assignment.CreateAssignment(data);

            // Clear values
            assignmentTitle.Text = "";
            assignmentType.SelectedIndex = 0;
            assignmentClass.SelectedIndex = 0;
            assignmentCalendar.Text = "";
            assignmentTime.Text = "";
            assignmentPriority.SelectedIndex = 0;
            assignmentRepeat.SelectedIndex = 0;
            assignmentReminder.SelectedIndex = 0;
            assignmentNotes.Text = "";

            AddAssignment(data);
        }

        /// <summary>
        /// This does the same thing as saveAndAddButton except
        /// it closes the window after we save.
        /// </summary>
        private void saveButton_Click(object sender, RoutedEventArgs e) {
            saveAndAddButton_Click(sender, e);

            newAssignmentGrid.Visibility = Visibility.Hidden; // Hide assignment window
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

        private void LoadAssignmentsFromFile() {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            path += "/Homework-App/assignment";
            DirectoryInfo d = new DirectoryInfo(path);

            foreach (var file in d.GetFiles("*.json")) {
                Assignment.AssignmentData data = Assignment.ReadAssignment(file.FullName);
                AddAssignment(data);
            }
        }
        
        // 0: Today
        // 1: Tomorrow
        // 2: Next three days
        // 3: This week
        // 4: Next week
        // 5: All
        private static int[] margins = new int[] { 0, 0, 0, 0, 0, 0};

        /// <summary>
        /// Adds an assignment to the homework tab
        /// </summary>
        /// <param name="data">The assignment data from the file</param>
        private void AddAssignment(Assignment.AssignmentData data) {
            // This uses the same assumptions as margins variable
            bool[] addPlaces = new bool[] {
                false, false, false, false, false, true
            };

            // First we check the day, which changes some values later on
            // such as which grid it's sorted into and the margin height
            // on outerGrid
            DateTime today = DateTime.Today;
            string todayString = today.ToString("M/dd/yyyy");
            string tomorrowString = today.AddDays(1).ToString("M/dd/yyyy");

            string[] threeDays = new string[] {
                today.ToString("M/dd/yyyy"),
                today.AddDays(1).ToString("M/dd/yyyy"),
                today.AddDays(2).ToString("M/dd/yyyy")
            };

            string[] sevenDays = new string[] {
                today.ToString("M/dd/yyyy"),
                today.AddDays(1).ToString("M/dd/yyyy"),
                today.AddDays(2).ToString("M/dd/yyyy"),
                today.AddDays(3).ToString("M/dd/yyyy"),
                today.AddDays(4).ToString("M/dd/yyyy"),
                today.AddDays(5).ToString("M/dd/yyyy"),
                today.AddDays(6).ToString("M/dd/yyyy")
            };

            string[] nextSevenDays = new string[] {
                today.AddDays(7).ToString("M/dd/yyyy"),
                today.AddDays(8).ToString("M/dd/yyyy"),
                today.AddDays(9).ToString("M/dd/yyyy"),
                today.AddDays(10).ToString("M/dd/yyyy"),
                today.AddDays(11).ToString("M/dd/yyyy"),
                today.AddDays(12).ToString("M/dd/yyyy"),
                today.AddDays(13).ToString("M/dd/yyyy")
            };

            // Figure out what range the assignment's due date falls in
            if (todayString == data.Date) { // Due today
                margins[0] += 70;
                addPlaces[0] = true;
            }
            else if (tomorrowString == data.Date) { // Due tomorrow
                margins[1] += 70;
                addPlaces[1] = true;
            }
            foreach (string day in threeDays) { // Due in the next three days
                if (day == data.Date) {
                    margins[2] += 70;
                    addPlaces[2] = true;
                }
            }
            foreach (string day in sevenDays) { // Due this week
                if (day == data.Date) {
                    margins[3] += 70;
                    addPlaces[3] = true;
                }
            }
            foreach (string day in nextSevenDays) { // Due in the next seven days
                if (day == data.Date) {
                    margins[4] += 70;
                    addPlaces[4] = true;
                }
            }

            margins[5] += 70;

            // TODO: Figure out how to format this another way
            string dateString = data.Date;
            // Attach time to date ("3/2/2222 - 2:00 PM")
            if (data.Time != "") {
                dateString += " - " + data.Time;
            }

            // Outer grid

            for (int i = 0; i < addPlaces.Length; i++) {
                if (addPlaces[i]) {
                    Grid outerGrid = new Grid();
                    outerGrid.Height = 60;
                    // We take - 70 because the first one would be at 0
                    outerGrid.Margin = new Thickness(0, margins[i] - 70, 0, 0);
                    outerGrid.VerticalAlignment = VerticalAlignment.Top;

                    ColumnDefinition column1 = new ColumnDefinition();
                    column1.Width = new GridLength(1, GridUnitType.Star);
                    ColumnDefinition column2 = new ColumnDefinition();
                    column2.Width = new GridLength(7, GridUnitType.Star);
                    ColumnDefinition column3 = new ColumnDefinition();
                    column3.Width = new GridLength(5, GridUnitType.Star);

                    RowDefinition row1 = new RowDefinition();
                    row1.Height = new GridLength(3, GridUnitType.Star);
                    RowDefinition row2 = new RowDefinition();
                    row2.Height = new GridLength(2, GridUnitType.Star);

                    outerGrid.ColumnDefinitions.Add(column1);
                    outerGrid.ColumnDefinitions.Add(column2);
                    outerGrid.ColumnDefinitions.Add(column3);

                    outerGrid.RowDefinitions.Add(row1);
                    outerGrid.RowDefinitions.Add(row2);

                    // Outside border
                    Border outsideBorder = new Border();
                    outsideBorder.SetValue(Grid.ColumnSpanProperty, 3);
                    outsideBorder.SetValue(Grid.RowSpanProperty, 3);
                    outsideBorder.Margin = new Thickness(0, 0, 0, -5);
                    outsideBorder.BorderThickness = new Thickness(0, 0, 0, 1);
                    outsideBorder.BorderBrush = new BrushConverter().ConvertFrom("#FF6C6C6C") as Brush;
                    outsideBorder.Background = new SolidColorBrush(Colors.White);
                    outerGrid.Children.Add(outsideBorder);

                    // Check box
                    Border checkboxBorder = new Border();
                    checkboxBorder.HorizontalAlignment = HorizontalAlignment.Right;
                    checkboxBorder.SetValue(Grid.ColumnProperty, 0);
                    checkboxBorder.Margin = new Thickness(0, 5, 5, 0);
                    checkboxBorder.Width = 18;
                    checkboxBorder.Height = 18;

                    string checkBoxBrush = "#FF707070";
                    if (data.Priority == "Low") {
                        checkBoxBrush = "#FF4BB86E";
                    }
                    else if (data.Priority == "Medium") {
                        checkBoxBrush = "#FFB8B44B";
                    }
                    else if (data.Priority == "High") {
                        checkBoxBrush = "#FFB84B4B";
                    }

                    checkboxBorder.BorderBrush = new BrushConverter().ConvertFrom(checkBoxBrush) as Brush;
                    checkboxBorder.BorderThickness = new Thickness(2);

                    Label checkboxLabel = new Label();
                    checkboxLabel.MouseEnter += Label_MouseEnter;
                    checkboxLabel.MouseLeave += Label_MouseLeave;
                    checkboxLabel.FontSize = 8;
                    checkboxLabel.Margin = new Thickness(-3);
                    checkboxLabel.VerticalAlignment = VerticalAlignment.Center;
                    checkboxLabel.HorizontalAlignment = HorizontalAlignment.Center;
                    checkboxLabel.Cursor = Cursors.Hand;
                    checkboxLabel.Foreground = new BrushConverter().ConvertFrom("#FF5B5B5B") as Brush;

                    checkboxBorder.Child = checkboxLabel;
                    outerGrid.Children.Add(checkboxBorder);

                    // Assignment name
                    Label assignmentName = new Label();
                    assignmentName.SetValue(Grid.ColumnProperty, 1);
                    assignmentName.Margin = new Thickness(0, 5, 0, 0);
                    assignmentName.VerticalAlignment = VerticalAlignment.Top;
                    assignmentName.Content = data.Title;

                    TextBlock block = new TextBlock();
                    block.FontSize = 14;
                    block.Text = data.Title;
                    block.Cursor = Cursors.Hand;
                    block.MouseEnter += AssignmentMouseEnter;
                    block.MouseLeave += AssignmentMouseLeave;
                    assignmentName.Content = block;
                    outerGrid.Children.Add(assignmentName);

                    // Due date
                    Label dueDate = new Label();
                    dueDate.FontSize = 13;
                    dueDate.SetValue(Grid.ColumnProperty, 3);
                    dueDate.HorizontalAlignment = HorizontalAlignment.Right;
                    dueDate.VerticalAlignment = VerticalAlignment.Center;

                    // TODO: Sort items into different scrollviewers by date

                    dueDate.Content = dateString;
                    dueDate.Foreground = new BrushConverter().ConvertFrom("#FF717171") as Brush;
                    outerGrid.Children.Add(dueDate);

                    // Assignment type
                    Label type = new Label();
                    type.FontSize = 13;
                    type.SetValue(Grid.ColumnProperty, 2);
                    type.SetValue(Grid.RowProperty, 1);
                    type.HorizontalAlignment = HorizontalAlignment.Right;
                    type.VerticalAlignment = VerticalAlignment.Center;
                    type.Margin = new Thickness(0, -3, 0, 0);
                    type.Content = data.Type;
                    type.Foreground = new BrushConverter().ConvertFrom("#FF717171") as Brush;
                    outerGrid.Children.Add(type);

                    // Class info
                    Grid classGrid = new Grid();
                    classGrid.SetValue(Grid.RowProperty, 1);
                    classGrid.SetValue(Grid.ColumnProperty, 1);

                    ColumnDefinition classColumn1 = new ColumnDefinition();
                    classColumn1.Width = new GridLength(1, GridUnitType.Star);
                    ColumnDefinition classColumn2 = new ColumnDefinition();
                    classColumn2.Width = new GridLength(10, GridUnitType.Star);

                    classGrid.ColumnDefinitions.Add(classColumn1);
                    classGrid.ColumnDefinitions.Add(classColumn2);

                    Ellipse classEllipse = new Ellipse();
                    classEllipse.Width = 10;
                    classEllipse.Height = 10;
                    classEllipse.Margin = new Thickness(5, 0, 0, 0);
                    classEllipse.HorizontalAlignment = HorizontalAlignment.Left;
                    classEllipse.VerticalAlignment = VerticalAlignment.Center;
                    classEllipse.StrokeThickness = 0;
                    // TODO: Get class color for ellipse
                    classEllipse.Fill = new BrushConverter().ConvertFrom("#FFF37646") as Brush;
                    classGrid.Children.Add(classEllipse);

                    Label classLabel = new Label();
                    classLabel.FontSize = 13;
                    classLabel.Margin = new Thickness(-5, -3, 0, 0);
                    classLabel.HorizontalAlignment = HorizontalAlignment.Left;
                    classLabel.VerticalAlignment = VerticalAlignment.Center;
                    classLabel.SetValue(Grid.ColumnProperty, 1);
                    classLabel.Content = data.Class;
                    classGrid.Children.Add(classLabel);

                    outerGrid.Children.Add(classGrid);

                    // Add the assignment to the proper grid
                    switch (i) {
                        case 0:
                            todayHomeworkGrid.Height += 70;
                            todayHomeworkGrid.Children.Add(outerGrid);
                            break;
                        case 1:
                            tomorrowHomeworkGrid.Height += 70;
                            tomorrowHomeworkGrid.Children.Add(outerGrid);
                            break;
                        case 2:
                            nextThreeDaysHomeworkGrid.Height += 70;
                            nextThreeDaysHomeworkGrid.Children.Add(outerGrid);
                            break;
                        case 3:
                            thisWeekHomeworkGrid.Height += 70;
                            thisWeekHomeworkGrid.Children.Add(outerGrid);
                            break;
                        case 4:
                            nextWeekHomeworkGrid.Height += 70;
                            nextWeekHomeworkGrid.Children.Add(outerGrid);
                            break;
                        case 5:
                            allHomeworkGrid.Height += 70;
                            allHomeworkGrid.Children.Add(outerGrid);
                            break;
                    }
                }
            }
           
        }

        private void AssignmentMouseEnter(object sender, MouseEventArgs e) {
            TextBlock t = (TextBlock)sender;
            t.TextDecorations = TextDecorations.Underline;
        }
        private void AssignmentMouseLeave(object sender, MouseEventArgs e) {
            TextBlock t = (TextBlock)sender;
            t.TextDecorations = null;
        }

        /// <summary>
        /// Set one grid to be visible and the rest to be hidden.
        /// </summary>
        /// <param name="grid">The grid to show.</param>
        private void ChangeGridVisibility(Grid grid) {
            Grid[] grids = new Grid[] {
                todayHomeworkGrid,
                tomorrowHomeworkGrid,
                nextThreeDaysHomeworkGrid,
                thisWeekHomeworkGrid,
                nextWeekHomeworkGrid,
                allHomeworkGrid
            };

            foreach (Grid g in grids) {
                if (grid == g) {
                    g.Visibility = Visibility.Visible;
                }
                else {
                    g.Visibility = Visibility.Hidden;
                }
            }
        }

        private void ChangeAssignmentDisplay1() {
            switch (Properties.Settings.Default.AssignmentDisplay1) {
                case 0:
                    assignmentDisplay1Label.Content = "Due today";
                    ChangeGridVisibility(todayHomeworkGrid);
                    break;
                case 1:
                    assignmentDisplay1Label.Content = "Due tomorrow";
                    ChangeGridVisibility(tomorrowHomeworkGrid);
                    break;
                case 2:
                    assignmentDisplay1Label.Content = "Due in next three days";
                    ChangeGridVisibility(nextThreeDaysHomeworkGrid);
                    break;
                case 3:
                    assignmentDisplay1Label.Content = "Due this week";
                    ChangeGridVisibility(thisWeekHomeworkGrid);
                    break;
                case 4:
                    assignmentDisplay1Label.Content = "Due next week";
                    ChangeGridVisibility(nextWeekHomeworkGrid);
                    break;
                case 5:
                    assignmentDisplay1Label.Content = "All assignments";
                    ChangeGridVisibility(allHomeworkGrid);
                    break;
            }
        }

        private static bool isSwitchingAnimationRunning = false;
        private void assignmentSwitchButton_MouseDown(object sender, MouseButtonEventArgs e) {
            Properties.Settings.Default.AssignmentDisplay1++;

            if (Properties.Settings.Default.AssignmentDisplay1 == 6) {
                Properties.Settings.Default.AssignmentDisplay1 = 0;
            }
            Properties.Settings.Default.Save();

            ChangeAssignmentDisplay1();

            // ANIMATION
            if (!isSwitchingAnimationRunning) {
                isSwitchingAnimationRunning = true;
                Dispatcher.Invoke(new Action(() => {
                    ThicknessAnimation animation = new ThicknessAnimation();

                    animation.From = new Thickness(0, 0, 0, 0);
                    animation.To = new Thickness(50, 0, 0, 0);
                    animation.Duration = TimeSpan.FromSeconds(0.2);

                    assignmentSwitchButton.BeginAnimation(MarginProperty, animation);
                }));

                Thread thread = new Thread(() => {
                    Thread.Sleep(200);
                    Dispatcher.Invoke(new Action(() => {
                        assignmentSwitchButton.Margin = new Thickness(-50, 0, 0, 0);
                        ThicknessAnimation animation = new ThicknessAnimation();

                        animation.From = new Thickness(-50, 0, 0, 0);
                        animation.To = new Thickness(0, 0, 0, 0);
                        animation.Duration = TimeSpan.FromSeconds(0.2);

                        assignmentSwitchButton.BeginAnimation(MarginProperty, animation);
                        isSwitchingAnimationRunning = false;
                    }));
                });
                thread.Start();
            }

        }
    }
}
