using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using CursorGuard.Helpers;

namespace CursorGuard
{
    internal class ConfigurationManager : IConfigurationManager
    {
        private const string folderName = "Cursor Guard";
        private const string fileName = "config.xml";

        private Dictionary<string, ApplicationProfile> profiles;
        
        public ConfigurationManager()
        {
            ReloadConfiguration();
        }

        public IEnumerable<ApplicationProfile> GetApplicationProfiles()
        {
            return profiles.Select(kvp => kvp.Value).ToList();
        }

        public ApplicationProfile GetProfileForExecutable(string executablePath)
        {
            Ensure.ArgumentNotNullOrEmptyString(executablePath, nameof(executablePath));

            if (!profiles.ContainsKey(executablePath))
            {
                return null;
            }

            var p = profiles[executablePath];
            Ensure.ResultNotNull(p);
            return p;
        }

        public void SaveConfiguration()
        {
            throw new NotImplementedException();
        }

        public void ReloadConfiguration()
        {
            var appDataFolderPath = GetAppDataFolderPath();
            if (!Directory.Exists(appDataFolderPath))
            {
                Directory.CreateDirectory(appDataFolderPath);
            }

            try
            {
                var serializer = new XmlSerializer(typeof(SerializableConfiguration));
                using (var fs = File.OpenRead(GetConfigFilePath()))
                {
                    var configModel = (SerializableConfiguration) serializer.Deserialize(fs);

                    profiles = new Dictionary<string, ApplicationProfile>();
                    foreach (var profile in configModel.ApplicationProfiles)
                    {
                        profiles.Add(profile.ExecutablePath, ApplicationProfileFromSerializable(profile));
                    }
                }
            }
            catch (Exception)
            {
                CreateDefaultConfiguration();
            }
        }

        private void CreateDefaultConfiguration()
        {
            profiles = new Dictionary<string, ApplicationProfile>();
        }

        private string GetAppDataFolderPath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), folderName);
        }

        private string GetConfigFilePath()
        {
            return Path.Combine(GetAppDataFolderPath(), fileName);
        }

        private ApplicationProfile ApplicationProfileFromSerializable(SerializableApplicationProfile serializable)
        {
            Ensure.ArgumentNotNull(serializable, nameof(serializable));

            return new ApplicationProfile
            {
                ExecutablePath = serializable.ExecutablePath
            };
        }
    }

    /// <summary>
    /// Manages configuration
    /// </summary>
    public interface IConfigurationManager
    {
        /// <summary>
        /// Gets a list of configured application profiles
        /// </summary>
        /// <returns>A list of application profiles</returns>
        IEnumerable<ApplicationProfile> GetApplicationProfiles();

        /// <summary>
        /// Gets profile by executable name
        /// </summary>
        /// <param name="executablePath">Path to application's executable</param>
        /// <returns><see cref="ApplicationProfile"/> if found, else null</returns>
        ApplicationProfile GetProfileForExecutable(string executablePath);

        /// <summary>
        /// Saves configuration to file
        /// </summary>
        void SaveConfiguration();

        /// <summary>
        /// Restores configuration from file or creates default if none found
        /// </summary>
        void ReloadConfiguration();
    }

    [XmlRoot("configuration")]
    public class SerializableConfiguration
    {
        [XmlArray("profiles")]
        [XmlArrayItem("profile")]
        public List<SerializableApplicationProfile> ApplicationProfiles { get; set; }
    }

    public class SerializableApplicationProfile
    {
        [XmlAttribute("executable")]
        public string ExecutablePath { get; set; }
    }
}
