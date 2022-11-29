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
            return new List<string>()
            {
                "Its hair style, as the witness describes, is ridiculous.",
                "Its hair is very messy."
            };
        }
    }

    public override List<string> FakeDescriptions
    {
        get
        {
            return new List<string>()
            {
                "It wore its hair in a high ponytail, as the security camera can tell.",
                "We don't have information of its head yet."
            };
        }
    }
}

public class HairCutTagBuzzCut : HaircutTag
{
    public override List<string> RealDescriptions
    {
        get
        {
            return new List<string>()
            {
                "According to the security camera, the convict's hair is short.",
                "Its hair is short, about half inch."
            };
        }
    }

    public override List<string> FakeDescriptions
    {
        get
        {
            return new List<string>()
            {
                "Its hair is wavy.",
                "We don't have information of its head yet."
            };
        }
    }
}

public class HairCutTagMiddleScore : HaircutTag
{
    public override List<string> RealDescriptions
    {
        get
        {
            return new List<string>()
            {
                "The convict has an Asian style haircut.",
                "Its hairstyle is center parted."
            };
        }
    }

    public override List<string> FakeDescriptions
    {
        get
        {
            return new List<string>()
            {
                "It has a black, long hair.",
                "We don't have information of its head yet."
            };
        }
    }
}

public class HairCutTagPonyTail : HaircutTag
{
    public override List<string> RealDescriptions
    {
        get
        {
            return new List<string>()
            {
                "Its hair is in a high ponytail, as the security camera can tell.",
                "Its hair was tied back."
            };
        }
    }

    public override List<string> FakeDescriptions
    {
        get
        {
            return new List<string>()
            {
                "Its hair style, as the witness describes, is ridiculous.",
                "We don't have information of its head yet."
            };
        }
    }
}

public class HairCutTagLongBlackHair : HaircutTag
{
    public override List<string> RealDescriptions
    {
        get
        {
            return new List<string>()
            {
                "Its hair is long and black, as the security camera can tell.",
                "Its long hair hangs down its back."
            };
        }
    }

    public override List<string> FakeDescriptions
    {
        get
        {
            return new List<string>()
            {
                "Its hair style, as the witness describes, is ridiculous.",
                "We don't have information of its head yet."
            };
        }
    }
}



