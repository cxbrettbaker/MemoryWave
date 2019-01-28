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

    List<TimingPoints> timingPointsList = new List<TimingPoints>();
    public List<HitObject> hitObjectsList = new List<HitObject>();
    double startTime;
    double noteStartTime;
    int index;
    int timingIndex;
    public double offsetTime;
    double noteOffsetTime;
    public float scrollDelay;
    bool noteFlag = false;

    public GameObject simonSaysController;
    public GameObject noteController;
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

    public static GameManager instance;

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
        Dictionary<string, string> song = SongSelectParser.Instance.selectedSong;
        string filename = song["SongMap"];
        audioClip = Resources.Load<AudioClip>(song["SongPreview"]);
        songLength = audioClip.length;
        audioSource.clip = audioClip;
        
        //StartCoroutine(PlaySong(audioClip));

        string line;
        bool timingPointsStart = false;
        bool hitObjectsStart = false;
        string[] tmp;
        

        // Read the file and display it line by line.  
        System.IO.StreamReader file =
        new System.IO.StreamReader(filename);
        while ((line = file.ReadLine()) != null)
        {
            System.Console.WriteLine(line);
            line = line.Trim();
            if (line.Length == 0 || line[0] == '/')
            {
                continue;
            }
            //Debug.Log(line);

            if (line == "[TimingPoints]")
            {
                //Debug.Log("TimingPoints start");
                hitObjectsStart = false;
                timingPointsStart = true;
                continue;
            }
            if (line == "[HitObjects]")
            {
                //Debug.Log("hitobject start");
                hitObjectsStart = true;
                timingPointsStart = false;
                continue;
            }

            if (hitObjectsStart)
            {
                //Debug.Log(line);
                tmp = line.Split(',');
                HitObject hitObjects = new HitObject();
                hitObjects.setX(tmp[0]);
                hitObjects.setY(tmp[1]);
                hitObjects.setOffset(tmp[2]);
                hitObjects.setIsNote(tmp[3]);
                hitObjects.setIsMine(tmp[3]);
                hitObjects.setColour(tmp[3]);
                hitObjects.setFlashBlack(tmp[3]);
                hitObjects.setIsHold(tmp[3]);
                hitObjects.setEndOffset(tmp[4]);
                hitObjectsList.Add(hitObjects);
            }

            if (timingPointsStart)
            {
                tmp = line.Split(',');
                TimingPoints timingPoints = new TimingPoints(Convert.ToInt32(tmp[0]), Convert.ToSingle(tmp[1]), Convert.ToInt32(tmp[2]), Convert.ToInt32(tmp[3]), Convert.ToInt32(tmp[4]));
                timingPointsList.Add(timingPoints);
            }
        }
        file.Close();
    }

    void Start() {
        instance = this;

        // File parser for Kevin to do stuff with.		
        loadLevel();
        // Start Song

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
        noteScroller.GetComponent<NoteScroller>().UpdateBPM(timingPointsList[0].getMsPerBeat());
        scrollDelay = noteScroller.GetComponent<NoteScroller>().ReturnDelay();

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


        while (index < hitObjectsList.Count)
        {
            //gets latest timing points
            offsetTime = (AudioSettings.dspTime - startTime) * 1000 + scrollDelay;
            
            //Debug.Log("OFFSET TIME: " + offsetTime);
            if (timingIndex < timingPointsList.Count && offsetTime >= timingPointsList[timingIndex].getOffset())
            {
                noteScroller.GetComponent<NoteScroller>().UpdateBPM(timingPointsList[timingIndex].getMsPerBeat());
                scrollDelay = noteScroller.GetComponent<NoteScroller>().ReturnDelay();
                //Debug.Log("ScrollDelay: " + scrollDelay);
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
        
            //generate notes for simon says and ddr
            noteOffsetTime = (AudioSettings.dspTime - startTime) * 1000;
            //Debug.Log("Note start time: " + noteOffsetTime);
            noteOffsetTime += scrollDelay;
            HitObject hitObject = hitObjectsList[index];

            if (noteOffsetTime >= hitObject.getOffset()) {
                //Debug.Log("Note " + hitObject.getOffset() + " Spawn time: " + offsetTime + "\nNote offset: " + noteOffsetTime);
                if ((hitObject.IsNote() || hitObject.IsHold()) && !noteFlag)
                {
                    //Debug.Log("Spawning note " + hitObject.getOffset() + " to be hit at: " + noteOffsetTime);
                    //Debug.Log("noteStartTime: " + noteStartTime);
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
            else //if next hit object dont need to be spawned now
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

    void spawnStepNote(GameObject ring, Transform spawner, HitObject hitObject, KeyCode key)
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
            currentRing.GetComponent<Ring>().spawnerPos = spawner.position;
            currentRing.GetComponent<Ring>().hitboxPos = hitbox.transform.position;
            currentRing.GetComponent<Ring>().beatOfThisNote = hitObject.getOffset();
            currentRing.GetComponent<Ring>().beatsShownInAdvance = scrollDelay;
            currentRing.GetComponent<Ring>().keyCode = key;
        }

    }

    IEnumerator spawnStepHold(GameObject ring, Transform spawner, HitObject hitObject, int offsetDiff, KeyCode key)
    {
        GameObject startRing = Instantiate(ring, spawner.position, spawner.rotation);
        startRing.transform.SetParent(parentObject.transform);
        startRing.transform.localScale = ring.transform.localScale;
        startRing.AddComponent<StartHold>();
        startRing.GetComponent<StartHold>().spawnerPos = spawner.position;
        startRing.GetComponent<StartHold>().hitboxPos = hitbox.transform.position;
        startRing.GetComponent<StartHold>().beatOfThisNote = hitObject.getOffset();
        startRing.GetComponent<StartHold>().beatsShownInAdvance = scrollDelay;
        startRing.GetComponent<StartHold>().keyCode = key;

        //Debug.Log("Insantiated at: " + AudioSettings.dspTime * 1000);
        GameObject endRing = Instantiate(ring, spawner.position, spawner.rotation);
        endRing.transform.SetParent(parentObject.transform);
        endRing.transform.localScale = ring.transform.localScale;

        GameObject midHold = Instantiate(midRing, spawner.position, spawner.rotation);
        midHold.transform.SetParent(parentObject.transform);
        midHold.AddComponent<MidHold>();
        midHold.GetComponent<MidHold>().spawnerPos = spawner.position;
        midHold.GetComponent<MidHold>().hitboxPos = hitbox.transform.position;
        midHold.GetComponent<MidHold>().beatOfThisNote = hitObject.getOffset();
        midHold.GetComponent<MidHold>().beatsShownInAdvance = scrollDelay;
        midHold.GetComponent<MidHold>().keyCode = key;
        midHold.GetComponent<MidHold>().startRing = startRing;
        midHold.GetComponent<MidHold>().endRing = endRing;
        yield return new WaitForSecondsRealtime(offsetDiff / 1000f);
        //Debug.Log("Moving at: " + AudioSettings.dspTime * 1000);
        endRing.AddComponent<EndHold>();
        endRing.GetComponent<EndHold>().spawnerPos = spawner.position;
        endRing.GetComponent<EndHold>().hitboxPos = hitbox.transform.position;
        endRing.GetComponent<EndHold>().beatOfThisNote = hitObject.getEndOffset();
        endRing.GetComponent<EndHold>().beatsShownInAdvance = scrollDelay;
        endRing.GetComponent<EndHold>().keyCode = key;
        endRing.GetComponent<EndHold>().startHold = startRing;
        endRing.GetComponent<EndHold>().midHold = midHold;
    }

    void spawnMemoryNote(GameObject ring, Transform spawner, HitObject hitObject, KeyCode key)
    {
        var currentRing = Instantiate(ring, spawner.localPosition, Quaternion.identity);
        currentRing.transform.SetParent(parentDiamond.transform, false);
        currentRing.GetComponent<DiamondRing>().hitboxScale = hitboxDiamond.transform.localScale;
        currentRing.GetComponent<DiamondRing>().beatOfThisNote = hitObject.getOffset();
        currentRing.GetComponent<DiamondRing>().beatsShownInAdvance = scrollDelay;
        currentRing.GetComponent<DiamondRing>().keyCode = key;
    }

    public void spawnNotes(HitObject hitObject)
    {
        switch(hitObject.getX())
        {
            case 64:   // LEFT
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
            case 192:  // DOWN
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
            case 320:  // UP
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
            case 448:  // RIGHT
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