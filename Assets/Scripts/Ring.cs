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

    public KeyCode keyCode;
    HitEvent currentNote;

    // Update is called once per frame
    void Update()
    {
        double _localCurrentOffset = GameManager.Instance.timer * 1000f;
        if (ScoreManager.Instance.InHitWindow(currentNote, _localCurrentOffset))
        {
            if (Input.GetKeyDown(keyCode))
            {
                ScoreManager.Instance.ScoreNote(currentNote, ScoreManager.Instance.GetHitScore(currentNote, _localCurrentOffset), ScoreManager.Instance.STEPBASESCORE);
                Destroy(gameObject);
            }
        }
        else if (ScoreManager.Instance.MissedHitWindow(currentNote, _localCurrentOffset))
        {
            ScoreManager.Instance.ScoreNote(currentNote, ScoreManager.Instance.MISS, ScoreManager.Instance.STEPBASESCORE);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "despawner")
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        songPosInBeats = Convert.ToSingle(FindObjectOfType<GameManager>().timer * 1000);
        transform.position = Vector3.Lerp(spawnerPos, new Vector3(spawnerPos.x, hitboxPos.y-(spawnerPos.y - hitboxPos.y), spawnerPos.z), (beatsShownInAdvance - (beatOfThisNote - songPosInBeats))/(beatsShownInAdvance*2));
    }

    public void Initialize(Vector3 spawner, Vector3 hitbox, int offset, float scrollDelay, KeyCode key, HitEvent currentNote)
    {
        spawnerPos = spawner;
        hitboxPos = hitbox;
        beatOfThisNote = Convert.ToSingle(offset);
        beatsShownInAdvance = scrollDelay;
        keyCode = key;
        this.currentNote = currentNote;
    }
}
