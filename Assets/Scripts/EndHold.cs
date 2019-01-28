using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class EndHold : MonoBehaviour
{
    public Vector3 hitboxPos;
    public Vector3 spawnerPos;
    public float beatsShownInAdvance;
    public float beatOfThisNote;
    public float songPosInBeats;

    public bool hit;
    public KeyCode keyCode;
    public bool goodHit;

    public GameObject startHold;
    public GameObject midHold;
    public bool passed;
    public GameObject fakeStartRing;

    // Start is called before the first frame update
    void Start()
    {
        passed = false;
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(startHold.GetComponent<StartHold>().fakeStartRing != null)
            startHold.GetComponent<StartHold>().fakeStartRing.GetComponent<Image>().enabled = false;
        if (collision.tag == "despawner")
        {
            GameManager.instance.NoteHit(goodHit);
            Destroy(startHold.GetComponent<StartHold>().fakeStartRing);
            Destroy(startHold);
            Destroy(midHold);
            Destroy(gameObject);
        }
        else if (midHold.GetComponent<MidHold>().passed && Input.GetKey(keyCode))
        {
            gameObject.GetComponent<Image>().enabled = false;
            passed = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (passed)
        {
            GameManager.instance.NoteHit(true);
        }
        else if(collision.tag == "normalHitbox")
        {
            gameObject.GetComponent<Image>().color = Color.black;
            GameManager.instance.NoteMissed();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        songPosInBeats = Convert.ToSingle(FindObjectOfType<GameManager>().timer * 1000);
        transform.position = Vector3.Lerp(spawnerPos, new Vector3(spawnerPos.x, hitboxPos.y - (spawnerPos.y - hitboxPos.y), spawnerPos.z), (beatsShownInAdvance - (beatOfThisNote - songPosInBeats)) / (beatsShownInAdvance * 2));
    }

    public void Initialize(Vector3 spawner, Vector3 hitbox, int offset, float scrollDelay, KeyCode key, GameObject startRing, GameObject midHold)
    {
        spawnerPos = spawner;
        hitboxPos = hitbox;
        beatOfThisNote = Convert.ToSingle(offset);
        beatsShownInAdvance = scrollDelay;
        keyCode = key;
        startHold = startRing;
        this.midHold = midHold;
    }
}
