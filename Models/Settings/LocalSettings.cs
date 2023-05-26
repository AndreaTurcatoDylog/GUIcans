using Common;
using Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class LocalSettings : Settings
    {
        #region Members

        private static LocalSettings _Instance;
        private Status _Status;
        private Commands _Commands;

        #endregion

        #region Properties

        /// <summary>
        /// Get the Instance of the object (SingleTon)
        /// </summary>
        public static LocalSettings Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new LocalSettings();
                }

                return _Instance;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Set the values of local settins. They are used when software is running
        // Its internal values are setted by (in order of loading):
        // 1) Settings value (Level 1)
        // 2) Status values (Level 2) (possibility of override)
        // 3) Commands values (Level 3) (possibility of override)
        /// </summary>
        public void Load(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                // Load all the Models
                var settings = LoadSettings(path);
                _Status = LoadStatus(path);
                _Commands = LoadCommands(path);

                // Update the local settings with the loaded values
                Copy(settings);
                InsertStatusValues(_Status);
                InsertCommandsValues(_Commands);
            }
        }

        /// <summary>
        /// Load the Settings
        /// </summary>
        private Settings LoadSettings(string settingsPath)
        {
            try
            {
                var path = Path.Combine(settingsPath, Constants.SettingsFile);
                if (File.Exists(path))
                {
                    return FileJsonManager<Settings>.LoadFromFile(path, Encoding.UTF8);
                }
            }
            catch (Exception)
            {

            }

            // if something goes wrong return defualt values
            var settings = new Settings();
            settings.SetDefaultValue();
            return settings;
        }

        /// <summary>
        /// Load the Commands
        /// </summary>
        private Commands LoadCommands(string settingsPath)
        {
            try
            {
                var path = Path.Combine(settingsPath, Constants.CommandsFile);
                if (File.Exists(path))
                {
                    return FileJsonManager<Commands>.LoadFromFile(path, Encoding.UTF8);
                }
            }
            catch (Exception)
            { }

            // if something goes wrong return default values
            return new Commands();
        }

        /// <summary>
        /// Load the Status
        /// </summary>
        private Status LoadStatus(string settingsPath)
        {
            try
            {
                var path = Path.Combine(settingsPath, Constants.StatusFile);
                if (File.Exists(path))
                {
                    return FileJsonManager<Status>.LoadFromFile(path, Encoding.UTF8);
                }
            }
            catch (Exception)
            { }

            // if something goes wrong return default values
            return new Status();
        }

        /// <summary>
        /// Save the settings
        /// </summary>
        public void Save()
        {
            try
            {
                var pathApplication = $@"{Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName)}";
                var statusFilePath = Path.Combine(pathApplication, Constants.StatusFile);
                var settingsFilePath = Path.Combine(pathApplication, Constants.SettingsFile);

                //if (SaveStatusOnExit)
                //{
                //    var status = new Status();
                //    // Set all the Status properties

                //    // Save Status file              
                //    FileJsonManager<Status>.SaveFile(statusFilePath, status);
                //}
                //else
                //{
                //    // Delete file is exists               
                //    if (File.Exists(statusFilePath))
                //    {
                //        File.Delete(statusFilePath);
                //    }
                //}                

                // Save Setting file
                FileJsonManager<Settings>.SaveFile(settingsFilePath, this);
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Insert the values from Status
        /// </summary>
        private void InsertStatusValues(Status status)
        {
            if (status != null)
            {
            }
        }

        /// <summary>
        /// Insert the values from Commands
        /// </summary>
        private void InsertCommandsValues(Commands commands)
        {
            if (commands != null)
            {
            }
        }

        #endregion
    }
}
