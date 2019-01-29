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

    public bool hit;
    public KeyCode keyCode;
    public bool goodHit;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (hit && Input.GetKeyDown(keyCode))
        {
            GameManager.Instance.NoteHit(goodHit);
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
        }
        if (collision.tag == "normalHitbox")
        {
            hit = true;
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
            GameManager.Instance.NoteMissed();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        songPosInBeats = Convert.ToSingle(FindObjectOfType<GameManager>().timer * 1000);
        transform.position = Vector3.Lerp(spawnerPos, new Vector3(spawnerPos.x, hitboxPos.y-(spawnerPos.y - hitboxPos.y), spawnerPos.z), (beatsShownInAdvance - (beatOfThisNote - songPosInBeats))/(beatsShownInAdvance*2));
    }

    public void Initialize(Vector3 spawner, Vector3 hitbox, int offset, float scrollDelay, KeyCode key)
    {
        spawnerPos = spawner;
        hitboxPos = hitbox;
        beatOfThisNote = Convert.ToSingle(offset);
        beatsShownInAdvance = scrollDelay;
        keyCode = key;
    }
}
