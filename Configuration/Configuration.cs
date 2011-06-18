using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

    }
}
