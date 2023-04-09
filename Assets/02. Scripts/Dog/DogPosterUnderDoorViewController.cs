using System;
using _02._Scripts.GameEvents.Dog;
using _02._Scripts.Poster;
using MikroFramework.Architecture;
using MikroFramework.Event;
using UnityEngine.EventSystems;

namespace _02._Scripts.Dog {
	public class DogPosterUnderDoorViewController: AbstractMikroController<MainGame>, IPointerClickHandler {
		private void Awake() {
			this.RegisterEvent<OnDogPosterUnderDoorDelivered>(OnDogPosterUnderDoorDelivered)
				.UnRegisterWhenGameObjectDestroyed(gameObject);
			gameObject.SetActive(false);
		}

		private void OnDogPosterUnderDoorDelivered(OnDogPosterUnderDoorDelivered obj) {
			gameObject.SetActive(true);
		}

		public void OnPointerClick(PointerEventData eventData) {
			DogModel dogModel = this.GetModel<DogModel>();
			this.GetModel<PosterModel>()
				.AddPoster(new MissingDogPoster(dogModel.MissingDogPhoneNumber, dogModel.MissingDogBodyInfo));
			gameObject.SetActive(false);
			
		}
	}
}