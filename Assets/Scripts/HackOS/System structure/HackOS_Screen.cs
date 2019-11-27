using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HackOS_Screen : MonoBehaviour
{
    HackOS_KeyInput keyInput;
    bool active;

    public virtual void Start()
    {
        keyInput = FindObjectOfType<HackOS_KeyInput>();
    }

    public bool IsActive { get => active; set => active = value; }
    public HackOS_KeyInput KeyInput { get => keyInput; }

    public abstract void OnEscape();
    public virtual void OnEnter() { }


    public virtual void OnAnyKey() { }

    public virtual void OnUpArrow() { }
    public virtual void OnDownArrow() { }

    public virtual void OnRightArrow() { }
    public virtual void OnLeftArrow() { }
    public virtual void OnTab(int tab) { }

    public virtual void Update ()
    {
        if (active)
        {
            int t = 0;
            keyInput.ScanKeys(null, ref t);
            keyInput.ScanSpecialKeys(this);
        }
    }
}
