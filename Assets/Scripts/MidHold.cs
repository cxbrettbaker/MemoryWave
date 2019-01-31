using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class MidHold : MonoBehaviour
{

    public Vector3 hitboxPos;
    public Vector3 spawnerPos;
    public float beatsShownInAdvance;
    public float beatOfThisNote;
    public float songPosInBeats;

    public KeyCode keyCode;

    public RectTransform rt;
    public GameObject startRing;
    public GameObject endRing;
    HitEvent currentNote;

    // Start is called before the first frame update
    void Start()
    {
        rt = gameObject.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float halfwayPoint = (endRing.transform.localPosition.y + startRing.transform.localPosition.y) / 2f;
        transform.localPosition = new Vector3(endRing.transform.localPosition.x, halfwayPoint, 0f);
        rt.sizeDelta = new Vector2(endRing.GetComponent<RectTransform>().sizeDelta.x, endRing.GetComponent<RectTransform>().sizeDelta.y + endRing.transform.localPosition.y - startRing.transform.localPosition.y);
        if (rt.sizeDelta.y <= 0)
            startRing.GetComponent<Image>().enabled = false;
    }

    public void Initialize(Vector3 spawner, Vector3 hitbox, int offset, float scrollDelay, KeyCode key, GameObject startRing, GameObject endRing, HitEvent currentNote)
    {
        spawnerPos = spawner;
        hitboxPos = hitbox;
        beatOfThisNote = Convert.ToSingle(offset);
        beatsShownInAdvance = scrollDelay;
        keyCode = key;
        this.startRing = startRing;
        this.endRing = endRing;
        transform.localScale = new Vector3(startRing.transform.localScale.x, 1f, 1f);
        this.currentNote = currentNote;
    }
}
