using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HackOS_AddRemoveFolder : HackOS_System
{
    public HackOS_CommandParser.NativeHackOSCommand protectionModifier;

    public override IEnumerator ParseCommand(string[] words, HackOS_TerminalScreen terminal, HackOS_systemPath targetPath)
    {
        if (words[0]==entryPoints[0].command) //create folder
        {
            if (words.Length >= 2)
            {
                if (targetPath.Extention == "" && targetPath.File == "")
                {
                    HackOS_directory directory = terminal.ActiveSession.drive.CheckDirectory(targetPath.Directories);
                    bool failed = false;
                    string name = words[1];
                    ProtectionLevel protectionLevel = ProtectionLevel.Public;

                    if(words.Length >= 3)
                    {
                        if(words[2] == protectionModifier.command)
                        {
                            if(words.Length == 4)
                            {
                                int lvl = -1; 
                                int.TryParse(words[3], out lvl);
                                if(lvl!=-1)
                                {
                                    if(lvl >= (int)terminal.ActiveSession.profile.userLevel)
                                    {
                                        protectionLevel = (ProtectionLevel)lvl;
                                    } else
                                    {
                                        terminal.InsertLine("Insufficient priviledge level", true);
                                        failed = true;
                                    }
                                }else
                                {
                                    terminal.InsertLine("Unknown keyword: " + words[3], true);
                                    failed = true;
                                }
                            }
                        } else
                        {
                            terminal.InsertLine("Unknown keyword: " + words[2], true);
                            failed = true;
                        }
                    }
                    if(!failed)
                    {
                        string dir = "~/";
                        foreach (string s in targetPath.Directories)
                        {
                            dir += s + "/";
                        }
                        if (name.Contains("."))
                        {
                            string[] file = name.Split('.');
                            if(file.Length == 2)
                            {
                                directory.data.Add(new HackOS_file(file[0], file[1], HackOSEncrytpion.None));
                                terminal.InsertLine("Created file" + dir + file[0] + "." + file[1], true);
                            } else
                            {
                                terminal.InsertLine("Invalid name " + name, true);
                            }
                        }
                        else
                        {
                            directory.data.Add(new HackOS_directory(name, false, protectionLevel));
                            terminal.InsertLine("Created folder" + dir + name, true);
                        }
                    }

                } else
                {
                    terminal.InsertLine(targetPath.File + "." + targetPath.Extention + " is not a valid folder", true);
                }
            } else
            {
                terminal.InsertLine("Specifiy name", true);
            }
        }
        else if (words[0]==entryPoints[1].command) //delete
        {
            if (words.Length == 1)
            {
                if (targetPath.File == "" && targetPath.Extention == "")
                {
                    if (targetPath.Directories[targetPath.Directories.Count - 1] == terminal.ActiveSession.MountedDirectories[terminal.ActiveSession.MountedDirectories.Count - 1])
                    {
                        string dir = "~/";
                        foreach (string s in targetPath.Directories)
                        {
                            dir += s + "/";
                        }
                        Debug.Log(dir);
                        terminal.InsertLine("Can't delete mounted folder", true);
                    }
                    else
                    {
                        if (targetPath.Directories.Count > 1)
                        {
                            List<string> dirs = new List<string>();
                            for (int i = 0; i < targetPath.Directories.Count - 1; i++)
                            {
                                dirs.Add(targetPath.Directories[i]);
                            }
                            HackOS_directory directory = terminal.ActiveSession.drive.CheckDirectory(dirs);
                            if (directory != null)
                            {
                                bool found = false;
                                foreach (HackOS_data data in directory.data)
                                {
                                    if (data.name == targetPath.Directories[targetPath.Directories.Count - 1] && data is HackOS_directory)
                                    {
                                        found = true;
                                        if ((int)data.protectionLevel <= (int)terminal.ActiveSession.profile.userLevel)
                                        {
                                            directory.data.Remove(data);
                                            terminal.InsertLine("Deleted " + targetPath.Directories[targetPath.Directories.Count - 1], true);
                                        }
                                        else
                                            terminal.InsertLine("Insufficient priviledge level", true);
                                        break;
                                    }
                                }
                                if (!found)
                                {
                                    string dir = "~/";
                                    foreach (string s in targetPath.Directories)
                                    {
                                        dir += s + "/";
                                    }
                                    terminal.InsertLine("unknown directory " + dir, true);
                                }
                            }
                            else
                            {
                                string dir = "~/";
                                foreach (string s in targetPath.Directories)
                                {
                                    dir += s + "/";
                                }
                                terminal.InsertLine("unknown directory " + dir, true);
                            }
                        }
                        else
                        {
                            terminal.InsertLine("Can't delete root folder", true);
                        }
                    }
                }
                else
                {
                    HackOS_directory directory = terminal.ActiveSession.drive.CheckDirectory(targetPath.Directories);
                    if (directory != null)
                    {
                        bool found = false;
                        foreach (HackOS_data data in directory.data)
                        {
                            if (data is HackOS_file)
                            {
                                HackOS_file file = (HackOS_file)data;
                                if (file.name == targetPath.File && file.extension == targetPath.Extention)
                                {
                                    found = true;
                                    if ((int)data.protectionLevel <= (int)terminal.ActiveSession.profile.userLevel)
                                    {
                                        directory.data.Remove(file);
                                        terminal.InsertLine("Deleted " + targetPath.File + "." + targetPath.Extention, true);
                                    }
                                    else
                                        terminal.InsertLine("Insufficient priviledge level", true);

                                    break;
                                }
                            }
                        }
                        if (!found)
                        {
                            string dir = "~/";
                            foreach (string s in targetPath.Directories)
                            {
                                dir += s + "/";
                            }
                            terminal.InsertLine("unknown directory " + dir + targetPath.File + "." + targetPath.Extention, true);
                        }
                    }
                }
            }
            else
            {
                terminal.InsertLine("unknown keyword: " + words[1], true);
            }
        }


        yield return null;
        SetComplete();
    }
}
