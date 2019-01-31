using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ScoreManager : MonoBehaviour
{
    public int GREAT = 300;
    public int GOOD  = 100;
    public int OK     = 50;
    public int MISS    = 0;
    public int STEPBASESCORE            = 2;
    public int STEPHOLDBASESCORE        = 1;
    public int MEMORYBASESCORE         = 10;
    public int INVERTEDMEMORYBASESCORE = 15;

    public static ScoreManager Instance;
    double greatHitWindow;
    double goodHitWindow;
    double okHitWindow;
    public GameObject hitIndicator;

    int maxGreat;
    public int numGreat;
    public int numGood;
    public int numOK;
    public int numMiss;
    public int score;
    public int maxScore;
    public float accuracy;
    public int maxCombo;
    int combo;
    int combobreakThreshold = 13;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        maxGreat = 0;
        numGreat = 0;
        numGood = 0;
        numOK = 0;
        numMiss = 0;
        score = 0;
        maxScore = 0;
        accuracy = 0f;
        maxCombo = 0;
        combo = 0;

    }

    public void SetOverallDifficulty(int overallDifficulty)
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
            UpdateCombo();
            numGreat++;
        }
        else if(hitScore == GOOD)
        {
            flashColor = Color.green;
            UpdateCombo();
            numGood++;
        }
        else if(hitScore == OK)
        {
            flashColor = Color.blue;
            UpdateCombo();
            numOK++;
        }
        else
        {
            flashColor = Color.red;
            ComboBreak();
            numMiss++;
        }
        FlashManager.Instance.Flash(hitIndicator, Color.clear, flashColor, GameManager.Instance.scrollDelay / 20000f, GameManager.Instance.scrollDelay / 20000f);
        score += hitScore * baseScoreMultiplier;
        maxScore += GREAT * baseScoreMultiplier;
        maxGreat++;
        Debug.Log("SCORE: " + score + "\nMAX: " + maxScore);
    }

    void ComboBreak()
    {
        if (combo >= combobreakThreshold)
            Debug.Log("COMBO BREAK");
        combo = 0;
    }

    void UpdateCombo()
    {
        combo++;
        if (maxCombo <= combo)
            maxCombo = combo;
    }

    public float CalculateAccuracy()
    {
        if (maxGreat != 0)
        {
            return (Convert.ToSingle(numGreat) / Convert.ToSingle(maxGreat));
        }
        else
            return 0;
    }

}
