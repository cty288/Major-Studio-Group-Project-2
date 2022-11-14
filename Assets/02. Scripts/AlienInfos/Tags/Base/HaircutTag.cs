using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHaircutTag : IAlienTag
{

}

public abstract class HaircutTag : AbstractAlienTag, IHaircutTag
{
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

public class HairCutTagBuzzCut : HaircutTag
{
    public override List<string> RealDescriptions
    {
        get
        {
            return new List<string>() { "Buzz Cut Hair" };
        }
    }

    public override List<string> FakeDescriptions
    {
        get
        {
            return new List<string>() { "Bald" };
        }
    }
}

public class HairCutTagMiddleScore : HaircutTag
{
    public override List<string> RealDescriptions
    {
        get
        {
            return new List<string>() { "He likes playing basketball." };
        }
    }

    public override List<string> FakeDescriptions
    {
        get
        {
            return new List<string>() { "He is addicted to playing MikroCosmos" };
        }
    }
}

