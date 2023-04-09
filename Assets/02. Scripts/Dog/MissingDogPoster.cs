using System.Collections.Generic;

namespace _02._Scripts.Dog {
	public class MissingDogPoster: Poster.Poster {
		public string PhoneNumber;
		public BodyInfo MissingDogBodyInfo;
		public MissingDogPoster(string phoneNumber, BodyInfo bodyInfo) {
			this.PhoneNumber = phoneNumber;
			this.MissingDogBodyInfo = bodyInfo;
		}
		
		public MissingDogPoster(){}
		
	}
}