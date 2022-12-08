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

        if(fatness == FatType.Fat)
        {
            RealDescriptions = new List<string>() { "The convict is a <color=yellow>fatty</color>, according to the witness." };
            FakeDescriptions = new List<string>() { "It is <color=yellow>strong</color>, as the witness reported." };
        }
        else if(fatness == FatType.Thin)
        {
            RealDescriptions = new List<string>() { "The victim's body is reported to be <color=yellow>slim</color>." };
            FakeDescriptions = new List<string>() { "The witness saw this victim's <color=yellow>batter down a wall with its fist</color>." };
        }
    }

    public override List<string> RealDescriptions { get; } = new List<string>() {};

    public override List<string> FakeDescriptions { get; } = new List<string>() {};


}


