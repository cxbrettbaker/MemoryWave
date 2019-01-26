using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class NoteScroller : MonoBehaviour
{
    public Transform hitbox;
    public Transform spawner;
    public float scrollSpeed; // around 3


    public float beatTempo;
    // Start is called before the first frame update
    void Start()
    {

        //beatTempo = 90;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //transform.position -= new Vector3(0f, beatTempo * Time.fixedDeltaTime, 0f);
        //transform.position -= new Vector3(0f, 3f, 0f);
    }
    
    public void UpdateBPM(float msPerBeat)
    {
        beatTempo = msPerBeat; 
        //Debug.Log("BPM CHANGE: " + beatTempo);
    }

    public float ReturnDelay()
    {
        //Debug.Log("Spawn " + spawner.localPosition.y + " Hitbox " + hitbox.localPosition.y + "\nPosition diff: " + (spawner.localPosition.y - hitbox.localPosition.y));
        return beatTempo * Time.fixedDeltaTime * (spawner.localPosition.y - hitbox.localPosition.y)/4;
    }

}
