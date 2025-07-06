using System;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;

namespace ReduxLib.Input;

[JsonConverter(typeof(KeyBindConverter)),PublicAPI]
public struct KeyBind
{
    public KeyCode Code;

    public KeyBind(KeyCode code)
    {
        Code = code;
    }

    public bool Down => !InputHelper.IsRebindingHappening && UnityEngine.Input.GetKeyDown(Code);
    public bool Up => !InputHelper.IsRebindingHappening && UnityEngine.Input.GetKeyUp(Code);
    public bool Held => !InputHelper.IsRebindingHappening && UnityEngine.Input.GetKey(Code);

    public override string ToString() => Code.ToString();
}

public class KeyBindConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        var keyBind = (KeyBind)value!;
        writer.WriteValue(keyBind.Code.ToString());
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        var code = KeyCode.None;
        if (reader.ValueType == typeof(int))
        {
            code = (KeyCode)(int)reader.Value!;
        } else if (reader.ValueType == typeof(string))
        {
            if (!Enum.TryParse((string)reader.Value!, out code))
            {
                code = KeyCode.None;
            }
        }
        return new KeyBind(code);
    }

    public override bool CanConvert(Type objectType) => objectType == typeof(KeyBind);
}