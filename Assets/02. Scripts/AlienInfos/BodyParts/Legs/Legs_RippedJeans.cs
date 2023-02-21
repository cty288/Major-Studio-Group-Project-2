using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Legs_RippedJeans : AlienBodyPartInfo {
	public override List<IAlienTag> SelfTags { get; } = new List<IAlienTag>() {
		new LegsTag_RippedJeans(),
	};
	public override BodyPartType BodyPartType { get; } = BodyPartType.Legs;
}
