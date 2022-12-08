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
                "When the corpse was found in the farmland by the next morning, the body was necked and exposed. The victim's family reported that a  <color=yellow>blue T-shirt and pants</color> were worn by the time he left home.",
                "According to information provided by the family, the victim was wearing a  <color=yellow>shirt</color> before disappearing."
            };
        }
    }

    public override List<string> FakeDescriptions
    {
        get
        {
            return new List<string>() {
                "The victim's <color=yellow>dress</color> was taken way.", 
                "Police have not been able to find any information relating to the victim's top clothing."
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
                "We still don't know what's under the camouflage, <color=yellow>a cream-white sweater</color> that was worn by the victim.",
                "We found black wool from where the victim was killed, and investigators speculate the victim was wearing a <color=yellow>black sweater</color>."
            };
        }
    }

    public override List<string> FakeDescriptions
    {
        get
        {
            return new List<string>() {
                "For some reasons, the creature took all the outwears from the victim, including a <color=yellow>grey wool coat and a cotton jacket</color>.",
                "We have not been able to find any information relating to the victim's top clothing."
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
                "For some reasons, the creature took all the outwears from the victim, including a <color=yellow>grey wool coat and a cotton jacket</color>.",
                "A friend of the victim provided us with photos showing the man wearing a <color=yellow>red cotton vest</color> before he was killed."
            };
        }
    }

    public override List<string> FakeDescriptions
    {
        get
        {
            return new List<string>() {
                "According to the witness, the creature was wearing a <color=yellow>yellow scarf</color>.",
                "We have not been able to find any information relating to the victim's top clothing."
            };
        }
    }
}

public class ClothTagLongDress : ClothTag
{
    public override List<string> RealDescriptions
    {
        get
        {
            return new List<string>() {
                "The police suspects that this creature is sensitive to chill, as it took away all of the victim's outwear.",
                "Some reports showed that the victim's <color=yellow>trench coat</color> was taken by a mysterious suspect.",
            };
        }
    }

    public override List<string> FakeDescriptions
    {
        get
        {
            return new List<string>() {
                "According to the witness, the creature was wearing a <color=yellow>blue scarf</color>.",
                "For now, we have not been able to find any information relating to the victim's clothing. We will keep searching."
            };
        }
    }
}

public class ClothTagWatch : ClothTag
{
    public override List<string> RealDescriptions
    {
        get
        {
            return new List<string>() {
                "The victim's wife reported that their 15th year anniversary gift -- <color=yellow>a brand-new stainless steal watch</color> -- was also disappeared in the crime scene.",
                "The victim was wearing <color=yellow>a watch</color> when he left home, but it was missing when the police arrived at the crime scene."
            };
        }
    }

    public override List<string> FakeDescriptions
    {
        get
        {
            return new List<string>() {
                "When the corpse was found in the farmland by the next morning, the body was necked and exposed. The victim's family reported that a <color=yellow>blue T-shirt and pants</color> were worn by the time he left home.",
                "The victim's <color=yellow>hand was brutally cut off</color>. We are obtaining additional information."
            };
        }
    }
}

public class ClothTagScarf : ClothTag
{
    public override List<string> RealDescriptions
    {
        get
        {
            return new List<string>() {
                "According to the witness, the victim was wearing <color=yellow>a yellow scarf</color>.",
                "According to the investigation, the victim wore <color=yellow>a blue scarf</color> when he left home. However, the scarf was missing when the police arrived at the crime scene.",
                
            };
        }
    }

    public override List<string> FakeDescriptions
    {
        get
        {
            return new List<string>() {
                "For some reasons, the creature took all the outwears from the victim, including <color=yellow>a grey wool coat and a cotton jacket</color>.",
                "We have not been able to find any information relating to the victim's top clothing."
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
                "Possibly wearing <color=yellow>the shorts</color> took away from the victim, the creature is by still now unaccounted for.",
                "<color=yellow>A short pants</color> was found on the victim's body, but it was not the one he was wearing when he left home.",
                "By witness's report, the victim's <color=yellow>short pants</color> was taken away.",
            };
        }
    }

    public override List<string> FakeDescriptions
    {
        get
        {
            return new List<string>() {
                "According to the witness, the creature was wearing <color=yellow>long yellow pants</color>.",
                "We have not been able to find any information relating to the victim's bottom clothing."
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
                "The witness saw it ran away along the 4th Avenue, but we have no clue where it is now. It was confirmed that it was last seen in <color=yellow>a white shirt and black pants</color>.",
                "The victim's <color=yellow>long trousers</color>, which color is black, was disappeared in the crime scene.",
            };
        }
    }

    public override List<string> FakeDescriptions
    {
        get
        {
            return new List<string>() {
                "We still don't know what's under the camouflage, <color=yellow>a cream-white sweater</color> that was worn by the victim.",
                "We have not been able to find any information relating to the victim's bottom clothing."
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
                "The victim's <color=yellow>skirt</color> was taken way.",
                "Surveillance showed that the victim was wearing <color=yellow>a short skirt</color> for a period of time before being killed."
            };
        }
    }

    public override List<string> FakeDescriptions
    {
        get
        {
            return new List<string>() {
                "The victim's wife reported that their 15th year anniversary gift -- <color=yellow>a brand-new stainless steal watch</color> -- was also unfound in the crime scene.",
                "We have not been able to find any information relating to the victim's bottom clothing."
            };
        }
    }
}

#endregion