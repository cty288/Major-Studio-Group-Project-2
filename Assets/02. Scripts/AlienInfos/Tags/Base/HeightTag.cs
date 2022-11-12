using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IHeightTag : IAlienTag
{
    public float Height { get; }
}
public class HeightTag : AbstractAlienTag, IHeightTag {
    public float Height { get; set; }

    public HeightTag(float height) {
        this.Height = height;
    }

    public override List<string> RealDescriptions { get; } = new List<string>() { };
    public override List<string> FakeDescriptions { get;} = new List<string>();
}
