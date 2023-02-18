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
    public  List<string> RealRadioDescriptions { get; } = new List<string>() {};

    public  List<string> FakeRadioDescriptions { get; } = new List<string>() {};
    public FatTag(FatType fatness) {
        this.fatness = fatness;
        if(fatness == FatType.Fat)
        {
            RealRadioDescriptions = new List<string>() { "The convict is a <color=yellow>fatty</color>, according to the witness." };
            FakeRadioDescriptions = new List<string>() { "It is <color=yellow>strong</color>, as the witness reported." };
        }
        else if(fatness == FatType.Thin)
        {
            RealRadioDescriptions = new List<string>() { "The victim's body is reported to be <color=yellow>slim</color>." };
            FakeRadioDescriptions = new List<string>() { "The witness saw this victim's <color=yellow>batter down a wall with its fist</color>." };
        }
       
    }

    public override string GetRandomRadioDescription(out bool isReal) {
        isReal = Random.Range(0, 2) == 0;
        if (isReal) {
            return RealRadioDescriptions[Random.Range(0, RealRadioDescriptions.Count)];
        } else {
            return FakeRadioDescriptions[Random.Range(0, FakeRadioDescriptions.Count)];
        }
    }

    public override string GetRandomRadioDescription(bool isReal) {
        if (isReal) {
            return RealRadioDescriptions[Random.Range(0, RealRadioDescriptions.Count)];
        } else {
            return FakeRadioDescriptions[Random.Range(0, FakeRadioDescriptions.Count)];
        }
    }

    public override List<string> GetShortDescriptions() {
       return new List<string>() { "" };
    }


    public override string TagName { get; } = "Fattness";
}


