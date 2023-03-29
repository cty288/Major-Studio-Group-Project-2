using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadioHint : MonoBehaviour
{
    private static RadioHint instance;
    public GameObject hintBG;
    public GameObject hint, hintTxt_stop, hintTxt_turn;

    public static RadioHint Instance { get { return instance; } }

    
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }

        hint = gameObject.transform.Find("HintLayout").gameObject;
        hintBG = GameObject.Find("HintBG");
        hintTxt_stop = gameObject.transform.Find("HintTxt_Stop").gameObject;
        hintTxt_turn = gameObject.transform.Find("HintTxt_Turn").gameObject;

    }

    public void Show()
    {
        hintBG.SetActive(true);
        hint.gameObject.SetActive(true);
        hintTxt_stop.gameObject.SetActive(true);
        hintTxt_turn.gameObject.SetActive(true);
    }

    public void Hide()
    {
        hint.gameObject.SetActive(false);
        hintTxt_stop.gameObject.SetActive(false);
        hintTxt_turn.gameObject.SetActive(false);
    }
    
}
