using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public interface IAlienTag {
    List<string> RealDescriptions { get; }
    List<string> FakeDescriptions { get; }
    string GetRandomDescription(out bool isReal);
    string GetRandomDescription(bool isReal);
}

[Serializable]
public abstract class AbstractAlienTag : IAlienTag {
 
    public abstract List<string> RealDescriptions { get; }
 
    public abstract List<string> FakeDescriptions { get;}
    public string GetRandomDescription(out bool isReal) {
        List<string> targetList = Random.Range(0, 2) == 0 ? RealDescriptions : FakeDescriptions;
        isReal = targetList == RealDescriptions;
        return targetList[Random.Range(0, targetList.Count)];
    }

    public string GetRandomDescription(bool isReal) {
        List<string> targetList = isReal ? RealDescriptions : FakeDescriptions;
        return targetList[Random.Range(0, targetList.Count)];
    }
}
