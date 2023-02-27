using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Head_GreyMiddlePart : HeadBodyPartInfo {
	public override List<IAlienTag> SelfTags { get; } = new List<IAlienTag>() {
		new HairTag_GreyMiddlePart()
	};

	public override BodyPartType BodyPartType { get; } = BodyPartType.Head;
}
