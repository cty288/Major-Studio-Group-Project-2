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
            RealDescriptions = new List<string>() { "The convict is a fatty, according to sthe witness." };
            FakeDescriptions = new List<string>() { "It is strong, as the witness reported." };
        }
        else if(fatness == FatType.Thin)
        {
            RealDescriptions = new List<string>() { "The creature's body is reported to be slim." };
            FakeDescriptions = new List<string>() { "The witness saw this creature batter down a wall with its fist." };
        }
    }

    public override List<string> RealDescriptions { get; } = new List<string>() {};

    public override List<string> FakeDescriptions { get; } = new List<string>() {};


}


