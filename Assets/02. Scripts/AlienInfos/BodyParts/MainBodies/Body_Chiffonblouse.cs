using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body_Chiffonblouse : AlienBodyPartInfo {
	public override List<IAlienTag> SelfTags { get; } = new List<IAlienTag>() {
		new MainBodyTag_Chiffonblouse()
	};
	public override BodyPartType BodyPartType { get; } = BodyPartType.Body;
}
