using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IOrnamentTag : IAlienTag
{

}

public abstract class OrnamentTag : AbstractAlienTag, IOrnamentTag
{

}

#region Head Ornaments


#endregion


#region Body Ornaments

public class OrnamentTagWatch : OrnamentTag
{
    public override List<string> RealDescriptions
    {
        get
        {
            return new List<string>() {
                "A watch is on his arm.",
            };
        }
    }

    public override List<string> FakeDescriptions
    {
        get
        {
            return new List<string>() {
                "It is wearing gloves.",
            };
        }
    }
}

#endregion


#region Legs Ornaments

#endregion
