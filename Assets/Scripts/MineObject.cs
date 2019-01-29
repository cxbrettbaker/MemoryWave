using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineObject : MonoBehaviour
{
    public bool hit;
    public KeyCode keyCode;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (hit && Input.GetKeyDown(keyCode))
        {
            GameManager.Instance.NoteMissed();
            Debug.Log("hit the mine");
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "cats")
        {
            hit = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "cats")
        {
            hit = false;
            GameManager.Instance.NoteHit(false);
        }
    }
}
