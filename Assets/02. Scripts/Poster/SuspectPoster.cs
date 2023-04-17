namespace _02._Scripts.Poster {
	public class SuspectPoster: Poster {
		[field: ES3Serializable]
		public long SuspectId { get; set; }
		[field: ES3Serializable]
		public SuspectInfo SuspectInfo { get; set; }
		public SuspectPoster(long suspectID, SuspectInfo suspectInfo) {
			this.SuspectId = suspectID;
			this.SuspectInfo = suspectInfo;
		}
		
		public SuspectPoster(){}
	}
}