using System.Collections.Generic;
using MikroFramework.Architecture;

namespace _02._Scripts.BodyManagmentSystem {
	public class BodyModel: AbstractSavableModel {
		[field: ES3Serializable]
		public List<BodyTimeInfo> allBodyTimeInfos { get; protected set; } = new List<BodyTimeInfo>();
		
		[ES3Serializable]
		private List<BodyTimeInfo> nextDayDeternimedBodies = new List<BodyTimeInfo>();

		public List<BodyTimeInfo> NextDayDeternimedBodies => nextDayDeternimedBodies;

		public List<BodyTimeInfo> Aliens {
			get {
				return allBodyTimeInfos.FindAll(bodyTimeInfo => bodyTimeInfo.BodyInfo.IsAlien);
			}
		}

		public List<BodyTimeInfo> Humans {
			get {
				return allBodyTimeInfos.FindAll(bodyTimeInfo => !bodyTimeInfo.BodyInfo.IsAlien);
			}
		}
		
		
		public BodyInfo GetBodyInfoByID(long id) {
			return allBodyTimeInfos.Find(bodyTimeInfo => bodyTimeInfo.BodyInfo.ID == id)?.BodyInfo;
		}
    
		public bool IsInAllBodyTimeInfos(BodyInfo bodyInfo) {
			return allBodyTimeInfos.FindIndex(bodyTimeInfo => bodyTimeInfo.BodyInfo.ID == bodyInfo.ID) != -1;
		}
		
		public void RemoveBodyInfo(BodyInfo bodyInfo) {
			allBodyTimeInfos.RemoveAll(bodyTimeInfo => {

				if (bodyTimeInfo.BodyInfo.ID == bodyInfo.ID) {
					this.SendEvent<OnBodyInfoRemoved>(new OnBodyInfoRemoved() {ID = bodyTimeInfo.BodyInfo.ID});
					return true;
				}

				return false;
			});
		}
		
		
		public void AddNewBodyTimeInfoToNextDayDeterminedBodiesQueue(BodyTimeInfo bodyTimeInfo) {
			nextDayDeternimedBodies.Add(bodyTimeInfo);
		}

		public void AddToAllBodyTimeInfos(BodyTimeInfo bodyTimeInfo) {
			allBodyTimeInfos.Add(bodyTimeInfo);
		}

	}
}