using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HackOSSystem_navigateDrive : HackOS_System
{
    public override IEnumerator ParseCommand(string[] words, HackOS_TerminalScreen terminal, HackOS_systemPath targetPath)
    {
        //return base.ParseCommand(words, terminal, targetPath);
        if(words[0]==entryPoints[0].command.ToLower()) // cd
        {
            if(words.Length < 3)
            {
                if(words.Length==1)
                {
                    terminal.InsertLine("Must specify value", true);
                } else
                {
                    if(words[1] == "..")
                    {
                        if (terminal.ActiveSession.MountedDirectories.Count > 1) {
                            List<string> mountedDirs = terminal.ActiveSession.MountedDirectories;
                            terminal.ActiveSession.MountedDirectories = new List<string>();
                            for (int i = 0; i < mountedDirs.Count - 1; i++)
                                terminal.ActiveSession.MountedDirectories.Add(mountedDirs[i]);
                        } else
                        {
                            terminal.InsertLine("You can't navigate further back", true);
                        }
                    } else
                    {
                        string path = words[1];
                        if (path.EndsWith("/"))
                        {
                            path = path.Remove(path.Length - 1);
                        }
                        if (path.StartsWith("/"))
                        {
                            path = path.Remove(0);
                        }

                        string[] splitPath = path.Contains("/") ? path.Split('/') : new string[1] { path } ;
                        if (splitPath[splitPath.Length - 1].Contains("."))
                        {
                            terminal.InsertLine("Can't mount a file", true);
                        }
                        else
                        {
                            List<string> dirs = splitPath[0] == terminal.ActiveSession.drive.rootDirectory.name ? new List<string>() : new List<string>(terminal.ActiveSession.MountedDirectories);
                            foreach(string dir in splitPath)
                            {
                                dirs.Add(dir);
                            }
                            HackOS_directory directory = terminal.ActiveSession.drive.CheckDirectory(dirs);
                            if (directory != null)
                            {
                                if((int)directory.protectionLevel <= (int)terminal.ActiveSession.profile.userLevel)
                                    terminal.ActiveSession.MountedDirectories = dirs;
                                else
                                    terminal.InsertLine("Insufficient priviledge level", true);
                            }
                            else
                            {
                                string dir = "~/";
                                foreach (string s in dirs)
                                {
                                    dir += s + "/";
                                }
                                terminal.InsertLine("unknown directory " + dir, true);
                            }
                        }
                    }
                }
            } else
            {
                terminal.InsertLine("Unknown keyword:" + words[2], true);
            }
        }
        else if (words[0] == entryPoints[1].command.ToLower()) // dir
        {
            if (words.Length == 1)
            {
                if (targetPath.File == "" && targetPath.Extention == "")
                {
                    HackOS_directory directory = terminal.ActiveSession.drive.CheckDirectory(targetPath.Directories);
                    if (directory.data.Count > 0)
                    {
                        foreach (HackOS_data data in directory.data)
                        {
                            HackOS_directory dir = data as HackOS_directory;
                            if (dir != null)
                            {
                                terminal.InsertLine(GenericFunctions.StepIn("- " + dir.name, 20, 3) + "[Directory]", true);
                            }

                            HackOS_file file = data as HackOS_file;
                            if (file != null)
                            {
                                terminal.InsertLine(GenericFunctions.StepIn("- " + file.name + "." + file.extension, 20, 3) + "<size is coming>", true);
                            }
                        }
                    } else
                    {
                        terminal.InsertLine("Directory is empty", true);
                    }
                }
                else
                {
                    terminal.InsertLine("Invalid path", true);
                }
            } else
            {
                terminal.InsertLine("Unknown keyword: " + words[1], true);
            }
        }

        yield return null;
        SetComplete();
    }
}
