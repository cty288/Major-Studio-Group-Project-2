using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IClothTag : IAlienTag {
    
}

public abstract class ClothTag : AbstractAlienTag, IClothTag {
}


public class TestClothTag : ClothTag {
    public override List<string> RealDescriptions {
        get {
            return new List<string>() {"Fancy Cloth", "Very Expensive cloth"};
        }
    }

    public override List<string> FakeDescriptions {
        get {
            return new List<string>() {"Not fancy cloth", "Very cheap cloth"};
        }
    }
}

#region Body Clothes
public class ClothTagShirt : ClothTag
{
    public override List<string> RealDescriptions
    {
        get
        {
            return new List<string>() {
                "It is wearing a Shirt", 
                "It is wearing a Black Shirt"
            };
        }
    }

    public override List<string> FakeDescriptions
    {
        get
        {
            return new List<string>() {
                "It is wearing a Coat.", 
                "It is wearing a White Jacket."
            };
        }
    }
}

public class ClothTagSweater : ClothTag
{
    public override List<string> RealDescriptions
    {
        get
        {
            return new List<string>() {
                "It is wearing a Sweater.",
                "It is wearing a fluffy Sweater."
            };
        }
    }

    public override List<string> FakeDescriptions
    {
        get
        {
            return new List<string>() {
                "It is wearing a T-Shirt.",
                "It is Naked."
            };
        }
    }
}


public class ClothTagCottonVest : ClothTag
{
    public override List<string> RealDescriptions
    {
        get
        {
            return new List<string>() {
                "It is wearing a thick Vest.",
                "Its arms are exposed."
            };
        }
    }

    public override List<string> FakeDescriptions
    {
        get
        {
            return new List<string>() {
                "It is wearing a folded quilt.",
                "It is wearing a Huge cape."
            };
        }
    }
}
#endregion


#region Legs Clothes

public class ClothTagShortPants: ClothTag
{
    public override List<string> RealDescriptions
    {
        get
        {
            return new List<string>() {
                "It is wearing Short pants.", 
                "It is not long pants."
            };
        }
    }

    public override List<string> FakeDescriptions
    {
        get
        {
            return new List<string>() {
                "It is wearing a Skirt.",
                "It is wearing an Underwear."
            };
        }
    }
}

public class ClothTagLongPants : ClothTag
{
    public override List<string> RealDescriptions
    {
        get
        {
            return new List<string>() {
                "It is wearing Long pants.",
                "It is not short pants."
            };
        }
    }

    public override List<string> FakeDescriptions
    {
        get
        {
            return new List<string>() {
                "It is wearing a Skirt.",
                "It is wearing an Underwear."
            };
        }
    }
}

public class ClothTagShortSkirt : ClothTag
{
    public override List<string> RealDescriptions
    {
        get
        {
            return new List<string>() {
                "It is wearing Skirt.",
                "It just left from working in K-Pop company."
            };
        }
    }

    public override List<string> FakeDescriptions
    {
        get
        {
            return new List<string>() {
                "It is wearing a one hundred kilometers long dress.",
                "It is wearing a big sock that contains half of its body."
            };
        }
    }
}
#endregion