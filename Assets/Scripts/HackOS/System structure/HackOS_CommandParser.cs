using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HackOS_CommandParser : MonoBehaviour
{

    public YNResult ynTextResult;
    public int ynResult;
    public PWResult pwResult;

    public NativeHackOSCommand mountCommand, switchProfileCommand;

    public NativeHackOSCommand mountListModifier, mountJackModifier, mountFloppyModifier;
    public NativeHackOSCommand switchProfileGetModifier, switchProfileSetModifier;

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
        bool usedNativeCommand = false;
        bool usedSystem = false;
        if (words[0] == mountCommand.command.ToLower())
        {
            usedNativeCommand = true;
            if (words.Length < 3)
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
        } else if(words[0]==switchProfileCommand.command.ToLower())
        {
            usedNativeCommand = true;
            if (words.Length < 3)
            {
                if(words.Length==1)
                {
                    terminal.InsertLine("Must specify value", true);
                } else
                {
                    if(words[1]==switchProfileGetModifier.command.ToLower())
                    {
                        if (terminal.ActiveSession != null)
                        {
                            terminal.InsertLine("Profile: " + terminal.ActiveSession.profile.username, true);
                            terminal.InsertLine("Privilidge Level: " + terminal.ActiveSession.profile.userLevel.ToString(), true);
                        } else
                        {
                            terminal.InsertLine("No active session", true);
                        }
                    } else if (words[1]==switchProfileSetModifier.command)
                    {
                        if (terminal.ActiveSession != null)
                        {
                            pwResult = new PWResult();
                            StartCoroutine(LoginPrompt(terminal));
                            while (pwResult.position != 3)
                            {
                                yield return null;
                            }
                            HackOS_profile profile = terminal.ActiveSession.drive.CheckLoginDetails(pwResult.username, pwResult.password);
                            if (profile == null)
                            {
                                terminal.InsertLine("Invalid username or password", true);
                            }
                            else
                            {
                                if ((int)profile.userLevel >= (int)terminal.ActiveSession.drive.CheckDirectory(terminal.ActiveSession.MountedDirectories).protectionLevel)
                                {
                                    terminal.ActiveSession.profile = profile;
                                }
                                else
                                {
                                    terminal.InsertLine("Insufficient priviledge level", true);
                                }
                            }
                        } else
                        {
                            terminal.InsertLine("Mount a drive to begin a session", true);
                        }
                    }
                    else
                    {
                        terminal.InsertLine("Unknown keyword: " + words[1], true);
                    }
                }
            } else
            {
                terminal.InsertLine("Unknown keyword: " + words[2], true);
            }
        } else if(terminal.ActiveSession != null && !usedNativeCommand)
        {
            HackOS_systemPath targetPath;
            if(words[0].Contains("/") || IsFolder(words[0], terminal.ActiveSession))
            {
                string path = words[0];
                if(path.EndsWith("/"))
                {
                    path = path.Remove(path.Length - 1);
                }
                if (path.StartsWith("/"))
                {
                    path = path.Remove(0);
                }

                string[] splitPath = path.Contains("/") ? path.Split('/') : new string[1] { path };
                if (splitPath[splitPath.Length-1].Contains("."))
                {
                    List<string> dirs = splitPath[0] == terminal.ActiveSession.drive.rootDirectory.name ? new List<string>() : new List<string>(terminal.ActiveSession.MountedDirectories);
                    for(int i = 0; i < splitPath.Length - 1; i++)
                    {
                        dirs.Add(splitPath[i]);
                    }
                    string[] file = splitPath[splitPath.Length - 1].Split('.');
                    targetPath = new HackOS_systemPath(dirs, file[0], file[1]);
                } else
                {
                    List<string> dirs = splitPath[0] == terminal.ActiveSession.drive.rootDirectory.name ? new List<string>() : new List<string>(terminal.ActiveSession.MountedDirectories);
                    for (int i = 0; i < splitPath.Length; i++)
                    {
                        dirs.Add(splitPath[i]);
                    }
                    targetPath = new HackOS_systemPath(dirs, "", "");
                }

                string[] removedPath = new string[words.Length - 1];
                for(int i = 1; i < words.Length; i++)
                {
                    removedPath[i - 1] = words[i];
                }
                words = removedPath;
            } else
            {
                targetPath = new HackOS_systemPath(terminal.ActiveSession.MountedDirectories, "", "");
            }
            foreach (HackOS_System OSsystem in terminal.ActiveSession.drive.systems)
            {
                foreach (NativeHackOSCommand systemCommand in OSsystem.entryPoints)
                {
                    if (words[0] == systemCommand.command) {
                        usedSystem = true;
                        bool allow = true;
                        if (systemCommand.mustConfirm)
                        {
                            allow = false;
                            StartCoroutine(YNPrompt(terminal));
                            while (ynResult == 0)
                            {
                                yield return null;
                            }

                            if (ynResult == 1)
                            {
                                allow = true;
                            }
                        }
                        if (allow)
                        {
                            StartCoroutine(OSsystem.ParseCommand(words, terminal, targetPath));
                            int loop = 0;
                            while (!OSsystem.IsComplete || loop > 1000)
                            {
                                //Debug.Log("stuck" + 0 + ", " + OSsystem.name);
                                loop++;
                                yield return null;
                            }
                        }
                        break;
                    }
                }
                if (usedSystem)
                    break;
            }
        }

        if (!usedSystem && !usedNativeCommand)
        {
            terminal.InsertLine("Unknown keyword: " + words[0], true);
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

    bool IsFolder (string word, ActiveSession session)
    {
        if (session.drive.rootDirectory.name == word)
            return true;

        HackOS_directory cDir = session.drive.CheckDirectory(session.MountedDirectories);
        


        foreach (HackOS_data data in cDir.data)
        {
            if (data.name.ToLower() == word && data is HackOS_directory)
                return true;
        }

        return false;
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
