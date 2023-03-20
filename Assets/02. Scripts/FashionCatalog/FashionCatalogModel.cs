﻿using System;
using System.Collections.Generic;
using MikroFramework.Architecture;

namespace _02._Scripts.FashionCatalog {
	public struct OnFashionCatalogGenerated {
		public BodyPartIndicesUpdateInfo BodyPartIndicesUpdateInfo;
		public int Week;
	}
	public class FashionCatalogModel: AbstractSavableModel {
		[ES3Serializable] private Dictionary<DateTime, BodyPartIndicesUpdateInfo> _bodyPartIndicesUpdateInfo =
			new Dictionary<DateTime, BodyPartIndicesUpdateInfo>();

		[ES3Serializable] private int week = 0;
		public void AddBodyPartIndicesUpdateInfo(BodyPartIndicesUpdateInfo bodyPartIndicesUpdateInfo) {
			_bodyPartIndicesUpdateInfo.Add(bodyPartIndicesUpdateInfo.Time, bodyPartIndicesUpdateInfo);
			week++;
			this.SendEvent<OnFashionCatalogGenerated>(new OnFashionCatalogGenerated() {
				BodyPartIndicesUpdateInfo = bodyPartIndicesUpdateInfo,
				Week = week
			});
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