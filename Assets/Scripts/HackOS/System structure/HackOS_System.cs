using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HackOS_System : MonoBehaviour
{
    public List<HackOS_CommandParser.NativeHackOSCommand> entryPoints;

    bool complete;
    public virtual void SetComplete ()
    {
        complete = true;
    }
    public bool IsComplete { get { bool c = complete; complete = false; return c; } }

    public virtual IEnumerator ParseCommand (string[] words, HackOS_TerminalScreen terminal, HackOS_systemPath targetPath)
    {
        SetComplete();
        throw new System.NotImplementedException();
    }
}
