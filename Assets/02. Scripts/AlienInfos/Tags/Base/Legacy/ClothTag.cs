using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IClothTag : IAlienTag {
    
}

public abstract class ClothTag : AbstractAlienTag, IClothTag {
}




#region Body Clothes
public class ClothTagShirt : ClothTag {
    [field: ES3Serializable]
    public override string TagName { get; } = "Cloth_Shirt";
}

public class ClothTagSweater : ClothTag {
    [field: ES3Serializable]
    public override string TagName { get; } = "Cloth_Sweater";
}


public class ClothTagCottonVest : ClothTag {
    [field: ES3Serializable]
    public override string TagName { get; } = "Cloth_CottonVest";
}

public class ClothTagLongDress : ClothTag {
    [field: ES3Serializable]
    public override string TagName { get; } = "Cloth_LongDress";
}

public class ClothTagWatch : ClothTag {
    [field: ES3Serializable]
    public override string TagName { get; } = "Watch";
}


#endregion


#region Legs Clothes

public class ClothTagShortPants: ClothTag {
    public override string TagName { get; } = "Cloth_ShortPants";
}

public class ClothTagLongPants : ClothTag { 
    public override string TagName { get; } = "Cloth_LongPants";
}

public class ClothTagShortSkirt : ClothTag
{
    public override string TagName { get; } =  "Cloth_ShortSkirt";
}

#endregion