using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Xml;

namespace AppConfig
{
    #region Types
    public enum ParameterState
    {
        Unchanged = 0,
        Changed,
        Added,
        Deleted,
        Unknown
    }

    public class AttributeInfo
    {
        public string Value { get; set; }
        public string DefaultValue { get; set; }
        public ParameterState State { get; set; }

        private AttributeInfo(){ }

        public AttributeInfo(string DefaultValue)
        {
            this.Value = null;
            this.DefaultValue = DefaultValue;
            this.State = ParameterState.Unknown;
        }

    }
    #endregion

    public class Configuration
    {
        #region members

        private string _AppName = null;
        public string AppName 
        {
            get
            {
                if (String.IsNullOrEmpty(_AppName))
                {
                    throw new Exception("Не задано поле AppName и ConfigFilePath");
                }
                else return _AppName;
            }
            set { _AppName = value; }
        }
        private string _ConfigFilePath = null;
        public string ConfigFilePath
        {
            get
            {
                if (String.IsNullOrEmpty(_ConfigFilePath))
                {
                    string path = "";
                    path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppName);
                    path = Path.Combine(path, "config.xml");
                    return path;
                }
                else return _ConfigFilePath;
            }
            set
            {
                _ConfigFilePath = value;
            }
        }

        public Dictionary<string, AttributeInfo> InitAttributes = new Dictionary<string, AttributeInfo>();
        private Dictionary<string, AttributeInfo> LoadAttributes = new Dictionary<string, AttributeInfo>();

        #endregion

        private void InitConfigDefaultValue()
        {
            FileStream fs;
            Directory.CreateDirectory(Path.GetDirectoryName(ConfigFilePath));
            using (fs = File.Create(ConfigFilePath))
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                using (XmlWriter writer = XmlWriter.Create(fs, settings))
                {
                    writer.WriteStartElement("settings");
                    foreach (KeyValuePair<string, AttributeInfo> kvp in InitAttributes)
                    {
                        //todo: запись в xml
                        writer.WriteElementString(kvp.Key, kvp.Value.DefaultValue);
                        LoadAttributes.Add(kvp.Key, kvp.Value);
                    }
                    writer.WriteEndElement();
                }
            }
        }

        public void LoadConfig()
        {
            FileStream fs;
            if (!File.Exists(ConfigFilePath))
            {
                InitConfigDefaultValue();
                return;
            }
            else
            {
                fs = new FileStream(ConfigFilePath, FileMode.Open);
            }
            //todo: load xml

            fs.Close();
            
        }
    }
}
