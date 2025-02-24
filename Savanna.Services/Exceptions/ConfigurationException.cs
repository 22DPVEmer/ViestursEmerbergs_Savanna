using System;

namespace Savanna.Services.Exceptions
{
    public class ConfigurationException : Exception
    {
        public string ConfigFile { get; }

        public ConfigurationException(string message) 
            : base(message)
        {
        }

        public ConfigurationException(string message, string configFile) 
            : base(message)
        {
            ConfigFile = configFile;
        }

        public ConfigurationException(string message, string configFile, Exception innerException) 
            : base(message, innerException)
        {
            ConfigFile = configFile;
        }
    }
} 