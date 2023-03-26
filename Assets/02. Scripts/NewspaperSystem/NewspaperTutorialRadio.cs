using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crosstales.RTVoice.Model.Enum;
using MikroFramework.Architecture;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;


public class NewspaperTutorialRadio : RadioEvent<RadioTextContent> {

	[field: ES3Serializable]
	protected RadioTextContent radioContent { get; set; }

	protected override RadioTextContent GetRadioContent() {
		return radioContent;
	}
	protected override void SetRadioContent(RadioTextContent radioContent) {
		this.radioContent = radioContent;
	}
    
    public NewspaperTutorialRadio(TimeRange startTimeRange, AudioMixerGroup mixer) : 
	    base(startTimeRange, new RadioTextContent("", 1f, Gender.MALE, mixer),
     RadioChannel.AllChannels) {
	    this.radioContent.SetContent(this.GetModel<HotUpdateDataModel>().GetData("NewspaperTutorialRadio").values[0]);
    }
         
    public NewspaperTutorialRadio(): base(){}
     [field: ES3Serializable]
    public override float TriggerChance { get; } = 1;
    public override void OnEnd() {
        
    }

    public override void OnMissed() {
	    DateTime currentTime = gameTimeManager.CurrentTime.Value;
	    int nextEventInterval = Random.Range(8,15);


	    gameEventSystem.AddEvent(new NewspaperTutorialRadio(
		    new TimeRange(currentTime + new TimeSpan(0, nextEventInterval, 0),
			    currentTime + new TimeSpan(0, nextEventInterval + 10, 0)), this.radioContent.mixer));
    }
    

    protected override void OnRadioStart() {
        
    }
    
    protected override void OnPlayedWhenRadioOff() {
	    OnMissed();
    }
        
}

