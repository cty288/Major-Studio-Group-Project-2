using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFatTag : IAlienTag {
    public float Fatness { get; }
}

public class FatTag : AbstractAlienTag, IFatTag {
    [SerializeField] private float fatness;
    public float Fatness => fatness;

    public FatTag(float fatness) {
        this.fatness = fatness;
    }
}


