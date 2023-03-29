using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHaircutTag : IAlienTag
{

}
[ES3Serializable]
public abstract class HaircutTag : AbstractAlienTag, IHaircutTag {
    public override List<string> GetFakeRadioDescription() {
        return bodyTagInfoModel.GetFakeRadioDescription(TagName,
            description => typeof(IHaircutTag).IsAssignableFrom(description.TagType));
    }

}


public class HairCutTagHorseShit : HaircutTag {
    [field: ES3Serializable]
    public override string TagName { get; } = "Hair_Messy";
}

public class HairCutTagBuzzCut : HaircutTag { 
    [field: ES3Serializable]
    public override string TagName { get; } = "Hair_BuzzCut";
}

public class HairCutTagMiddleScore : HaircutTag { 
    [field: ES3Serializable]
    public override string TagName { get; } = "Hair_MiddlePart";
}

public class HairCutTagPonyTail : HaircutTag
{
    [field: ES3Serializable]
    public override string TagName { get; } = "Hair_PonyTail";
}

public class HairCutTagLongBlackHair : HaircutTag {
    [field: ES3Serializable]
    public override string TagName { get; } = "Hair_Long";
}



