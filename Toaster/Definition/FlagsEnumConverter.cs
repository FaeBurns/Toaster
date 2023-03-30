#nullable enable
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Toaster.Definition;

public class FlagsEnumConverter : StringEnumConverter
{
    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.StartArray)
        {
            reader.Read();
            return ReadArray(reader, objectType, existingValue, serializer);
        }

        return base.ReadJson(reader, objectType, existingValue, serializer);
    }

    private object? ReadArray(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        List<object> flags = new List<object>();

        while (reader.TokenType != JsonToken.EndArray)
        {
            flags.Add(base.ReadJson(reader, objectType, existingValue, serializer)!);
            reader.Read();
        }

        int result = 0;

        foreach (object flag in flags)
        {
            int flagNum = (int)flag;
            result = result | flagNum;
        }

        return result;
    }
}
