using System.Collections.Generic;

namespace _02._Scripts.Dog {
	public class MissingDogImportantNewsContent: IImportantNewspaperPageContent {
		public string PhoneNumber;
		public BodyInfo MissingDogBodyInfo;
		public MissingDogImportantNewsContent(string phoneNumber, BodyInfo bodyInfo) {
			this.PhoneNumber = phoneNumber;
			this.MissingDogBodyInfo = bodyInfo;
		}
		
		public MissingDogImportantNewsContent(){}
		
		public List<IImportantNewspaperPageContent> GetPages() {
			return new List<IImportantNewspaperPageContent>() {this};
		}
	}
}