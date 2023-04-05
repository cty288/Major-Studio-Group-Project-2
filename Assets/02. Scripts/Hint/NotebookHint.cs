using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotebookHint : MonoBehaviour
{
    //private static NotebookHint instance;
    public GameObject hintBG;
    public GameObject hint, hintTxt_prev, hintTxt_next, pen, eraser, pen_txt, eraser_txt;
    public bool IsShow = false;


    public void Show()
    {
        hintBG.SetActive(true);
        hint.gameObject.SetActive(true);
        hintTxt_prev.gameObject.SetActive(true);
        hintTxt_next.gameObject.SetActive(true);
        pen.gameObject.SetActive(true);
        eraser.gameObject.SetActive(true);
        pen_txt.gameObject.SetActive(true);
        eraser_txt.gameObject.SetActive(true);
        IsShow = true;
    }

    public void Hide()
    {
        hintBG.SetActive(false);
        hint.gameObject.SetActive(false);
        hintTxt_prev.gameObject.SetActive(false);
        hintTxt_next.gameObject.SetActive(false);
        pen.gameObject.SetActive(false);
        eraser.gameObject.SetActive(false);
        pen_txt.gameObject.SetActive(false);
        eraser_txt.gameObject.SetActive(false);
        IsShow = false;
    }
    
}
