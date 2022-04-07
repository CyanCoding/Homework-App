using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Homework_App.Pages; 

public partial class Homework : Page {
    public Homework() {
        InitializeComponent();
        
        ChangeAssignmentDisplay(1, AssignmentDisplay1Label);
        ChangeAssignmentDisplay(2, AssignmentDisplay2Label);
        LoadAssignmentsFromFile();
    }
    
    /// <summary>
    /// Executed when the window is resized
    /// </summary>
    private void Window_SizeChanged(object sender, SizeChangedEventArgs e) {
        var height = (int)this.ActualHeight;
        //int width = (int)this.ActualWidth;

        // Change height of assignment grids
        AssignmentGrid1.Height = height - 175;
        AssignmentGrid2.Height = height - 55;
    }
    
    /// <summary>
    /// Loads assignments into their grids from assignment files.
    /// </summary>
    private void LoadAssignmentsFromFile() {
        var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        path += "/Homework-App/assignment";
        var d = new DirectoryInfo(path);

        foreach (var file in d.GetFiles("*.json")) {
            var data = Assignment.ReadAssignment(file.FullName);
            AddAssignment(data);
        }
    }

    /// <summary>
    /// Adds an assignment to the homework tab
    /// </summary>
    /// <param name="data">The assignment data from the file</param>
    // TODO: Move this function to Assignment class
    // 0: Today
    // 1: Tomorrow
    // 2: Next three days
    // 3: This week
    // 4: Next week
    // 5: All
    // 6: Past
    private static readonly int[] Margins = new int[] { 0, 0, 0, 0, 0, 0, 0};
    private void AddAssignment(Assignment.AssignmentData data) {
        // If this assignment is complete, don't add it
        if (data.Complete == "true") {
            return;
        }

        // This uses the same assumptions as margins variable
        var inPast = true;
        var addPlaces = new[] {
            false, false, false, false, false, true, false
        };

        // First we check the day, which changes some values later on
        // such as which grid it's sorted into and the margin height
        // on outerGrid
        var today = DateTime.Today;

        // We use "Today" and "Tomorrow" instead of dates
        var dueToday = false;
        var dueTomorrow = false;

        // Figure out what range the assignment's due date falls in
        if (today.ToString("M/dd/yyyy") == data.Date) { // Due today
            Margins[0] += 70;
            addPlaces[0] = true;
            inPast = false;
            dueToday = true;
        }
        else if (today.AddDays(1).ToString("M/dd/yyyy") == data.Date) { // Due tomorrow
            Margins[1] += 70;
            addPlaces[1] = true;
            inPast = false;
            dueTomorrow = true;
        }
        for (var i = 0; i < 3; i++) { // Due in next three days
            if (data.Date == today.AddDays(i).ToString("M/dd/yyyy")) {
                Margins[2] += 70;
                addPlaces[2] = true;
                inPast = false;
                break;
            }
        }
        for (var i = 0; i < 7; i++) { // Due this week
            if (data.Date == today.AddDays(i).ToString("M/dd/yyyy")) {
                Margins[3] += 70;
                addPlaces[3] = true;
                inPast = false;
                break;
            }
        }
        for (var i = 7; i < 14; i++) { // Due next week
            if (data.Date == today.AddDays(i).ToString("M/dd/yyyy")) {
                Margins[4] += 70;
                addPlaces[4] = true;
                inPast = false;
                break;
            }
        }
        
        if (inPast) {
            // We just use 1000 as a generic large number
            for (var i = 0; i < 1000; i++) {
                if (data.Date == today.AddDays(i).ToString("M/dd/yyyy")) {
                    inPast = false;
                    break;
                }
            }
            if (inPast) {
                Margins[6] += 70;
                addPlaces[6] = true;
            }
        }

        // Update all assignments margin
        Margins[5] += 70;

        // TODO: Figure out how to format this another way
        string dateString;
        if (dueToday) {
            dateString = "Today";
        }
        else if (dueTomorrow) {
            dateString = "Tomorrow";
        }
        else {
            dateString = data.Date;
        }
        
        // Attach time to date ("3/2/2222 - 2:00 PM")
        if (data.Time != "") {
            dateString += " - " + data.Time;
        }

        for (var i = 0; i < addPlaces.Length; i++) {
            if (addPlaces[i]) { // Only add an item if it rests in that date range

                // We create two assignments because you can't have
                // an assignment as the child of both our Scroll-viewer grids
                for (var j = 0; j < 2; j++) {
                    // Outer grid
                    var outerGrid = new Grid {
                        Height = 60,
                        // We take - 70 because the first one would be at 0
                        Margin = new Thickness(0, Margins[i] - 70, 0, 0),
                        VerticalAlignment = VerticalAlignment.Top,
                        Name = "b" + data.FileName
                    };

                    var column1 = new ColumnDefinition {
                        Width = new GridLength(1, GridUnitType.Star)
                    };
                    var column2 = new ColumnDefinition {
                        Width = new GridLength(7, GridUnitType.Star)
                    };
                    var column3 = new ColumnDefinition {
                        Width = new GridLength(5, GridUnitType.Star)
                    };

                    var row1 = new RowDefinition {
                        Height = new GridLength(3, GridUnitType.Star)
                    };
                    var row2 = new RowDefinition {
                        Height = new GridLength(2, GridUnitType.Star)
                    };

                    outerGrid.ColumnDefinitions.Add(column1);
                    outerGrid.ColumnDefinitions.Add(column2);
                    outerGrid.ColumnDefinitions.Add(column3);

                    outerGrid.RowDefinitions.Add(row1);
                    outerGrid.RowDefinitions.Add(row2);

                    // Outside border
                    var outsideBorder = new Border();
                    outsideBorder.SetValue(Grid.ColumnSpanProperty, 3);
                    outsideBorder.SetValue(Grid.RowSpanProperty, 3);
                    outsideBorder.Margin = new Thickness(0, 0, 0, -5);
                    outsideBorder.BorderThickness = new Thickness(0, 0, 0, 1);
                    outsideBorder.BorderBrush = new BrushConverter().ConvertFrom("#FF6C6C6C") as Brush;
                    outsideBorder.Background = new SolidColorBrush(Colors.White);
                    outerGrid.Children.Add(outsideBorder);

                    // Check box
                    var checkboxBorder = new Border {
                        Name = "checkboxBorder",
                        HorizontalAlignment = HorizontalAlignment.Right,
                        Margin = new Thickness(0, 5, 5, 0),
                        Width = 18,
                        Height = 18
                    };
                    checkboxBorder.SetValue(Grid.ColumnProperty, 0);

                    var checkBoxBrush = "#FF707070";
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

                    var checkboxLabel = new Label();
                    checkboxLabel.MouseEnter += Checkmark_MouseEnter;
                    checkboxLabel.MouseLeave += Checkmark_MouseLeave;
                    checkboxLabel.MouseDown += AssignmentCompleted;
                    checkboxLabel.FontSize = 8;
                    checkboxLabel.Margin = new Thickness(-3);
                    checkboxLabel.VerticalAlignment = VerticalAlignment.Center;
                    checkboxLabel.HorizontalAlignment = HorizontalAlignment.Center;
                    checkboxLabel.Cursor = Cursors.Hand;
                    checkboxLabel.Foreground = new BrushConverter().ConvertFrom("#FF5B5B5B") as Brush;
                    checkboxLabel.Name = "c" + data.FileName;

                    checkboxBorder.Child = checkboxLabel;
                    outerGrid.Children.Add(checkboxBorder);

                    // Assignment name
                    var assignmentName = new Label();
                    assignmentName.SetValue(Grid.ColumnProperty, 1);
                    assignmentName.Margin = new Thickness(0, 5, 0, 0);
                    assignmentName.VerticalAlignment = VerticalAlignment.Top;
                    assignmentName.Content = data.Title;

                    var block = new TextBlock {
                        FontSize = 14,
                        Text = data.Title,
                        Cursor = Cursors.Hand
                    };
                    block.MouseEnter += AssignmentMouseEnter;
                    block.MouseLeave += AssignmentMouseLeave;
                    assignmentName.Content = block;
                    outerGrid.Children.Add(assignmentName);

                    // Due date
                    var dueDate = new Label {
                        FontSize = 13,
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Center,
                        Content = dateString,
                        Foreground = new BrushConverter().ConvertFrom("#FF717171") as Brush,
                    };
                    dueDate.SetValue(Grid.ColumnProperty, 3);
                    outerGrid.Children.Add(dueDate);

                    // Assignment type
                    var type = new Label {
                        FontSize = 13,
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(0, -3, 0, 0),
                        Content = data.Type,
                        Foreground = new BrushConverter().ConvertFrom("#FF717171") as Brush
                    };
                    type.SetValue(Grid.ColumnProperty, 2);
                    type.SetValue(Grid.RowProperty, 1);
                    outerGrid.Children.Add(type);

                    // Class info
                    var classGrid = new Grid();
                    classGrid.SetValue(Grid.RowProperty, 1);
                    classGrid.SetValue(Grid.ColumnProperty, 1);

                    var classColumn1 = new ColumnDefinition {
                        Width = new GridLength(1, GridUnitType.Star)
                    };
                    var classColumn2 = new ColumnDefinition {
                        Width = new GridLength(10, GridUnitType.Star)
                    };

                    classGrid.ColumnDefinitions.Add(classColumn1);
                    classGrid.ColumnDefinitions.Add(classColumn2);

                    // Here we figure out the color for that class
                    // We also make sure the class exists
                    var tuple = Classes.GetClassesNameAndColor();
                    var color = "";
                    
                    // We currently have class as something like "e1". We need to convert
                    // that back to a real name
                    var realName = "";
                    for (int k = 0; k < tuple.Item1.Length; k++) {
                        if (data.Class == tuple.Item3[k]) {
                            color = tuple.Item2[k];
                            realName = tuple.Item1[k];
                        }
                    }

                    var classEllipse = new Ellipse {
                        Width = 10,
                        Height = 10,
                        Margin = new Thickness(5, 0, 0, 0),
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Center,
                        StrokeThickness = 0
                    };
                    // We couldn't match that class name with an actual class, maybe it was deleted?
                    if (color == "") {
                        realName = "";
                    }
                    else {
                        classEllipse.Fill =
                            new BrushConverter().ConvertFrom(Classes.GetHexFromColor(color)) as Brush;
                    }
                    classGrid.Children.Add(classEllipse);

                    var classLabel = new Label {
                        FontSize = 13,
                        Margin = new Thickness(-5, -3, 0, 0),
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Center,
                        Content = realName
                    };
                    classLabel.SetValue(Grid.ColumnProperty, 1);
                    classGrid.Children.Add(classLabel);

                    if (data.Class == "") {
                        classEllipse.Visibility = Visibility.Hidden;
                    }

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
                        todayGrid = TodayHomeworkGrid;
                        tomGrid = TomorrowHomeworkGrid;
                        threeGrid = NextThreeDaysHomeworkGrid;
                        weekGrid = ThisWeekHomeworkGrid;
                        nextWeekGrid = NextWeekHomeworkGrid;
                        allGrid = AllHomeworkGrid;
                        pastGrid = PastHomeworkGrid;
                    }
                    else {
                        todayGrid = TodayHomeworkGrid2;
                        tomGrid = TomorrowHomeworkGrid2;
                        threeGrid = NextThreeDaysHomeworkGrid2;
                        weekGrid = ThisWeekHomeworkGrid2;
                        nextWeekGrid = NextWeekHomeworkGrid2;
                        allGrid = AllHomeworkGrid2;
                        pastGrid = PastHomeworkGrid2;
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
    
    private void AssignmentMouseEnter(object sender, MouseEventArgs e) {
        var t = (TextBlock)sender;
        t.TextDecorations = TextDecorations.Underline;
    }
    private void AssignmentMouseLeave(object sender, MouseEventArgs e) {
        var t = (TextBlock)sender;
        t.TextDecorations = null;
    }
    
    /// <summary>
    /// When the user presses the checkbox or otherwise
    /// completes an assignment.
    /// </summary>
    private void AssignmentCompleted(object sender, MouseButtonEventArgs e) {
        var l = (Label)sender;
            
        // Example name: n133443 (n is used to make it a valid name) 
        string fileName = l.Name.Substring(1, l.Name.Length - 1); 
        fileName += ".json"; // Now we have the file name! (e.g. 133443.json) 
 
        Assignment.MarkAssignmentCompleted(fileName); 

        // Hides the assignment by hiding the parents of the checkbox label
        if (l.Parent is Border {Parent: Grid outerGrid}) {
            outerGrid.Visibility = Visibility.Hidden;
        }

        // Hide assignment from other Scroll-viewer
        var nameToFind = "b" + l.Name.Substring(1, l.Name.Length - 1);

        HideAssignment(nameToFind);
    }
    
            /// <summary>
        /// Searches the grids for an assignment and hides it
        /// </summary>
        /// <param name="searchName">The number to search for (e.g. '65543')</param>
        private void HideAssignment(string searchName) {
            var gridsToHide = new[] {
                TodayHomeworkGrid2,
                TomorrowHomeworkGrid2,
                NextThreeDaysHomeworkGrid2,
                ThisWeekHomeworkGrid2,
                NextThreeDaysHomeworkGrid2,
                AllHomeworkGrid2,
                PastHomeworkGrid2,

                TodayHomeworkGrid,
                TomorrowHomeworkGrid,
                NextThreeDaysHomeworkGrid,
                ThisWeekHomeworkGrid,
                NextWeekHomeworkGrid,
                AllHomeworkGrid,
                PastHomeworkGrid
            };

            // With this we'll know which grids the assignment
            // to hide is in - that way we know which margins
            // we have to change.
            var assignmentPresentGrids = new Grid[14];

            var i = 0;
            var marginHeight = 0.0; // This will equal our grid's margin height
            // First we hide all the assignments from the grids
            foreach (var grid in gridsToHide) {
                // b21234 indicates the grid name
                foreach (var child in grid.Children) {
                    if (((Grid)child).Name == searchName) { 
                        ((Grid)child).Visibility = Visibility.Hidden;
                        marginHeight = ((Grid)child).Margin.Top;
                        assignmentPresentGrids[i] = grid;
                        i++;
                    }
                }
            }

            // Now we need to update the other assignments in each
            // grid the assignment WAS present in because
            // there's a gap. So we update their margins
            foreach (var grid in assignmentPresentGrids) {
                if (grid != null) {
                    foreach (var child in grid.Children) {
                        if (child is Grid grid1) {
                            var thick = grid1.Margin;
                
                            // We only lower assignments that are above the current one
                            if (thick.Top >= marginHeight) { // 0 top thickness means it was already at the top
                                thick.Top -= 70;
                            }

                            grid1.Margin = thick; // Update thickness
                        }
                    } 
                }
            }
        }
    
    private void Checkmark_MouseEnter(object sender, MouseEventArgs e) {
        var box = (Label)sender;
        box.Content = "✔";
    }

    private void Checkmark_MouseLeave(object sender, MouseEventArgs e) {
        var box = (Label)sender;
        box.Content = "";
    }
    
    /// <summary>
    /// Changes the display of a desired assignment grid
    /// </summary>
    /// <param name="gridNum">The index of the grid to change (1 or 2).</param>
    /// <param name="l">The label to change</param>
    private void ChangeAssignmentDisplay(int gridNum, Label l) {
        // Choose which setting to use based on Scroll-viewer grid
        var propertyVal = gridNum == 1 ? Properties.Settings.Default.AssignmentDisplay1 : 
            Properties.Settings.Default.AssignmentDisplay2;

        switch (propertyVal) {
            case 0:
                l.Content = "Due today";
                ChangeGridVisibility(gridNum == 1 ? TodayHomeworkGrid :
                    TodayHomeworkGrid2, gridNum);
                break;
            case 1:
                l.Content = "Due tomorrow";
                ChangeGridVisibility(gridNum == 1 ? TomorrowHomeworkGrid :
                    TomorrowHomeworkGrid2, gridNum);
                break;
            case 2:
                l.Content = "Due in next three days";
                ChangeGridVisibility(gridNum == 1 ? NextThreeDaysHomeworkGrid :
                        NextThreeDaysHomeworkGrid2,
                    gridNum);
                break;
            case 3:
                l.Content = "Due this week";
                ChangeGridVisibility(gridNum == 1 ? ThisWeekHomeworkGrid :
                    ThisWeekHomeworkGrid2, gridNum);
                break;
            case 4:
                l.Content = "Due next week";
                ChangeGridVisibility(gridNum == 1 ? NextWeekHomeworkGrid :
                    NextWeekHomeworkGrid2, gridNum);
                break;
            case 5:
                l.Content = "All assignments";
                ChangeGridVisibility(gridNum == 1 ? AllHomeworkGrid : AllHomeworkGrid2, gridNum);
                break;
            case 6:
                l.Content = "Past assignments";
                ChangeGridVisibility(gridNum == 1 ? PastHomeworkGrid : PastHomeworkGrid2, gridNum);
                break;
        }
    }

    public static bool _isSwitchingAnimationRunning;

    /// <summary>
    /// When the user switches the Scroll-viewer display option
    /// by clicking on the little two-arrow button
    /// </summary>
    private void assignmentSwitchButton_MouseDown(object sender, MouseButtonEventArgs e) {
        // We get the name of the object to tell us if we're operating
        // on the first grid or the second
        var l = (Label)sender;
        if (l.Name == "AssignmentSwitchButton") {
            Properties.Settings.Default.AssignmentDisplay1++;

            if (Properties.Settings.Default.AssignmentDisplay1 == 7) {
                Properties.Settings.Default.AssignmentDisplay1 = 0;
            }

            ChangeAssignmentDisplay(1, AssignmentDisplay1Label);
        }
        else {
            Properties.Settings.Default.AssignmentDisplay2++;

            if (Properties.Settings.Default.AssignmentDisplay2 == 7) {
                Properties.Settings.Default.AssignmentDisplay2 = 0;
            }

            ChangeAssignmentDisplay(2, AssignmentDisplay2Label);
        }

        Properties.Settings.Default.Save();

        // ANIMATION
        if (!_isSwitchingAnimationRunning) {
            _isSwitchingAnimationRunning = true;
            Dispatcher.Invoke(() => {
                var animation = new ThicknessAnimation {
                    From = new Thickness(0, 0, 0, 0),
                    To = new Thickness(50, 0, 0, 0),
                    Duration = TimeSpan.FromSeconds(0.2)
                };

                l.BeginAnimation(MarginProperty, animation);
            });

            var thread = new Thread(() => {
                Thread.Sleep(200);
                Dispatcher.Invoke(() => {
                    l.Margin = new Thickness(-50, 0, 0, 0);
                    var animation = new ThicknessAnimation {
                        From = new Thickness(-50, 0, 0, 0),
                        To = new Thickness(0, 0, 0, 0),
                        Duration = TimeSpan.FromSeconds(0.2)
                    };

                    l.BeginAnimation(MarginProperty, animation);
                    _isSwitchingAnimationRunning = false;
                });
            });
            thread.Start();
        }

    }
    
    /// <summary>
    /// Checks if the user has filled in the assignment title AND datepicker
    /// </summary>
    /// <returns>True if both are filled in.</returns>
    private bool AssignmentFilledDetails() {
        var returnVal = true;
        // Title text box is required
        if (AssignmentTitle.Text == "") {
            AssignmentTitle.BorderBrush = new BrushConverter().ConvertFrom("#FFAA2929") as Brush;
            AssignmentTitleRequiredLabel.Visibility = Visibility.Visible;
            returnVal = false;
        } else {
            AssignmentTitle.BorderBrush = new BrushConverter().ConvertFrom("#FFABADB3") as Brush;
            AssignmentTitleRequiredLabel.Visibility = Visibility.Hidden;
        }
        // Valid calendar day is required
        if (AssignmentCalendar.SelectedDate == null) {
            AssignmentCalendar.BorderBrush = new BrushConverter().ConvertFrom("#FFAA2929") as Brush;
            AssignmentCalendarRequiredLabel.Visibility = Visibility.Visible;
            returnVal = false;
        } else {
            AssignmentCalendar.BorderBrush = new BrushConverter().ConvertFrom("#FFABADB3") as Brush;
            AssignmentCalendarRequiredLabel.Visibility = Visibility.Hidden;
        }

        return returnVal;
    }
    
    /// <summary>
    /// Set one grid to be visible and the rest to be hidden.
    /// </summary>
    /// <param name="grid">The grid to show.</param>
    /// <param name="gridNum">1 == firstGrids, 2 == secondGrids</param>
    private void ChangeGridVisibility(Grid grid, int gridNum) {
        var firstGrids = new[] {
            TodayHomeworkGrid,
            TomorrowHomeworkGrid,
            NextThreeDaysHomeworkGrid,
            ThisWeekHomeworkGrid,
            NextWeekHomeworkGrid,
            AllHomeworkGrid,
            PastHomeworkGrid
        };

        var secondGrids = new[] {
            TodayHomeworkGrid2,
            TomorrowHomeworkGrid2,
            NextThreeDaysHomeworkGrid2,
            ThisWeekHomeworkGrid2,
            NextWeekHomeworkGrid2,
            AllHomeworkGrid2,
            PastHomeworkGrid2
        };

        if (gridNum == 1) {
            foreach (var g in firstGrids) {
                g.Visibility = grid == g ? Visibility.Visible : Visibility.Hidden;
            }
        }
        else {
            foreach (var g in secondGrids) {
                g.Visibility = grid == g ? Visibility.Visible : Visibility.Hidden;
            }
        }

    }
    
    private void newAssignmentButton_Click(object sender, RoutedEventArgs e) {
        // We initialize the class combo box because that might have changed
        AssignmentClass.Items.Clear();
        var blankItem = new ComboBoxItem {
            Content = ""
        };
        AssignmentClass.Items.Add(blankItem);
            
        var tuple = Classes.GetClassesNameAndColor();

        for (int i = 0; i < tuple.Item1.Length; i++) {
            // Outer class grid
            var comboGrid = new Grid {
                Name = tuple.Item3[i]
            };
            var column1 = new ColumnDefinition {
                Width = new GridLength(1, GridUnitType.Star)
            };
            var column2 = new ColumnDefinition {
                Width = new GridLength(10, GridUnitType.Star)
            };

            comboGrid.ColumnDefinitions.Add(column1);
            comboGrid.ColumnDefinitions.Add(column2);
            
            // Color ellipse
            var color = new Ellipse {
                Margin = new Thickness(3, -3, 0, 0),
                Height = 10,
                Width = 10,
                StrokeThickness = 0,
                Fill = new BrushConverter().ConvertFrom(Classes.GetHexFromColor(tuple.Item2[i])) as Brush
            };
            comboGrid.Children.Add(color);

            var label = new Label {
                FontSize = 13,
                Margin = new Thickness(0, -4, 0, 0),
                Content = tuple.Item1[i]
            };
            label.SetValue(Grid.ColumnProperty, 1);
            comboGrid.Children.Add(label);
                
            // Add the whole grid to our class combo box
            AssignmentClass.Items.Add(comboGrid);
        }

            
        NewAssignmentGrid.Visibility = Visibility.Visible;
    }
    
    private void cancelButton_Click(object sender, RoutedEventArgs e) {
        NewAssignmentGrid.Visibility = Visibility.Hidden;
    }
    
    
    /// <summary>
    /// Saves and clears values (but doesn't close window).
    /// </summary>
    private void saveAndAddButton_Click(object sender, RoutedEventArgs e) {
        // Make sure everything required is filled
        if (!AssignmentFilledDetails()) {
            return;
        }

        // Add assignment
        var data = new Assignment.AssignmentData {
            Title = AssignmentTitle.Text,
            Type = AssignmentType.Text,
            Class = ((Grid)AssignmentClass.SelectedItem).Name,
            Date = AssignmentCalendar.Text,
            Time = AssignmentTime.Text,
            Priority = AssignmentPriority.Text,
            Repeat = AssignmentRepeat.Text,
            Reminder = AssignmentReminder.Text,
            Notes = AssignmentNotes.Text,
            Complete = "false",
            FileName = "" // This is set when the file is read
        };


        Assignment.CreateAssignment(data);

        // Clear values
        AssignmentTitle.Text = "";
        AssignmentType.SelectedIndex = 0;
        AssignmentClass.SelectedIndex = 0;
        AssignmentCalendar.Text = "";
        AssignmentTime.Text = "";
        AssignmentPriority.SelectedIndex = 0;
        AssignmentRepeat.SelectedIndex = 0;
        AssignmentReminder.SelectedIndex = 0;
        AssignmentNotes.Text = "";

        AddAssignment(data);
    }

    /// <summary>
    /// This does the same thing as saveAndAddButton except
    /// it closes the window after we save.
    /// </summary>
    private void saveButton_Click(object sender, RoutedEventArgs e) {
        saveAndAddButton_Click(sender, e);

        NewAssignmentGrid.Visibility = Visibility.Hidden; // Hide assignment window
    }
    
    private void AssignmentTitle_OnTextChanged(object sender, TextChangedEventArgs e) {
        var input = AssignmentTitle.Text.ToLower();

        if (input.Contains("homework") || input.Contains("assignment")) {
            AssignmentType.SelectedIndex = 1;
        }
        if (input.Contains("lesson") || input.Contains("module")) {
            AssignmentType.SelectedIndex = 2;
        }
        if (input.Contains("paper")) {
            AssignmentType.SelectedIndex = 3;
        }
        if (input.Contains("video")) {
            AssignmentType.SelectedIndex = 4;
        }
        if (input.Contains("slideshow")) {
            AssignmentType.SelectedIndex = 5;
        }
        if (input.Contains("worksheet")) {
            AssignmentType.SelectedIndex = 6;
        }
        if (input.Contains("presentation")) {
            AssignmentType.SelectedIndex = 7;
        }
        if (input.Contains("project")) {
            AssignmentType.SelectedIndex = 8;
        }
        if (input.Contains("group")) { // We use group instead of group project because group is more likely
            AssignmentType.SelectedIndex = 9;
        }
        if (input.Contains("discussion")) {
            AssignmentType.SelectedIndex = 10;
        }
        if (input.Contains("lab")) {
            AssignmentType.SelectedIndex = 12;
        }
        if (input.Contains("quiz")) {
            AssignmentType.SelectedIndex = 13;
        }
        if (input.Contains("exam")) {
            AssignmentType.SelectedIndex = 14;
        }
        if (input.Contains("midterm")) {
            AssignmentType.SelectedIndex = 15;
        }
        if (input.Contains("final")) {
            AssignmentType.SelectedIndex = 16;
        }
        // We put study as the last item here because someone might type "Study for exam" and want
        // to categorize it as study and not exam.
        if (input.Contains("study")) {
            AssignmentType.SelectedIndex = 11;
        }
    }
}