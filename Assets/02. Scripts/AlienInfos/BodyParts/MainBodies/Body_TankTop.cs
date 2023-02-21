using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body_TankTop : AlienBodyPartInfo {
	public override List<IAlienTag> SelfTags { get; } = new List<IAlienTag>() {
		new MainBodyTag_TankTop()
	};
	public override BodyPartType BodyPartType { get; } = BodyPartType.Body;
}
