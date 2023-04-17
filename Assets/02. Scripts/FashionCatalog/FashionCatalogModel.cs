using System;
using System.Collections.Generic;
using MikroFramework.Architecture;

namespace _02._Scripts.FashionCatalog {
	public struct OnFashionCatalogGenerated {
		public BodyPartIndicesUpdateInfo BodyPartIndicesUpdateInfo;
		public int CurrentSpriteIndex;
	}
	public class FashionCatalogModel: AbstractSavableModel {
		[ES3Serializable] private Dictionary<DateTime, BodyPartIndicesUpdateInfo> _bodyPartIndicesUpdateInfo =
			new Dictionary<DateTime, BodyPartIndicesUpdateInfo>();

		[ES3Serializable] private int currentFashionCatalogIndex = 0;
		public void AddBodyPartIndicesUpdateInfo(BodyPartIndicesUpdateInfo bodyPartIndicesUpdateInfo) {
			if (_bodyPartIndicesUpdateInfo.ContainsKey(bodyPartIndicesUpdateInfo.Time)) {
				return;
			}
			_bodyPartIndicesUpdateInfo.Add(bodyPartIndicesUpdateInfo.Time, bodyPartIndicesUpdateInfo);
			
			this.SendEvent<OnFashionCatalogGenerated>(new OnFashionCatalogGenerated() {
				BodyPartIndicesUpdateInfo = bodyPartIndicesUpdateInfo,
				CurrentSpriteIndex = currentFashionCatalogIndex
			});
		}

		public void UpdateSpriteIndex() {
			int listLength = FashionCatalogAssets.Singleton.GetSpriteCount();
			currentFashionCatalogIndex = (currentFashionCatalogIndex + 1) % listLength;
		}
		
		public BodyPartIndicesUpdateInfo GetBodyPartIndicesUpdateInfo(DateTime time) {
			if (_bodyPartIndicesUpdateInfo.ContainsKey(time)) {
				return _bodyPartIndicesUpdateInfo[time];
			}
			return null;
		}
		
		public void RemoveBodyPartIndicesUpdateInfo(DateTime time) {
			if (_bodyPartIndicesUpdateInfo.ContainsKey(time)) {
				_bodyPartIndicesUpdateInfo.Remove(time);
			}
		}
	}
}