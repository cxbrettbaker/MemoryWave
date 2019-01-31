using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlashManager : MonoBehaviour
{
    public static FlashManager Instance;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

    // Changes the color of a GameObject over a period of time
    public void TurnOn(GameObject flashObject, Color initialColor, Color finalColor, float turnOnTime)
    {
        LeanTween.value(flashObject, initialColor, finalColor, turnOnTime)
                .setOnUpdate((Color color) =>
                {
                    flashObject.GetComponent<Image>().color = color;
                });
    }

    // Changes the color of a GameObject briefly, then reverts it back after a delay
    public void Flash(GameObject flashObject, Color initialColor, Color finalColor, float flashTime, float delayTime)
    {
        TurnOn(flashObject, initialColor, finalColor, flashTime);

        LeanTween.value(flashObject, finalColor, initialColor, flashTime)
            .setDelay(delayTime)
            .setOnUpdate((Color color) =>
            {
                flashObject.GetComponent<Image>().color = color;
            });
    }

}
