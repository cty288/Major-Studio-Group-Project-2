using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

//反射获取所有继承此类的类

public enum BodyPartType {
    Head,
    Body,
    Legs
}
public abstract class AlienBodyPartInfo : MonoBehaviour {
    public abstract List<IAlienTag> Tags { get; }
    public abstract BodyPartType BodyPartType { get; }

    public Transform JointPoint;

    [FormerlySerializedAs("OppositeTraitBodyPart")] public List<GameObject> OppositeTraitBodyParts = new List<GameObject>();

    [HideInInspector]
    public bool IsAlienOnly = false;

    
  //  public HeightType Height;

}


