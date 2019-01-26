using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Ring : MonoBehaviour
{
    public Vector3 hitboxPos;
    public Vector3 spawnerPos;
    public float beatsShownInAdvance;
    public float beatOfThisNote;
    public float songPosInBeats;

    // Update is called once per frame
    void FixedUpdate()
    {
        songPosInBeats = Convert.ToSingle(FindObjectOfType<GameManager>().offsetTime);
        transform.position = Vector3.Lerp(spawnerPos, new Vector3(spawnerPos.x, hitboxPos.y-(spawnerPos.y - hitboxPos.y), spawnerPos.z), (beatsShownInAdvance - (beatOfThisNote - songPosInBeats))/(beatsShownInAdvance*2));
    }
}
