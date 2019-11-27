using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HackOSTerminal : MonoBehaviour
{

    public JackSocket mainframeSocket;
    public Transform cameraPosition;
    public HackOS_Screen mainScreen;

    public void ActivateScreen ()
    {
        mainScreen.IsActive = true;
    }

    public void DeactivateScreen()
    {
        mainScreen.IsActive = false;
    }

}
