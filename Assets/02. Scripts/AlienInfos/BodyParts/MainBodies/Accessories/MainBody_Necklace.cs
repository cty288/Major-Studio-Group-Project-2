using System.Collections;
using System.Collections.Generic;
using _02._Scripts.AlienInfos.Tags.Base;
using UnityEngine;

public class MainBody_Necklace : AlienBodyPartInfo {
	public override List<IAlienTag> SelfTags { get; } = new List<IAlienTag>() {
		//new DistinctiveTag(),
		new AccessoryTag_Necklace()
	};
	public override BodyPartType BodyPartType { get; }
}
