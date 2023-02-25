using System.Collections;
using System.Collections.Generic;
using _02._Scripts.AlienInfos.Tags.Base;
using UnityEngine;

public class Head_Bleachwaves : HeadBodyPartInfo {
	public override List<IAlienTag> SelfTags { get; } = new List<IAlienTag>() {
		new HairTag_Bleachwaves(),
		new DistinctiveTag()
	};
	public override BodyPartType BodyPartType { get; } = BodyPartType.Head;
}