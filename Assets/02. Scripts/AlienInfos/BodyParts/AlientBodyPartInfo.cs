using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

    public GameObject OppositeTraitBodyPart;

    [HideInInspector]
    public bool IsAlienOnly = false;

    
  //  public HeightType Height;

}


