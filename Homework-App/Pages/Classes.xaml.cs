using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Homework_App.Resources; 

public partial class Classes : Page {
    public Classes() {
        InitializeComponent();
    }
    
    private void cancelButton_Click(object sender, RoutedEventArgs e) {
        NewClassGrid.Visibility = Visibility.Hidden;
    }
    
    /// <summary>
    /// Runs when the user hits the "Save + make another" class button
    /// </summary>
    private void SaveAndAddClass(object sender, RoutedEventArgs e) {
        if (!CheckClassRequirements()) {
            return;
        }
        
        // TODO: Input a little timer that counts how many assignments the user has or has completed
        
        // Assemble DaysEachWeek
        bool?[] classDays = {
            MonCheckbox.IsChecked,
            TueCheckbox.IsChecked,
            WedCheckbox.IsChecked,
            ThuCheckbox.IsChecked,
            FriCheckbox.IsChecked,
            SatCheckbox.IsChecked,
            SunCheckbox.IsChecked
        };

        // Fix possible null values
        for (var i = 0; i < classDays.Length; i++) {
            classDays[i] ??= false;
        }

        var data = new ClassData {
            Name = ClassName.Text,
            Building = ClassBuilding.Text,
            Room = ClassRoom.Text,
            Professor = ClassProfessor.Text,
            Pronouns = ClassProfessorPronouns.Text,
            StartDate = StartDate.Text,
            EndDate = EndDate.Text,
            Time = ClassTime.Text,
            Number = ClassNumber.Text, // This isn't an int, it's something like "CRN 13321"
            DaysEachWeek = classDays,
            Color = ((Grid) ClassColor.SelectedItem).Name, // Not a hex value. e.g. "RubyColor"
            Reminder = ClassReminder.Text,
            Notes = ClassNotes.Text
        };
        
        Homework_App.Classes.CreateClass(data);
    }

    /// <summary>
    /// Runs when the user hits the save button
    /// </summary>
    private void SaveClass(object sender, RoutedEventArgs e) {
        if (!CheckClassRequirements()) {
            return;
        }
        
        SaveAndAddClass(sender, e);
        NewClassGrid.Visibility = Visibility.Hidden;
    }
    
    /// <summary>
    /// Checks to make sure the user has filled in necessary class details.
    /// Requires start date, end date, and class.
    /// </summary>
    /// <returns>Returns true if requirements met</returns>
    private bool CheckClassRequirements() {
        var successful = true;
        
        if (ClassName.Text == "") {
            ClassNameRequired.Visibility = Visibility.Visible;
            ClassName.BorderBrush = new BrushConverter().ConvertFrom("#FFAA2929") as Brush;
            successful = false;
        }
        else {
            ClassName.BorderBrush = new BrushConverter().ConvertFrom("#FFABADB3") as Brush;
            ClassNameRequired.Visibility = Visibility.Hidden;
        }

        if (StartDate.Text == "") {
            StartDate.BorderBrush = new BrushConverter().ConvertFrom("#FFAA2929") as Brush;
            StartDateRequired.Visibility = Visibility.Visible;
            successful = false;
        }
        else {
            StartDate.BorderBrush = new BrushConverter().ConvertFrom("#FFABADB3") as Brush;
            StartDateRequired.Visibility = Visibility.Hidden;
        }

        if (EndDate.Text == "") {
            EndDate.BorderBrush = new BrushConverter().ConvertFrom("#FFAA2929") as Brush;
            EndDateRequired.Visibility = Visibility.Visible;
            successful = false;
        }
        else {
            EndDate.BorderBrush = new BrushConverter().ConvertFrom("#FFABADB3") as Brush;
            EndDateRequired.Visibility = Visibility.Hidden;
        }

        return successful;
    }
    
    private void NewClassButtonClicked(object sender, RoutedEventArgs e) {
        // We could add an animation here where we fade or slowly appear
        NewClassGrid.Visibility = Visibility.Visible;
    }

    private void ClassChecksChanged(object sender, RoutedEventArgs e) {
        var text = "";
        var count = 0;
        if (MonCheckbox.IsChecked == true) {
            text = "Mon";
            count++;
        }

        if (TueCheckbox.IsChecked == true) {
            if (count > 0) {
                text += ", Tue";
            }
            else {
                text += "Tue";
            }

            count++;
        }
        
        if (WedCheckbox.IsChecked == true) {
            if (count > 0) {
                text += ", Wed";
            }
            else {
                text += "Wed";
            }

            count++;
        }
        
        if (ThuCheckbox.IsChecked == true) {
            if (count > 0) {
                text += ", Thu";
            }
            else {
                text += "Thu";
            }

            count++;
        }
        
        if (FriCheckbox.IsChecked == true) {
            if (count > 0) {
                text += ", Fri";
            }
            else {
                text += "Fri";
            }

            count++;
        }
        
        if (SatCheckbox.IsChecked == true) {
            if (count > 0) {
                text += ", Sat";
            }
            else {
                text += "Sat";
            }

            count++;
        }
        
        if (SunCheckbox.IsChecked == true) {
            if (count > 0) {
                text += ", Sun";
            }
            else {
                text += "Sun";
            }
        }

        ClassTimesComboBox.Text = text;
    }

    private void ClassTimesComboBox_OnDropDownClosed(object? sender, EventArgs e) {
        ClassChecksChanged(sender!, null!);
    }
}