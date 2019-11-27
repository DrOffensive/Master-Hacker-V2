using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class HackOS_data 
{
    public string name;
    public string data;
    public bool hidden;
    public ProtectionLevel protectionLevel;

    public virtual float SizeInBytes ()
    {
        return data.Length + name.Length + 8;
    }
}

public enum ProtectionLevel
{
    Public, Private, Locked
}