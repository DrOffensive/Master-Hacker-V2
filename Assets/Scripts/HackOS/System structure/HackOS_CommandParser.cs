using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HackOS_CommandParser : MonoBehaviour
{

    public YNResult ynTextResult;
    public int ynResult;
    public PWResult pwResult;

    public NativeHackOSCommand mountCommand, navigateCommand, listDirectoryCommand;

    public NativeHackOSCommand mountListModifier, mountJackModifier, mountFloppyModifier;
    public NativeHackOSCommand navigateBackModifier;

    public void ParseCommand (HackOS_TerminalScreen terminal, HackOS_driveStructure drive, string command)
    {
        string[] commands = command.Split(';');

        foreach (string c in commands)
        {
            if (drive != null)
                drive.EnqueueCommand(ExecuteCommand(terminal, drive, c));
            else
                StartCoroutine(ExecuteCommand(terminal, drive, c));
        }
    }

    public IEnumerator ExecuteCommand(HackOS_TerminalScreen terminal, HackOS_driveStructure drive, string command)
    {
        string[] words = command.ToLower().Split(' ');

        if(words[0] == mountCommand.command.ToLower())
        {
            if(words.Length < 3)
            {
                if (words.Length == 1)
                {
                    terminal.InsertLine("Must specify value", true);
                }
                else
                {

                    if (words[1] == mountListModifier.command.ToLower())
                    {
                        terminal.InsertLine("Mainframe: " + (terminal.jackConnection == null ? "Not connected" : "Connected"), true);
                        terminal.InsertLine("Floppy: " + (terminal.floppyDrive == null ? "Not connected" : "Connected"), true);
                    }
                    else if (words[1] == mountJackModifier.command.ToLower())
                    {
                        if(terminal.jackConnection != null)
                        {
                            if (terminal.jackConnection.rootDirectory.protectionLevel != ProtectionLevel.Public)
                            {
                                pwResult = new PWResult();
                                StartCoroutine(LoginPrompt(terminal));
                                while (pwResult.position != 3)
                                {
                                    yield return null;
                                }
                                HackOS_profile profile = terminal.jackConnection.CheckLoginDetails(pwResult.username, pwResult.password);
                                if (profile == null)
                                {
                                    terminal.InsertLine("Unknown username or password", true);
                                }
                                else
                                {
                                    if ((int)profile.userLevel >= (int)terminal.jackConnection.rootDirectory.protectionLevel)
                                    {
                                        ActiveSession session = new ActiveSession(profile, terminal.jackConnection);
                                        session.MountedDirectories = new List<string>() { terminal.jackConnection.rootDirectory.name };
                                        terminal.ActiveSession = session;
                                    }
                                    else
                                    {
                                        terminal.InsertLine("Insufficient priviledge level", true);
                                    }
                                }
                            } else
                            {
                                HackOS_profile profile = new HackOS_profile();
                                profile.username = "guest";
                                profile.password = "";
                                profile.userLevel = UserLevel.Guest;

                                ActiveSession session = new ActiveSession(profile, terminal.jackConnection);
                                session.MountedDirectories = new List<string>() { terminal.jackConnection.rootDirectory.name };
                                terminal.ActiveSession = session;
                            }
                        } else
                        {
                            terminal.InsertLine("Unable to connect to: Mainframe", true);
                        }
                    }
                    else if (words[1] == mountFloppyModifier.command.ToLower())
                    {

                        if (terminal.floppyDrive != null)
                        {
                            if (terminal.floppyDrive.rootDirectory.protectionLevel != ProtectionLevel.Public)
                            {
                                pwResult = new PWResult();
                                StartCoroutine(LoginPrompt(terminal));
                                while (pwResult.position != 3)
                                {
                                    yield return null;
                                }
                                HackOS_profile profile = terminal.floppyDrive.CheckLoginDetails(pwResult.username, pwResult.password);
                                if (profile == null)
                                {
                                    terminal.InsertLine("Unknown username or password", true);
                                }
                                else
                                {
                                    if ((int)profile.userLevel >= (int)terminal.jackConnection.rootDirectory.protectionLevel)
                                    {
                                        ActiveSession session = new ActiveSession(profile, terminal.floppyDrive);
                                        session.MountedDirectories = new List<string>() { terminal.floppyDrive.rootDirectory.name };
                                        terminal.ActiveSession = session;
                                    }
                                    else
                                    {
                                        terminal.InsertLine("Insufficient priviledge level", true);
                                    }
                                }
                            }
                            else
                            {
                                HackOS_profile profile = new HackOS_profile();
                                profile.username = "guest";
                                profile.password = "";
                                profile.userLevel = UserLevel.Guest;

                                ActiveSession session = new ActiveSession(profile, terminal.floppyDrive);
                                session.MountedDirectories = new List<string>() { terminal.floppyDrive.rootDirectory.name };
                                terminal.ActiveSession = session;
                            }
                        }
                        else
                        {
                            terminal.InsertLine("Unable to connect to: Floppy", true);
                        }
                    }
                    else
                    {
                        terminal.InsertLine("Unknown keyword " + words[1], true);
                    }
                }
            } else
            {
                terminal.InsertLine("Unknown keyword: " + words[2], true);
            }
        }

        if(terminal.ActiveSession != null)
        {

        }


        //How to handle Yes/No Prompt
        /////////////////////////////
        /*StartCoroutine(YNPrompt(terminal));
        while (ynResult==0)
        {
            yield return null;
        }

        if(ynResult == 1)
        {
            terminal.InsertLine("You chose [YES]", true);
        } else
        {
            terminal.InsertLine("You chose [NO]", true);
        }*/


        yield return null;

        if(drive != null)
            drive.CurrentCommand = null;

        terminal.ScrollLine();
    }


    public IEnumerator YNPrompt (HackOS_TerminalScreen screen)
    {
        ynResult = 0;
        while (ynResult==0)
        {
            screen.SetMode = HackOS_TerminalScreen.InputMode.YNPrompt;
            screen.ScrollLine("Are you sure [Y/N] ?: ");
            while (ynTextResult==null)
            {
                yield return null;
            }

            string input = ynTextResult.input;
            if (input.ToLower() == "y")
            {
                ynResult = 1;
            }
            else if (input.ToLower() == "n")
            {
                ynResult = -1;
            } else
            {
                ynTextResult = null;
            }
        }
        screen.SetMode = HackOS_TerminalScreen.InputMode.Normal;
    }

    public IEnumerator LoginPrompt(HackOS_TerminalScreen screen)
    {
        ynResult = 0;
        bool redo = true;
        while (redo)
        {
            screen.SetMode = HackOS_TerminalScreen.InputMode.PasswordFormula;
            screen.ScrollLine("Username: ");
            while (pwResult.position == 0)
            {
                yield return null;
            }

            string input = pwResult.username.ToLower();
            if (input == "")
                pwResult.position = 0;
            else
            {
                redo = false;
            }
            yield return null;
        }
        redo = true;
        while(redo)
        {
            screen.ScrollLine("Password: ");
            screen.SetPasswordField();
            while (pwResult.position == 1)
            {
                yield return null;
            }
            string input = pwResult.username.ToLower();
            if (input == "")
                pwResult.position = 1;
            else
            {
                pwResult.position = 3;
                redo = false;
            }
        }
        screen.SetMode = HackOS_TerminalScreen.InputMode.Normal;
    }
    public class YNResult
    {
        public string input;

        public YNResult(string input)
        {
            this.input = input;
        }
    }

    public class PWResult
    {
        public string username, password;

        public void Next (string input)
        {
            if(position == 0)
            {
                position = 1;
                username = input;
            } else
            {
                position = 2;
                password = input;
            }
        }

        public int position = 0;
    }

    [System.Serializable]
    public class NativeHackOSCommand
    {
        public string command;
        public bool mustConfirm;
    }
}
