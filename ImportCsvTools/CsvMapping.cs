using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;


namespace ImportCsvTools
{
    [DataContract]
    internal class CsvMappingConfig
    {
        [DataMember]
        public string LibraryName { get; set; }

        [DataMember]
        public bool AllowInvalidToolImport { get; set; } = false;

        [DataMember]
        public string CsvInputUnits { get; set; } = "in";

        [DataMember]
        public List<CsvMapping> Mappings { get; set; } = new List<CsvMapping>();

        public static CsvMappingConfig Load(string filePath)
        {
            using (var stream = File.OpenRead(filePath))
            {
                var serializer = new DataContractJsonSerializer(typeof(CsvMappingConfig));
                var result = serializer.ReadObject(stream) as CsvMappingConfig;
                if (result == null)
                {
                    throw new InvalidOperationException("Mapping file did not contain a valid mapping configuration.");
                }

                result.Mappings = result.Mappings ?? new List<CsvMapping>();
                return result;
            }
        }
    }

    [DataContract]
    internal class CsvMapping
    {
        [DataMember]
        public string CsvColumn { get; set; }

        [DataMember]
        public string ToolField { get; set; }

        [DataMember]
        public Dictionary<string, string> ValueMap { get; set; } = new Dictionary<string, string>();

        [DataMember]
        public string DefaultValue { get; set; }

        [DataMember]
        public string EnumType { get; set; }

        [DataMember]
        public string Expression { get; set; }
    }
}
