using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HackOS_directory : HackOS_data
{
    new public List<HackOS_data> data;

    public HackOS_directory(HackOS_directory copyFrom)
    {
        name = copyFrom.name;
        data = copyFrom.data;
        hidden = copyFrom.hidden;
        protectionLevel = copyFrom.protectionLevel;
    }

    public HackOS_directory(string name, bool hidden, ProtectionLevel protection)
    {
        data = new List<HackOS_data>();
        this.name = name;
        this.hidden = hidden;
        this.protectionLevel = protection;
    }
}
