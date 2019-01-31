using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{

    HitEvent currentNote;
    public static InputManager Instance;
    double currentOffset;
    bool canHit;
    double hitWindow;



    // Start is called before the first frame update
    void Start()
    {
        Instance = this;   
    }

    public void LoadNote(HitEvent note)
    {
        currentNote = note;
        canHit = true;

    }

    public void SetOverallDifficulty()
    {

    }

    // Update is called once per frame
    void Update()
    {
        currentOffset = GameManager.Instance.timer * 1000f;

        if (currentNote.getPlayMode() == 1 || currentNote.getPlayMode() == -1) // memory/inverted memory
        {
            HandleMemoryNote();
        }
        else // step
        {
            if (currentNote.IsMine()) // mine 
            {
                HandleMine();
            }
            else if (currentNote.IsHold()) // hold note
            {
                HandleHoldNote();
            }
            else // normal note
            {
                HandleStepNote();
            }
        }

        if (currentOffset >= currentNote.getOffset() - hitWindow && currentOffset <= currentNote.getOffset() + hitWindow) // In hit window
        {
            if (Input.GetKeyDown(currentNote.getkeyCode())) // hit note
            {
                
            }
        }
        else if (currentOffset > currentNote.getOffset() + hitWindow) // Missed hit window
        {
            if (currentNote.getPlayMode() == 1 || currentNote.getPlayMode() == -1) // memory/inverted memory
            {
                MemoryNoteManager.Instance.NoteHit();
            }
            else // step
            {
                if (currentNote.IsMine()) // mine 
                {
                    //GameManager.Instance.NoteMissed();
                }
                else if (currentNote.IsHold()) // hold note
                {
                    HandleHoldNote();
                }
                else // normal note
                {
                    //GameManager.Instance.NoteHit(true);
                }
            }
        }
    }

    void HandleHoldNote()
    {

    }

    void HandleStepNote()
    {

    }

    void HandleMine()
    {

    }

    void HandleMemoryNote()
    {

    }
}
