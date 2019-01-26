using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteObject : MonoBehaviour
{
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
        }
        if (collision.tag == "normalHitbox")
        {
            hit = true;
        }else if(collision.tag == "despawner")
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
}
