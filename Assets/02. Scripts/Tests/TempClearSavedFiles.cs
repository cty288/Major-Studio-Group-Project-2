using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TempClearSavedFiles : MonoBehaviour
{
	private void Awake() {
		this.GetComponent<Button>().onClick.AddListener(OnClicked);
	}

	private void OnClicked() {
		ES3.DeleteFile("models.es3");
		ES3.DeleteFile("SaveFile.es3");
	}
}
