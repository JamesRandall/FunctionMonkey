using System;
using System.IO;
using Newtonsoft.Json.Linq;

namespace FunctionMonkey.Testing.Implementation
{
    internal static class EnvironmentVariableManager
    {
        private static bool _environmentVariablesRegistered;
        private static readonly object LoadingAppVariablesLock = new object();

        /// <summary>
        /// Add environment variables from a stream containing a Functions app settings file
        /// </summary>
        /// <param name="appSettingsStream">The app settings stream</param>
        /// <param name="oneTimeOnly">Defaults to true, if true only set the variables one time</param>
        public static void AddEnvironmentVariables(Stream appSettingsStream, bool oneTimeOnly = true)
        {
            if (_environmentVariablesRegistered && oneTimeOnly)
            {
                return;
            }

            SetEnvironmentVariables(appSettingsStream, oneTimeOnly);
        }

        /// <summary>
        /// Add environment variables from a file containing a Functions app settings file
        /// </summary>
        /// <param name="appSettingsPath">A path to the app settings file</param>
        /// <param name="oneTimeOnly">Defaults to true, if true only set the variables one time</param>
        public static void AddEnvironmentVariables(string appSettingsPath, bool oneTimeOnly = true)
        {
            if (_environmentVariablesRegistered && oneTimeOnly)
            {
                return;
            }

            using (Stream stream = new FileStream(appSettingsPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                SetEnvironmentVariables(stream, oneTimeOnly);
            }
        }

        private static void SetEnvironmentVariables(Stream appSettings, bool oneTimeOnly)
        {
            lock (LoadingAppVariablesLock)
            {
                if (_environmentVariablesRegistered && oneTimeOnly)
                {
                    return;
                }

                string json;
                using (StreamReader reader = new StreamReader(appSettings))
                {
                    json = reader.ReadToEnd();
                }

                if (!String.IsNullOrWhiteSpace(json))
                {
                    JObject settings = JObject.Parse(json);
                    JObject values = (JObject)settings["Values"];
                    if (values != null)
                    {
                        foreach (JProperty property in values.Properties())
                        {
                            if (property.Value != null)
                            {
                                Environment.SetEnvironmentVariable(property.Name, property.Value.ToString());
                            }
                        }
                    }
                }

                _environmentVariablesRegistered = true;
            }

        }
    }
}
