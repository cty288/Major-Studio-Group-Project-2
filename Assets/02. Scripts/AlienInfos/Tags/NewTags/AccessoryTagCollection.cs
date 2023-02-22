﻿public interface IAccessoryTag : IAlienTag {
    
}

public abstract class AccessoryTag : AbstractAlienTag, IClothTag {
}


public class AccessoryTag_Sunglasses : AccessoryTag {
	public override string TagName { get; } = "Headwear_Sunglasses";
}

public class AccessoryTag_Necklace : AccessoryTag {
	public override string TagName { get; } = "Bodywear_Necklace";
}

public class AccessoryTag_Beret : AccessoryTag {
	public override string TagName { get; } = "Headwear_Beret";
}

public class ClothTagScarf : AccessoryTag {
	
	public override string TagName { get; } = "Cloth_Scarf";
}

public class AccessoryTag_TeddyBear : AccessoryTag {
	
	public override string TagName { get; } = "Bodywear_Bear";
}


