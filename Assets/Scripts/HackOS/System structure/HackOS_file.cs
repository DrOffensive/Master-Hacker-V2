using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HackOS_file : HackOS_data
{
    public string extension;
    public HackOSEncrytpion encrytpionLevel;

    public HackOS_file(string name, string extension, HackOSEncrytpion encrytpionLevel)
    {
        this.name = name;
        this.extension = extension;
        this.encrytpionLevel = encrytpionLevel;
    }
}

public enum HackOSEncrytpion
{
    None, Low, Medium, high
}