using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HackOS_TerminalScreen : HackOS_Screen
{
    public HackOSTerminal terminal;

    ActiveSession activeSession;

    public HackOS_driveStructure jackConnection, floppyDrive;

    InputMode mode = InputMode.Normal;

    public HackOS_InputField[] inputFields;
    int field, textIndex;
    HackOS_InputField currentField;

    public InputMode SetMode { set => mode = value; }
    public ActiveSession ActiveSession { get => activeSession; set => activeSession = value; }

    public override void OnEscape()
    {
        FindObjectOfType<ZoomToPC>().ReturnToPlayer();
        terminal.DeactivateScreen();
    }

    public override void Start()
    {
        base.Start();
        currentField = inputFields[field];
        currentField.prefix = "";
        InsertLine("Hack OS terminal v2.0", false);
        ScrollLine();
    }

    public override void OnEnter()
    {
        if (currentField != null)
        {
            switch (mode) {
                case InputMode.Normal:
                    string t = currentField.Text;
                    DefocusField();
                    FindObjectOfType<HackOS_CommandParser>().ParseCommand(this, activeSession != null ? activeSession.drive : null, t);
                    //ScrollLine();
                    break;

                case InputMode.YNPrompt:
                    string t1 = currentField.Text;
                    DefocusField();
                    FindObjectOfType<HackOS_CommandParser>().ynTextResult = new HackOS_CommandParser.YNResult(t1);
                    break;

                case InputMode.PasswordFormula:
                    string t2 = currentField.Text;
                    DefocusField();
                    FindObjectOfType<HackOS_CommandParser>().pwResult.Next(t2);
                    break;

            }
        }
    }

    public override void Update()
    {
        if (IsActive)
        {
            if (currentField != null)
            {
                KeyInput.ScanKeys(currentField, ref textIndex);
            } else
            {
                int t = 0;
                KeyInput.ScanKeys(null, ref t);
            }
            KeyInput.ScanSpecialKeys(this);
        }
    }

    public override void OnLeftArrow()
    {
        if(textIndex > 0 && currentField != null)
        {
            textIndex--;
            currentField.UpdateText(textIndex);
        }
    }

    public override void OnRightArrow()
    {
        if (currentField != null && textIndex < currentField.Text.Length)
        {
            textIndex++;
            currentField.UpdateText(textIndex);
        }
    }

    public void ScrollLine ()
    {
        if (field+1 < inputFields.Length)
        {
            field++;
            FocusField();
        } else
        {
            for(int i = 1; i < inputFields.Length; i++)
            {
                HackOS_InputField cField = inputFields[i];
                inputFields[i - 1].Text = cField.Text;
                inputFields[i - 1].prefix = cField.prefix;
                inputFields[i - 1].password = cField.password;
                inputFields[i - 1].UpdateText();
            }
            inputFields[inputFields.Length - 1].Text = "";
            inputFields[inputFields.Length - 1].password = false;
            FocusField();
        }
        textIndex = 0;
    }

    public void ScrollLine(string customPrefix)
    {
        if (field + 1 < inputFields.Length)
        {
            field++;
            FocusField(customPrefix);
        }
        else
        {
            for (int i = 1; i < inputFields.Length; i++)
            {
                HackOS_InputField cField = inputFields[i];
                inputFields[i - 1].Text = cField.Text;
                inputFields[i - 1].prefix = cField.prefix;
                inputFields[i - 1].password = cField.password;
                inputFields[i - 1].UpdateText();
            }
            inputFields[inputFields.Length-1].Text = "";
            inputFields[inputFields.Length - 1].password = false;
            FocusField(customPrefix);
        }
        textIndex = 0;
    }

    public void DefocusField ()
    {
        currentField.UpdateText();
        currentField = null;
    }

    public void FocusField()
    {
        if(field < inputFields.Length)
        {
            currentField = inputFields[field];
            if(activeSession == null)
            {
                currentField.prefix = "-> ";
            } else
            {
                string prefix = "~/";
                foreach (string dir in activeSession.MountedDirectories)
                {
                    prefix += dir + "/";
                }
                prefix += "> ";
                currentField.prefix = prefix;
            }
        }
    }

    public void FocusField (string customPrefix)
    {
        if (field < inputFields.Length)
        {
            currentField = inputFields[field];
            currentField.prefix = customPrefix;
        }
    }

    public void SetPasswordField()
    {
        currentField.password = true;
    }

    public void InsertLine (string line, bool scroll)
    {
        if(scroll)
            ScrollLine("");

        currentField.Text = line;
        currentField.UpdateText();
        DefocusField();
    }

    public enum InputMode
    {
        Normal, YNPrompt, PasswordFormula
    }
}


[System.Serializable]
public class ActiveSession
{
    public HackOS_profile profile;
    public HackOS_driveStructure drive;
    public bool showHiddenFolders = false;
    List<string> mountedDirectories;
    public List<string> MountedDirectories { get => mountedDirectories; set => mountedDirectories = value; }

    public ActiveSession (HackOS_profile profile, HackOS_driveStructure drive)
    {
        this.profile = profile;
        this.drive = drive;
    }
}
