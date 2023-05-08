using System.Collections;
using System.Collections.Generic;
using _02._Scripts.Stats;
using MikroFramework;
using MikroFramework.Architecture;
using UnityEngine;
using UnityEngine.UI;

public class StatsPanel : AbstractMikroController<MainGame>
{
    //private GameObject BG_States;
    private Animator BG_States_Ani;

    private Button Btn_Back;
    private GameObject Btn_Back_Obj;
    [SerializeField] private Button Btn_States;
    [SerializeField] private GameObject statsElementPrefab;
    
    
    private Transform statsElementContainer;
    private StatsModel statsModel;
    private bool isOn = false;
    private void Awake()
    {
        //Get Stuffs
       // BG_States = GameObject.Find("Panel_States").gameObject;
        BG_States_Ani = GetComponent<Animator>();
        Btn_Back_Obj = GameObject.Find("BG_States").transform.Find("Btn_Back").gameObject;
        Btn_Back = Btn_Back_Obj .GetComponent<Button>();
        Btn_States = GameObject.Find("Stats").GetComponent<Button>();

        statsElementContainer = transform.Find("BG_States/Scroll View/Viewport/Layout");
        //Btns
        Btn_States.onClick.AddListener(TurnOnStatsPanel);
        Btn_Back.onClick.AddListener(BackToDiePanel);
        statsModel = this.GetModel<StatsModel>();
    }


    public void TurnOnStatsPanel() {
        if (isOn) {
            return;
        }
        isOn = true;
        BG_States_Ani.SetTrigger("ON");
        List<SaveData> stats = statsModel.GetStats();
        foreach (var stat in stats) {
            GameObject statsElement = Instantiate(statsElementPrefab, statsElementContainer);
            StatsElement element = statsElement.GetComponent<StatsElement>();
            element.SetData(stat.DisplayName, stat.Value);
            this.Delay(0.5f, () => {
                element.Show(0.5f);
            });
           
        }
    }

    public void BackToDiePanel()
    {
        if (!isOn) {
            return;
        }
        isOn = false;
        BG_States_Ani.SetTrigger("OFF");
        foreach (Transform child in statsElementContainer) {
            StatsElement element = child.GetComponent<StatsElement>();
            element.Stop(0.5f);
        }
    }
}
