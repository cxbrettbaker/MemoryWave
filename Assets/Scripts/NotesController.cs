using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NotesController : MonoBehaviour
{
    public Transform leftSpawnerBig;
    public Transform leftSpawnerSmall;
    public Transform rightSpawnerBig;
    public Transform rightSpawnerSmall;

    public GameObject leftBigRing;
    public GameObject leftSmallRing;
    public GameObject rightBigRing;
    public GameObject rightSmallRing;
    public GameObject leftBigMine;
    public GameObject leftSmallMine;
    public GameObject rightBigMine;
    public GameObject rightSmallMine;
    public GameObject parentObject;

    void spawnLeftBigRing()
    {
        var currentRing = Instantiate(leftBigRing, leftSpawnerBig.position, leftSpawnerBig.rotation);
        currentRing.transform.SetParent(parentObject.transform);
        currentRing.transform.localScale = new Vector3(1.15f, 0.85f, 0);
        currentRing.AddComponent<Ring>();
        currentRing.GetComponent<Ring>().spawnerPos = leftSpawnerBig.position;
        currentRing.tag = "rings";
    }

    void spawnLeftSmallRing()
    {
        var currentRing = Instantiate(leftSmallRing, leftSpawnerSmall.position, leftSpawnerSmall.rotation);
        currentRing.transform.SetParent(parentObject.transform);
        currentRing.transform.localScale = new Vector3(1.0f, 0.85f, 0);
        currentRing.AddComponent<Ring>();
        currentRing.GetComponent<Ring>().spawnerPos = leftSpawnerSmall.position;
        currentRing.tag = "rings";
    }

    void spawnRightBigRing()
    {
        var currentRing = Instantiate(rightBigRing, rightSpawnerBig.position, rightSpawnerBig.rotation);
        currentRing.transform.SetParent(parentObject.transform);
        currentRing.transform.localScale = new Vector3(1.15f, 0.85f, 0);
        currentRing.AddComponent<Ring>();
        currentRing.GetComponent<Ring>().spawnerPos = rightSpawnerBig.position;
        currentRing.tag = "rings";
    }

    void spawnRightSmallRing()
    {
        var currentRing = Instantiate(rightSmallRing, rightSpawnerSmall.position, rightSpawnerSmall.rotation);
        currentRing.transform.SetParent(parentObject.transform);
        currentRing.transform.localScale = new Vector3(1.0f, 0.85f, 0);
        currentRing.AddComponent<Ring>();
        currentRing.GetComponent<Ring>().spawnerPos = rightSpawnerSmall.position;
        currentRing.tag = "rings";
    }

    void spawnLeftBigMine()
    {
        var currentRing = Instantiate(leftBigMine, leftSpawnerBig.position, leftSpawnerBig.rotation);
        currentRing.transform.SetParent(parentObject.transform);
        currentRing.transform.localScale = new Vector3(1.15f, 0.85f, 0);
        currentRing.AddComponent<Ring>();
        currentRing.GetComponent<Ring>().spawnerPos = leftSpawnerBig.position;
        currentRing.tag = "rings";
    }

    void spawnLeftSmallMine()
    {
        var currentRing = Instantiate(leftSmallMine, leftSpawnerSmall.position, leftSpawnerSmall.rotation);
        currentRing.transform.SetParent(parentObject.transform);
        currentRing.transform.localScale = new Vector3(1.0f, 0.85f, 0);
        currentRing.AddComponent<Ring>();
        currentRing.GetComponent<Ring>().spawnerPos = leftSpawnerSmall.position;
        currentRing.tag = "rings";
    }

    void spawnRightBigMine()
    {
        var currentRing = Instantiate(rightBigMine, rightSpawnerBig.position, rightSpawnerBig.rotation);
        currentRing.transform.SetParent(parentObject.transform);
        currentRing.transform.localScale = new Vector3(1.15f, 0.85f, 0);
        currentRing.AddComponent<Ring>();
        currentRing.GetComponent<Ring>().spawnerPos = rightSpawnerBig.position;
        currentRing.tag = "rings";
    }

    void spawnRightSmallMine()
    {
        var currentRing = Instantiate(rightSmallMine, rightSpawnerSmall.position, rightSpawnerSmall.rotation);
        currentRing.transform.SetParent(parentObject.transform);
        currentRing.transform.localScale = new Vector3(1.0f, 0.85f, 0);
        currentRing.AddComponent<Ring>();
        currentRing.GetComponent<Ring>().spawnerPos = rightSpawnerSmall.position;
        currentRing.tag = "rings";
    }

    public void spawnNotes(HitObject hitObject)
    {
        if (hitObject.getX() == 64)
        {
            if(hitObject.IsMine())
            {
                spawnLeftBigMine();
            } else
            {
                spawnLeftBigRing();
            }
        }
        else if (hitObject.getX() == 192)
        {
            if (hitObject.IsMine())
            {
                spawnLeftSmallMine();
            }
            else
            {
                spawnLeftSmallRing();
            }
        }
        else if (hitObject.getX() == 320)
        {
            if (hitObject.IsMine())
            {
                spawnRightBigMine();
            }
            else
            {
                spawnRightBigRing();
            }
        }
        else if (hitObject.getX() == 448)
        {
            if (hitObject.IsMine())
            {
                spawnRightSmallMine();
            }
            else
            {
                spawnRightSmallRing();
            }
        }
    }


}
