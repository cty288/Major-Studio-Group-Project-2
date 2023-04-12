using _02._Scripts.AlienInfos.Tags.Base.KnockBehavior;
using _02._Scripts.BodyManagmentSystem;
using _02._Scripts.GameTime;
using MikroFramework.Architecture;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[ES3Serializable]
public class Hunting : Activity {

    //Variables

    public int allyNum = 4;
    public BodyInfo[] attenders = new BodyInfo[8];
    public bool[] attenderIsAlien = new bool[8];

    float nonAlienChance = 1f;
    BodyModel bodyModel;


    [field: ES3Serializable]
	public override string Name { get; protected set; } = "HuntingGround_Hunting";
	[field: ES3Serializable]
	public override string SceneAssetName { get; } = "HuntingGround_Hunting";
	
	public Hunting(): base(){}
	protected override void OnInit() {
		Debug.Log("Hunting OnInit");
	}

	protected override void OnDayStarts(DateTime date) {
		Debug.Log("Hunting OnDayStarts");
        bodyModel = this.GetModel<BodyModel>();
        GenerateAttenders();
        //Generate Random Attender at Begining of Day

        getNonAlineChance(this.GetModel<GameTimeModel>().GetDay(date));
    }

    void getNonAlineChance(int day)
    {
        int getMapDate = 14;
        if(nonAlienChance < getMapDate + 7)
        {
            nonAlienChance = 0;
        }
        else
        {
            float minChance = Math.Min((day - getMapDate) * 0.05f,0.2f);
            float maxChance = 0.75f;
            nonAlienChance = Math.Max(minChance, maxChance) * Random.Range(0.75f, 1.25f);
        }
    }
	protected override void OnDayEnds(DateTime date) {
		Debug.Log("OnDayEnds");
	}

	protected override void OnEnterPlayer() {
		Debug.Log("OnEnterPlayer");//Transfer Information
	}

	protected override void OnLeavePlayer() {
		
		Debug.Log("OnLeavePlayer");
	}

	protected override void OnAvailable() {
		Debug.Log("Hunting is available");
	}

	protected override void OnUnavailable() {
		Debug.Log("Hunting is unavailable");
	}

    //generate alien 
    private Dictionary<BodyPartType, HashSet<int>> GetAvailableBodyPartIndicesa()
    {
        Dictionary<BodyPartType, HashSet<int>> indices = bodyModel.AvailableBodyPartIndices;

        foreach (BodyPartType bodyPartType in indices.Keys)
        {
            BodyPartCollection collection =
                AlienBodyPartCollections.Singleton.GetBodyPartCollectionByBodyType(bodyPartType);
            int count = collection.HeightSubCollections[0].NewspaperBodyPartDisplays.HumanTraitPartsPrefabs.Count;

            int additionalCount = Random.Range(1, 4);
            for (int i = 0; i < additionalCount; i++)
            {
                indices[bodyPartType].Add(Random.Range(0, count));
            }
        }

        return indices;


    }
    void GenerateAttenders()
    {
        List<BodyTimeInfo> Aliens = bodyModel.Aliens;

        var availableBodyPartIndices = GetAvailableBodyPartIndicesa();
        for (int i=0;i < attenders.Length; i++)
		{
            if ((Random.Range(0f, 1f) <= nonAlienChance || Aliens.Count == 0 ) || (i >= allyNum))
            {
                attenderIsAlien[i] = false;
                attenders[i] = BodyInfo.GetRandomBodyInfo(BodyPartDisplayType.Shadow, attenderIsAlien[i], false,
                   new NormalKnockBehavior(3, Random.Range(4, 7), null), availableBodyPartIndices);
                // Debug.Log("Spawned a non-alien");
            }
            else
            {
                attenders[i] = Aliens[Random.Range(0, Aliens.Count)].BodyInfo;
                //Debug.Log("Spawned an alien!");
                attenderIsAlien[i] = true;
            }
        }
    }
}
