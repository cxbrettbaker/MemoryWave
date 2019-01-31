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
    public KeyCode keyCode;

    public GameObject midHold;
    public GameObject endHold;
    public bool passedStart;
    bool finishedStart;
    bool passedMid;
    bool finishedEnd;
    bool passedEnd;
    GameObject parentObject;
    HitEvent currentNote;
    double timeFrozen;
    bool frozen;

    // Start is called before the first frame update
    void Start()
    {
        passedStart = false;
        finishedStart = false;
        passedMid = true;
        finishedEnd = false;
        passedEnd = false;
        frozen = false;
        timeFrozen = 0;
    }

    // Update is called once per frame
    void Update()
    {
        double _localCurrentOffset = GameManager.Instance.timer * 1000f;
        if (ScoreManager.Instance.InStartHoldHitWindow(currentNote, _localCurrentOffset) && !passedStart)
        {
            if (Input.GetKeyDown(keyCode))
            {
                ScoreManager.Instance.ScoreNote(currentNote, ScoreManager.Instance.GetHitScore(currentNote, _localCurrentOffset), ScoreManager.Instance.STEPBASESCORE);
                passedStart = true;
                StartCoroutine(Freeze());
            }
        }
        else if (ScoreManager.Instance.MissedHitWindow(currentNote, _localCurrentOffset) && !finishedStart)
        {
            if (!passedStart)
            {
                DisableHold();
                ScoreManager.Instance.ScoreNote(currentNote, ScoreManager.Instance.MISS, ScoreManager.Instance.STEPBASESCORE);
            }
            finishedStart = true;
        }
        if (finishedStart)
        {
            if (!(ScoreManager.Instance.InEndHitWindow(currentNote, _localCurrentOffset) || ScoreManager.Instance.MissedEndHitWindow(currentNote, _localCurrentOffset))) // Finished start, in the middle of the hold note
            {
                if (Input.GetKeyUp(keyCode) && passedMid && passedStart) // detects if key was released
                {
                    passedMid = false;
                    UnFreeze();
                    DisableHold();
                    ScoreManager.Instance.ScoreNote(currentNote, ScoreManager.Instance.MISS, ScoreManager.Instance.STEPBASESCORE);
                }
                else
                {
                    if (passedMid && passedStart)
                        ScoreManager.Instance.ScoreNote(currentNote, ScoreManager.Instance.GREAT, ScoreManager.Instance.STEPBASESCORE);
                    else
                    {
                        ScoreManager.Instance.ScoreNote(currentNote, ScoreManager.Instance.MISS, ScoreManager.Instance.STEPBASESCORE);
                    }
                }
            }
            if (ScoreManager.Instance.InEndHitWindow(currentNote, _localCurrentOffset) && passedStart && passedMid && !passedEnd) // Finished start and mid, at end note
            {
                gameObject.GetComponent<Image>().enabled = false;
                if (Input.GetKeyUp(keyCode))
                {
                    ScoreManager.Instance.ScoreNote(currentNote, ScoreManager.Instance.GREAT, ScoreManager.Instance.STEPBASESCORE);
                    endHold.GetComponent<Image>().enabled = false;
                    passedEnd = true;
                }
            }
            else if (ScoreManager.Instance.MissedEndHitWindow(currentNote, _localCurrentOffset) && !finishedEnd)
            {
                if (!passedEnd)
                {
                    ScoreManager.Instance.ScoreNote(currentNote, ScoreManager.Instance.MISS, ScoreManager.Instance.STEPBASESCORE);
                    midHold.GetComponent<Image>().enabled = false;
                    DisableHold();
                }
                finishedEnd = true;
            }
        }
    }

    IEnumerator Freeze() // Freeze the start of hold note on top of the drum
    {
        float delay = beatOfThisNote - songPosInBeats;
        if (beatOfThisNote - songPosInBeats <= 0)
            delay = 0;
        yield return new WaitForSecondsRealtime(delay / 1000f);
        frozen = true;
        transform.position = Vector3.Lerp(spawnerPos, new Vector3(spawnerPos.x, hitboxPos.y - (spawnerPos.y - hitboxPos.y), spawnerPos.z), (beatsShownInAdvance) / (beatsShownInAdvance * 2));
    }

    void DisableHold() // Paint everything black
    {
        midHold.GetComponent<Image>().color = Color.black;
        gameObject.GetComponent<Image>().color = Color.black;
        endHold.GetComponent<Image>().color = Color.black;
    }

    void UnFreeze() // Resume linearly interpolating this note
    {
        timeFrozen = GameManager.Instance.timer * 1000f - beatOfThisNote;
        frozen = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!frozen)
        {
            songPosInBeats = Convert.ToSingle(FindObjectOfType<GameManager>().timer * 1000f - timeFrozen);
            transform.position = Vector3.Lerp(spawnerPos, new Vector3(spawnerPos.x, hitboxPos.y - (spawnerPos.y - hitboxPos.y), spawnerPos.z), (beatsShownInAdvance - (beatOfThisNote - songPosInBeats)) / (beatsShownInAdvance * 2));
        }
    }

    public void Initialize(Vector3 spawner, Vector3 hitbox, int offset, float scrollDelay, KeyCode key, GameObject midHold, GameObject endHold, GameObject parentObject, HitEvent currentNote)
    {
        spawnerPos = spawner;
        hitboxPos = hitbox;
        beatOfThisNote = Convert.ToSingle(offset);
        beatsShownInAdvance = scrollDelay;
        keyCode = key;
        this.midHold = midHold;
        this.endHold = endHold;
        this.parentObject = parentObject;
        this.currentNote = currentNote;
    }
}
