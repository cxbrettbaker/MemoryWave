using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MemoryManager : MonoBehaviour
{
 

	public GameObject gameButtonPrefab;

    public List<ButtonSetting> buttonSettings;

    public Transform gameFieldPanelTransform;

	
	public GameObject redButton;
	public GameObject blueButton;
	public GameObject yellowButton;
	public GameObject greenButton;
	
	
    List<GameObject> gameButtons;

    int bleepCount = 3;

    List<int> bleeps = new List<int>();
    public List<int> playerBleeps = new List<int>();

    bool inputEnabled = false;
	
	// Calls to init button objects  
    void Start() {
        gameButtons = new List<GameObject>();
	
        ButtonSetter(0, redButton);
		ButtonSetter(1, blueButton);
		ButtonSetter(2, yellowButton);
		ButtonSetter(3, greenButton);
		
    }

	
	// actually create the buttons 
	void ButtonSetter(int index, GameObject gameButton) {
		gameButton.GetComponent<Image>().color = buttonSettings[index].normalColor;
		gameButtons.Add(gameButton);
    }

	
	// Play the audio upon button press
	public void PlayAudio(int index) {
	float length = 0.5f;
	float frequency = 0.001f * ((float)index + 1f);

	AnimationCurve volumeCurve = new AnimationCurve(new Keyframe(0f, 1f, 0f, -1f), new Keyframe(length, 0f, -1f, 0f));
	AnimationCurve frequencyCurve = new AnimationCurve(new Keyframe(0f, frequency, 0f, 0f), new Keyframe(length, frequency, 0f, 0f));

	LeanAudioOptions audioOptions = LeanAudio.options();
	audioOptions.setWaveSine();
	audioOptions.setFrequency(44100);

	AudioClip audioClip = LeanAudio.createAudio(volumeCurve, frequencyCurve, audioOptions);

	LeanAudio.play(audioClip, 0.5f);
    }
	
	
	// Call to show a BLEEP. 
	public void Bleep (int index, bool isBlack) {
        if (!isBlack)
        {
            LeanTween.value(gameButtons[index], buttonSettings[index].normalColor, buttonSettings[index].highlightColor, 0.25f)
                .setOnUpdate((Color color) =>
                {
                    gameButtons[index].GetComponent<Image>().color = color;
                });

            LeanTween.value(gameButtons[index], buttonSettings[index].highlightColor, buttonSettings[index].normalColor, 0.25f)
                .setDelay(0.5f)
                .setOnUpdate((Color color) =>
                {
                    gameButtons[index].GetComponent<Image>().color = color;
                });
        }
        else
        {
            LeanTween.value(gameButtons[index], buttonSettings[index].normalColor, buttonSettings[4].highlightColor, 0.25f)
                .setOnUpdate((Color color) =>
                {
                    gameButtons[index].GetComponent<Image>().color = color;
                });

            LeanTween.value(gameButtons[index], buttonSettings[4].highlightColor, buttonSettings[index].normalColor, 0.25f)
                .setDelay(0.5f)
                .setOnUpdate((Color color) =>
                {
                    gameButtons[index].GetComponent<Image>().color = color;
                });
        }

        PlayAudio(index);
        
	}
	
	// Store the given color into the list of bleeps to be pressed. 
	public void StoreBleep(int index, bool isBlack) {
		Bleep(index, isBlack);
		bleeps.Add(index);
		bleepCount++;
    }


    void Success() {
		// handle success
	}
	
	void Missed() {
		inputEnabled = false;
	}
	
}
