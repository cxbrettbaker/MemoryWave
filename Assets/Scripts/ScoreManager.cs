using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public int GREAT = 300;
    public int GOOD  = 100;
    public int OK     = 50;
    public int MISS    = 0;
    public int STEPBASESCORE            = 1;
    public int MEMORYBASESCORE          = 5;
    public int INVERTEDMEMORYBASESCORE = 10;

    public static ScoreManager Instance;
    double greatHitWindow;
    double goodHitWindow;
    double okHitWindow;
    int overallDifficulty;
    public GameObject hitIndicator;

    int maxGreat;
    int numGreat;
    int numGood;
    int numOK;
    int numMiss;
    int score;
    float accuracy;
    int maxCombo;
    int combo;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        overallDifficulty = 0;
        SetOverallDifficulty();

        maxGreat = 0;
        numGreat = 0;
        numGood = 0;
        numOK = 0;
        numMiss = 0;
        score = 0;
        accuracy = 0f;
        maxCombo = 0;
        combo = 0;

    }

    public void SetOverallDifficulty()
    {
        okHitWindow = 150 + 50 * (5 - overallDifficulty) / 5;
        goodHitWindow = 100 + 40 * (5 - overallDifficulty) / 5;
        greatHitWindow = 50 + 30 * (5 - overallDifficulty) / 5;
    }

    bool InGreatHitWindow(HitEvent currentNote, double _localCurrentOffset)
    {
        if (_localCurrentOffset >= currentNote.getOffset() - greatHitWindow && _localCurrentOffset <= currentNote.getOffset() + greatHitWindow)
            return true;
        else
            return false;
    }

    bool InGoodHitWindow(HitEvent currentNote, double _localCurrentOffset)
    {
        if (_localCurrentOffset >= currentNote.getOffset() - goodHitWindow && _localCurrentOffset <= currentNote.getOffset() + goodHitWindow)
            return true;
        else
            return false;
    }

    public bool InHitWindow(HitEvent currentNote, double _localCurrentOffset)
    {
        if (_localCurrentOffset >= currentNote.getOffset() - okHitWindow && _localCurrentOffset <= currentNote.getOffset() + okHitWindow)
            return true;
        else
            return false;
    }

    public bool InStartHoldHitWindow(HitEvent currentNote, double _localCurrentOffset)
    {
        if (_localCurrentOffset >= currentNote.getOffset() - greatHitWindow && _localCurrentOffset <= currentNote.getOffset() + okHitWindow)
            return true;
        else
            return false;
    }

    public bool InEndHitWindow(HitEvent currentNote, double _localCurrentOffset)
    {
        if (_localCurrentOffset >= currentNote.getEndOffset() - okHitWindow && _localCurrentOffset <= currentNote.getEndOffset() + okHitWindow)
            return true;
        else
            return false;
    }

    public bool MissedHitWindow(HitEvent currentNote, double _localCurrentOffset)
    {
        if (_localCurrentOffset > currentNote.getOffset() + okHitWindow)
            return true;
        else
            return false;
    }

    public bool MissedEndHitWindow(HitEvent currentNote, double _localCurrentOffset)
    {
        if (_localCurrentOffset > currentNote.getEndOffset() + okHitWindow)
            return true;
        else
            return false;
    }

    public int GetHitScore(HitEvent currentNote, double _localCurrentOffset)
    {
        if (InGreatHitWindow(currentNote, _localCurrentOffset))
            return GREAT;
        else if (InGoodHitWindow(currentNote, _localCurrentOffset))
            return GOOD;
        else if (InHitWindow(currentNote, _localCurrentOffset))
            return OK;
        else
            return MISS;
    }

    public void ScoreNote(HitEvent note, int hitScore, int baseScoreMultiplier)
    {
        Color flashColor;
        if(hitScore == GREAT)
        {
            flashColor = Color.yellow;
        }
        else if(hitScore == GOOD)
        {
            flashColor = Color.green;
        }
        else if(hitScore == OK)
        {
            flashColor = Color.blue;
        }
        else
        {
            flashColor = Color.red;
        }
        FlashManager.Instance.Flash(hitIndicator, Color.clear, flashColor, GameManager.Instance.scrollDelay / 20000f, GameManager.Instance.scrollDelay / 20000f);
        score += hitScore * baseScoreMultiplier;
        maxGreat++;
    }

    float CalculateAccuracy()
    {
        return (numGreat / maxGreat);
    }

}
