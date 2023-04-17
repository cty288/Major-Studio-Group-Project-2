
using System.Collections.Generic;
using Crosstales.RTVoice.Model.Enum;
using UnityEngine.Audio;

public enum RadioContentType {
	Text,
	Music,
	DialogueText
}
public interface IRadioContent {
	public RadioContentType ContentType { get; }
	
	public void SetContent(object content);
	
}

public class RadioDialogueContent : IRadioContent {
	[field: ES3Serializable] public List<RadioTextContent> TextContents { get; set; } = new List<RadioTextContent>();
	[field: ES3Serializable]
	public RadioContentType ContentType { get; } = RadioContentType.DialogueText;
	public void SetContent(object content) {
		TextContents = (List<RadioTextContent>) content;
	}
	
	public RadioDialogueContent(List<RadioTextContent> textContents) {
		TextContents = textContents;
		
	}
	
	public RadioDialogueContent() {
		
	}
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

	[field: ES3Serializable] public string DisplayName { get; } = "Radio";

	public void SetContent(object content) {
		speakContent = ((string) content).TrimEnd();
	}

	public RadioTextContent(string speakContent, float speakRate, Gender speakGender, AudioMixerGroup mixer, string displayName = "Radio") {
		this.speakContent = speakContent.TrimEnd();
		this.speakRate = speakRate;
		this.speakGender = speakGender;
		this.mixer = mixer;
		DisplayName = displayName;
	}
	
	public RadioTextContent() {
	}
	
}

public class RadioMusicContent : IRadioContent {
	[field: ES3Serializable]
	public RadioContentType ContentType { get; } = RadioContentType.Music;

	[field: ES3Serializable] public int MusicIndexInPlayList { get; set; } = 0;

	
	public RadioMusicContent(int musicIndexInPlayList) {
		MusicIndexInPlayList = musicIndexInPlayList;
	}
	
	public void SetContent(object content) {
		this.MusicIndexInPlayList = (int) content;
	}
	
	public RadioMusicContent() {
	}
}
