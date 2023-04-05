using System;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.BindableProperty;
using NHibernate.Mapping;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

[ES3Serializable]
public class BodyPartIndicesUpdateInfo {
	public Dictionary<BodyPartType, HashSet<int>> AvailableBodyPartIndices;
	public int BodyPartCount;
	public DateTime Time;
}

namespace _02._Scripts.BodyManagmentSystem {
	public class BodyModel: AbstractSavableModel {
		[field: ES3Serializable]
		public List<BodyTimeInfo> allBodyTimeInfos { get; protected set; } = new List<BodyTimeInfo>();
		
		[ES3Serializable]
		private List<BodyTimeInfo> nextDayDeternimedBodies = new List<BodyTimeInfo>();
		
		[field: ES3Serializable]
		private List<BodyInfo> managedBodyInfos { get; set; } = new List<BodyInfo>();

		[ES3Serializable] private Dictionary<BodyPartType, Dictionary<int, BindableProperty<int>>>
			availableBodyPartIndices = null;


		[field: ES3Serializable] public int AvailableBodyPartIndexCount { get; protected set; } =6;
		
		
		

		public Dictionary<BodyPartType, HashSet<int>> AvailableBodyPartIndices {
			get {
				if (availableBodyPartIndices == null) {
					UpdateAvailableBodyPartIndices(AvailableBodyPartIndexCount);
				}
				Dictionary<BodyPartType, HashSet<int>> result = new Dictionary<BodyPartType, HashSet<int>>();
				foreach (var bodyPartType in availableBodyPartIndices.Keys) {
					result.Add(bodyPartType, new HashSet<int>());
					foreach (var index in availableBodyPartIndices[bodyPartType].Keys) {
						if (availableBodyPartIndices[bodyPartType][index] > 0) {
							result[bodyPartType].Add(index);
						}
					}
				}
				return result;
			}
		}

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
		
		public List<BodyTimeInfo> AllTodayDeadBodies {
			get {
				return allBodyTimeInfos.FindAll(bodyTimeInfo => bodyTimeInfo.IsTodayDead);
			}
		}

		public void UpdateAvailableBodyPartIndices(int count) {
			if(availableBodyPartIndices == null) {
				availableBodyPartIndices = new Dictionary<BodyPartType, Dictionary<int, BindableProperty<int>>>();
				availableBodyPartIndices.Add(BodyPartType.Head, new Dictionary<int, BindableProperty<int>>());
				availableBodyPartIndices.Add(BodyPartType.Body, new Dictionary<int, BindableProperty<int>>());
			}

			Dictionary<BodyPartType, List<int>> removedIndices = new Dictionary<BodyPartType, List<int>>();
			foreach (BodyPartType bodyPartType in availableBodyPartIndices.Keys) {
				
				foreach (int index in availableBodyPartIndices[bodyPartType].Keys) {
					availableBodyPartIndices[bodyPartType][index].Value--;
					if (availableBodyPartIndices[bodyPartType][index] <= 0) {

						if (!removedIndices.ContainsKey(bodyPartType)) {
							removedIndices.Add(bodyPartType, new List<int>());
						}
						removedIndices[bodyPartType].Add(index);
					}
				}
			}
			
			foreach (BodyPartType bodyPartType in removedIndices.Keys) {
				foreach (int index in removedIndices[bodyPartType]) {
					availableBodyPartIndices[bodyPartType].Remove(index);
				}
			}

			AvailableBodyPartIndexCount = count;

			foreach (BodyPartType bodyPartType in availableBodyPartIndices.Keys) {
				var targetList = AlienBodyPartCollections.Singleton.GetBodyPartCollectionByBodyType(bodyPartType, false)
					.HeightSubCollections[0].NewspaperBodyPartDisplays.HumanTraitPartsPrefabs;
				
				int actualCount = Mathf.Min(count, targetList.Count);
				
				while (availableBodyPartIndices[bodyPartType].Count < actualCount) {
					int randomIndex = Random.Range(0, targetList.Count);
					if (!availableBodyPartIndices[bodyPartType].ContainsKey(randomIndex)) {
						availableBodyPartIndices[bodyPartType].Add(randomIndex, new BindableProperty<int>(Random.Range(1, 3)));
					}
				}
			}
			
			this.SendEvent<BodyPartIndicesUpdateInfo>(new BodyPartIndicesUpdateInfo(){
				AvailableBodyPartIndices = AvailableBodyPartIndices,
				BodyPartCount = AvailableBodyPartIndexCount
			});
			
		}
		
		
		
		public BodyInfo GetBodyInfoByID(long id) {
			
			BodyInfo result = allBodyTimeInfos.Find(bodyTimeInfo => bodyTimeInfo.BodyInfo.ID == id)?.BodyInfo;
			if(result == null) {
				result = managedBodyInfos.Find(bodyInfo => bodyInfo.ID == id);
			}

			return result;
		}
		
		public void AddToManagedBodyInfos(BodyInfo bodyInfo) {
			managedBodyInfos.Add(bodyInfo);
		}
    
		public bool IsInAllBodyTimeInfos(BodyInfo bodyInfo) {
			return allBodyTimeInfos.FindIndex(bodyTimeInfo => bodyTimeInfo.BodyInfo.ID == bodyInfo.ID) != -1;
		}

		public void KillBodyInfo(BodyInfo bodyInfo) {
			foreach (BodyTimeInfo bodyTimeInfo in allBodyTimeInfos) {
				if (bodyTimeInfo.BodyInfo.ID == bodyInfo.ID) {
					this.SendEvent<OnBodyInfoKilled>(new OnBodyInfoKilled() {ID = bodyTimeInfo.BodyInfo.ID});
					break;
				}
			}

			foreach (BodyInfo info in managedBodyInfos) {
				if (info.ID == bodyInfo.ID) {
					this.SendEvent<OnBodyInfoKilled>(new OnBodyInfoKilled() {ID = info.ID});
					break;
				}
			}
			
			RemoveTimedBodyInfo(bodyInfo);
			
		}
		
		private void RemoveTimedBodyInfo(BodyInfo bodyInfo) {
			allBodyTimeInfos.RemoveAll(bodyTimeInfo => {

				if (bodyTimeInfo.BodyInfo.ID == bodyInfo.ID) {
					//this.SendEvent<OnBodyInfoKilled>(new OnBodyInfoKilled() {ID = bodyTimeInfo.BodyInfo.ID});
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