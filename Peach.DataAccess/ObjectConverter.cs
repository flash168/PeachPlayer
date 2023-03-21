using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peach.DataAccess
{
    public class ObjectConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.ValueType == typeof(int))
            {
                int intValue = (int)reader.Value;
                return intValue.ToString();
            }
            else if (reader.ValueType == typeof(bool))
            {
                bool boolValue = (bool)reader.Value;
                return boolValue.ToString();
            }
            else if (reader.ValueType == typeof(DateTime))
            {
                DateTime dateTimeValue = (DateTime)reader.Value;
                return dateTimeValue.ToString("o");
            }
            else if (reader.ValueType == typeof(string))
            {
                return reader.Value.ToString();
            }
            else
            {
                if (reader?.Value == null)
                    return null;
                // Handle other value types as needed.
                return reader.Value.ToString();
            }
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
