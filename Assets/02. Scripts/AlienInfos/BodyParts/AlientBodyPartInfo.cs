using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping;
using UnityEngine;
using UnityEngine.Serialization;

//反射获取所有继承此类的类

public enum BodyPartType {
    Head,
    Body,
    Legs,
    Accessory,
}

public class BodyPartPrefabInfo {
	[ES3Serializable]
	private GameObject prefab;
	public BodyPartPrefabInfo SubBodyPartInfo;
	
	[ES3NonSerializable]
	private List<IAlienTag> tags;

	[ES3NonSerializable] private AlienBodyPartInfo bodyPartInfo;

	public AlienBodyPartInfo BodyPartInfo {
		get {
			if (bodyPartInfo == null) {
				bodyPartInfo = prefab.GetComponent<AlienBodyPartInfo>();
			}
			return bodyPartInfo;
		}
	}

	public BodyPartPrefabInfo(){}

	public BodyPartPrefabInfo(GameObject prefab) {
		this.prefab = prefab;
		AlienBodyPartInfo info = prefab.GetComponent<AlienBodyPartInfo>();
		bodyPartInfo = this.prefab.GetComponent<AlienBodyPartInfo>();
		if (info.SubBodyPartCollectionIndex < 0) {
			return;
		}
		bool hasSubBodyPart = Random.Range(0, 100) < 30;
		if (!hasSubBodyPart) {
			return;
		}

		BodyPartCollection accessoryCollection = AlienBodyPartCollections.Singleton.AccessoryCollections[info.SubBodyPartCollectionIndex];
		bool isTall = info.IsTall;
		bool isNews = info.IsNewspaper;
		BodyPartHeightSubCollection subCollection = AlienBodyPartCollections.Singleton.TryGetBodyPartHeightSubCollection(accessoryCollection,
			isTall ? HeightType.Tall : HeightType.Short);

		var infos = AlienBodyPartCollections.Singleton.GetBodyPartDisplayByType(subCollection,
			isNews ? BodyPartDisplayType.Newspaper : BodyPartDisplayType.Shadow);
		
		
		SubBodyPartInfo =
			infos.HumanTraitPartsPrefabs[Random.Range(0, infos.HumanTraitPartsPrefabs.Count)].GetComponent<AlienBodyPartInfo>().GetBodyPartPrefabInfo();
	}

	public List<IAlienTag> Tags {
		get {
			if(tags == null) {
				tags = new List<IAlienTag>();
				tags.AddRange(BodyPartInfo.SelfTags);
				BodyPartPrefabInfo sub = SubBodyPartInfo;
				while (sub != null) {
					tags.AddRange(sub.BodyPartInfo.SelfTags);
					sub = sub.SubBodyPartInfo;
				}
			}
			return tags;
		}
	}
}

public abstract class AlienBodyPartInfo : MonoBehaviour {
    public abstract List<IAlienTag> SelfTags { get; }
    public abstract BodyPartType BodyPartType { get; }

    public Transform JointPoint;

    [FormerlySerializedAs("OppositeTraitBodyPart")] 
    public List<GameObject> OppositeTraitBodyParts = new List<GameObject>();

    [HideInInspector]
    public bool IsAlienOnly = false;

    public int SubBodyPartCollectionIndex = -1;

    public bool IsTall;
    public bool IsNewspaper;

    public BodyPartPrefabInfo GetBodyPartPrefabInfo() {
	    return new BodyPartPrefabInfo(this.gameObject);
	}

    public void Init(BodyPartPrefabInfo prefabInfo) {
	    if(prefabInfo.SubBodyPartInfo == null) {
		    return;
	    }

	    BodyPartPrefabInfo subprefabInfo = prefabInfo.SubBodyPartInfo;

	    GameObject spawnedBodyPart = Instantiate(subprefabInfo.BodyPartInfo.gameObject, JointPoint.transform);
	    spawnedBodyPart.GetComponent<AlienBodyPartInfo>().Init(subprefabInfo);
    }

	

}


