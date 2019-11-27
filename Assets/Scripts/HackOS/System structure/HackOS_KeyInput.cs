using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HackOS_KeyInput : MonoBehaviour
{

    int tab = 0;
    public int Tab
    {
        get
        {
            int t = tab;
            tab = 0;
            return t;
        }
    }
    bool escape;
    public bool Escape
    {
        get
        {
            bool a = escape;
            escape = false;
            return a;
        }
    }
    bool enter;
    public bool Enter
    {
        get
        {
            bool a = enter;
            enter = false;
            return a;
        }
    }
    bool anyKey;
    public bool AnyKey
    {
        get
        {
            bool a = anyKey;
            anyKey = false;
            return a;
        }
    }
    bool upArrow;
    public bool UpArrow
    {
        get
        {
            bool a = upArrow;
            upArrow = false;
            return a;
        }
    }
    bool downArrow;
    public bool DownArrow
    {
        get
        {
            bool a = downArrow;
            downArrow = false;
            return a;
        }
    }
    bool leftArrow;
    public bool LeftArrow
    {
        get
        {
            bool a = leftArrow;
            leftArrow = false;
            return a;
        }
    }
    bool rightArrow;
    public bool RightArrow
    {
        get
        {
            bool a = rightArrow;
            rightArrow = false;
            return a;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ScanSpecialKeys (HackOS_Screen screen)
    {
        if (AnyKey)
            screen.OnAnyKey();
        if (Enter)
            screen.OnEnter();
        if (Escape)
            screen.OnEscape();
        if (tab != 0)
            screen.OnTab(Tab);
        if (RightArrow)
            screen.OnRightArrow();
        if (LeftArrow)
            screen.OnLeftArrow();
        if (UpArrow)
            screen.OnUpArrow();
        if (DownArrow)
            screen.OnDownArrow();
    }

    public void ScanKeys(HackOS_InputField inputField, ref int textIndex)
    {
        if (inputField != null)
        {
            foreach (char c in Input.inputString)
            {
                if (c == '\b') // has backspace/delete been pressed?
                {
                    anyKey = true;
                    if (inputField.Text.Length != 0)
                    {
                        {
                            if (textIndex != 0)
                            {
                                string t1 = "";
                                int t1Length = textIndex - 1;
                                for (int i = 0; i < t1Length; i++)
                                {
                                    t1 += inputField.Text[i];
                                }

                                int t2Length = inputField.Text.Length - (t1Length + 1);
                                for (int i = 0; i < t2Length; i++)
                                {
                                    t1 += inputField.Text[t1Length + 1 + i];
                                }
                                inputField.Text = t1;
                                textIndex--;
                            }
                        }
                    }
                }
                else if (c == '')
                {
                    anyKey = true;
                    string[] words = inputField.Text.Split(' ');
                    if (words.Length > 0)
                    {
                        int soFar = 0;
                        int w = words.Length - 1;
                        for (int i = 0; i < words.Length; i++)
                        {
                            soFar += words[i].Length + 1;
                            if (textIndex < soFar)
                            {
                                w = i;
                                break;
                            }
                        }
                        string t = "";
                        for (int i = 0; i < words.Length; i++)
                        {
                            if (i != w)
                            {
                                t += words[i] + (i == words.Length - 1 ? "" : " ");
                            }
                            else
                            {
                                textIndex = t.Length - 1 > 0 ? t.Length - 1 : 0;
                            }
                        }
                        inputField.Text = t;
                    }
                }
                else if ((c == '\n') || (c == '\r')) // enter/return
                {
                    enter = true;
                }
                else
                {
                    anyKey = true;
                    if (inputField.Text.Length < inputField.maxChars)
                    {
                        string t = "";
                        for (int i = 0; i < textIndex; i++)
                        {
                            t += inputField.Text[i];
                        }
                        t += c;
                        for (int i = 0; i < inputField.Text.Length - Mathf.Clamp(textIndex - 1, 0, Mathf.Abs(textIndex)) - 1; i++)
                        {
                            t += inputField.Text[i + textIndex];
                        }

                        inputField.Text = t;
                        textIndex++;
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.Delete))
            {
                anyKey = true;
                string t1 = "";
                int t1Length = textIndex;
                for (int i = 0; i < t1Length; i++)
                {
                    t1 += inputField.Text[i];
                }

                int t2Length = inputField.Text.Length - (t1Length + 1);
                for (int i = 0; i < t2Length; i++)
                {
                    t1 += inputField.Text[t1Length + 1 + i];
                }
                inputField.Text = t1;
            }
        } else
        {
            if (Input.GetKeyDown(KeyCode.Return))
                enter = true;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
            escape = true;

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            anyKey = true;
            if (Input.GetKey(KeyCode.LeftControl))
            {
                tab = -1;
            }
            else
            {
                tab = 1;
            }
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            upArrow = true;
            anyKey = true;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            downArrow = true;
            anyKey = true;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            rightArrow = true;
            anyKey = true;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            leftArrow = true;
            anyKey = true;
        }
        if(inputField !=null)
            inputField.UpdateText(textIndex);
    }
}
