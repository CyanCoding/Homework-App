using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Homework_App {
    internal static class Assignment {

        /// <summary>
        /// Creates the app data directory if it doesn't exist.
        /// </summary>
        private static void VerifyDirectory() {
            var dir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            dir += "/Homework-App";

            if (!Directory.Exists(dir)) {
                Directory.CreateDirectory(dir);
            }

            var classDir = dir + "/class";
            if (!Directory.Exists(classDir)) {
                Directory.CreateDirectory(classDir);
            }

            var assignmentDir = dir + "/assignment";
            if (!Directory.Exists(assignmentDir)) {
                Directory.CreateDirectory(assignmentDir);
            }
        }

        /// <summary>
        /// Creates a new assignment
        /// 
        /// Accepts the following fields:
        /// - Title
        /// - Type
        /// - Class
        /// - Date
        /// - Time
        /// - Priority
        /// - Repeat
        /// - Reminder
        /// - Notes
        /// - Complete (true/false as a string)
        /// - file name
        /// - Attachments * coming soon *
        /// </summary>
        /// <returns>True if created successfully</returns>
        internal static bool CreateAssignment(AssignmentData data, string path = "") {
            var sb = new StringBuilder();
            var sw = new StringWriter(sb);

            var writeValues = new Dictionary<string, string>() {
                { "Title", data.Title },
                { "Type", data.Type },
                { "Class", data.Class },
                { "Date", data.Date },
                { "Time", data.Time },
                { "Priority", data.Priority },
                { "Repeat", data.Repeat },
                { "Reminder", data.Reminder },
                { "Notes", data.Notes },
                { "Complete", data.Complete }
            };

            using (JsonWriter js = new JsonTextWriter(sw)) {
                js.Formatting = Formatting.Indented;

                js.WriteStartObject();

                // Iterate through writeValues. If the value isn't set, we use the key value
                foreach (var (key, value) in writeValues) {
                    js.WritePropertyName(key);
                    js.WriteValue(value != "" ? value : "");
                }

                js.WriteEndObject();
            }
            WriteAssignment(sw.ToString(), path);

            sb.Clear();
            sw.Close();

            return false;
        }

        /// <summary>
        /// This is called by CreateAssignment ONLY!
        /// </summary>
        /// <param name="json">JSON string to write</param>
        /// <param name="path">The path to write to</param>
        private static void WriteAssignment(string json, string path = "") {
            VerifyDirectory();

            // This happens if we are overwriting an assignment
            if (path != "") {
                File.WriteAllText(path, json);
                return;
            }

            // Create a random number for the file title
            var rnd = new Random();
            while (true) {
                var fileName = rnd.Next(1, 100000).ToString() + ".json";

                path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                path += "/Homework-App/assignment/" + fileName;

                // If the file doesn't exist, write. Otherwise make a new name
                if (!File.Exists(path)) {
                    File.WriteAllText(path, json);
                    break;
                }
            }
        }

        internal struct AssignmentData {
            internal string Title;
            internal string Type;
            internal string Class;
            internal string Date;
            internal string Time;
            internal string Priority;
            internal string Repeat;
            internal string Reminder;
            internal string Notes;
            internal string Complete;
            internal string FileName;
        }

        private class AssignmentStructure {
            public AssignmentStructure(string title, string type, string @class, string date, string time, string priority, string repeat, string reminder, string notes, string complete) {
                Title = title;
                Type = type;
                Class = @class;
                Date = date;
                Time = time;
                Priority = priority;
                Repeat = repeat;
                Reminder = reminder;
                Notes = notes;
                Complete = complete;
            }

            public string Title { get; }
            public string Type { get; }
            public string Class { get; }
            public string Date { get; }
            public string Time { get; }
            public string Priority { get; }
            public string Repeat { get; }
            public string Reminder { get; }
            public string Notes { get; }
            public string Complete { get; }
        }

        internal static AssignmentData ReadAssignment(string path) {
            var data = new AssignmentData();

            var json = File.ReadAllText(path);
            
            var settings =
                JsonConvert.DeserializeObject<AssignmentStructure>(json)!;

            data.Title = settings.Title;
            data.Type = settings.Type;
            data.Class = settings.Class;
            data.Date = settings.Date;
            data.Time = settings.Time;
            data.Priority = settings.Priority;
            data.Repeat = settings.Repeat;
            data.Reminder = settings.Reminder;
            data.Notes = settings.Notes;
            data.Complete = settings.Complete;

            // Removes '.json' from the file name (example value: "45532")
            data.FileName = Path.GetFileName(path).Remove(Path.GetFileName(path).Length - 5, 5);

            return data;
        }

        internal static void MarkAssignmentCompleted(string fileName) {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            path += "/Homework-App/assignment/" + fileName;

            var json = File.ReadAllText(path);

            var settings =
                JsonConvert.DeserializeObject<AssignmentStructure>(json);

            var data = new AssignmentData {
                Title = settings!.Title,
                Type = settings.Type,
                Class = settings.Class,
                Date = settings.Date,
                Time = settings.Time,
                Repeat = settings.Repeat,
                Reminder = settings.Reminder,
                Notes = settings.Notes,
                Complete = "true"
            };

            CreateAssignment(data, path);
        }
    }
}
