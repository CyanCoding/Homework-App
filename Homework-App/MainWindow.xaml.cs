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

            ChangeAssignmentDisplay(1, assignmentDisplay1Label);
            ChangeAssignmentDisplay(2, assignmentDisplay2Label);
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
            data.Complete = "false";
            data.FileName = ""; // This is set when the file is read

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
        // 6: Past
        private static int[] margins = new int[] { 0, 0, 0, 0, 0, 0, 0};

        /// <summary>
        /// Adds an assignment to the homework tab
        /// </summary>
        /// <param name="data">The assignment data from the file</param>
        private void AddAssignment(Assignment.AssignmentData data) {
            // If this assignment is complete, don't add it
            if (data.Complete == "true") {
                return;
            }

            // This uses the same assumptions as margins variable
            bool inPast = true;
            bool[] addPlaces = new bool[] {
                false, false, false, false, false, true, false
            };

            // First we check the day, which changes some values later on
            // such as which grid it's sorted into and the margin height
            // on outerGrid
            DateTime today = DateTime.Today;

            // Figure out what range the assignment's due date falls in
            if (today.ToString("M/dd/yyyy") == data.Date) { // Due today
                margins[0] += 70;
                addPlaces[0] = true;
                inPast = false;
            }
            else if (today.AddDays(1).ToString("M/dd/yyyy") == data.Date) { // Due tomorrow
                margins[1] += 70;
                addPlaces[1] = true;
                inPast = false;
            }
            for (int i = 0; i < 3; i++) { // Due in next three days
                if (data.Date == today.AddDays(i).ToString("M/dd/yyyy")) {
                    margins[2] += 70;
                    addPlaces[2] = true;
                    inPast = false;
                    break;
                }
            }
            for (int i = 0; i < 7; i++) { // Due this week
                if (data.Date == today.AddDays(i).ToString("M/dd/yyyy")) {
                    margins[3] += 70;
                    addPlaces[3] = true;
                    inPast = false;
                    break;
                }
            }
            for (int i = 7; i < 14; i++) { // Due next week
                if (data.Date == today.AddDays(i).ToString("M/dd/yyyy")) {
                    margins[4] += 70;
                    addPlaces[4] = true;
                    inPast = false;
                    break;
                }
            }
            
            if (inPast) {
                // We just use 1000 as a genaric large number
                for (int i = 0; i < 1000; i++) {
                    if (data.Date == today.AddDays(i).ToString("M/dd/yyyy")) {
                        inPast = false;
                        break;
                    }
                }
                if (inPast) {
                    margins[6] += 70;
                    addPlaces[6] = true;
                }
            }

            // Update all assignments margin
            margins[5] += 70;

            // TODO: Figure out how to format this another way
            string dateString = data.Date;
            // Attach time to date ("3/2/2222 - 2:00 PM")
            if (data.Time != "") {
                dateString += " - " + data.Time;
            }

            for (int i = 0; i < addPlaces.Length; i++) {
                if (addPlaces[i]) { // Only add an item if it rests in that date range

                    // We create two assignments because you can't have
                    // an assignment as the child of both our Scrollviewer grids
                    for (int j = 0; j < 2; j++) {
                        // Outer grid
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
                        checkboxBorder.Name = "checkboxBorder";
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
                        checkboxLabel.MouseDown += AssignmentCompleted;
                        if (j == 0) {
                            checkboxLabel.Name = "n" + data.FileName; // Used to identify file of Scrollviewer 1
                        }
                        else {
                            checkboxLabel.Name = "m" + data.FileName; // Used to identify file of Scrollviewer 2
                        }
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
                        Grid todayGrid;
                        Grid tomGrid;
                        Grid threeGrid;
                        Grid weekGrid;
                        Grid nextWeekGrid;
                        Grid allGrid;
                        Grid pastGrid;
                        if (j == 0) {
                            todayGrid = todayHomeworkGrid;
                            tomGrid = tomorrowHomeworkGrid;
                            threeGrid = nextThreeDaysHomeworkGrid;
                            weekGrid = thisWeekHomeworkGrid;
                            nextWeekGrid = nextWeekHomeworkGrid;
                            allGrid = allHomeworkGrid;
                            pastGrid = pastHomeworkGrid;
                        }
                        else {
                            todayGrid = todayHomeworkGrid2;
                            tomGrid = tomorrowHomeworkGrid2;
                            threeGrid = nextThreeDaysHomeworkGrid2;
                            weekGrid = thisWeekHomeworkGrid2;
                            nextWeekGrid = nextWeekHomeworkGrid2;
                            allGrid = allHomeworkGrid2;
                            pastGrid = pastHomeworkGrid2;
                        }

                        // Figure out which grid to add to
                        switch (i) {
                            case 0:
                                todayGrid.Height += 70;
                                todayGrid.Children.Add(outerGrid);
                                break;
                            case 1:
                                tomGrid.Height += 70;
                                tomGrid.Children.Add(outerGrid);
                                break;
                            case 2:
                                threeGrid.Height += 70;
                                threeGrid.Children.Add(outerGrid);
                                break;
                            case 3:
                                weekGrid.Height += 70;
                                weekGrid.Children.Add(outerGrid);
                                break;
                            case 4:
                                nextWeekGrid.Height += 70;
                                nextWeekGrid.Children.Add(outerGrid);
                                break;
                            case 5:
                                allGrid.Height += 70;
                                allGrid.Children.Add(outerGrid);
                                break;
                            case 6:
                                pastGrid.Height += 70;
                                pastGrid.Children.Add(outerGrid);
                                break;
                        }
                    }
                }
            }
           
        }

        private void Label_MouseEnter(object sender, MouseEventArgs e) {
            Label box = (Label)sender;
            box.Content = "✔";
        }

        private void Label_MouseLeave(object sender, MouseEventArgs e) {
            Label box = (Label)sender;
            box.Content = "";
        }

        /// <summary>
        /// When the user presses the checkbox or otherwise
        /// completes an assignment.
        /// </summary>
        private void AssignmentCompleted(object sender, MouseButtonEventArgs e) {
            Label l = (Label)sender;

            // Example name: n133443 (n is used to make it a valid name)
            string fileName = l.Name.Substring(1, l.Name.Length - 1);
            fileName += ".json"; // Now we have the file name! (e.g. 133443.json)

            //Assignment.MarkAssignmentCompleted(fileName);

            // Hides the assignment by hiding the parents of the checkbox label
            Border? checkboxParent = l.Parent as Border;
            if (checkboxParent != null) {
                Grid? outerGrid = checkboxParent.Parent as Grid;
                if (outerGrid != null) {
                    outerGrid.Visibility = Visibility.Hidden;
                }
            }

            // Hide assignment from other scrollviewer
            string nameToFind = nameToFind = l.Name.Substring(1, l.Name.Length - 1);

            HideAssignment(nameToFind);
        }

        private void HideAssignment(string searchName) {
            Grid[] gridsToHide = new Grid[] {
                todayHomeworkGrid2,
                tomorrowHomeworkGrid2,
                nextThreeDaysHomeworkGrid2,
                thisWeekHomeworkGrid2,
                nextThreeDaysHomeworkGrid2,
                allHomeworkGrid2,
                pastHomeworkGrid2,

                todayHomeworkGrid,
                tomorrowHomeworkGrid,
                nextThreeDaysHomeworkGrid,
                thisWeekHomeworkGrid,
                nextWeekHomeworkGrid,
                allHomeworkGrid,
                pastHomeworkGrid
            };

            foreach (Grid grid in gridsToHide) {
                // Locates all the assignment grids from the scrollviewer sub-grid
                foreach (object child in grid.Children) {
                    Grid? childGrid = child as Grid;

                    if (childGrid != null) {
                        // Gets each object from the assignment grid
                        foreach (object c in childGrid.Children) {
                            // Checkbox grids are Border objects with the name "checkboxBorder"
                            if (c is Border && ((Border)c).Name == "checkboxBorder") {
                                Label label = (Label)((Border)c).Child;

                                // If the label name matches nameToFind, it's the
                                // duplicate we're looking for.

                                // 'm' and 'n' here are used to designate which scrollviewer
                                // an item is in.
                                if (label.Name == "m" + searchName || label.Name == "n" + searchName) {
                                    // Hide the duplicate
                                    childGrid.Visibility = Visibility.Hidden;
                                }
                            }
                        }

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
        /// <param name="gridNum">1 == firstGrids, 2 == secondGrids</param>
        private void ChangeGridVisibility(Grid grid, int gridNum) {
            Grid[] firstGrids = new Grid[] {
                todayHomeworkGrid,
                tomorrowHomeworkGrid,
                nextThreeDaysHomeworkGrid,
                thisWeekHomeworkGrid,
                nextWeekHomeworkGrid,
                allHomeworkGrid,
                pastHomeworkGrid
            };

            Grid[] secondGrids = new Grid[] {
                todayHomeworkGrid2,
                tomorrowHomeworkGrid2,
                nextThreeDaysHomeworkGrid2,
                thisWeekHomeworkGrid2,
                nextWeekHomeworkGrid2,
                allHomeworkGrid2,
                pastHomeworkGrid2
            };

            if (gridNum == 1) {
                foreach (Grid g in firstGrids) {
                    if (grid == g) {
                        g.Visibility = Visibility.Visible;
                    }
                    else {
                        g.Visibility = Visibility.Hidden;
                    }
                }
            }
            else {
                foreach (Grid g in secondGrids) {
                    if (grid == g) {
                        g.Visibility = Visibility.Visible;
                    }
                    else {
                        g.Visibility = Visibility.Hidden;
                    }
                }
            }

        }

        /// <summary>
        /// Changes the display of a desired assignment grid
        /// </summary>
        /// <param name="gridNum">The index of the grid to change (1 or 2).</param>
        private void ChangeAssignmentDisplay(int gridNum, Label l) {
            int propertyVal;

            // Choose which setting to use based on Scrollviewer grid
            if (gridNum == 1) {
                propertyVal = Properties.Settings.Default.AssignmentDisplay1;
            }
            else {
                propertyVal = Properties.Settings.Default.AssignmentDisplay2;
            }

            switch (propertyVal) {
                case 0:
                    l.Content = "Due today";
                    if (gridNum == 1) {
                        ChangeGridVisibility(todayHomeworkGrid, gridNum);
                    }
                    else {
                        ChangeGridVisibility(todayHomeworkGrid2, gridNum);
                    }
                    break;
                case 1:
                    l.Content = "Due tomorrow";
                    if (gridNum == 1) {
                        ChangeGridVisibility(tomorrowHomeworkGrid, gridNum);
                    }
                    else {
                        ChangeGridVisibility(tomorrowHomeworkGrid2, gridNum);
                    }
                    break;
                case 2:
                    l.Content = "Due in next three days";
                    if (gridNum == 1) {
                        ChangeGridVisibility(nextThreeDaysHomeworkGrid, gridNum);
                    }
                    else {
                        ChangeGridVisibility(nextThreeDaysHomeworkGrid2, gridNum);
                    }
                    break;
                case 3:
                    l.Content = "Due this week";
                    if (gridNum == 1) {
                        ChangeGridVisibility(thisWeekHomeworkGrid, gridNum);
                    }
                    else {
                        ChangeGridVisibility(thisWeekHomeworkGrid2, gridNum);
                    }
                    break;
                case 4:
                    l.Content = "Due next week";
                    if (gridNum == 1) {
                        ChangeGridVisibility(nextWeekHomeworkGrid, gridNum);
                    }
                    else {
                        ChangeGridVisibility(nextWeekHomeworkGrid2, gridNum);
                    }
                    break;
                case 5:
                    l.Content = "All assignments";
                    if (gridNum == 1) {
                        ChangeGridVisibility(allHomeworkGrid, gridNum);
                    }
                    else {
                        ChangeGridVisibility(allHomeworkGrid2, gridNum);
                    }
                    break;
                case 6:
                    l.Content = "Past assignments";
                    if (gridNum == 1) {
                        ChangeGridVisibility(pastHomeworkGrid, gridNum);
                    }
                    else {
                        ChangeGridVisibility(pastHomeworkGrid2, gridNum);
                    }
                    break;
            }
        }

        private static bool isSwitchingAnimationRunning = false;
        /// <summary>
        /// When the user switches the Scrollviewer display option
        /// by clicking on the little two-arrow button
        /// </summary>
        private void assignmentSwitchButton_MouseDown(object sender, MouseButtonEventArgs e) {
            // We get the name of the object to tell us if we're operating
            // on the first grid or the second
            Label l = (Label)sender;
            if (l.Name == "assignmentSwitchButton") {
                Properties.Settings.Default.AssignmentDisplay1++;

                if (Properties.Settings.Default.AssignmentDisplay1 == 7) {
                    Properties.Settings.Default.AssignmentDisplay1 = 0;
                }

                ChangeAssignmentDisplay(1, assignmentDisplay1Label);
            }
            else {
                Properties.Settings.Default.AssignmentDisplay2++;

                if (Properties.Settings.Default.AssignmentDisplay2 == 7) {
                    Properties.Settings.Default.AssignmentDisplay2 = 0;
                }

                ChangeAssignmentDisplay(2, assignmentDisplay2Label);
            }

            Properties.Settings.Default.Save();

            // ANIMATION
            if (!isSwitchingAnimationRunning) {
                isSwitchingAnimationRunning = true;
                Dispatcher.Invoke(new Action(() => {
                    ThicknessAnimation animation = new ThicknessAnimation();

                    animation.From = new Thickness(0, 0, 0, 0);
                    animation.To = new Thickness(50, 0, 0, 0);
                    animation.Duration = TimeSpan.FromSeconds(0.2);

                    l.BeginAnimation(MarginProperty, animation);
                }));

                Thread thread = new Thread(() => {
                    Thread.Sleep(200);
                    Dispatcher.Invoke(new Action(() => {
                        l.Margin = new Thickness(-50, 0, 0, 0);
                        ThicknessAnimation animation = new ThicknessAnimation();

                        animation.From = new Thickness(-50, 0, 0, 0);
                        animation.To = new Thickness(0, 0, 0, 0);
                        animation.Duration = TimeSpan.FromSeconds(0.2);

                        l.BeginAnimation(MarginProperty, animation);
                        isSwitchingAnimationRunning = false;
                    }));
                });
                thread.Start();
            }

        }
    }
}
