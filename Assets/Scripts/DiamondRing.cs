using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class DiamondRing : MonoBehaviour
{
    public Vector3 hitboxScale;
    public Vector3 spawnScale;
    public float beatsShownInAdvance;
    public float beatOfThisNote;
    public float songPosInBeats;
    public bool hit;
    public KeyCode keyCode;
    public bool goodHit;
    bool broadcast;

    // Update is called once per frame
    void Update()
    {
        if(!broadcast)
        {
            //Debug.Log("Expected " + keyCode);
            broadcast = true;
        }
        if (hit && Input.GetKeyDown(keyCode))
        {
            MemoryManager.Instance.NoteHit();
            Destroy(gameObject);
            Debug.Log("hit " + keyCode);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "normalDiamondHitbox")
        {
            hit = true;
            //gameObject.GetComponent<LineRenderer>().enabled = false;
        }
        else if (collision.tag == "despawnerDiamond")
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "normalDiamondHitbox")
        {
            hit = false;
            goodHit = false;
            MemoryManager.Instance.NoteMissed();
        }
    }

    private void Start()
    {
        spawnScale = new Vector3(9.061569f, 9.635226f, 0f);
        transform.localScale = spawnScale;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        songPosInBeats = Convert.ToSingle(FindObjectOfType<GameManager>().timer *1000);
        transform.localScale = Vector3.Lerp(spawnScale, hitboxScale - (spawnScale-hitboxScale), (beatsShownInAdvance - (beatOfThisNote - songPosInBeats)) / (beatsShownInAdvance*2));
    }
}
