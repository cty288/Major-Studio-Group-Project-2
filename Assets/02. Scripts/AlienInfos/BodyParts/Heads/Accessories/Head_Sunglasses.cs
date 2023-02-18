using System.Collections;
using System.Collections.Generic;
using _02._Scripts.AlienInfos.Tags.Base;
using UnityEngine;

public class Head_Sunglasses : AlienBodyPartInfo {
	public override List<IAlienTag> SelfTags { get; } = new List<IAlienTag>() {
		new AccessoryTag_Sunglasses(),
		//new DistinctiveTag()
	};
	public override BodyPartType BodyPartType { get; } = BodyPartType.Accessory;
}
