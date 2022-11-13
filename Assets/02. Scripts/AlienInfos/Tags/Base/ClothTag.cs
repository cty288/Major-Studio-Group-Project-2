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
#endregion