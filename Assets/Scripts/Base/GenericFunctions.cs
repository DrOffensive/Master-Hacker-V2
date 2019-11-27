using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GenericFunctions
{
    public static void ShuffleList<T>(ref List<T> list)
    {
        for(int i = list.Count-1; i >= 0; i--)
        {
            int random = Random.Range(0, i);
            T stored = list[i];
            list[i] = list[random];
            list[random] = stored;
        }
    }

    public static float LimitDecimals (float value, int decimals)
    {
        float pow = Mathf.Pow(10, decimals);
        return (Mathf.Floor(value * pow) / pow);
    }

    public static string[] FileSizeUnits()
    {
        return new string[] { "Byte(s)", "Kilobyte(s)", "Megabyte(s)", "Gigabyte(s)", "Terabyte(s)", "Petabyte(s)" };
    }

    public static string RGBAtoHex (Color rgba)
    {
        return "#" + ColorUtility.ToHtmlStringRGBA(rgba);
    }

    public static Color HexToRGBA (string hex)
    {
        Color color = new Color();
        if (hex.StartsWith("#"))
        {
            ColorUtility.TryParseHtmlString(hex, out color);
            return color;
        }
        else
            return Color.black;
    }

    public static string RemoveChar (char charToDelete, string text)
    {
        string t = "";
        foreach(char c in text)
        {
            if (c != charToDelete)
                t += c;
        }
        return t;
    }

    public static string RemoveChars (List<char> charsToDelete, string text)
    {
        string t = "";
        foreach (char c in text)
        {
            if (!charsToDelete.Contains(c))
                t += c;
        }
        return t;
    }

    public static float ParseFloat (string input)
    {
        string output = input;
        if(output.Contains("."))
        {
            output = output.Replace(".", ",");
        }
        if (output[0] == ',')
            output = "0" + output;

        return float.Parse(output);
    }   

    /// <summary>
    /// Add copy-paste functionality to any text field
    /// Returns changed text or NULL.
    /// Usage: text = HandleCopyPaste (controlID) ?? text;
    /// </summary>
    public static string HandleCopyPaste(int controlID)
    {
        if (controlID == GUIUtility.keyboardControl)
        {
            if (Event.current.type == EventType.KeyUp && (Event.current.modifiers == EventModifiers.Control || Event.current.modifiers == EventModifiers.Command))
            {
                if (Event.current.keyCode == KeyCode.C)
                {
                    Event.current.Use();
                    TextEditor editor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
                    editor.Copy();
                }
                else if (Event.current.keyCode == KeyCode.V)
                {
                    Event.current.Use();
                    TextEditor editor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
                    editor.Paste();
                    return editor.text;
                }
            }
        }
        return null;
    }

    /// <summary>
    /// TextField with copy-paste support
    /// </summary>
    public static string TextField(string value, params GUILayoutOption[] options)
    {
        int textFieldID = GUIUtility.GetControlID("TextField".GetHashCode(), FocusType.Keyboard) + 1;
        if (textFieldID == 0)
            return value;

        // Handle custom copy-paste
        value = HandleCopyPaste(textFieldID) ?? value;

        return GUILayout.TextField(value);
    }
}

[System.Serializable]
public struct RangeConverter
{
    [System.Serializable]
    public struct RangeItem
    {
        public float value, output;
    }

    public List<RangeItem> items;

    public float Evaluate (float input)
    {
        for(int i = 0; i < items.Count; i++)
        {
            if (input <= items[i].value)
                return items[i].output;
        }
        return items[items.Count - 1].output;
    }
}