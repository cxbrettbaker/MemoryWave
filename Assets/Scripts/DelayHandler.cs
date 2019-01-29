using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class DelayHandler : MonoBehaviour
{
    public Transform hitbox;
    public Transform spawner;
    
    public float UpdateBPM(float msPerBeat)
    {
        return msPerBeat * Time.fixedDeltaTime * (spawner.localPosition.y - hitbox.localPosition.y) / 4;
        //Debug.Log("BPM CHANGE: " + beatTempo);
    }

}
