using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CatController : MonoBehaviour
{
    private Image image;
    public Sprite handsUpSprite;
    public Sprite leftHandSprite;
    public Sprite rightHandSprite;
    public Sprite handsDownSprite;

    public KeyCode leftHandKey;
    public KeyCode rightHandKey;

    private bool leftHandKeyPressed;
    private bool rightHandKeyPressed;
	
	AudioSource[] bongoSounds;
	
	
    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
		bongoSounds = GetComponents<AudioSource>();
        leftHandKeyPressed = false;
        rightHandKeyPressed = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(leftHandKey))
        {
            image.sprite = leftHandSprite;
            leftHandKeyPressed = true;
			bongoSounds[0].Play();
        }

        if (Input.GetKeyUp(leftHandKey))
        {
            leftHandKeyPressed = false;
            if (rightHandKeyPressed)
            {
                image.sprite = rightHandSprite;
            }
            else
            {
                image.sprite = handsUpSprite;
            }
        }

        if (Input.GetKeyDown(rightHandKey))
        {
            image.sprite = rightHandSprite;
            rightHandKeyPressed = true;
			bongoSounds[1].Play();
        }

        if (Input.GetKeyUp(rightHandKey))
        {
            rightHandKeyPressed = false;
            if (leftHandKeyPressed)
            {
                image.sprite = leftHandSprite;
            }
            else
            {
                image.sprite = handsUpSprite;
            }
        }

        if (leftHandKeyPressed && rightHandKeyPressed)
        {
            image.sprite = handsDownSprite;
        }
    }
}
