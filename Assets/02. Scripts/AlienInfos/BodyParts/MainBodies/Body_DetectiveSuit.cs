using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body_DetectiveSuit : AlienBodyPartInfo {
	public override List<IAlienTag> SelfTags { get; } = new List<IAlienTag>() {
		new MainBodyTag_DetectiveSuit()
	};
	public override BodyPartType BodyPartType { get; } = BodyPartType.Body;
}
