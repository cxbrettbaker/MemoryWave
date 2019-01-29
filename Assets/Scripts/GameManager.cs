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

    public GameObject memoryController;
    public GameObject delayHandler;
    public AudioSource audioSource;
    public double timer = 0.0f;
    private float songLength;
    public double score;
    private double memoryBaseScore;
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
    public bool invertedMemoryMode;
    public GameObject diamondRing;
    public GameObject parentDiamond;
    public GameObject hitboxDiamond;


    public KeyCode keyLeft;
    public KeyCode keyDown;
    public KeyCode keyUp;
    public KeyCode keyRight;

    public Queue<HitEvent> currentMemorySequence;
    public GameObject spriteLeftBig;
    public GameObject spriteLeftSmall;
    public GameObject spriteRightSmall;
    public GameObject spriteRightBig;
    Color[] colorArray;
    int[] colorIntArray;
    public GameObject memoryFlash;
    public GameObject invertedMemoryFlash;
    bool isSequenceStart;

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
        currentMemorySequence = new Queue<HitEvent>();

        loadLevel();

        startTime = 0;
        index = 0;
        timingIndex = 0;
        score = 0;
        numNormalHit = 0;
        numGoodHit = 0;
        memoryBaseScore = 100;
        noteBaseScore = 10;
        timeBuffer = 3000; // TODO: change this to adapt to the song
        isSequenceStart = true;

        // Set the default color sprite order in case HitEvent contains no colorArray information
        colorArray = new Color[4];
        colorArray[0] = Color.yellow;
        colorArray[1] = Color.green;
        colorArray[2] = Color.red;
        colorArray[3] = Color.blue;
        colorIntArray = new int[4];
        colorIntArray[0] = 0;
        colorIntArray[1] = 1;
        colorIntArray[2] = 2;
        colorIntArray[3] = 3;

        // Grab first timing point information
        scrollDelay = delayHandler.GetComponent<DelayHandler>().UpdateBPM(timingPointsList[0].getMsPerBeat());

    }

    public void NoteHit(bool goodHit)
    {

        //Debug.Log("note hit AAYYYYYYYYYYYYYYYYYYYYYY");
        //Debug.Log(string.Format("good hit: {0}", goodHit));
        if (goodHit)
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

        if (startTime == 0)
        {
            startTime = AudioSettings.dspTime + (timeBuffer / 1000);
            audioSource.PlayDelayed(timeBuffer / 1000);
            //Debug.Log("start time: " + startTime);
        }

        FetchCurrentNote();

        //Debug.Log("Song length: " + songLength);
        timer = AudioSettings.dspTime - startTime;
        if ((timer + timeBuffer / 1000) >= songLength)
        {
            SceneManager.LoadScene("RankingPanel");
        }
    }

    void FetchCurrentNote()
    {
        while (index < hitEventsList.Count)
        {
            //gets latest timing points
            offsetTime = (AudioSettings.dspTime - startTime) * 1000 + scrollDelay;

            //Debug.Log("OFFSET TIME: " + offsetTime);
            if (timingIndex < timingPointsList.Count && offsetTime >= timingPointsList[timingIndex].getOffset())
            {
                scrollDelay = delayHandler.GetComponent<DelayHandler>().UpdateBPM(timingPointsList[timingIndex].getMsPerBeat());
                if (timingPointsList[timingIndex].getPlaymode() == 0) // step
                {
                    baseScore = noteBaseScore;
                    memoryMode = false;
                    invertedMemoryMode = false;
                }
                else if (timingPointsList[timingIndex].getPlaymode() == 1) // memory
                {
                    baseScore = memoryBaseScore;
                    memoryMode = true;
                    invertedMemoryMode = false;
                }
                else if (timingPointsList[timingIndex].getPlaymode() == -1) // inverted memory
                {
                    baseScore = memoryBaseScore;
                    memoryMode = false;
                    invertedMemoryMode = true;
                }
                timingIndex++;
            }

            noteOffsetTime = (AudioSettings.dspTime - startTime) * 1000;
            noteOffsetTime += scrollDelay;
            HitEvent hitObject = hitEventsList[index];

            if (noteOffsetTime >= hitObject.getOffset())
            {
                //Debug.Log("Note " + hitObject.getOffset() + " Spawn time: " + offsetTime + "\nNote offset: " + noteOffsetTime);
                if ((hitObject.IsNote() || hitObject.IsHold()) && !noteFlag)
                {
                    spawnNotes(hitObject);
                }
                noteFlag = true;
                if (hitObject.isFlashYellow() || hitObject.isFlashGreen() || hitObject.isFlashRed() || hitObject.isFlashBlue())
                {
                    HitEvent memoryNote = new HitEvent();
                    memoryNote.setSequenceStart(isSequenceStart);
                    if (isSequenceStart)
                        Debug.Log("START " + hitObject.getOffset());
                    if (hitObject.isFlashYellow())
                    {
                        StartCoroutine(FlashMemoryPrompt("yellow", hitObject.IsflashBlack()));
                        memoryNote.setKey("0");
                        currentMemorySequence.Enqueue(memoryNote);
                    }
                    if (hitObject.isFlashGreen())
                    {
                        StartCoroutine(FlashMemoryPrompt("green", hitObject.IsflashBlack()));
                        memoryNote.setKey("1");
                        currentMemorySequence.Enqueue(memoryNote);
                    }
                    if (hitObject.isFlashRed())
                    {
                        StartCoroutine(FlashMemoryPrompt("red", hitObject.IsflashBlack()));
                        memoryNote.setKey("2");
                        currentMemorySequence.Enqueue(memoryNote);
                    }
                    if (hitObject.isFlashBlue())
                    {
                        StartCoroutine(FlashMemoryPrompt("blue", hitObject.IsflashBlack()));
                        memoryNote.setKey("3");
                        currentMemorySequence.Enqueue(memoryNote);
                    }
                    isSequenceStart = false;
                }
                else
                {
                    isSequenceStart = true;
                }


                if (index + 1 < hitEventsList.Count)
                {

                    HitEvent nextHitObject = hitEventsList[index + 1];
                    int nextHitMode = FetchNotePlayMode(nextHitObject);
                    // Check if the next note is a memory input corresponding to the start of memory sequence
                    if (nextHitMode != 0 && nextHitObject.IsNote() && currentMemorySequence.Peek().isSequenceStart())
                    {
                        StopCoroutine(HandleMemoryStart(nextHitObject, nextHitMode));
                        StartCoroutine(HandleMemoryStart(nextHitObject, nextHitMode));
                    }
                    StopCoroutine(HandleMemorySprites(hitObject, FetchNotePlayMode(hitObject)));
                    StartCoroutine(HandleMemorySprites(hitObject, FetchNotePlayMode(hitObject)));
                }
            }
            else // if next hit object doesn't need to be spawned now
            {
                break;
            }

            noteFlag = false;
            index++;
        }
    }

    int FetchNotePlayMode(HitEvent nextHitObject)
    {
        if (timingIndex < timingPointsList.Count && nextHitObject.getOffset() >= timingPointsList[timingIndex].getOffset())
        {
            return timingPointsList[timingIndex].getPlaymode();
        }
        else if (timingIndex < timingPointsList.Count && nextHitObject.getOffset() < timingPointsList[timingIndex].getOffset())
        {
            return timingPointsList[timingIndex - 1].getPlaymode();
        }
        else
        {
            return 0; // Default to "step" mode if neither apply
        }
    }

    IEnumerator HandleMemoryStart(HitEvent nextHitObject, int playMode)
    {
        yield return new WaitForSecondsRealtime(scrollDelay / 2000f); // half of scrollDelay

        // Handle memory prompt
        if (playMode == 1)
        {
            LeanTween.value(memoryFlash, Color.clear, Color.white, scrollDelay / 8000f) // eighth of scrollDelay
                .setOnUpdate((Color color) =>
                {
                    memoryFlash.GetComponent<Image>().color = color;
                });

            LeanTween.value(memoryFlash, Color.white, Color.clear, scrollDelay / 8000f)
                .setDelay(scrollDelay / 8000f)
                .setOnUpdate((Color color) =>
                {
                    memoryFlash.GetComponent<Image>().color = color;
                });
        }

        // Handle inverted memory prompt
        else if (playMode == -1)
        {
            LeanTween.value(invertedMemoryFlash, Color.clear, Color.white, scrollDelay / 8000f)
                .setOnUpdate((Color color) =>
                {
                    invertedMemoryFlash.GetComponent<Image>().color = color;
                });

            LeanTween.value(invertedMemoryFlash, Color.white, Color.clear, scrollDelay / 8000f)
                .setDelay(scrollDelay / 8000f)
                .setOnUpdate((Color color) =>
                {
                    invertedMemoryFlash.GetComponent<Image>().color = color;
                });
        }

       
    }

    IEnumerator HandleMemorySprites(HitEvent nextHitObject, int playMode)
    {
        if (nextHitObject.getColorArray().Length == 4) // Next hit event contains colorArray information
        {
            colorArray = nextHitObject.getColorArray();
        }
        if ((playMode == 1 || playMode == -1) && nextHitObject.IsNote()) // Memory or inverted memory note, draw sprites
        {
            LeanTween.value(spriteLeftBig, spriteLeftBig.GetComponent<Image>().color, colorArray[0], scrollDelay / 20000f) // 1/20th of scrollDelay
                .setOnUpdate((Color color) =>
                {
                    spriteLeftBig.GetComponent<Image>().color = color;
                });
            LeanTween.value(spriteLeftSmall, spriteLeftSmall.GetComponent<Image>().color, colorArray[1], scrollDelay / 20000f)
                .setOnUpdate((Color color) =>
                {
                    spriteLeftSmall.GetComponent<Image>().color = color;
                });
            LeanTween.value(spriteRightSmall, spriteRightSmall.GetComponent<Image>().color, colorArray[2], scrollDelay / 20000f)
                .setOnUpdate((Color color) =>
                {
                    spriteRightSmall.GetComponent<Image>().color = color;
                });
            LeanTween.value(spriteRightBig, spriteRightBig.GetComponent<Image>().color, colorArray[3], scrollDelay / 20000f)
                .setOnUpdate((Color color) =>
                {
                    spriteRightBig.GetComponent<Image>().color = color;
                });
        }
        else if (playMode == 0) // Step note, clear sprites
        {
            LeanTween.value(spriteLeftBig, spriteLeftBig.GetComponent<Image>().color, Color.clear, scrollDelay / 10000f) // 1/10th of scrollDelay
                .setDelay(scrollDelay / 10000f)
                .setOnUpdate((Color color) =>
                {
                    spriteLeftBig.GetComponent<Image>().color = color;
                });
            LeanTween.value(spriteLeftSmall, spriteLeftSmall.GetComponent<Image>().color, Color.clear, scrollDelay / 10000f)
                .setDelay(scrollDelay / 10000f)
                .setOnUpdate((Color color) =>
                {
                    spriteLeftSmall.GetComponent<Image>().color = color;
                });
            LeanTween.value(spriteRightSmall, spriteRightSmall.GetComponent<Image>().color, Color.clear, scrollDelay / 10000f)
                .setDelay(scrollDelay / 10000f)
                .setOnUpdate((Color color) =>
                {
                    spriteRightSmall.GetComponent<Image>().color = color;
                });
            LeanTween.value(spriteRightBig, spriteRightBig.GetComponent<Image>().color, Color.clear, scrollDelay / 10000f)
                .setDelay(scrollDelay / 10000f)
                .setOnUpdate((Color color) =>
                {
                    spriteRightBig.GetComponent<Image>().color = color;
                });
        }
        yield return null;
    }

    IEnumerator FlashMemoryPrompt(string color, bool isBlack)
    {
        yield return new WaitForSecondsRealtime(scrollDelay / 1000f);
        switch(color)
        {
            case "red":
                memoryController.GetComponent<MemoryManager>().StoreBleep(0, isBlack);
                break;
            case "blue":
                memoryController.GetComponent<MemoryManager>().StoreBleep(1, isBlack);
                break;
            case "yellow":
                memoryController.GetComponent<MemoryManager>().StoreBleep(2, isBlack);
                break;
            case "green":
                memoryController.GetComponent<MemoryManager>().StoreBleep(3, isBlack);
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
            if (hitObject.IsMine())
                currentRing.AddComponent<Mine>().Initialize(spawner.position, hitbox.transform.position, hitObject.getOffset(), scrollDelay, key);
            else
                currentRing.AddComponent<Ring>().Initialize(spawner.position, hitbox.transform.position, hitObject.getOffset(), scrollDelay, key);
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
        if (currentMemorySequence.Count > 0)
        {
            HitEvent memNote = currentMemorySequence.Dequeue();
            var currentRing = Instantiate(ring, spawner.localPosition, Quaternion.identity);
            currentRing.transform.SetParent(parentDiamond.transform, false);
            currentRing.GetComponent<DiamondRing>().hitboxScale = hitboxDiamond.transform.localScale;
            currentRing.GetComponent<DiamondRing>().beatOfThisNote = hitObject.getOffset();
            currentRing.GetComponent<DiamondRing>().beatsShownInAdvance = scrollDelay;
            if(hitObject.getColorIntArray().Length == 4)
                colorIntArray = hitObject.getColorIntArray();
            int intKey = Array.IndexOf(colorIntArray, memNote.getKey());
            switch (intKey)
            {
                case 0:   // LEFT
                    currentRing.GetComponent<DiamondRing>().keyCode = keyLeft;
                    break;
                case 1:  // DOWN
                    currentRing.GetComponent<DiamondRing>().keyCode = keyDown;
                    break;
                case 2:  // UP
                    currentRing.GetComponent<DiamondRing>().keyCode = keyUp;
                    break;
                case 3:  // RIGHT
                    currentRing.GetComponent<DiamondRing>().keyCode = keyRight;
                    break;
                default:
                    break;
            }
        }
    }


    public void spawnNotes(HitEvent hitObject)
    {
        switch(hitObject.getKey())
        {
            case 0:   // LEFT
                if (memoryMode || invertedMemoryMode) // Currently in a memory timing section
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
                if (memoryMode || invertedMemoryMode)
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
                if (memoryMode || invertedMemoryMode)
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
                if (memoryMode || invertedMemoryMode)
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