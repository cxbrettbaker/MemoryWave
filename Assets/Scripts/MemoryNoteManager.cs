using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MemoryNoteManager : MonoBehaviour
{
	public GameObject gameButtonPrefab;

    public List<ButtonSetting> buttonSettings;

    public Transform gameFieldPanelTransform;

    public static MemoryNoteManager Instance;

    public GameObject redButton;
	public GameObject blueButton;
	public GameObject yellowButton;
	public GameObject greenButton;

    public Queue<HitEvent> currentMemorySequence;
    
    public int sequenceSize;
    public int sequenceHitCount;

    List<GameObject> gameButtons;

    int bleepCount = 3;

    List<int> bleeps = new List<int>();
    public List<int> playerBleeps = new List<int>();

    bool inputEnabled = false;
	
	// Calls to init button objects  
    void Start() {
        Instance = this;

        gameButtons = new List<GameObject>();
	
        ButtonSetter(0, redButton);
		ButtonSetter(1, blueButton);
		ButtonSetter(2, yellowButton);
		ButtonSetter(3, greenButton);
        currentMemorySequence = new Queue<HitEvent>();
    }

    // actually create the buttons 
    void ButtonSetter(int index, GameObject gameButton) {
		gameButton.GetComponent<Image>().color = buttonSettings[index].normalColor;
		gameButtons.Add(gameButton);
    }

	
	// Play the audio upon button press
	public void PlayAudio(int index) {
	float length = GameManager.Instance.scrollDelay / 2000f;
	float frequency = 0.001f * ((float)index + 1f);

	AnimationCurve volumeCurve = new AnimationCurve(new Keyframe(0f, 1f, 0f, -1f), new Keyframe(length, 0f, -1f, 0f));
	AnimationCurve frequencyCurve = new AnimationCurve(new Keyframe(0f, frequency, 0f, 0f), new Keyframe(length, frequency, 0f, 0f));

	LeanAudioOptions audioOptions = LeanAudio.options();
	audioOptions.setWaveSine();
	audioOptions.setFrequency(44100);

	AudioClip audioClip = LeanAudio.createAudio(volumeCurve, frequencyCurve, audioOptions);

	LeanAudio.play(audioClip, length);
    }
	
	
	// Call to show a BLEEP. 
	public void Bleep (int index, bool isBlack) {
        FlashManager.Instance.Flash(gameButtons[index], buttonSettings[index].normalColor, isBlack ? buttonSettings[4].highlightColor : buttonSettings[index].highlightColor, GameManager.Instance.scrollDelay/10000f, GameManager.Instance.scrollDelay/4000f);
        PlayAudio(index);     
	}

    public void SignalNewSequence()
    {
        sequenceSize = currentMemorySequence.Count;
        sequenceHitCount = 0;
    }


    public void NoteHit()
    {
        //GameManager.Instance.NoteHit(true);
        sequenceHitCount++;
        if(sequenceHitCount >= sequenceSize)
        {
            SequenceSuccess();
        }
    }

    public void NoteMissed()
    {
        //GameManager.Instance.NoteMissed();
        SequenceMissed();
    }

    void SequenceSuccess()
    {
        // handle success
        Debug.Log("FULL SEQUENCE HIT");
	}
	
	void SequenceMissed()
    {

	}
	
}
