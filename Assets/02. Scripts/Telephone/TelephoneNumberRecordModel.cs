using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TelephoneNumberRecordInfo {
	public string PhoneNumber;
	public string RecordedName;
	
	public TelephoneNumberRecordInfo(string phoneNumber, string recordedName) {
		PhoneNumber = phoneNumber;
		RecordedName = recordedName;
	}
}
public class TelephoneNumberRecordModel : AbstractSavableModel {
	[ES3Serializable] private Dictionary<string, TelephoneNumberRecordInfo> _telephoneNumberRecordInfoDict =
		new Dictionary<string, TelephoneNumberRecordInfo>();
	
	public void AddOrEditRecord(string phoneNumber, string recordedName) {
		if (_telephoneNumberRecordInfoDict.ContainsKey(phoneNumber)) {
			_telephoneNumberRecordInfoDict[phoneNumber].RecordedName = recordedName;
		} else {
			_telephoneNumberRecordInfoDict.Add(phoneNumber, new TelephoneNumberRecordInfo(phoneNumber, recordedName));
		}
	}
	
	public List<TelephoneNumberRecordInfo> GetRecordList() {
		return new List<TelephoneNumberRecordInfo>(_telephoneNumberRecordInfoDict.Values);
	}
 
	public void RemoveRecord(string phoneNumber) {
		if (_telephoneNumberRecordInfoDict.ContainsKey(phoneNumber)) {
			_telephoneNumberRecordInfoDict.Remove(phoneNumber);
		}
	}
	
	
	public bool ContainsRecord(string phoneNumber) {
		return _telephoneNumberRecordInfoDict.ContainsKey(phoneNumber);
	}
}
