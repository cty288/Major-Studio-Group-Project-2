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


public class PrologueDeliveryRadio : RadioEvent<RadioTextContent> {

   
	[field: ES3Serializable]
	protected override RadioTextContent radioContent { get; set; }
    public PrologueDeliveryRadio(TimeRange startTimeRange, AudioMixerGroup mixer) :
     base(startTimeRange, new RadioTextContent("", 1f, Gender.MALE, mixer),
     RadioChannel.AllChannels) {
	    this.radioContent.SetContent(this.GetModel<HotUpdateDataModel>().GetData("PrologueDeliveryRadio").values[0]);
    }
         
    public PrologueDeliveryRadio(): base(){}
     [field: ES3Serializable]
    public override float TriggerChance { get; } = 1;
    public override void OnEnd() {
        
    }
    protected override void OnPlayedWhenRadioOff() {
	    OnMissed();
    }
    public override void OnMissed() {
        
    }
    

    protected override void OnRadioStart() {
        
    }
        
}

