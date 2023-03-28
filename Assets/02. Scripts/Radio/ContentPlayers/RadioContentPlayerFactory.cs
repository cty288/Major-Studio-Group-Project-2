using System.Collections.Generic;
using Crosstales;
using MikroFramework.Singletons;
using UnityEngine;

namespace _02._Scripts.Radio {
	public class RadioContentPlayerFactory : MonoMikroSingleton<RadioContentPlayerFactory> {
		[SerializeField]
		private List<GameObject> RadioContentPlayerPrefabs;
		
		[SerializeField]
		private List<AudioClip> musicSources;

		private List<int> availableIndices = new List<int>();
		public int GetMusicSourceIndex() {
			if(availableIndices.Count == 0) {
				for (int i = 0; i < musicSources.Count; i++) {
					availableIndices.Add(i);
				}
				availableIndices.CTShuffle();
			}
			
			int index = availableIndices[0];
			availableIndices.RemoveAt(0);
			return index;
		}
		
		public AudioClip GetMusicSource(int index) {
			return musicSources[index];
		}
		
		public GameObject SpawnPlayerPrefabByType(RadioContentType type, Transform parent) {
			int index = (int) type;
			GameObject prefab = RadioContentPlayerPrefabs[index];
			GameObject player = Instantiate(prefab, parent);
			return player;
		}
	}
}