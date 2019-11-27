using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HackOS_file : HackOS_data
{
    public string extension;
    public HackOSEncrytpion encrytpionLevel;
}

public enum HackOSEncrytpion
{
    None, Low, Medium, high
}