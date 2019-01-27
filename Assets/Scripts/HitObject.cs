
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitObject
{
    private int x;
    private int y;
    private int offset;
    private int endOffset;

    private bool isNote; //true for note
    private bool isMine;
    private bool flashRed;
    private bool flashGreen;
    private bool flashBlue;
    private bool flashYellow;
    private bool isHold;
    private bool flashBlack;
    

    public HitObject()
    {

    }

    /************************
     * getters
     ***********************/
     public void printAll()
    {
        Debug.Log(string.Format("HitObject: {0} {1} {2} {3} {4} | {5} {6} {7} {8} {9} {10}",x, y, offset, isNote, isMine, flashRed, flashGreen, flashBlue, flashYellow, isHold, flashBlack));
    }

    public int getX()
    {
        return x;
    }

    public int getY()
    {
        return y;
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

    /************************
     * setters
     ***********************/
    

    public void setX(string input)
    {
        x = Convert.ToInt32(input);
    }

    public void setY(string input)
    {
        y = Convert.ToInt32(input);
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
        flashRed = readBit(num, 2);
        flashBlue = readBit(num, 3);
        flashYellow = readBit(num, 4);
        flashGreen = readBit(num, 5);
    }

    public void setFlashBlack(string input)
    {
        flashBlack = readBit(Convert.ToInt32(input), 6);
    }

    public void setIsHold(string input)
    {
        isHold = readBit(Convert.ToInt32(input), 7);
    }


}
