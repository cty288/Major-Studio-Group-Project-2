using System;
using System.Collections;
using System.Collections.Generic;
using Crosstales;
using Crosstales.RTVoice.Model.Enum;
using MikroFramework.Architecture;
using MikroFramework.Singletons;
//using UnityEditor.ShaderGraph;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

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

    
    
    private  List<Antlr.Runtime.Misc.Func<RadioMessage>> _radioMessages = new List<Antlr.Runtime.Misc.Func<RadioMessage>>();
    private List<Antlr.Runtime.Misc.Func<RadioMessage>> radioMessageCopies = new List<Antlr.Runtime.Misc.Func<RadioMessage>>();


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
        RegisterRadioMessages(Message_Missing_Something);
        RegisterRadioMessages(Message_Weather_Report);
        RegisterRadioMessages(Message_Announcement_Of_President);
        RegisterRadioMessages(Message_Hacked);
        RegisterRadioMessages(Message_Christian_Church);
        RegisterRadioMessages(Message_Research);
        RegisterRadioMessages(Message_Military);

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
    
    private RadioMessage Message_Missing_Something()
    {
        string content = "Accroding to an informer, "
                         + GetRandomString("their child is missing. ", "her wallet got robbed. ",
                             "his pet dog is missing. ")
                         + "We're not entirely sure yet that this was aliens did. But the possibility is extremely high.";
        return new RadioMessage(content, Random.Range(0.8f, 1.2f), Gender.MALE, Random.Range(0.2f, 0.8f), 1);
    }

    private RadioMessage Message_Weather_Report()
    {
        string content = "Weather report of tomorrow. "
                         + GetRandomString("In tomorrow morning ", "On tomorrow afternoon", "In tomorrow evening")
                         + "The temperature will be "
                         + GetRandomString("40", "41", "43", "44", "45", "47", "52", "57", "59", "62", "74", "76", "83", "88","91", "97")
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
                             "He fought an creature and won.",
                             "He is forcing Congress to pass the bill. He claimed that we have run out of time to hesitate.");
        return new RadioMessage(content, Random.Range(0.8f, 1.2f), Gender.MALE, Random.Range(0.2f, 0.8f), 1);
    }

    private RadioMessage Message_Hacked()
    {
        string content = "This is us. For human. Surrender then live. Otherwise, die.";
        return new RadioMessage(content, 0.5f, Gender.MALE, 1f, 1);
    }

    private RadioMessage Message_Military()
    {
        string content =
            "Residents! This is Colonel Sanders! On behalf of the military, I request you not to go out in the middle of the night! ";
        content += "these creatures can camouflage as humans! You are likely to be in danger if you go out late at night. ";
        content += "We will often repeat this message to ensure safety of everyone.";
        return new RadioMessage(content, 1f, Gender.MALE, 1f, 1);
    }

    private RadioMessage Message_Christian_Church()
    {
        string content =
            "Children! And all terrified people! I am the priest of the local church. Do not be afraid. We are praying together during this difficult time!";
        return new RadioMessage(content, 0.9f, Gender.MALE, 1f, 1);
    }

    private RadioMessage Message_Research()
    {
        string content =
            "According to research, this mysterious creature have only a slight advantage over humans in terms of bodily functions. If you have a firearm in your home, make sure it is always ready to use.";
        return new RadioMessage(content, 1.2f, Gender.MALE, 1f, 1);
    }

    private RadioMessage Message_KFCVMe50() {
        DateTime today = this.GetSystem<GameTimeManager>().CurrentTime.Value;
        float chance = 0f;
        if (today.DayOfWeek == DayOfWeek.Thursday) {
            chance = 1;
        }

        string content = "Crazy Thursday is back! Call 233333 to get a V50 combo for half price!";
        return new RadioMessage(content, 1.2f, Gender.MALE, chance, 1);
    }
    private void RegisterRadioMessages(Antlr.Runtime.Misc.Func<RadioMessage> message) {
        _radioMessages.Add(message);
    }

    public IArchitecture GetArchitecture() {
        return MainGame.Interface;
    }
}
