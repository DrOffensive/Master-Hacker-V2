using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomToPC : MonoBehaviour
{
    Transform parent;
    public Transform PCCam;
    [Range(0.01f,3)]
    public float moveSpeed;
    public PlayerController playerController;
    public ZoomedCamAim zoomedCamAim;
    TransformData returnPos;

    private void Start()
    {
        parent = PCCam.transform.parent;
    }

    public void ZoomToTarget (Transform target, HackOSTerminal terminal)
    {
        IEnumerator zoom = Zoom(target, terminal);
        StartCoroutine(zoom);
    }

    public void ReturnToPlayer ()
    {
        StartCoroutine("Return");
    }

    IEnumerator Zoom (Transform target, HackOSTerminal terminal)
    {
        playerController.Frozen = true;
        PCCam.transform.parent = null;
        PCCam.GetComponent<Camera>().enabled = true;
        Quaternion startRot = transform.rotation;
        Vector3 line = target.position - PCCam.position;
        returnPos = new TransformData(PCCam);
        float moved = 0;

        while (moved < line.magnitude)
        {
            float spd = moveSpeed * Time.deltaTime;
            if(spd > line.magnitude - moved)
            {
                spd = line.magnitude - moved;
            }
            moved += spd;

            PCCam.transform.position += line.normalized * spd;

            PCCam.transform.rotation = Quaternion.Lerp(startRot, target.rotation, (1f / line.magnitude) * moved);

            yield return null;
        }

        //HackDosBase hackDos = FindObjectOfType<HackDosBase>();
        //hackDos.Enable(true);
        PCCam.transform.parent = target;
        zoomedCamAim.On = true;
        terminal.ActivateScreen();
    }

    IEnumerator Return ()
    {
        Cursor.visible = false;
        zoomedCamAim.On = false;
        PCCam.transform.parent = null;
        Quaternion startRot = transform.rotation;
        Vector3 line = returnPos.Position - PCCam.position;
        float moved = 0;

        while (moved < line.magnitude)
        {
            float spd = moveSpeed * Time.deltaTime;
            if (spd > line.magnitude - moved)
            {
                spd = line.magnitude - moved;
            }
            moved += spd;

            PCCam.transform.position += line.normalized * spd;

            PCCam.transform.rotation = Quaternion.Lerp(startRot, returnPos.Rotation, (1f / line.magnitude) * moved);

            yield return null;
        }

        playerController.Frozen = false;
        PCCam.transform.parent = parent;
        PCCam.GetComponent<Camera>().enabled = false;
        //HackDosBase hackDos = FindObjectOfType<HackDosBase>();
        //hackDos.Enable(false);
    }
}