using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace ImportCsvTools
{
    /// <summary>
    /// Helper class for serializing/deserializing CSV row data to/from Tool.Aux_data field.
    /// This enables complete round-trip fidelity by preserving all CSV columns,
    /// including those not mapped to Tool properties.
    /// </summary>
    [DataContract]
    public class CsvAuxData
    {
        /// <summary>
        /// Dictionary of CSV column name to original value from import
        /// </summary>
        [DataMember]
        public Dictionary<string, string> OriginalCsvColumns { get; set; }

        public CsvAuxData()
        {
            OriginalCsvColumns = new Dictionary<string, string>();
        }

        /// <summary>
        /// Serializes CSV column data to JSON string for storage in Aux_data
        /// </summary>
        public static string Serialize(Dictionary<string, string> csvColumns)
        {
            if (csvColumns == null || csvColumns.Count == 0)
            {
                return null;
            }

            var auxData = new CsvAuxData { OriginalCsvColumns = csvColumns };

            using (var stream = new MemoryStream())
            {
                var serializer = new DataContractJsonSerializer(typeof(CsvAuxData));
                serializer.WriteObject(stream, auxData);
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        /// <summary>
        /// Deserializes JSON string from Aux_data back to CSV column dictionary
        /// </summary>
        public static Dictionary<string, string> Deserialize(string auxData)
        {
            if (string.IsNullOrWhiteSpace(auxData))
            {
                return null;
            }

            try
            {
                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(auxData)))
                {
                    var serializer = new DataContractJsonSerializer(typeof(CsvAuxData));
                    var result = serializer.ReadObject(stream) as CsvAuxData;
                    return result?.OriginalCsvColumns;
                }
            }
            catch
            {
                // If deserialization fails (e.g., Aux_data contains other data format),
                // return null to indicate no CSV data available
                return null;
            }
        }

        /// <summary>
        /// Attempts to deserialize CSV data from Aux_data, returns false if not CSV format
        /// </summary>
        public static bool TryDeserialize(string auxData, out Dictionary<string, string> csvColumns)
        {
            csvColumns = Deserialize(auxData);
            return csvColumns != null;
        }
    }
}
