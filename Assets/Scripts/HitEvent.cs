
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEvent
{
    private int key;
    private int offset;
    private int endOffset;
    private Color[] colorArray = new Color[1];
    private int[] colorIntArray = new int[1];
    private bool sequenceStart;

    private bool isNote; //true for note
    private bool isMine;
    private bool flashRed;
    private bool flashGreen;
    private bool flashBlue;
    private bool flashYellow;
    private bool isHold;
    private bool flashBlack;

    KeyCode keyCode;
    int playMode;

    /************************
     * getters
     ***********************/

    public int getKey()
    {
        return key;
    }

    public int getOffset()
    {
        return offset;
    }

    public int getEndOffset()
    {
        return endOffset;
    }

    public bool IsNote()
    {
        return isNote;
    }

    public bool IsMine()
    {
        return isMine;
    }

    public bool isFlashRed()
    {
        return flashRed;
    }

    public bool isFlashGreen()
    {
        return flashGreen;
    }

    public bool isFlashYellow()
    {
        return flashYellow;
    }

    public bool isFlashBlue()
    {
        return flashBlue;
    }

    public bool IsHold()
    {
        return isHold;
    }

    public bool IsflashBlack()
    {
        return flashBlack;
    }

    public bool isSequenceStart()
    {
        return sequenceStart;
    }

    public Color[] getColorArray()
    {
        return colorArray;
    }

    public int[] getColorIntArray()
    {
        return colorIntArray;
    }

    public KeyCode getkeyCode()
    {
        return keyCode;
    }

    public int getPlayMode()
    {
        return playMode;
    }

    /************************
     * setters
     ***********************/


    public void setKey(string input)
    {
        key = Convert.ToInt32(input);
    }

    public void setOffset(string input)
    {
        offset = Convert.ToInt32(input);
    }

    public void setEndOffset(string input)
    {
        endOffset = Convert.ToInt32(input);
    }

    private bool readBit(int num, int index)
    {
        return (num & (1 << index)) != 0;
    }

    public void setIsNote(string input)
    {
        isNote = readBit(Convert.ToInt32(input), 0);
    }

    public void setIsMine(string input)
    {
        isMine = readBit(Convert.ToInt32(input), 1);
    }

    public void setColour(string input)
    {
        int num = Convert.ToInt32(input);
        flashBlue = readBit(num, 2);
        flashRed = readBit(num, 3);
        flashGreen = readBit(num, 4);
        flashYellow = readBit(num, 5);
    }

    public void setFlashBlack(string input)
    {
        flashBlack = readBit(Convert.ToInt32(input), 6);
    }

    public void setIsHold(string input)
    {
        isHold = readBit(Convert.ToInt32(input), 7);
    }

    public void setColorArray(string input)
    {
        String[] colors = input.Split(':');
        if (colors.Length > 1)
        {
            colorArray = new Color[4];
            colorIntArray = new int[4];
            for (int i = 0; i < 4; i++)
            {
                colorIntArray[i] = Convert.ToInt32(colors[i]);
                switch (colors[i])
                {
                    case "0":
                        colorArray[i] = Color.yellow;
                        break;
                    case "1":
                        colorArray[i] = Color.green;
                        break;
                    case "2":
                        colorArray[i] = Color.red;
                        break;
                    case "3":
                        colorArray[i] = Color.blue;
                        break;
                    default:
                        break;
                }
            }
        }
        else // input is actually for endOffset
        {
            setEndOffset(input);
        }
    }

    public void setSequenceStart(bool input)
    {
        sequenceStart = input;
    }

    public void setkeyCode(KeyCode keyCode)
    {
        this.keyCode = keyCode;
    }

    public void setPlayMode(int playMode)
    {
        this.playMode = playMode;
    }

}
