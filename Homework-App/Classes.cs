using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Homework_App; 

internal static class Classes {
    /// <summary>
    /// Contains a dictionary of color names to hex values.
    /// </summary>
    /// <param name="color">The name of the color</param>
    /// <returns>The corresponding hex value</returns>
    internal static string GetHexFromColor(string color) {
        var colorValues = new Dictionary<string, string>() {
            {"VelvetGreenColor", "#006b3c"},
            {"AvocadoColor", "#568203"},
            {"SpringGreenColor", "#33b864"},
            {"BlueOpalColor", "#0f3b57"},
            {"BlueFlowerColor", "#2282a8"},
            {"ElectricBlueColor", "#0075b3"},
            {"BurntRedColor", "#9f2305"},
            {"CherryColor", "#f2013f"},
            {"ChristmasRedColor", "#b01b2e"},
            {"CandyCornColor", "#fcfc5d"},
            {"SunshineColor", "#FFD428"},
            {"BakedPotatoColor", "#b69e87"},
            {"BarkColor", "#996633"},
            {"BrownCoffeeColor", "#4a2c2a"},
            {"BrilliantPurpleColor", "#4b0082"},
            {"CadmiumVioletColor", "#7f3e98"},
            {"PaleMauveColor", "#7a547f"},
            {"CalmPurpleColor", "#9878f8"},
            {"LilacColor", "#cea2fd"}
        };

        return colorValues[color];
    }
    
    /// <summary>
    /// Gets every class name and color for the assignment class combo box
    /// </summary>
    /// <returns>A tuple of two string arrays
    /// First is name, second is color.</returns>
    internal static Tuple<string[], string[], string[]> GetClassesNameAndColor() {
        var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        path += "/Homework-App/class";
        var d = new DirectoryInfo(path);

        var classNames = new List<string>();
        var classColors = new List<string>();
        var codes = new List<string>();

        foreach (var file in d.GetFiles("*.json")) {
            var text = File.ReadAllText(file.FullName);
            var data = JsonConvert.DeserializeObject<ClassManifest>(text)!;
            
            classNames.Add(data.Name);
            classColors.Add(data.Color);
            codes.Add("e" + codes.Count);
        }
        
        return new Tuple<string[], string[], string[]>(classNames.ToArray(), classColors.ToArray(), codes.ToArray());
    }
    
    /// <summary>
    /// Creates a file in our directory for the class
    /// </summary>
    /// <param name="json">The JSON text to write for the class</param>
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
                js.WriteValue(val == true);
            }
            js.WriteEnd();
            
            js.WriteEndObject();
        }

        WriteClass(sw.ToString());

        sb.Clear();
        sw.Close();
    }
}