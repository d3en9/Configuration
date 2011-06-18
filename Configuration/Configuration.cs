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

        public AttributeInfo(string Value, string DefaultValue)
        {
            this.Value = Value;
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

        #region methods

        #region public

        #region Load/Flush
        public void LoadConfig()
        {
            if (!File.Exists(ConfigFilePath))
            {
                InitConfigDefaultValue();
                return;
            }
            else
            {
                LoadConfigFromFile();
            }
        }

        public void Flush()
        {
            foreach (KeyValuePair<string, AttributeInfo> kvp in InitAttributes)
            {
                if (!LoadAttributes.ContainsKey(kvp.Key))
                {
                    AttributeInfo ai = new AttributeInfo(kvp.Value.DefaultValue, kvp.Value.DefaultValue);
                    ai.State = ParameterState.Added;
                    LoadAttributes.Add(kvp.Key, ai);
                }
            }
            RewriteFile();
        }
        #endregion

        #region Set/Get
        public string Get(string key)
        {
            return LoadAttributes[key].Value;
        }

        public void Set(string key, string value)
        {
            AttributeInfo ai = new AttributeInfo(value, value);
            if (LoadAttributes.ContainsKey(key))
            {
                ai.State = ParameterState.Changed;
                LoadAttributes[key] = ai;
            }
            else
            {
                ai.State = ParameterState.Added;
                LoadAttributes.Add(key, ai);
            }
        }
        #endregion

        #endregion

        #region private

        #region Save
        private void RewriteFile()
        {
            using (FileStream fs = File.Open(ConfigFilePath, FileMode.Create))
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                using (XmlWriter writer = XmlWriter.Create(fs, settings))
                {
                    writer.WriteStartElement("settings");
                    foreach (KeyValuePair<string, AttributeInfo> kvp in LoadAttributes)
                    {
                        writer.WriteElementString(kvp.Key, kvp.Value.Value);
                    }
                    writer.WriteEndElement();
                }
            }
        }
        #endregion

        #region Init
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
                        writer.WriteElementString(kvp.Key, kvp.Value.DefaultValue);
                        LoadAttributes.Add(kvp.Key, kvp.Value);
                    }
                    writer.WriteEndElement();
                }
            }
        }
        #endregion

        #region Load
        private void LoadConfigFromFile()
        {
            using (XmlReader reader = XmlReader.Create(ConfigFilePath))
            {
                bool firstElement = true;
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (firstElement) firstElement = false;
                        else
                        {
                            string Name = reader.Name;
                            reader.Read();
                            string Value = reader.ReadContentAsString();
                            reader.ReadEndElement();
                            LoadAttributes.Add(Name, new AttributeInfo(Value, Value));
                        }
                    }
                }
            }
        }
        #endregion
        #endregion

        #endregion
    }
}
