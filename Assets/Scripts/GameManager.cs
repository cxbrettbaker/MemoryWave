using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class GameManager : MonoBehaviour
{
  	float songPosition;
	float currentOffset;

	int nextIndex = 0;

    public List<TimingPoints> timingPointsList = new List<TimingPoints>();
    public List<HitEvent> hitEventsList = new List<HitEvent>();
    double startTime;
    double noteStartTime;
    int index;
    int timingIndex;
    public double offsetTime;
    double noteOffsetTime;
    public float scrollDelay;
    bool noteFlag = false;

    public GameObject simonSaysController;
    public GameObject noteScroller;
    public AudioSource audioSource;
    public double timer = 0.0f;
    private float songLength;
    public double score;
    private double simSaysBaseScore;
    private double noteBaseScore;
    public double baseScore;

    public int numNormalHit;
    public int numGoodHit;
	
	public Texture whiteTexture;
    public int timeBuffer; // time in miliseconds before song starts
    public AudioClip audioClip;

    public static GameManager Instance;

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
    public GameObject hitbox;
    public GameObject midRing;
    public bool memoryMode;
    public GameObject diamondRing;
    public GameObject parentDiamond;
    public GameObject hitboxDiamond;

    public KeyCode keyLeft;
    public KeyCode keyDown;
    public KeyCode keyUp;
    public KeyCode keyRight;

    void loadLevel()
    {
        Dictionary<string, string> song = LevelParser.Instance.selectedSong;
        string filename = song["SongMap"];
        audioClip = Resources.Load<AudioClip>(song["SongPreview"]);
        songLength = audioClip.length;
        audioSource.clip = audioClip;
        LevelParser.Instance.ParseLevel(filename);
    }

    void Start() {
        Instance = this;

        timingPointsList = new List<TimingPoints>();
        hitEventsList = new List<HitEvent>();
	
        loadLevel();

        startTime = 0;
        index = 0;
        timingIndex = 0;
        score = 0;
        numNormalHit = 0;
        numGoodHit = 0;
        simSaysBaseScore = 100;
        noteBaseScore = 10;
        timeBuffer = 3000; // TODO: change this to adapt to the song

        // Grab first timing point information
        scrollDelay = noteScroller.GetComponent<NoteScroller>().UpdateBPM(timingPointsList[0].getMsPerBeat());

    }
	
    public void NoteHit(bool goodHit)
    {

        //Debug.Log("note hit AAYYYYYYYYYYYYYYYYYYYYYY");
        //Debug.Log(string.Format("good hit: {0}", goodHit));
        if(goodHit)
        {
            numGoodHit++;
        } else
        {
            numNormalHit++;
        }
        score += baseScore;
    }

    public void NoteMissed()
    {
        //Debug.Log("note missed :(");
        score -= baseScore;
    }


    void Update() {

        if(startTime == 0)
        {
            startTime = AudioSettings.dspTime + (timeBuffer/1000);
            audioSource.PlayDelayed(timeBuffer / 1000);
            //Debug.Log("start time: " + startTime);
        }


        while (index < hitEventsList.Count)
        {
            //gets latest timing points
            offsetTime = (AudioSettings.dspTime - startTime) * 1000 + scrollDelay;
            
            //Debug.Log("OFFSET TIME: " + offsetTime);
            if (timingIndex < timingPointsList.Count && offsetTime >= timingPointsList[timingIndex].getOffset())
            {
                scrollDelay = noteScroller.GetComponent<NoteScroller>().UpdateBPM(timingPointsList[timingIndex].getMsPerBeat());            
                if (timingPointsList[timingIndex].getPlaymode() == 0) //note mode
                {
                    baseScore = noteBaseScore;
                    memoryMode = false;
                } else
                {
                    baseScore = simSaysBaseScore;
                    memoryMode = true;
                }
                timingIndex++;
            }
        
            noteOffsetTime = (AudioSettings.dspTime - startTime) * 1000;
            noteOffsetTime += scrollDelay;
            HitEvent hitObject = hitEventsList[index];

            if (noteOffsetTime >= hitObject.getOffset()) {
                //Debug.Log("Note " + hitObject.getOffset() + " Spawn time: " + offsetTime + "\nNote offset: " + noteOffsetTime);
                if ((hitObject.IsNote() || hitObject.IsHold()) && !noteFlag)
                {
                    spawnNotes(hitObject);
                }
                noteFlag = true;
                if (hitObject.isFlashRed())
                {
                    StartCoroutine(FlashMemoryPrompt("red", hitObject.IsflashBlack()));
                }
                if (hitObject.isFlashBlue())
                {
                    StartCoroutine(FlashMemoryPrompt("blue", hitObject.IsflashBlack()));
                }
                if (hitObject.isFlashYellow())
                {
                    StartCoroutine(FlashMemoryPrompt("yellow", hitObject.IsflashBlack()));
                }
                if (hitObject.isFlashGreen())
                {
                    StartCoroutine(FlashMemoryPrompt("green", hitObject.IsflashBlack()));
                }
            }
            else // if next hit object doesn't need to be spawned now
            {
                break;
            }

            noteFlag = false;
            index++;
        }

        //Debug.Log("Song length: " + songLength);
        timer = AudioSettings.dspTime - startTime;
        if ((timer + timeBuffer/1000) >= songLength)
        {
            SceneManager.LoadScene("RankingPanel");
        }
    }

    IEnumerator FlashMemoryPrompt(string color, bool isBlack)
    {
        yield return new WaitForSecondsRealtime(scrollDelay / 1000f);
        switch(color)
        {
            case "red":
                simonSaysController.GetComponent<MemoryManager>().StoreBleep(0, isBlack);
                break;
            case "blue":
                simonSaysController.GetComponent<MemoryManager>().StoreBleep(1, isBlack);
                break;
            case "yellow":
                simonSaysController.GetComponent<MemoryManager>().StoreBleep(2, isBlack);
                break;
            case "green":
                simonSaysController.GetComponent<MemoryManager>().StoreBleep(3, isBlack);
                break;
        }
    }

    void spawnStepNote(GameObject ring, Transform spawner, HitEvent hitObject, KeyCode key)
    {
        if (hitObject.IsHold())
        {
            int offsetDiff = hitObject.getEndOffset() - hitObject.getOffset();
            StopCoroutine(spawnStepHold(ring, spawner, hitObject, offsetDiff, key));
            StartCoroutine(spawnStepHold(ring, spawner, hitObject, offsetDiff, key));
        }
        else
        {
            var currentRing = Instantiate(ring, spawner.position, spawner.rotation);
            currentRing.transform.SetParent(parentObject.transform);
            currentRing.transform.localScale = ring.transform.localScale;
            currentRing.AddComponent<Ring>();
            currentRing.GetComponent<Ring>().Initialize(spawner.position, hitbox.transform.position, hitObject.getOffset(), scrollDelay, key);
        }

    }

    IEnumerator spawnStepHold(GameObject ring, Transform spawner, HitEvent hitObject, int offsetDiff, KeyCode key)
    {
        GameObject startHold = Instantiate(ring, spawner.position, spawner.rotation);
        startHold.transform.SetParent(parentObject.transform);
        startHold.transform.localScale = ring.transform.localScale;
        startHold.AddComponent<StartHold>();

        GameObject midHold = Instantiate(midRing, spawner.position, spawner.rotation);
        midHold.transform.SetParent(parentObject.transform);
        midHold.AddComponent<MidHold>();
        
        GameObject endHold = Instantiate(ring, spawner.position, spawner.rotation);
        endHold.transform.SetParent(parentObject.transform);
        endHold.transform.localScale = ring.transform.localScale;

        startHold.GetComponent<StartHold>().Initialize(spawner.position, hitbox.transform.position, hitObject.getOffset(), scrollDelay, key, midHold, endHold, parentObject, ring);
        midHold.GetComponent<MidHold>().Initialize(spawner.position, hitbox.transform.position, hitObject.getOffset(), scrollDelay, key, startHold, endHold);
        
        yield return new WaitForSecondsRealtime(offsetDiff / 1000f);
        endHold.AddComponent<EndHold>();
        endHold.GetComponent<EndHold>().Initialize(spawner.position, hitbox.transform.position, hitObject.getEndOffset(), scrollDelay, key, startHold, midHold);
    }

    void spawnMemoryNote(GameObject ring, Transform spawner, HitEvent hitObject, KeyCode key)
    {
        var currentRing = Instantiate(ring, spawner.localPosition, Quaternion.identity);
        currentRing.transform.SetParent(parentDiamond.transform, false);
        currentRing.GetComponent<DiamondRing>().hitboxScale = hitboxDiamond.transform.localScale;
        currentRing.GetComponent<DiamondRing>().beatOfThisNote = hitObject.getOffset();
        currentRing.GetComponent<DiamondRing>().beatsShownInAdvance = scrollDelay;
        currentRing.GetComponent<DiamondRing>().keyCode = key;
    }

    public void spawnNotes(HitEvent hitObject)
    {
        switch(hitObject.getKey())
        {
            case 0:   // LEFT
                if (memoryMode) // Currently in a memory timing section
                {
                    spawnMemoryNote(diamondRing, diamondRing.transform, hitObject, keyLeft);
                }
                else if (hitObject.IsMine())
                {
                    spawnStepNote(leftBigMine, leftSpawnerBig, hitObject, keyLeft);
                }
                else
                {
                    spawnStepNote(leftBigRing, leftSpawnerBig, hitObject, keyLeft);
                }
                break;
            case 1:  // DOWN
                if (memoryMode)
                {
                    spawnMemoryNote(diamondRing, diamondRing.transform, hitObject, keyDown);
                }
                else if (hitObject.IsMine())
                {
                    spawnStepNote(leftSmallMine, leftSpawnerSmall, hitObject, keyDown);
                }
                else
                {
                    spawnStepNote(leftSmallRing, leftSpawnerSmall, hitObject, keyDown);
                }
                break;
            case 2:  // UP
                if (memoryMode)
                {
                    spawnMemoryNote(diamondRing, diamondRing.transform, hitObject, keyUp);
                }
                else if (hitObject.IsMine())
                {
                    spawnStepNote(rightSmallMine, rightSpawnerSmall, hitObject, keyUp);
                }
                else
                {
                    spawnStepNote(rightSmallRing, rightSpawnerSmall, hitObject, keyUp);
                }
                break;
            case 3:  // RIGHT
                if (memoryMode)
                {
                    spawnMemoryNote(diamondRing, diamondRing.transform, hitObject, keyRight);
                }
                else if (hitObject.IsMine())
                {
                    spawnStepNote(rightBigMine, rightSpawnerBig, hitObject, keyRight);
                }
                else
                {
                    spawnStepNote(rightBigRing, rightSpawnerBig, hitObject, keyRight);
                }
                break;
            default:
                break;
        }
        
    }

}