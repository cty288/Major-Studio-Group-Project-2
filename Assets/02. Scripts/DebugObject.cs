using System;
using UnityEngine;

namespace _02._Scripts {
	public class DebugObject : MonoBehaviour{
		private void Awake() {
			gameObject.SetActive(Application.isEditor);
		}
	}
	
}