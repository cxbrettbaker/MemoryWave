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

    int maxGreat = 0;
    public int numGreat = 0;
    public int numGood = 0;
    public int numOK = 0;
    public int numMiss = 0;
    public int score = 0;
    public int maxScore = 0;
    public int maxCombo = 0;
    int combo = 0;
    int combobreakThreshold = 13;
    
    void Awake()
    {
        Instance = this;
        SetOverallDifficulty(5);
    }

    public void SetOverallDifficulty(float overallDifficulty)
    {
        Debug.Log("DIFFICULTY: " + overallDifficulty);
        okHitWindow = 150f + 50f * (5f - overallDifficulty) / 5f;
        goodHitWindow = 100f + 40f * (5f - overallDifficulty) / 5f;
        greatHitWindow = 50f + 30f * (5f - overallDifficulty) / 5f;
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
            return (Convert.ToSingle(numGreat) / Convert.ToSingle(maxGreat) * 100);
        }
        else
            return 0;
    }

}
