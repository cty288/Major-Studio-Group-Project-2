using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainBody_TeddyBear : AlienBodyPartInfo {
	public override List<IAlienTag> SelfTags { get; } = new List<IAlienTag>() {
		new AccessoryTag_TeddyBear(),
	};
	public override BodyPartType BodyPartType { get; } = BodyPartType.Accessory;
}
