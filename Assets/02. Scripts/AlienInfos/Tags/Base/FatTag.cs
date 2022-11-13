using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFatTag : IAlienTag {
    public FatType Fatness { get; }
}

public enum FatType {
    Fat,
    Thin
}
public class FatTag : AbstractAlienTag, IFatTag {
    [SerializeField] private FatType fatness;
    public FatType Fatness => fatness;

    public FatTag(FatType fatness) {
        this.fatness = fatness;
    }

    public override List<string> RealDescriptions { get;  } = new List<string>() {

    };

    public override List<string> FakeDescriptions { get; } = new List<string>() {

    };
}


