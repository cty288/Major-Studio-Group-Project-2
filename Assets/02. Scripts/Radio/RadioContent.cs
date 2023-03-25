
using Crosstales.RTVoice.Model.Enum;
using UnityEngine.Audio;

public enum RadioContentType {
	Text
}
public interface IRadioContent {
	public RadioContentType ContentType { get; }
	
	public void SetContent(object content);
	
}


public class RadioTextContent : IRadioContent {
	[field: ES3Serializable]
	public string speakContent;
	[field: ES3Serializable]
	public float speakRate;
	[field: ES3Serializable]
	public Gender speakGender;
	[field: ES3Serializable]
	public AudioMixerGroup mixer;
	[field: ES3Serializable]
	public RadioContentType ContentType { get; } = RadioContentType.Text;

	public void SetContent(object content) {
		speakContent = (string) content;
	}

	public RadioTextContent(string speakContent, float speakRate, Gender speakGender, AudioMixerGroup mixer) {
		this.speakContent = speakContent;
		this.speakRate = speakRate;
		this.speakGender = speakGender;
		this.mixer = mixer;
	}
	
	public RadioTextContent() {
	}
	
}
