using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable_terminal : Interactable
{
    public HackOSTerminal terminal;

    public override void Use(PlayerController player)
    {
        //throw new System.NotImplementedException();
        Transform target = terminal.cameraPosition;
        player.GetComponent<ZoomToPC>().ZoomToTarget(target, terminal);
    }
}
