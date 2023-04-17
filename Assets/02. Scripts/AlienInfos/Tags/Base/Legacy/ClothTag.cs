using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IClothTag : IAlienTag {
    public bool IsUpperCloth { get; }
}

public abstract class ClothTag : AbstractAlienTag, IClothTag {
    public override List<string> GetFakeRadioDescription(string alienName) {
        return bodyTagInfoModel.GetFakeRadioDescription(TagName,
            description => typeof(IClothTag).IsAssignableFrom(description.TagType) && description.IsUpperBody == IsUpperCloth,
            alienName);
    }

    public abstract bool IsUpperCloth { get; }
}




#region Body Clothes
public class ClothTagShirt : ClothTag {
    [field: ES3Serializable]
    public override string TagName { get; } = "Cloth_Shirt";

    [field: ES3Serializable]
    public override bool IsUpperCloth { get; } = true;
}

public class ClothTagSweater : ClothTag {
    [field: ES3Serializable]
    public override string TagName { get; } = "Cloth_Sweater";
    [field: ES3Serializable]
    public override bool IsUpperCloth { get; } = true;
}


public class ClothTagCottonVest : ClothTag {
    [field: ES3Serializable]
    public override string TagName { get; } = "Cloth_CottonVest";
    [field: ES3Serializable]
    public override bool IsUpperCloth { get; } = true;
}

public class ClothTagLongDress : ClothTag {
    [field: ES3Serializable]
    public override string TagName { get; } = "Cloth_LongDress";
    [field: ES3Serializable]
    public override bool IsUpperCloth { get; } = true;
}

public class ClothTagWatch : ClothTag {
    [field: ES3Serializable]
    public override string TagName { get; } = "Watch";
    
    [field: ES3Serializable]
    public override bool IsUpperCloth { get; } = true;
}


#endregion


#region Legs Clothes

public class ClothTagShortPants: ClothTag {
    public override string TagName { get; } = "Cloth_ShortPants";
    
    [field: ES3Serializable]
    public override bool IsUpperCloth { get; } = false;
}

public class ClothTagLongPants : ClothTag { 
    public override string TagName { get; } = "Cloth_LongPants";
    
    [field: ES3Serializable]
    public override bool IsUpperCloth { get; } = false;
}

public class ClothTagShortSkirt : ClothTag
{
    public override string TagName { get; } =  "Cloth_ShortSkirt";
    
    [field: ES3Serializable]
    public override bool IsUpperCloth { get; } = false;
}

#endregion