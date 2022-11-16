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
                "When the corpse was found in the farmland by the next morning, the body was necked and exposed. The victim's family reported that a blue T-shirt and pants were worn by the time he left home."
            };
        }
    }

    public override List<string> FakeDescriptions
    {
        get
        {
            return new List<string>() {
                "The victim's skirt was taken way.", 
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
                "We still don't know what's under the camouflage, a cream-white sweater that was worn by the victim."
            };
        }
    }

    public override List<string> FakeDescriptions
    {
        get
        {
            return new List<string>() {
                "For some reasons, the creature took all the outwears from the victim, including a grey wool coat and a cotton jacket."
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
                "For some reasons, the creature took all the outwears from the victim, including a grey wool coat and a cotton jacket."
            };
        }
    }

    public override List<string> FakeDescriptions
    {
        get
        {
            return new List<string>() {
                "According to the witness, the creature was wearing a yellow scarf."
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
            };
        }
    }

    public override List<string> FakeDescriptions
    {
        get
        {
            return new List<string>() {
                "According to the witness, the creature was wearing a blue scarf.",
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
                "The victim's wife reported that their 15th year anniversary gift -- a brand-new stainless steal watch -- was also unfound in the crime scene.",
            };
        }
    }

    public override List<string> FakeDescriptions
    {
        get
        {
            return new List<string>() {
                "When the corpse was found in the farmland by the next morning, the body was necked and exposed. The victim's family reported that a blue T-shirt and pants were worn by the time he left home.",
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
                "According to the witness, the creature was wearing a yellow scarf.",
            };
        }
    }

    public override List<string> FakeDescriptions
    {
        get
        {
            return new List<string>() {
                "For some reasons, the creature took all the outwears from the victim, including a grey wool coat and a cotton jacket.",
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
                "Possibly wearing the shorts took away from the victim, the creature is by still now unaccounted for.", 
            };
        }
    }

    public override List<string> FakeDescriptions
    {
        get
        {
            return new List<string>() {
                "According to the witness, the creature was wearing a yellow underwear.",
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
                "The witness saw it ran away along the 4th Avenue, but we have no clue where it is now. It was confirmed that it was last seen in a white shirt and black pants.",
            };
        }
    }

    public override List<string> FakeDescriptions
    {
        get
        {
            return new List<string>() {
                "We still don't know what's under the camouflage, a cream-white sweater that was worn by the victim.",
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
                "The victim's skirt was taken way.",
            };
        }
    }

    public override List<string> FakeDescriptions
    {
        get
        {
            return new List<string>() {
                "The victim's wife reported that their 15th year anniversary gift -- a brand-new stainless steal watch -- was also unfound in the crime scene.",
            };
        }
    }
}

#endregion