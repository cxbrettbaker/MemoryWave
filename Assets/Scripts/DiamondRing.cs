using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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

    // Update is called once per frame
    void Update()
    {
        if (hit && Input.GetKeyDown(keyCode))
        {
            GameManager.instance.NoteHit(goodHit);
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "goodHitbox")
        {
            goodHit = true;
            hit = true;
            gameObject.SetActive(false);
        }
        if (collision.tag == "normalHitbox")
        {
            hit = true;
            gameObject.GetComponent<LineRenderer>().enabled = false;
        }
        else if (collision.tag == "despawner")
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "goodHitbox")
        {
            goodHit = false;
            hit = false;
        }
        if (collision.tag == "normalHitbox")
        {
            hit = false;
            goodHit = false;
            GameManager.instance.NoteMissed();
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
        songPosInBeats = Convert.ToSingle(FindObjectOfType<GameManager>().offsetTime);
        transform.localScale = Vector3.Lerp(spawnScale, hitboxScale - (spawnScale-hitboxScale), (beatsShownInAdvance - (beatOfThisNote - songPosInBeats)) / (beatsShownInAdvance*2));
    }
}
