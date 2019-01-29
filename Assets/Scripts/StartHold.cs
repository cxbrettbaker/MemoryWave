using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class StartHold : MonoBehaviour
{
    public Vector3 hitboxPos;
    public Vector3 spawnerPos;
    public float beatsShownInAdvance;
    public float beatOfThisNote;
    public float songPosInBeats;

    public bool hit;
    public KeyCode keyCode;
    public bool goodHit;

    public GameObject midHold;
    public GameObject endHold;
    public bool passed;
    GameObject ring;
    public GameObject fakeStartRing;
    GameObject parentObject;

    // Start is called before the first frame update
    void Start()
    {
        passed = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (hit && Input.GetKeyDown(keyCode))
        {
            GameManager.Instance.NoteHit(goodHit);
            gameObject.GetComponent<Image>().enabled = false;
            passed = true;
            CreateFake();
        }
    }

    public void CreateFake()
    {
        fakeStartRing = Instantiate(ring, new Vector3(transform.position.x, hitboxPos.y, transform.position.z), transform.rotation);
        fakeStartRing.transform.SetParent(parentObject.transform);
        fakeStartRing.transform.localScale = ring.transform.localScale;
        midHold.GetComponent<MidHold>().startRing = fakeStartRing;
        midHold.GetComponent<MidHold>().realStartRing = gameObject;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "goodHitbox")
        {
            goodHit = true;
            hit = true;
        }
        if (collision.tag == "normalHitbox")
        {
            hit = true;
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
            GameManager.Instance.NoteMissed();
            if (!passed)
            {
                //Debug.Log("START FAILED");
                gameObject.GetComponent<Image>().color = Color.black;
                midHold.GetComponent<Image>().color = Color.black;
                endHold.GetComponent<Image>().color = Color.black;
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        songPosInBeats = Convert.ToSingle(FindObjectOfType<GameManager>().timer * 1000);
        transform.position = Vector3.Lerp(spawnerPos, new Vector3(spawnerPos.x, hitboxPos.y - (spawnerPos.y - hitboxPos.y), spawnerPos.z), (beatsShownInAdvance - (beatOfThisNote - songPosInBeats)) / (beatsShownInAdvance * 2));
    }

    public void Initialize(Vector3 spawner, Vector3 hitbox, int offset, float scrollDelay, KeyCode key, GameObject midHold, GameObject endHold, GameObject parentObject, GameObject ring)
    {
        spawnerPos = spawner;
        hitboxPos = hitbox;
        beatOfThisNote = Convert.ToSingle(offset);
        beatsShownInAdvance = scrollDelay;
        keyCode = key;
        this.midHold = midHold;
        this.endHold = endHold;
        this.ring = ring;
        this.parentObject = parentObject;
    }
}
