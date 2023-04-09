namespace _02._Scripts.Poster {
	public class SuspectPoster: Poster {
		[field: ES3Serializable]
		public long SuspectId { get; set; }
		[field: ES3Serializable]
		public GoodsInfo RewardsInfo { get; set; }
		public SuspectPoster(long suspectID, GoodsInfo rewardsInfo) {
			this.SuspectId = suspectID;
			this.RewardsInfo = rewardsInfo;
		}
		
		public SuspectPoster(){}
	}
}