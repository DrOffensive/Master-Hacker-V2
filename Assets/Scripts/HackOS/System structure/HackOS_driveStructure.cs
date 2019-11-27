using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HackOS_driveStructure : MonoBehaviour
{
    public List<HackOS_profile> profiles;
    public HackOS_directory rootDirectory;

    Queue<IEnumerator> commandQueue = new Queue<IEnumerator>();

    public List<HackOS_System> systems = new List<HackOS_System>();

    public void Start()
    {
        if (rootDirectory.data == null)
            rootDirectory.data = new List<HackOS_data>();
    }

    public void EnqueueCommand (IEnumerator command)
    {
        commandQueue.Enqueue(command);
    }

    IEnumerator currentCommand;

    public IEnumerator CurrentCommand { get => currentCommand; set => currentCommand = value; }

    private void Update()
    {
        if (commandQueue.Count > 0)
        {
            if (currentCommand == null)
            {
                currentCommand = commandQueue.Dequeue();
                StartCoroutine(currentCommand);
            }
        }
    }

    public HackOS_profile CheckLoginDetails (string username, string password)
    {
        foreach(HackOS_profile profile in profiles)
        {
            if(profile.username.ToLower()==username.ToLower())
            {
                if (profile.password.ToLower() == password.ToLower())
                    return profile;
                else
                    return null;

            }
        }

        return null;
    } 

    public HackOS_directory CheckDirectory (List<string> directories)
    {
        if (directories.Count > 0 && directories[0].Equals(rootDirectory.name))
        {
            HackOS_directory currentDir = rootDirectory;
            for(int i = 1; i < directories.Count;i++)
            {
                bool found = false;
                foreach(HackOS_data data in currentDir.data)
                {
                    if(data.name.Equals(directories[i]))
                    {
                        currentDir = data as HackOS_directory;
                        if (currentDir != null)
                        {
                            found = true;
                            break;
                        }
                    }
                }
                if (!found)
                    return null;
            }
            return currentDir;
        }
        else return null;
    }

    public UserLevel CheckDirectoryProtection (List<string> directories)
    {
        UserLevel level = UserLevel.Guest;
        if (directories.Count > 0 && directories[0].Equals(rootDirectory.name))
        {
            HackOS_directory currentDir = rootDirectory;
            level = (UserLevel)((int)currentDir.protectionLevel);
            for (int i = 1; i < directories.Count; i++)
            {
                bool found = false;
                foreach (HackOS_data data in currentDir.data)
                {
                    if (data.name.Equals(directories[i]))
                    {
                        currentDir = data as HackOS_directory;
                        level = (UserLevel)((int)currentDir.protectionLevel);
                        if (currentDir != null)
                        {
                            found = true;
                            break;
                        }
                    }
                }
                if (!found)
                    return level;
            }
            return level;
        }
        else return level;
    }

}

[System.Serializable]
public class HackOS_profile
{
    public string username, password;
    public UserLevel userLevel;
}

public enum UserLevel
{
    Guest, User, Admin
}

[System.Serializable]
public class HackOS_systemPath
{
    List<string> directories;
    string file, extention;

    public HackOS_systemPath(List<string> directories, string file, string extention)
    {
        this.directories = directories;
        this.file = file;
        this.extention = extention;
    }

    public List<string> Directories { get => directories; }
    public string File { get => file; }
    public string Extention { get => extention; }
}