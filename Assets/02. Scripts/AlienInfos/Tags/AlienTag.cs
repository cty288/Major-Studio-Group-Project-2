using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public interface IAlienTag {
    List<string> RealDescriptions { get; }
    List<string> FakeDescriptions { get; }
    string GetRandomDescription(out bool isReal);
}

[Serializable]
public abstract class AbstractAlienTag : IAlienTag {
    [field: SerializeField]
    public List<string> RealDescriptions { get; set; }
    [field: SerializeField]
    public List<string> FakeDescriptions { get; set; }
    public string GetRandomDescription(out bool isReal) {
        List<string> targetList = Random.Range(0, 2) == 0 ? RealDescriptions : FakeDescriptions;
        isReal = targetList == RealDescriptions;
        return targetList[Random.Range(0, targetList.Count)];
    }
}
