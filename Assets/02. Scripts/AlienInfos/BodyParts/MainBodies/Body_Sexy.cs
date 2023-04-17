using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body_Sexy : AlienBodyPartInfo {
	public override List<IAlienTag> SelfTags { get; } = new List<IAlienTag>() {
		new MainBodyTag_SP_Sexy()
	};
	public override BodyPartType BodyPartType { get; } = BodyPartType.Body;
}
