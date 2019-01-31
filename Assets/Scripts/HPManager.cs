using UnityEngine;
using System.Collections;
using System;

public class HPManager : MonoBehaviour
{
    public float maxHealthCap = 150f;
    public float minHealthCap = 50f;

    public float healthLoss = 10f;
    public float healthGain = 5f;
    public float maxHealthGain = 5f;
    public float maxHealthLoss = 10f;

    public static HPManager Instance;
    float hp = 100f;
    float maxHp = 100f;

    void Awake()
    {
        Instance = this;
        SetHPStats(5);
    }

    void Update()
    {
        FlashManager.Instance.Flash(gameObject, Color.clear, Color.white, 0.1f, 0.2f);
    }

    public void SetHPStats(float hpDrain)
    {
        if (hpDrain >= 0 && hpDrain <= 5)
        {
            healthLoss = 10f;
            healthGain = 10f + (hpDrain - 5f) * 2f;
        }
        else if (hpDrain >= 5 && hpDrain <= 10)
        {
            healthLoss = 10f + (5f - hpDrain) * 2f;
            healthGain = 10f;
        }
        else // Invalid state, default to HP 5
        {
            healthLoss = 10f;
            healthGain = 10f;
        }
        maxHealthLoss = 2 * healthLoss;
        maxHealthGain = 2 * healthGain;
    }

    public void LoseHP()
    {
        hp -= healthLoss;
        if (hp <= 0)
            HPIsZero();
    }

    public void GainHP()
    {
        hp += healthGain;
        if (hp > maxHp)
            hp = maxHp;
    }

    public void IncreaseMaxHP()
    {
        maxHp += maxHealthGain;
        if (maxHp > maxHealthCap)
            maxHp = maxHealthCap;
    }

    public void DecreaseMaxHP()
    {
        maxHp -= maxHealthLoss;
        if (maxHp < minHealthCap)
            maxHp = minHealthCap;
    }

    void HPIsZero()
    {
        GameManager.Instance.Fail();
    }
}
