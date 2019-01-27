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
    private double timer = 0.0f;
    private float songLength;
    public double score;
    private double simSaysBaseScore;
    private double noteBaseScore;
    public double baseScore;

    public int numNormalHit;
    public int numGoodHit;
	
	public Texture whiteTexture;
	

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
    public GameObject midHold;

    void loadLevel()
    {
        Dictionary<string, string> song = SongSelectParser.Instance.selectedSong;
        string filename = song["SongMap"];
        AudioClip audioClip = Resources.Load<AudioClip>(song["SongPreview"]);
        songLength = audioClip.length;
        audioSource.PlayOneShot(audioClip);

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

            if (line == "#TimingPoints")
            {
                //Debug.Log("TimingPoints start");
                hitObjectsStart = false;
                timingPointsStart = true;
                continue;
            }
            if (line == "#HitObjects")
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
        //GetComponent<AudioSource>().Play();

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


    void FixedUpdate() {

        if(startTime == 0)
        {
            startTime = AudioSettings.dspTime;
            //Debug.Log("start time: " + startTime);
        }

        //gets latest timing points
        offsetTime = (AudioSettings.dspTime - startTime) * 1000;
        //Debug.Log("OFFSET TIME: " + offsetTime);
        if (timingIndex < timingPointsList.Count && offsetTime >= timingPointsList[timingIndex].getOffset())
        {
            noteScroller.GetComponent<NoteScroller>().UpdateBPM(timingPointsList[timingIndex].getMsPerBeat());
            scrollDelay = noteScroller.GetComponent<NoteScroller>().ReturnDelay();
            //Debug.Log("ScrollDelay: " + scrollDelay);
            if (timingPointsList[timingIndex].getPlaymode() == 0) //note mode
            {
                baseScore = noteBaseScore;
                simonSaysController.GetComponent<simonSaysManager>().disableInput();
            } else
            {
                baseScore = simSaysBaseScore;
                simonSaysController.GetComponent<simonSaysManager>().enableInput();
            }
            timingIndex++;
        }

        //generate notes for simon says and ddr
        while (index < hitObjectsList.Count)
        {
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
        if (timer >= songLength)
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
                simonSaysController.GetComponent<simonSaysManager>().StoreBleep(0, isBlack);
                break;
            case "blue":
                simonSaysController.GetComponent<simonSaysManager>().StoreBleep(1, isBlack);
                break;
            case "yellow":
                simonSaysController.GetComponent<simonSaysManager>().StoreBleep(2, isBlack);
                break;
            case "green":
                simonSaysController.GetComponent<simonSaysManager>().StoreBleep(3, isBlack);
                break;
        }
    }

    void spawnLeftBigRing(HitObject hitObject)
    {
        var currentRing = Instantiate(leftBigRing, leftSpawnerBig.position, leftSpawnerBig.rotation);
        currentRing.transform.SetParent(parentObject.transform);
        currentRing.transform.localScale = new Vector3(1.15f, 0.85f, 0);
        currentRing.AddComponent<Ring>();
        currentRing.GetComponent<Ring>().spawnerPos = leftSpawnerBig.position;
        currentRing.GetComponent<Ring>().hitboxPos = hitbox.transform.position;
        currentRing.GetComponent<Ring>().beatOfThisNote = hitObject.getOffset();
        currentRing.GetComponent<Ring>().beatsShownInAdvance = scrollDelay;
        currentRing.tag = "rings";
    }

    void spawnLeftSmallRing(HitObject hitObject)
    {
        var currentRing = Instantiate(leftSmallRing, leftSpawnerSmall.position, leftSpawnerSmall.rotation);
        currentRing.transform.SetParent(parentObject.transform);
        currentRing.transform.localScale = new Vector3(1.0f, 0.85f, 0);
        currentRing.AddComponent<Ring>();
        currentRing.GetComponent<Ring>().spawnerPos = leftSpawnerSmall.position;
        currentRing.GetComponent<Ring>().hitboxPos = hitbox.transform.position;
        currentRing.GetComponent<Ring>().beatOfThisNote = hitObject.getOffset();
        currentRing.GetComponent<Ring>().beatsShownInAdvance = scrollDelay;
        currentRing.tag = "rings";
    }

    void spawnRightBigRing(HitObject hitObject)
    {
        var currentRing = Instantiate(rightBigRing, rightSpawnerBig.position, rightSpawnerBig.rotation);
        currentRing.transform.SetParent(parentObject.transform);
        currentRing.transform.localScale = new Vector3(1.15f, 0.85f, 0);
        currentRing.AddComponent<Ring>();
        currentRing.GetComponent<Ring>().spawnerPos = rightSpawnerBig.position;
        currentRing.GetComponent<Ring>().hitboxPos = hitbox.transform.position;
        currentRing.GetComponent<Ring>().beatOfThisNote = hitObject.getOffset();
        currentRing.GetComponent<Ring>().beatsShownInAdvance = scrollDelay;
        currentRing.tag = "rings";
    }

    void spawnRightSmallRing(HitObject hitObject)
    {
        var currentRing = Instantiate(rightSmallRing, rightSpawnerSmall.position, rightSpawnerSmall.rotation);
        currentRing.transform.SetParent(parentObject.transform);
        currentRing.transform.localScale = new Vector3(1.0f, 0.85f, 0);
        currentRing.AddComponent<Ring>();
        currentRing.GetComponent<Ring>().spawnerPos = rightSpawnerSmall.position;
        currentRing.GetComponent<Ring>().hitboxPos = hitbox.transform.position;
        currentRing.GetComponent<Ring>().beatOfThisNote = hitObject.getOffset();
        currentRing.GetComponent<Ring>().beatsShownInAdvance = scrollDelay;
        currentRing.tag = "rings";
    }

    void spawnLeftBigMine(HitObject hitObject)
    {
        var currentRing = Instantiate(leftBigMine, leftSpawnerBig.position, leftSpawnerBig.rotation);
        currentRing.transform.SetParent(parentObject.transform);
        currentRing.transform.localScale = new Vector3(1.15f, 0.85f, 0);
        currentRing.AddComponent<Ring>();
        currentRing.GetComponent<Ring>().spawnerPos = leftSpawnerBig.position;
        currentRing.GetComponent<Ring>().hitboxPos = hitbox.transform.position;
        currentRing.GetComponent<Ring>().beatOfThisNote = hitObject.getOffset();
        currentRing.GetComponent<Ring>().beatsShownInAdvance = scrollDelay;
        currentRing.tag = "rings";
    }

    void spawnLeftSmallMine(HitObject hitObject)
    {
        var currentRing = Instantiate(leftSmallMine, leftSpawnerSmall.position, leftSpawnerSmall.rotation);
        currentRing.transform.SetParent(parentObject.transform);
        currentRing.transform.localScale = new Vector3(1.0f, 0.85f, 0);
        currentRing.AddComponent<Ring>();
        currentRing.GetComponent<Ring>().spawnerPos = leftSpawnerSmall.position;
        currentRing.GetComponent<Ring>().hitboxPos = hitbox.transform.position;
        currentRing.GetComponent<Ring>().beatOfThisNote = hitObject.getOffset();
        currentRing.GetComponent<Ring>().beatsShownInAdvance = scrollDelay;
        currentRing.tag = "rings";
    }

    void spawnRightBigMine(HitObject hitObject)
    {
        var currentRing = Instantiate(rightBigMine, rightSpawnerBig.position, rightSpawnerBig.rotation);
        currentRing.transform.SetParent(parentObject.transform);
        currentRing.transform.localScale = new Vector3(1.15f, 0.85f, 0);
        currentRing.AddComponent<Ring>();
        currentRing.GetComponent<Ring>().spawnerPos = rightSpawnerBig.position;
        currentRing.GetComponent<Ring>().hitboxPos = hitbox.transform.position;
        currentRing.GetComponent<Ring>().beatOfThisNote = hitObject.getOffset();
        currentRing.GetComponent<Ring>().beatsShownInAdvance = scrollDelay;
        currentRing.tag = "rings";
    }

    void spawnRightSmallMine(HitObject hitObject)
    {
        var currentRing = Instantiate(rightSmallMine, rightSpawnerSmall.position, rightSpawnerSmall.rotation);
        currentRing.transform.SetParent(parentObject.transform);
        currentRing.transform.localScale = new Vector3(1.0f, 0.85f, 0);
        currentRing.AddComponent<Ring>();
        currentRing.GetComponent<Ring>().spawnerPos = rightSpawnerSmall.position;
        currentRing.GetComponent<Ring>().hitboxPos = hitbox.transform.position;
        currentRing.GetComponent<Ring>().beatOfThisNote = hitObject.getOffset();
        currentRing.GetComponent<Ring>().beatsShownInAdvance = scrollDelay;
        currentRing.tag = "rings";
    }

    void spawnLeftBigHold(HitObject hitObject)
    {
        int offsetDiff = hitObject.getEndOffset() - hitObject.getOffset();
        var currentRing = Instantiate(leftBigRing, leftSpawnerBig.position, leftSpawnerBig.rotation);
        var midRing = Instantiate(midHold, leftSpawnerBig.position, leftSpawnerBig.rotation);
        var endRing = Instantiate(leftBigRing, leftSpawnerBig.position + new Vector3(0,30,0), leftSpawnerBig.rotation);
        currentRing.transform.SetParent(parentObject.transform);
        currentRing.transform.localScale = new Vector3(1.15f, 0.85f, 0);
        currentRing.AddComponent<Ring>();
        currentRing.GetComponent<Ring>().spawnerPos = leftSpawnerBig.position;
        currentRing.GetComponent<Ring>().hitboxPos = hitbox.transform.position;
        currentRing.GetComponent<Ring>().beatOfThisNote = hitObject.getOffset();
        currentRing.GetComponent<Ring>().beatsShownInAdvance = scrollDelay;
        currentRing.tag = "rings";
        midRing.transform.SetParent(parentObject.transform);
        midRing.transform.localScale = new Vector3(1.15f, 0.85f, 0);
        midRing.AddComponent<Ring>();
        midRing.GetComponent<Ring>().spawnerPos = leftSpawnerBig.position;
        midRing.GetComponent<Ring>().hitboxPos = hitbox.transform.position;
        midRing.GetComponent<Ring>().beatOfThisNote = hitObject.getOffset();
        midRing.GetComponent<Ring>().beatsShownInAdvance = scrollDelay;
        midRing.tag = "hold";
        endRing.transform.SetParent(parentObject.transform);
        endRing.transform.localScale = new Vector3(1.15f, 0.85f, 0);
        endRing.AddComponent<Ring>();
        endRing.GetComponent<Ring>().spawnerPos = leftSpawnerBig.position;
        endRing.GetComponent<Ring>().hitboxPos = hitbox.transform.position;
        endRing.GetComponent<Ring>().beatOfThisNote = hitObject.getOffset();
        endRing.GetComponent<Ring>().beatsShownInAdvance = scrollDelay;
        endRing.tag = "end";
    }

    void spawnLeftSmallHold(HitObject hitObject)
    {
        var currentRing = Instantiate(leftSmallMine, leftSpawnerSmall.position, leftSpawnerSmall.rotation);
        currentRing.transform.SetParent(parentObject.transform);
        currentRing.transform.localScale = new Vector3(1.0f, 0.85f, 0);
        currentRing.AddComponent<Ring>();
        currentRing.GetComponent<Ring>().spawnerPos = leftSpawnerSmall.position;
        currentRing.GetComponent<Ring>().hitboxPos = hitbox.transform.position;
        currentRing.GetComponent<Ring>().beatOfThisNote = hitObject.getOffset();
        currentRing.GetComponent<Ring>().beatsShownInAdvance = scrollDelay;
        currentRing.tag = "rings";
    }

    void spawnRightBigHold(HitObject hitObject)
    {
        var currentRing = Instantiate(rightBigMine, rightSpawnerBig.position, rightSpawnerBig.rotation);
        currentRing.transform.SetParent(parentObject.transform);
        currentRing.transform.localScale = new Vector3(1.15f, 0.85f, 0);
        currentRing.AddComponent<Ring>();
        currentRing.GetComponent<Ring>().spawnerPos = rightSpawnerBig.position;
        currentRing.GetComponent<Ring>().hitboxPos = hitbox.transform.position;
        currentRing.GetComponent<Ring>().beatOfThisNote = hitObject.getOffset();
        currentRing.GetComponent<Ring>().beatsShownInAdvance = scrollDelay;
        currentRing.tag = "rings";
    }

    void spawnRightSmallHold(HitObject hitObject)
    {
        var currentRing = Instantiate(rightSmallMine, rightSpawnerSmall.position, rightSpawnerSmall.rotation);
        currentRing.transform.SetParent(parentObject.transform);
        currentRing.transform.localScale = new Vector3(1.0f, 0.85f, 0);
        currentRing.AddComponent<Ring>();
        currentRing.GetComponent<Ring>().spawnerPos = rightSpawnerSmall.position;
        currentRing.GetComponent<Ring>().hitboxPos = hitbox.transform.position;
        currentRing.GetComponent<Ring>().beatOfThisNote = hitObject.getOffset();
        currentRing.GetComponent<Ring>().beatsShownInAdvance = scrollDelay;
        currentRing.tag = "rings";
    }

    public void spawnNotes(HitObject hitObject)
    {
        if (hitObject.getX() == 64)
        {
            if (hitObject.IsMine())
            {
                spawnLeftBigMine(hitObject);
            }
            else if(hitObject.IsHold())
            {
                spawnLeftBigHold(hitObject);
            }
            else
            {
                spawnLeftBigRing(hitObject);
            }
        }
        else if (hitObject.getX() == 192)
        {
            if (hitObject.IsMine())
            {
                spawnLeftSmallMine(hitObject);
            }
            else
            {
                spawnLeftSmallRing(hitObject);
            }
        }
        else if (hitObject.getX() == 320)
        {
            if (hitObject.IsMine())
            {
                spawnRightBigMine(hitObject);
            }
            else
            {
                spawnRightBigRing(hitObject);
            }
        }
        else if (hitObject.getX() == 448)
        {
            if (hitObject.IsMine())
            {
                spawnRightSmallMine(hitObject);
            }
            else
            {
                spawnRightSmallRing(hitObject);
            }
        }
    }

}