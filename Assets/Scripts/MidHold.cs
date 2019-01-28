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

    public bool hit;
    public KeyCode keyCode;
    public bool goodHit;

    public RectTransform rt;
    public GameObject startRing;
    public GameObject endRing;
    [SerializeField]
    bool pressed;
    bool triggered;
    public bool passed;
    BoxCollider2D bc;
    public GameObject realStartRing;

    // Start is called before the first frame update
    void Start()
    {
        rt = gameObject.GetComponent<RectTransform>();
        pressed = false;
        passed = false;
        bc = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        if (triggered && pressed && Input.GetKeyUp(keyCode))
        {
            pressed = false; // detects if key was released
            //Debug.Log("MID FAILED");
            startRing.AddComponent<FakeStartRing>();
            startRing.GetComponent<FakeStartRing>().Initialize(spawnerPos, hitboxPos, beatsShownInAdvance);
            gameObject.GetComponent<Image>().color = Color.black;
            startRing.GetComponent<Image>().color = Color.black;
            endRing.GetComponent<Image>().color = Color.black;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        triggered = true;
        if (realStartRing.GetComponent<StartHold>() != null)
        {
            if (realStartRing.GetComponent<StartHold>().passed)
            {
                pressed = Input.GetKey(keyCode);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        triggered = false;
        passed = pressed;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float halfwayPoint = (endRing.transform.localPosition.y + startRing.transform.localPosition.y) / 2f;
        transform.localPosition = new Vector3(endRing.transform.localPosition.x, halfwayPoint, 0f);
        rt.sizeDelta = new Vector2(endRing.GetComponent<RectTransform>().sizeDelta.x, endRing.GetComponent<RectTransform>().sizeDelta.y + endRing.transform.localPosition.y - startRing.transform.localPosition.y);
        Vector2 realSizeDelta = new Vector2(rt.sizeDelta.x, endRing.GetComponent<RectTransform>().sizeDelta.y + endRing.transform.localPosition.y - realStartRing.transform.localPosition.y);
        bc.size = new Vector2(realSizeDelta.x, realSizeDelta.y - 2*endRing.GetComponent<RectTransform>().sizeDelta.y);
        bc.offset = new Vector2(0, (realStartRing.transform.localPosition.y - startRing.transform.localPosition.y)/2);
    }

    public void Initialize(Vector3 spawner, Vector3 hitbox, int offset, float scrollDelay, KeyCode key, GameObject startRing, GameObject endRing)
    {
        spawnerPos = spawner;
        hitboxPos = hitbox;
        beatOfThisNote = Convert.ToSingle(offset);
        beatsShownInAdvance = scrollDelay;
        keyCode = key;
        this.startRing = startRing;
        this.realStartRing = startRing;
        this.endRing = endRing;
        transform.localScale = new Vector3(startRing.transform.localScale.x, 1f, 1f);
    }
}
