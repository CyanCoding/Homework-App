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

        /// <summary>
        /// Adds an assignment to the homework tab
        /// </summary>
        /// <param name="data">The assignment data from the file</param>
        private void AddAssignment(Assignment.AssignmentData data) {
            /*<!-- This is an example homework assignment -->
            <Grid Height="60" VerticalAlignment="Top">
                <Grid.ColumnDefinitions>
                    <ColumnDefinition Width="1*"/>
                    <ColumnDefinition Width="8*"/>
                    <ColumnDefinition Width="4*"/>
                </Grid.ColumnDefinitions>
                <Grid.RowDefinitions>
                    <RowDefinition Height="3*"/>
                    <RowDefinition Height="2*"/>
                </Grid.RowDefinitions>
            
                <!-- Outside border -->
                <Border Grid.ColumnSpan="3" Grid.RowSpan="3" Margin="0,0,0,-5"  BorderThickness="0,0,0,1" BorderBrush="#FF6C6C6C" Background="White"/>
                           
                <!-- Check box -->
                <Border HorizontalAlignment="Right" Margin="0,5,5,0" Width="18" Height="18" BorderBrush="#FFCA2424" BorderThickness="2">
                    <Label x:Name="testy" Content="" MouseEnter="Label_MouseEnter" MouseLeave="Label_MouseLeave" FontSize="8" Margin="-3" VerticalAlignment="Center" HorizontalAlignment="Center" Cursor="Hand" Foreground="#FF5B5B5B"/>
                </Border>
                           
                <!-- Assignment name -->
                <Label FontSize="14" Grid.Column="1" Margin="0,5,0,0" VerticalAlignment="Top" Content="Complete WW2 Assignment"/>
                            
                <!-- Due date -->
                <Label FontSize="13" Grid.Column="2" HorizontalAlignment="Right" VerticalAlignment="Center" Content="Today - 2:00 PM" Foreground="#FF717171"/>
            
                <!-- Assignment type -->
                <Label FontSize="13" Grid.Column="2" Margin="0,-3,0,0" Grid.Row="1" HorizontalAlignment="Right" VerticalAlignment="Center" Content="Presentation" Foreground="#FF717171"/>

                <!-- Class info -->
                <Grid Grid.Row="1" Grid.Column="1">
                    <Grid.ColumnDefinitions>
                        <ColumnDefinition Width="1*"/>
                        <ColumnDefinition Width="10*"/>
                    </Grid.ColumnDefinitions>
                    <Ellipse Margin="5,0,0,0" HorizontalAlignment="Left" VerticalAlignment="Center" Height="10" StrokeThickness="0" Width="10" Fill="#FFF37646"/>
                    <Label FontSize="13" Margin="-5,-3,0,0" VerticalContentAlignment="Center" HorizontalAlignment="Left" Grid.Column="1" Content="American History"/>
                </Grid>
             </Grid>*/

            // Outer grid
            Grid outerGrid = new Grid();
            outerGrid.Height = 60;
            outerGrid.VerticalAlignment = VerticalAlignment.Top;

            ColumnDefinition column1 = new ColumnDefinition();
            column1.Width = new GridLength(1, GridUnitType.Star);
            ColumnDefinition column2 = new ColumnDefinition();
            column2.Width = new GridLength(8, GridUnitType.Star);
            ColumnDefinition column3 = new ColumnDefinition();
            column3.Width = new GridLength(4, GridUnitType.Star);

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
            assignmentName.FontSize = 14;
            assignmentName.SetValue(Grid.ColumnProperty, 1);
            assignmentName.Margin = new Thickness(0, 5, 0, 0);
            assignmentName.VerticalAlignment = VerticalAlignment.Top;
            assignmentName.Content = data.Title;
            outerGrid.Children.Add(assignmentName);

            // Due date
            Label dueDate = new Label();
            dueDate.FontSize = 13;
            dueDate.SetValue(Grid.ColumnProperty, 3);
            dueDate.HorizontalAlignment = HorizontalAlignment.Right;
            dueDate.VerticalAlignment = VerticalAlignment.Center;

            // TODO: Sort items into different scrollviewers by date
            DateTime today = DateTime.Today;
            string todayString = today.ToString("M/dd/yyy");

            string dateString = "";

            // Assignment is due today
            if (todayString == data.Date) {
                dateString = "Today";
            }
            else {
                dateString = data.Date;
            }

            // Attach time to date ("Today - 2:00 PM")
            if (data.Time != "") {
                dateString += " - " + data.Time;
            }

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

            dueTodayScrollview.Content = outerGrid;
        }
    }
}
