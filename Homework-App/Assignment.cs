using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework_App {
    internal class Assignment {

        /// <summary>
        /// Creates the app data directory if it doesn't exist.
        /// </summary>
        private static void VerifyDirectory() {
            string dir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            dir += "/Homework-App";

            if (!Directory.Exists(dir)) {
                Directory.CreateDirectory(dir);
            }

            string classDir = dir + "/class";
            if (!Directory.Exists(classDir)) {
                Directory.CreateDirectory(classDir);
            }

            string assignmentDir = dir + "/assignment";
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
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);

            Dictionary<string, string> writeValues = new Dictionary<string, string>() {
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
                foreach (KeyValuePair<string, string> kvp in writeValues) {
                    js.WritePropertyName(kvp.Key);
                    if (kvp.Value != null) {
                        js.WriteValue(kvp.Value);
                    }
                    else {
                        js.WriteValue("");
                    }
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
        private static void WriteAssignment(string json, string path = "") {
            VerifyDirectory();

            // This happens if we are overwriting an assignment
            if (path != "") {
                File.WriteAllText(path, json);
                return;
            }

            // Create a random number for the file title
            Random rnd = new Random();
            while (true) {
                string fileName = rnd.Next(1, 100000).ToString() + ".json";

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
            public string Title { get; set; }
            public string Type { get; set; }
            public string Class { get; set; }
            public string Date { get; set; }
            public string Time { get; set; }
            public string Priority { get; set; }
            public string Repeat { get; set; }
            public string Reminder { get; set; }
            public string Notes { get; set; }
            public string Complete { get; set; }
        }

        internal static AssignmentData ReadAssignment(string path) {
            AssignmentData data = new AssignmentData();

            string json = File.ReadAllText(path);

            AssignmentStructure settings =
                JsonConvert.DeserializeObject<AssignmentStructure>(json);

            data.Title = settings.Title;
            data.Type = settings.Type;
            data.Class = settings.Class;
            data.Date = settings.Date;
            data.Time = settings.Time;
            data.Repeat = settings.Repeat;
            data.Reminder = settings.Reminder;
            data.Notes = settings.Notes;
            data.Complete = settings.Complete;

            // Removes '.json' from the file name (example value: "45532")
            data.FileName = Path.GetFileName(path).Remove(Path.GetFileName(path).Length - 5, 5);

            return data;
        }

        internal static void MarkAssignmentCompleted(string fileName) {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            path += "/Homework-App/assignment/" + fileName;

            string json = File.ReadAllText(path);

            AssignmentStructure? settings =
                JsonConvert.DeserializeObject<AssignmentStructure>(json);

            AssignmentData data = new AssignmentData();

            data.Title = settings.Title;
            data.Type = settings.Type;
            data.Class = settings.Class;
            data.Date = settings.Date;
            data.Time = settings.Time;
            data.Repeat = settings.Repeat;
            data.Reminder = settings.Reminder;
            data.Notes = settings.Notes;
            data.Complete = "true";

            CreateAssignment(data, path);
        }
    }
}
