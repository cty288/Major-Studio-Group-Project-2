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
public abstract class AlienBodyPartInfo {
    public abstract IFatTag FatTag { get; }
    public abstract IHeightTag HeightTag { get; }
    
    public abstract List<IAlienTag> Tags { get; }

    public abstract string BodyPartPrefabAssetName { get; }

    public abstract BodyPartType BodyPartType { get; }

}


