using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHaircutTag : IAlienTag
{

}

public abstract class HaircutTag : AbstractAlienTag, IHaircutTag
{
}


public class TestHairCutTag : HaircutTag
{
    public override List<string> RealDescriptions
    {
        get
        {
            return new List<string>() { "Horse Shit Head" };
        }
    }

    public override List<string> FakeDescriptions
    {
        get
        {
            return new List<string>() { "Horse Tail Head" };
        }
    }
}

public class HairCutTagHorseShit : HaircutTag
{
    public override List<string> RealDescriptions
    {
        get
        {
            return new List<string>() { "Horse Shit Head" };
        }
    }

    public override List<string> FakeDescriptions
    {
        get
        {
            return new List<string>() { "Horse Tail Head" };
        }
    }
}

