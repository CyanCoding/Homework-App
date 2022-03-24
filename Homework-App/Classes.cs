using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Homework_App; 

internal static class Classes {
    private static void WriteClass(string json) {
        Assignment.VerifyDirectory();

        var rnd = new Random();
        while (true) {
            var fileName = "class-" + rnd.Next(1, 100000) + ".json";
            var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            path += "/Homework-App/class/" + fileName;

            if (File.Exists(path)) continue;
            
            File.WriteAllText(path, json);
            break;
        }
    }
    
    internal static void CreateClass(ClassData data) {
        var sb = new StringBuilder();
        var sw = new StringWriter(sb);

        // Doesn't include DaysEachWeek because that's not a string
        var writeValues = new Dictionary<string, string>() {
            {"Name", data.Name},
            {"Building", data.Building},
            {"Room", data.Room},
            {"Professor", data.Professor},
            {"Pronouns", data.Pronouns},
            {"StartDate", data.StartDate},
            {"EndDate", data.EndDate},
            {"Time", data.Time},
            {"Number", data.Number},
            {"Color", data.Color},
            {"Reminder", data.Reminder},
            {"Notes", data.Notes}
        };

        using (JsonWriter js = new JsonTextWriter(sw)) {
            js.Formatting = Formatting.Indented;
            js.WriteStartObject();

            foreach (var (key, value) in writeValues) {
                js.WritePropertyName(key);
                js.WriteValue(value);
            }
            
            js.WritePropertyName("DaysEachWeek");
            js.WriteStartArray();
            foreach (var val in data.DaysEachWeek) {
                js.WriteValue(val == true ? "True" : "False");
            }
            js.WriteEnd();
            
            js.WriteEndObject();
        }

        WriteClass(sw.ToString());

        sb.Clear();
        sw.Close();
    }
}