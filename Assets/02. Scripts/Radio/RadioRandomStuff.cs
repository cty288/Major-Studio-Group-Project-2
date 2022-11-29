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
    
    private string GetRandomString(params string[] stringArray)
    {
        return stringArray[Random.Range(0, stringArray.Length)];
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
        return new RadioMessage(content, Random.Range(0.8f, 1.2f), Gender.MALE, Random.Range(0.2f, 0.8f), 1);
    }
    
    private RadioMessage Message_MissingSomething()
    {
        string content = "Accroding to an informer, "
                         + GetRandomString("their child is missing. ", "her wallet got robbed. ",
                             "his pet dog is missing. ")
                         + "We're not entirely sure yet that this was aliens did. But the possibility is extremely high.";
        return new RadioMessage(content, Random.Range(0.8f, 1.2f), Gender.MALE, Random.Range(0.2f, 0.8f), 1);
    }

    private RadioMessage Message_WeatherReport()
    {
        string content = "Weather report of tomorrow. "
                         + GetRandomString("In tomorrow morning ", "On tomorrow afternoon", "In tomorrow evening")
                         + "The temperature will be "
                         + GetRandomString("40", "41", "43", "44", "45", "47", "52", "57", "59", "62")
                         + " Fahrenheit. Also, tomorrow day will be a "
                         +GetRandomString("rainy", "windy", "cloudy", "sunny", "snowy")
                         + " day.";
        return new RadioMessage(content, Random.Range(0.8f, 1.2f), Gender.FEMALE, Random.Range(0.2f, 0.8f), 1);
    }

    private RadioMessage Message_Announcement_Of_President()
    {
        string content = "Our great President, Barack J Biden, announced that " +
                         GetRandomString(
                             "The whole country must raise the alert level.",
                             "Troops are maintaining order in our vital cities.",
                             "He fought an alien and won.",
                             "He is forcing Congress to pass the bill. He claimed that we have run out of time to hesitate.");
        return new RadioMessage(content, Random.Range(0.8f, 1.2f), Gender.MALE, Random.Range(0.2f, 0.8f), 1);
    }

    private RadioMessage Message_Alien()
    {
        string content = "This is Alien. For human. Surrender then live. Otherwise, die.";
        return new RadioMessage(content, 0.5f, Gender.MALE, 1f, 1);
    }

    private void RegisterRadioMessages(Func<RadioMessage> message) {
        _radioMessages.Add(message);
    }

    public IArchitecture GetArchitecture() {
        return MainGame.Interface;
    }
}
