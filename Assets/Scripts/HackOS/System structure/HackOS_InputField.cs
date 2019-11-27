using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(Text))]
public class HackOS_InputField : MonoBehaviour
{
    Text textfield;
    public string prefix;
    string rawText;
    public bool password;
    public int maxChars = 40;

    public void Start()
    {
        textfield = GetComponent<Text>();
        rawText = "";
        prefix = "";
    }

    public string Text
    {
        get => rawText; set => rawText = value;
    }

    public void UpdateText ()
    {
        if (password)
        {
            string text = "";
            for(int i = 0; i < (rawText.Length); i++)
                text += HackOS_Statics.passwordChar;

            textfield.text = prefix + text;
        }
        else textfield.text = prefix + rawText;
    }
    public void UpdateText(int textIndex)
    {
        if (password)
        {
            string text = "";
            for (int i = 0; i < textIndex; i++)
                text += HackOS_Statics.passwordChar;
            text += StaticVariables.cursorChar;
            for (int i = textIndex; i < rawText.Length; i++)
                text += HackOS_Statics.passwordChar;

            textfield.text = prefix + text;
        }
        else
        {
            string text = "";
            for (int i = 0; i < textIndex; i++)
                text += rawText[i];
            text += StaticVariables.cursorChar;
            for (int i = textIndex; i < rawText.Length; i++)
                text += rawText[i];

            textfield.text = prefix + text;
        }
    }
}
