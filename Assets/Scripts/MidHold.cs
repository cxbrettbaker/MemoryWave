using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
    public bool move = false;

    // Start is called before the first frame update
    void Start()
    {
        rt = gameObject.GetComponent<RectTransform>();
        rt.localScale = new Vector3(1f, 1f, 1f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

    }

    private void OnTriggerExit2D(Collider2D collision)
    {

    }

    // Update is called once per frame
    void Update()
    {
        float halfwayPoint = (endRing.transform.localPosition.y + startRing.transform.localPosition.y) / 2f;
        transform.localPosition = new Vector3(endRing.transform.localPosition.x, halfwayPoint, 0f);
        rt.sizeDelta = new Vector2(endRing.GetComponent<RectTransform>().sizeDelta.x, endRing.GetComponent<RectTransform>().sizeDelta.y + endRing.transform.localPosition.y - startRing.transform.localPosition.y);
        Debug.Log("POSITION: " + rt.position + "\nDELTA: " + rt.sizeDelta);
        //if (move)
        //{
        //    songPosInBeats = Convert.ToSingle(FindObjectOfType<GameManager>().timer * 1000);
        //    transform.position = Vector3.Lerp(spawnerPos, new Vector3(spawnerPos.x, hitboxPos.y - (spawnerPos.y - hitboxPos.y), spawnerPos.z), (beatsShownInAdvance - (beatOfThisNote - songPosInBeats)) / (beatsShownInAdvance * 2));
        //}
    }
}
