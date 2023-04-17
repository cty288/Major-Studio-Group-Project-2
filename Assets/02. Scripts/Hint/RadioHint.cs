using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadioHint : MonoBehaviour
{
    private static RadioHint instance;
    public GameObject hintBG;
    public GameObject hint, hintTxt_stop, hintTxt_turn;
    public bool IsShowing = false;
    public void Show()
    {
        hintBG.SetActive(true);
        hint.gameObject.SetActive(true);
        hintTxt_stop.gameObject.SetActive(true);
        hintTxt_turn.gameObject.SetActive(true);
        IsShowing = true;
    }

    public void Hide()
    {
        hintBG.SetActive(false);
        hint.gameObject.SetActive(false);
        hintTxt_stop.gameObject.SetActive(false);
        hintTxt_turn.gameObject.SetActive(false);
        IsShowing = false;
    }
    
}
