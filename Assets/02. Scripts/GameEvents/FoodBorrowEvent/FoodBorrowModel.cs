namespace _02._Scripts.GameEvents.FoodBorrowEvent {
	public class FoodBorrowModel: AbstractSavableModel {
		[ES3Serializable] private long currentScammerId;
		public long CurrentScammerId {
			get => currentScammerId;
			set => currentScammerId = value;
		}
		[ES3Serializable] private long currenNnonScammerId;

		public long CurrentNonScammerId {
			get => currenNnonScammerId;
			set => currenNnonScammerId = value;
		}
	}
}