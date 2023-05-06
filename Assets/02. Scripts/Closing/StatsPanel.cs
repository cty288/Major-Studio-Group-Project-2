using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsPanel : MonoBehaviour
{
    //private GameObject BG_States;
    private Animator BG_States_Ani;

    private Button Btn_Back;
    private GameObject Btn_Back_Obj;
    [SerializeField] private Button Btn_States;
    private void Awake()
    {
        //Get Stuffs
       // BG_States = GameObject.Find("Panel_States").gameObject;
        BG_States_Ani = GetComponent<Animator>();
        Btn_Back_Obj = GameObject.Find("BG_States").transform.Find("Btn_Back").gameObject;
        Btn_Back = Btn_Back_Obj .GetComponent<Button>();
        Btn_States =GameObject.Find("Stats").GetComponent<Button>();

        //Btns
        Btn_States.onClick.AddListener(TurnOnStatsPanel);
        Btn_Back.onClick.AddListener(BackToDiePanel);
    }


    public void TurnOnStatsPanel()
    {
        BG_States_Ani.SetTrigger("ON");
    }

    public void BackToDiePanel()
    {
        BG_States_Ani.SetTrigger("OFF");
    }
}
