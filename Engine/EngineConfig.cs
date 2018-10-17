using System;
using System.Collections.Generic;
using System.IO;
using Abstractions;
using Newtonsoft.Json.Linq;

namespace Engine
{
    internal class EngineConfig : IEngineConfig
    {
        private Dictionary<string, string> _dataDict;
        private IReadOnlyList<IOCRProviderConfig> _ocrProviders;
        private const string _ocrProviderConfigKey = "OCRProviders";

        public EngineConfig(string configFile)
        {
            _dataDict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            Load(configFile);
        }

        private void Load(string configFile)
        {
            if (!File.Exists(configFile))
            {
                throw new ArgumentException("Engine config file is not found!");
            }

            // assume config file is not large
            string configContent = File.ReadAllText(configFile);
            JObject jsonObject = JObject.Parse(configContent);

            // parse OCR Provider section
            _ocrProviders = ParseOCRProviderConfig(jsonObject);

            // parse rest
            foreach (JToken token in jsonObject.Children())
            {
                JProperty prop = (JProperty)token;

                if (!string.Equals(prop.Name, _ocrProviderConfigKey, StringComparison.OrdinalIgnoreCase))
                {
                    _dataDict[prop.Name] = prop.Value.Value<string>();
                }
            }
        }

        private IReadOnlyList<IOCRProviderConfig> ParseOCRProviderConfig(JObject root)
        {
            if (root[_ocrProviderConfigKey] == null)
            {
                throw new Exception("Cannot parse EngineConfig.json because OCRProvider is missing!");
            }

            JArray jOCRProviders = (JArray)root[_ocrProviderConfigKey];
            List<IOCRProviderConfig> providers = new List<IOCRProviderConfig>();

            for (int i = 0; i < jOCRProviders.Count; i++)
            {
                string providerName = jOCRProviders[i]["Provider"].Value<string>();
                string endpoint = jOCRProviders[i]["Endpoint"].Value<string>();
                string key = Environment.ExpandEnvironmentVariables(jOCRProviders[i]["Key"].Value<string>());
                providers.Add(new OCRProviderConfig(providerName, endpoint, key));
            }

            return providers;
        }

        string IEngineConfig.this[string key] => _dataDict[key];

        public IReadOnlyList<IOCRProviderConfig> GetOCRProviders()
        {
            return _ocrProviders;
        }
    }

    internal class OCRProviderConfig : IOCRProviderConfig
    {
        public OCRProviderConfig(string providerName, string endpoint, string key)
        {
            if (string.IsNullOrEmpty(providerName)
                || string.IsNullOrEmpty(endpoint)
                || string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("One of property (Provider, Endpoint, or Key) of a OCR Provider in EngineConfig.json is missing!");
            }

            Provider = providerName;
            Endpoint = endpoint;
            Key = key;
        }

        public string Provider { get; }

        public string Endpoint { get; }

        public string Key { get; }
    }
}
