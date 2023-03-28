using System;
using System.Collections.Generic;
using MikroFramework.Architecture;

namespace _02._Scripts.Poster {
	public abstract class Poster {
		[field: ES3Serializable]
		public string ID { get; set; }
	}

	public struct OnPosterGet {
		public string ID;
	}
	public class PosterModel: AbstractSavableModel {
		[ES3Serializable]
		private Dictionary<string, Poster> posters = new Dictionary<string, Poster>();
		
		
		public string AddPoster(Poster poster) {
			poster.ID = Guid.NewGuid().ToString();
			posters.Add(poster.ID, poster);
			this.SendEvent<OnPosterGet>(new OnPosterGet() {ID = poster.ID});
			return poster.ID;
		}
		
		public void RemovePoster(string id) {
			if(!posters.ContainsKey(id)) {
				return;
			}
			posters.Remove(id);
		}
		
		public Poster GetPoster(string id) {
			if (posters.ContainsKey(id)) {
				return posters[id];
			}
			return null;
		}
	}
}