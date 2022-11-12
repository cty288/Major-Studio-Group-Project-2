using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//�����ȡ���м̳д������

public enum BodyPartType {
    Head,
    Body,
    Legs
}
public abstract class AlienBodyPartInfo : MonoBehaviour {
    public abstract IFatTag FatTag { get; }
    public abstract IHeightTag HeightTag { get; }
    
    public abstract List<IAlienTag> Tags { get; }
    public abstract BodyPartType BodyPartType { get; }

    public Transform JointPoint;

}


