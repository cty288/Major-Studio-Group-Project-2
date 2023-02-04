using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHaircutTag : IAlienTag
{

}

public abstract class HaircutTag : AbstractAlienTag, IHaircutTag
{
}


public class HairCutTagHorseShit : HaircutTag {
    public override string TagName { get; } = "Hair_Messy";
}

public class HairCutTagBuzzCut : HaircutTag { 
    public override string TagName { get; } = "Hair_BuzzCut";
}

public class HairCutTagMiddleScore : HaircutTag { 
    public override string TagName { get; } = "Hair_MiddlePart";
}

public class HairCutTagPonyTail : HaircutTag
{
    public override string TagName { get; } = "Hair_PonyTail";
}

public class HairCutTagLongBlackHair : HaircutTag
{
    public override string TagName { get; } = "Hair_Long";
}



