public interface IAccessoryTag : IAlienTag {
    
}

public abstract class AccessoryTag : AbstractAlienTag, IClothTag {
}

//Head
public class AccessoryTag_Sunglasses : AccessoryTag {
	public override string TagName { get; } = "Headwear_Sunglasses";
}


public class AccessoryTag_Beret : AccessoryTag {
	public override string TagName { get; } = "Headwear_Beret";
}

public class AccessoryTag_Hairclip : AccessoryTag
{
	public override string TagName { get; } = "Headwear_Hairclip";
}

public class AccessoryTag_TrapperHat : AccessoryTag
{
	public override string TagName { get; } = "Headwear_TrapperHat";
}

public class AccessoryTag_Facemask : AccessoryTag
{
	public override string TagName { get; } = "Headwear_Facemask";
}

public class AccessoryTag_Bandana : AccessoryTag
{
	public override string TagName { get; } = "Headwear_Bandana";
}

public class AccessoryTag_Tallhat : AccessoryTag
{
	public override string TagName { get; } = "Headwear_Tallhat";
}

public class AccessoryTag_Tiara : AccessoryTag
{
	public override string TagName { get; } = "Headwear_Tiara";
}

public class AccessoryTag_DetectiveHat : AccessoryTag
{
	public override string TagName { get; } = "Headwear_DetectiveHat";
}


//Body
public class ClothTagScarf : AccessoryTag {
	
	public override string TagName { get; } = "Cloth_Scarf";
}

public class AccessoryTag_Necklace : AccessoryTag
{
	public override string TagName { get; } = "Bodywear_Necklace";
}

public class AccessoryTag_TeddyBear : AccessoryTag {
	
	public override string TagName { get; } = "Bodywear_Bear";
}



