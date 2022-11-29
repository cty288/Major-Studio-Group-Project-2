using System.Collections;
using System.Collections.Generic;
using Antlr.Runtime.Misc;
using Crosstales;
using Crosstales.RTVoice.Model.Enum;
using MikroFramework.Architecture;
using MikroFramework.Singletons;
//using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.Audio;

public class RadioMessage {
    public string Content;
    public float SpeakSpeed;
    public Gender Gender;
    public int MixerIndex;
    public float TriggerChance;
    public RadioMessage(string Content, float speakSpeed, Gender gender, float triggerChance, int mixerIndex = 1) {
        this.Content = Content;
        this.SpeakSpeed = speakSpeed;
        this.Gender = gender;
        this.MixerIndex = mixerIndex;
        this.TriggerChance = triggerChance;
    }


}
public  class RadioRandomStuff :MikroSingleton<RadioRandomStuff>, IController {
    public int RandomRadioAverageTimeInterval = 30;
    private RadioRandomStuff() {
        Init();
    }

    
    
    private  List<Func<RadioMessage>> _radioMessages = new List<Func<RadioMessage>>();
    private List<Func<RadioMessage>> radioMessageCopies = new List<Func<RadioMessage>>();


    public RadioMessage GetNextRandomRadio() {
        radioMessageCopies.CTShuffle();

        RadioMessage targetMessage = null;

        while (targetMessage == null) {
            targetMessage = radioMessageCopies[0]();
            radioMessageCopies.RemoveAt(0);
            if (Random.Range(0f, 1f) > targetMessage.TriggerChance) {
                targetMessage = null;
            }

            if (radioMessageCopies.Count == 0) {
                radioMessageCopies.AddRange(_radioMessages);
            }
        }

        return targetMessage;
    }

    private  void Init() {
        RegisterRadioMessages(Message1);
        radioMessageCopies.AddRange(_radioMessages);
    }

    private RadioMessage Message1() {
        List<string> singers = new List<string>();
        singers.Add("Taylor Shift");
        singers.Add("Charlie Puss");
        singers.Add("Celena Gomez");
        singers.Add("Justin Barber");

        List<string> songNames = new List<string>();
        songNames.Add("My Alien Boyfriend");
        songNames.Add("Horror Outside my Door");
        songNames.Add("Nightmare");
        songNames.Add("Every Night is an Adventure");

        string singer = singers[Random.Range(0, singers.Count)];
        string songName = songNames[Random.Range(0, songNames.Count)];
        string content = $"The newest album from our beloved singer {singer}, \"{songName}\" is now available on the market. Please support our local artists!";
        return new RadioMessage("a ba a ba", Random.Range(0.8f, 1.2f), Gender.FEMALE, Random.Range(0.2f, 0.8f), 1);
    }
    
    private void RegisterRadioMessages(Func<RadioMessage> message) {
        _radioMessages.Add(message);
    }

    public IArchitecture GetArchitecture() {
        return MainGame.Interface;
    }
}
