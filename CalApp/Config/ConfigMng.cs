using System;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace CalApp.Config
{
    public class ConfigMng
    {
        private string m_sConfigFileName = Application.UserAppDataPath + "\\Preferences.xml";
        private ConfigDataObject configDataObject = new ConfigDataObject();
        private ConfigDataObject defaultConfig = new ConfigDataObject();

        // Instance-level: has this specific instance ever successfully loaded?
        // Ensures a brand new ConfigMng always reads from disk on first use,
        // regardless of the shared _isDirty flag.
        private bool _hasLoaded = false;

        // Shared across all instances: has the file changed since any instance
        // last loaded? Set to true on SaveConfig() so all instances reload
        // on their next LoadConfig() call.
        private static bool _isDirty = true;

        public ConfigDataObject ConfigDataObject
        {
            get { return configDataObject; }
            set { configDataObject = value; }
        }

        public ConfigDataObject DefaultConfig
        {
            get { return defaultConfig; }
        }

        public void LoadConfig()
        {
            // Always load if this instance has never loaded,
            // or if another instance has saved since we last loaded.
            if (_hasLoaded && !_isDirty)
                return;

            if (!File.Exists(m_sConfigFileName))
            {
                _hasLoaded = true;
                _isDirty = false;
                return;
            }

            try
            {
                using (StreamReader srReader = File.OpenText(m_sConfigFileName))
                {
                    XmlSerializer xsSerializer = new XmlSerializer(configDataObject.GetType());
                    configDataObject = (ConfigDataObject)xsSerializer.Deserialize(srReader);
                }
                _hasLoaded = true;
                _isDirty = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("LoadConfig failed, using defaults. Reason: " + ex.Message);
                configDataObject = new ConfigDataObject();
                _hasLoaded = true;
                _isDirty = false;
            }
        }

        public void SaveConfig()
        {
            try
            {
                // Record when this save happened (culture-invariant so it
                // round-trips regardless of the machine's locale).
                configDataObject.DateOfChange =
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

                using (StreamWriter swWriter = File.CreateText(m_sConfigFileName))
                {
                    Type tType = configDataObject.GetType();
                    if (tType.IsSerializable)
                    {
                        XmlSerializer xsSerializer = new XmlSerializer(tType);
                        xsSerializer.Serialize(swWriter, configDataObject);
                    }
                }
                // Signal all instances that the file has changed.
                _isDirty = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("SaveConfig failed. Reason: " + ex.Message);
                MessageBox.Show("Could not save preferences:\n" + ex.Message,
                    "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}