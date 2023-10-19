using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Pgotchi.Core.Common;

namespace Pgotchi.Apis.Converters;

public class CustomKeyValuePairConverter : KeyValuePairConverter
{
    private const string KeyName = "Key";
    private const string ValueName = "Value";

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value == null)
        {
            writer.WriteNull();
            return;
        }

        var type = value.GetType();
        var keyProp = type.GetProperty(KeyName);
        var valueProp = type.GetProperty(ValueName);
        var jsonKey = keyProp?.GetValue(value)?.ToString() ?? KeyName;
        var jsonValue = valueProp?.GetValue(value);

        writer.WriteStartObject();
        writer.WritePropertyName(jsonKey.ToCamelCase());
        serializer.Serialize(writer, jsonValue, valueProp?.PropertyType ?? typeof(Nullable));
        writer.WriteEndObject();
    }
}
