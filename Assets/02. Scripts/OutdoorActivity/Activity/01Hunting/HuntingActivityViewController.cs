using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using _02._Scripts.BodyManagmentSystem;
using MikroFramework.Architecture;
using MikroFramework.Examples;
using NHibernate.Mapping;
using Unity.VisualScripting;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class HuntingActivityViewController : ActivityViewController<Hunting> {

    public Animator huntingProcessAnimation;
    public GameObject positionOfAttenders;
    public GameObject buttons;

    GameObject[] obj_attenders = new GameObject[8];
    bool[] attenderIsAlien = new bool[8];
    enum EventStage
    {
        partner_selection,
        hunting_process,
        hunting_reward,
        hunting_fail,
        quit
    }

    EventStage stage;

	protected override void Awake() {
		base.Awake();
    }
    //AlienBody.BuildShadowBody(item,false)
    protected override void OnInit()
    {
        buttons.SetActive(true);
        huntingProcessAnimation.SetInteger("process", 0);
        stage = EventStage.partner_selection;
        attenderIsAlien = activity.attenderIsAlien;
        base.OnInit();
        for (int i = 0; i < 8; i++)
        {
            generateAttender(i);
        }
        Debug.Log("Hunt");
    }

    private void Update()
    {
        switch(stage)
        {
            case EventStage.partner_selection:

                break;
            case EventStage.hunting_process:
                huntingProcessAnimation.SetInteger("process",1);
                buttons.SetActive(false);
                StartCoroutine(huntingResultCheck(2f));
                break;
            case EventStage.hunting_reward:
                Debug.Log("You Are Safe!");
                break;
            case EventStage.hunting_fail:
                Debug.Log("You Are Killed!");
                break;
            case EventStage.quit:

                break;
        }
        //Debug.Log("Is Hunting");
    }

    public void startHunting()
    {
        stage = EventStage.hunting_process;
        Debug.Log("is Hunting!");
    }

    public void quitHunting()
    {
        stage = EventStage.quit;
        Debug.Log("End Hunting");
        //还没做 假装退出了
    }

    private void generateAttender(int index)
    {
        obj_attenders[index] = AlienBody.BuildShadowBody(activity.attenders[index], false);
        
        obj_attenders[index].transform.localScale = new Vector3(0.5f, 0.5f, 1f);

        if (index > activity.allyNum - 1)
        {
            obj_attenders[index].GetComponent<AlienBody>().HideColor(0f);
        }


        float pos_x = UnityEngine.Random.Range(-5f, 5f);
        float pos_y = UnityEngine.Random.Range(-2.4f, 2.4f);

        obj_attenders[index].transform.position = positionOfAttenders.transform.position + new Vector3(pos_x, pos_y, 0);
    }

    IEnumerator huntingResultCheck(float delay)
    {
        yield return new WaitForSeconds(delay);
        hasAlien();
    }

   void hasAlien()
    {
        foreach (var attender in attenderIsAlien)
        {
            if (attender)
            {
                Debug.Log("Alien!");
                stage = EventStage.hunting_fail;
                huntingProcessAnimation.SetInteger("process", 3);
                return;
            }
        }
        huntingProcessAnimation.SetInteger("process", 2);
        stage = EventStage.hunting_reward;
    } 

}
