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
            return new List<string>() { "Shirt", "Black Shirt" };
        }
    }

    public override List<string> FakeDescriptions
    {
        get
        {
            return new List<string>() { "Coat", "White Jacket" };
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
            return new List<string>() { "Short pants", "Not long pants" };
        }
    }

    public override List<string> FakeDescriptions
    {
        get
        {
            return new List<string>() { "Skirt", "Underwear" };
        }
    }
}
#endregion