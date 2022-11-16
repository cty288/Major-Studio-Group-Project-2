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
            return new List<string>() { "Its hair style, as the witness describes, is ridiculous." };
        }
    }

    public override List<string> FakeDescriptions
    {
        get
        {
            return new List<string>() { "It wore its hair in a high ponytail, as the security camera can tell." };
        }
    }
}

public class HairCutTagBuzzCut : HaircutTag
{
    public override List<string> RealDescriptions
    {
        get
        {
            return new List<string>() { "According to the security camera, the convict's hair is short." };
        }
    }

    public override List<string> FakeDescriptions
    {
        get
        {
            return new List<string>() { "Its hair is wavy." };
        }
    }
}

public class HairCutTagMiddleScore : HaircutTag
{
    public override List<string> RealDescriptions
    {
        get
        {
            return new List<string>() { "The convict has an Asian style haircut." };
        }
    }

    public override List<string> FakeDescriptions
    {
        get
        {
            return new List<string>() { "It has a black, long hair." };
        }
    }
}

public class HairCutTagPonyTail : HaircutTag
{
    public override List<string> RealDescriptions
    {
        get
        {
            return new List<string>() { "Its hair is in a high ponytail, as the security camera can tell." };
        }
    }

    public override List<string> FakeDescriptions
    {
        get
        {
            return new List<string>() { "Its hair style, as the witness describes, is ridiculous." };
        }
    }
}

public class HairCutTagLongBlackHair : HaircutTag
{
    public override List<string> RealDescriptions
    {
        get
        {
            return new List<string>() { "Its hair is in a high ponytail, as the security camera can tell." };
        }
    }

    public override List<string> FakeDescriptions
    {
        get
        {
            return new List<string>() { "Its hair style, as the witness describes, is ridiculous." };
        }
    }
}



