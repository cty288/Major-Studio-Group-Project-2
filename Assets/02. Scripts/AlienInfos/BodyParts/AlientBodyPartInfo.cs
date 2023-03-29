using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NHibernate.Mapping;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

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
	
	[ES3Serializable]
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

	public List<BodyPartPrefabInfo> GetSubBodyPartInfos() {
		List<BodyPartPrefabInfo> infos = new List<BodyPartPrefabInfo>() {this};
		if (SubBodyPartInfo != null) {
			infos.AddRange(SubBodyPartInfo.GetSubBodyPartInfos());
		}
		return infos;
	}

	public BodyPartPrefabInfo(){}

	public BodyPartPrefabInfo(GameObject prefab, int subBodyPartChance) {
		this.prefab = prefab;
		AlienBodyPartInfo info = prefab.GetComponent<AlienBodyPartInfo>();
		bodyPartInfo = this.prefab.GetComponent<AlienBodyPartInfo>();
		if (info.SubBodyPartCollectionIndex < 0) {
			return;
		}
		bool hasSubBodyPart = Random.Range(0, 100) < subBodyPartChance;
		if (!hasSubBodyPart) {
			return;
		}

		BodyPartCollection accessoryCollection = AlienBodyPartCollections.Singleton.AccessoryCollections[info.SubBodyPartCollectionIndex];

		if (accessoryCollection.HeightSubCollections.Count == 0) {
			return;
		}
		bool isTall = info.IsTall;
		bool isNews = info.IsNewspaper;
		BodyPartHeightSubCollection subCollection = AlienBodyPartCollections.Singleton.TryGetBodyPartHeightSubCollection(accessoryCollection,
			isTall ? HeightType.Tall : HeightType.Short);

		var infos = AlienBodyPartCollections.Singleton.GetBodyPartDisplayByType(subCollection,
			isNews ? BodyPartDisplayType.Newspaper : BodyPartDisplayType.Shadow);
		
		
		SubBodyPartInfo =
			infos.HumanTraitPartsPrefabs[Random.Range(0, infos.HumanTraitPartsPrefabs.Count)].GetComponent<AlienBodyPartInfo>().GetBodyPartPrefabInfo();
	}

	public List<IAlienTag> AllTags {
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
	
	public List<IAlienTag> SelfTags {
		get {
			return BodyPartInfo.SelfTags;
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

    protected SpriteRenderer spriteRenderer;

    public BodyPartPrefabInfo GetBodyPartPrefabInfo(int subBodyPartChance = 30) {
	    return new BodyPartPrefabInfo(this.gameObject, subBodyPartChance);
	}

    protected virtual void Awake() {
	    Transform sprite = transform.Find("Sprite");
	    if(sprite != null) {
		    spriteRenderer = sprite.GetComponent<SpriteRenderer>();
	    }
    }
    
    public void HideInstantly() {
	    if(spriteRenderer != null) {
		    spriteRenderer.color = new Color(0, 0, 0, 0);
	    }
	}
    
    public void Appear(float duration) {
	    if(spriteRenderer != null) {
		    spriteRenderer.DOFade(1, duration);
	    }
	}

    public void Disappear(float duration) {
	    if (spriteRenderer != null) {
		    spriteRenderer.DOFade(0, duration);
	    }
    }

    public void ShowColor(float duration, Color color) {
	    if (spriteRenderer != null) {
		    spriteRenderer.DOColor(color, duration);
	    }
    }
    
    public void HideColor(float duration) {
	    if (spriteRenderer != null) {
		    spriteRenderer.DOColor(Color.black, duration);
	    }
	}


    public void Init(BodyPartPrefabInfo prefabInfo) {
	    if(prefabInfo.SubBodyPartInfo == null) {
		    return;
	    }

	    BodyPartPrefabInfo subprefabInfo = prefabInfo.SubBodyPartInfo;

	    GameObject spawnedBodyPart = Instantiate(subprefabInfo.BodyPartInfo.gameObject, JointPoint.transform.position, Quaternion.identity, transform.parent);
	    spawnedBodyPart.transform.localScale = transform.localScale;
	    spawnedBodyPart.GetComponent<AlienBodyPartInfo>().Init(subprefabInfo);
    }

    public virtual void OnFlashStart(float fadeTime) {
	    
    }
    
    public virtual void OnFlashStop(float fadeTime) {
	    
    }

	

}

public abstract class HeadBodyPartInfo : AlienBodyPartInfo {
	[SerializeField] private List<SpriteRenderer> eyes = new List<SpriteRenderer>();

	protected override void Awake() {
		base.Awake();
		ShowEyesInstant();
	}

	public override void OnFlashStart(float fadeTime) {
		base.OnFlashStart(fadeTime);
		foreach (SpriteRenderer eye in eyes) {
			eye.DOFade(0, fadeTime);
		}
	}
	
	public override void OnFlashStop(float fadeTime) {
		base.OnFlashStop(fadeTime);
		foreach (SpriteRenderer eye in eyes) {
			eye.DOFade(1, fadeTime);
		}
	}

	private void ShowEyesInstant() {
		foreach (var eye in eyes) {
			var color = eye.color;
			color = new Color(color.r, color.g, color.b, 1);
			eye.color = color;
		}
	}
}


