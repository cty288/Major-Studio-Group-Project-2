using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainBody_Basketball : AlienBodyPartInfo {
	public override List<IAlienTag> SelfTags { get; } = new List<IAlienTag>() {
		new AccessoryTag_Basketball(),
	};
	public override BodyPartType BodyPartType { get; } = BodyPartType.Accessory;
}
