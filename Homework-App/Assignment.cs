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
        /// - Attachments * coming soon *
        /// </summary>
        /// <returns>True if created successfully</returns>
        internal static bool CreateAssignment(AssignmentData data) {
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
            WriteAssignment(sw.ToString());

            sb.Clear();
            sw.Close();

            return false;
        }

        internal static void WriteAssignment(string json) {
            VerifyDirectory();

            // Create a random number for the file title
            Random rnd = new Random();
            while (true) {
                string fileName = rnd.Next(1, 100000).ToString() + ".json";

                string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                path += "/Homework-App/assignment/" + fileName;

                // If the file doesn't exist, write. Otherwise make a new name
                if (!File.Exists(path)) {
                    File.WriteAllText(path, json);
                    break;
                }
            }
        }
    }
}
