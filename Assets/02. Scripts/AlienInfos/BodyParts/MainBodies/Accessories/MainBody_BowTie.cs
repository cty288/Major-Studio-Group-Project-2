using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainBody_BowTie : AlienBodyPartInfo {
	public override List<IAlienTag> SelfTags { get; } = new List<IAlienTag>() {
		new AccessoryTag_BowTie(),
	};
	public override BodyPartType BodyPartType { get; } = BodyPartType.Accessory;
}
