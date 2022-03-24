namespace Homework_App; 

public struct ClassData {
    public string Name;
    public string Building;
    public string Room;
    public string Professor;
    public string Pronouns;
    public string StartDate;
    public string EndDate;
    public string Time;
    public string Number; // This isn't an int, it's something like "CRN 13321"
    public bool?[] DaysEachWeek; // This should never have null values
    public string Color;
    public string Reminder;
    public string Notes;
}