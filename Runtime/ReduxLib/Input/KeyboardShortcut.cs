using System;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace ReduxLib.Input;


[JsonConverter(typeof(KeyboardShortcutConverter))]
public struct KeyboardShortcut
{
    public KeyCode[] Modifiers;
    public KeyCode Main;

    public bool Down
    {
        get
        {
            if (InputHelper.IsRebindingHappening) return false;
            foreach (var modifier in Modifiers)
            {
                if (!UnityEngine.Input.GetKey(modifier)) return false;
            }
            return UnityEngine.Input.GetKeyDown(Main);
        }
    }
    
    public bool Up
    {
        get
        {
            if (InputHelper.IsRebindingHappening) return false;
            foreach (var modifier in Modifiers)
            {
                if (!UnityEngine.Input.GetKey(modifier)) return false;
            }
            return UnityEngine.Input.GetKeyUp(Main);
        }
    }
    
    public bool Held
    {
        get
        {
            if (InputHelper.IsRebindingHappening) return false;
            foreach (var modifier in Modifiers)
            {
                if (!UnityEngine.Input.GetKey(modifier)) return false;
            }
            return UnityEngine.Input.GetKey(Main);
        }
    }
    
    public KeyboardShortcut(KeyCode main, params KeyCode[] modifiers)
    {
        Modifiers = modifiers;
        Main = main;
    }

    public KeyboardShortcut(string shortcut)
    {
        var split = shortcut.Split('+');
        Modifiers = new KeyCode[split.Length-1];
        Main = KeyCode.None;
        Enum.TryParse(split[^1], out Main);

        for (var i = 1; i < split.Length - 1; i++)
        {
            Modifiers[i] = KeyCode.None;
            Enum.TryParse(split[i], out Modifiers[i]);
        }
    }

    public override string ToString()
    {
        var list = Modifiers.ToList();
        list.Add(Main);
        return string.Join("+", list);
    }
}

public class KeyboardShortcutConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        writer.WriteValue(value!.ToString());
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        return reader.ValueType == typeof(string) ? new KeyboardShortcut((string)reader.Value!) : new KeyboardShortcut(KeyCode.None);
    }

    public override bool CanConvert(Type objectType) => objectType == typeof(KeyboardShortcut);
}