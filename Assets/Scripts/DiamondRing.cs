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
                ScoreManager.Instance.ScoreNote(currentNote, ScoreManager.Instance.GetHitScore(currentNote, _localCurrentOffset), currentNote.getPlayMode() == 1 ? ScoreManager.Instance.MEMORYBASESCORE : ScoreManager.Instance.INVERTEDMEMORYBASESCORE);
                Destroy(gameObject);
            }
        }
        else if (ScoreManager.Instance.MissedHitWindow(currentNote, _localCurrentOffset))
        {
            ScoreManager.Instance.ScoreNote(currentNote, ScoreManager.Instance.MISS, currentNote.getPlayMode() == 1 ? ScoreManager.Instance.MEMORYBASESCORE : ScoreManager.Instance.INVERTEDMEMORYBASESCORE);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "normalDiamondHitbox")
        {
            gameObject.GetComponent<LineRenderer>().enabled = false;
        }
        else if (collision.tag == "despawnerDiamond")
        {
            Destroy(gameObject);
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

    public void Initialize(Vector3 hitboxScale, float beatOfThisNote, float beatsShownInAdvance, HitEvent currentNote)
    {
        this.hitboxScale = hitboxScale;
        this.beatOfThisNote = beatOfThisNote;
        this.beatsShownInAdvance = beatsShownInAdvance;
        this.currentNote = currentNote;
    }
}
