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
                "Its hair style, as the witness describes, is <color=yellow>ridiculous</color>.",
                "Its hair is very <color=yellow>messy</color>."
            };
        }
    }

    public override List<string> FakeDescriptions
    {
        get
        {
            return new List<string>()
            {
                "It wore its hair in a <color=yellow>high ponytail</color>, as the security camera can tell.",
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
                "According to the security camera, the convict's hair is <color=yellow>short</color>.",
                "Its hair is <color=yellow>short, about half inch</color>."
            };
        }
    }

    public override List<string> FakeDescriptions
    {
        get
        {
            return new List<string>()
            {
                "Its hair is <color=yellow>wavy</color>.",
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
                "The convict has a <color=yellow>middle part haircut</color>.",
                "Its hairstyle is <color=yellow>center parted</color>."
            };
        }
    }

    public override List<string> FakeDescriptions
    {
        get
        {
            return new List<string>()
            {
                "It has a <color=yellow>black, long hair</color>.",
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
                "Its hair is in <color=yellow>a high ponytail</color>, as the security camera can tell.",
                "Its hair was <color=yellow>tied back<color>."
            };
        }
    }

    public override List<string> FakeDescriptions
    {
        get
        {
            return new List<string>()
            {
                "Its hair style, as the witness describes, is <color=yellow>ridiculous</color>.",
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
                "Its hair is <color=yellow>long and black</color>, as the security camera can tell.",
                "Its <color=yellow>long hair</color> hangs down its back."
            };
        }
    }

    public override List<string> FakeDescriptions
    {
        get
        {
            return new List<string>()
            {
                "Its hair style, as the witness describes, is <color=yellow>ridiculous</color>.",
                "We don't have information of its head yet."
            };
        }
    }
}



