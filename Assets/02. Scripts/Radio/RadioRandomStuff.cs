using System;
using System.Collections;
using System.Collections.Generic;
using _02._Scripts.Radio.RadioScheduling;
using Crosstales;
using Crosstales.RTVoice.Model.Enum;
using MikroFramework.Architecture;
using MikroFramework.Singletons;
//using UnityEditor.ShaderGraph;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

public class RadioTextMessageInfo {
    public string Content;
    public float SpeakSpeed;
    public Gender Gender;
    public int MixerIndex;
    public float TriggerChance;
    public RadioChannel Channel;
    public RadioProgramType ProgramType;

    public RadioTextMessageInfo(string Content, float speakSpeed, Gender gender, float triggerChance,
        RadioChannel radioChannel, RadioProgramType programType, int mixerIndex = 1) {
        this.Content = Content;
        this.SpeakSpeed = speakSpeed;
        this.Gender = gender;
        this.MixerIndex = mixerIndex;
        this.TriggerChance = triggerChance;
        this.Channel = radioChannel;

        this.ProgramType = programType;
    }
}
public  class RadioRandomStuff :MikroSingleton<RadioRandomStuff>, IController {
    public int RandomRadioAverageTimeInterval = 30;
    private RadioRandomStuff() {
        Init();
    }

    
    
    private Dictionary<RadioProgramType, List<Func<RadioTextMessageInfo>>> _radioMessages = new Dictionary<RadioProgramType,  List<Func<RadioTextMessageInfo>>>();
    private Dictionary<RadioProgramType, List<Func<RadioTextMessageInfo>>> radioMessageCopies = new Dictionary<RadioProgramType, List<Func<RadioTextMessageInfo>>>();


    public RadioTextMessageInfo GetNextRandomRadio(RadioProgramType programType) {
        if(!radioMessageCopies.ContainsKey(programType)) {
            return null;
        }
        
        radioMessageCopies[programType].CTShuffle();

        RadioTextMessageInfo targetTextMessageInfo = null;

        while (targetTextMessageInfo == null) {
            targetTextMessageInfo = radioMessageCopies[programType][0]();
            radioMessageCopies[programType].RemoveAt(0);
            if (Random.Range(0f, 1f) > targetTextMessageInfo.TriggerChance) {
                targetTextMessageInfo = null;
            }

            if (radioMessageCopies[programType].Count == 0) {
                radioMessageCopies[programType].AddRange(_radioMessages[programType]);
            }
        }

        return targetTextMessageInfo;
    }

    private  void Init() {
        RegisterRadioMessages(Message1, RadioProgramType.Ads);
        RegisterRadioMessages(Message_Makeup, RadioProgramType.Ads);
        RegisterRadioMessages(Message_Boombox, RadioProgramType.Ads);
        RegisterRadioMessages(Message_Game_Console, RadioProgramType.Ads);
        RegisterRadioMessages(Message_Missing_Something, RadioProgramType.Announcement);
        RegisterRadioMessages(Message_Weather_Report, RadioProgramType.Announcement);
        RegisterRadioMessages(Message_Announcement_Of_President, RadioProgramType.Announcement);
        RegisterRadioMessages(Message_Hacked, RadioProgramType.Announcement);
        RegisterRadioMessages(Message_Christian_Church, RadioProgramType.Announcement);
        RegisterRadioMessages(Message_Research, RadioProgramType.Announcement);
        RegisterRadioMessages(Message_Military, RadioProgramType.Announcement);
        RegisterRadioMessages(Message_Mayor, RadioProgramType.Announcement);
        RegisterRadioMessages(Message_Suply, RadioProgramType.Announcement);
        RegisterRadioMessages(Message_Newspaper, RadioProgramType.Announcement);

        foreach (RadioProgramType radioProgramType in _radioMessages.Keys) {
            if(!radioMessageCopies.ContainsKey(radioProgramType)) {
                radioMessageCopies.Add(radioProgramType, new List<Func<RadioTextMessageInfo>>());
            }
            radioMessageCopies[radioProgramType].AddRange(_radioMessages[radioProgramType]);
        }
    }
    
    private string GetRandomString(params string[] stringArray)
    {
        return stringArray[Random.Range(0, stringArray.Length)];
    }

    private RadioTextMessageInfo Message1() {
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
        return new RadioTextMessageInfo(content, Random.Range(0.8f, 1.2f), Gender.MALE, Random.Range(0.2f, 0.8f), RadioChannel.FM100, RadioProgramType.Ads,1);
    }

    private RadioTextMessageInfo Message_Makeup()
    {
        List<string> makeups = new List<string>();
        makeups.Add("Lux Queen");
        makeups.Add("Supreme Sparkle");
        makeups.Add("La Belle");
        makeups.Add("Lipsy");
        makeups.Add("Sweet Temptation");

        List<string> looks = new List<string>();
        looks.Add("bold colors and long-lasting wear");
        looks.Add("vibrant hues and smudge-proof formula");
        looks.Add("lustrous shades and waterproof finish");
        looks.Add("dewy glow and nourishing ingredients");
        looks.Add("flawless coverage and weightless feel");
        looks.Add("sheer shimmer and buildable color");
        looks.Add("matte finish and oil-free formula");


        string makeup = makeups[Random.Range(0, makeups.Count)];
        string look = looks[Random.Range(0, looks.Count)];

        string content = "Attention all beauty queens! Do you want to look your best? Then you need our new makeup line," +
            $" {makeup}! With {look}, {makeup} will have you looking stunning all day and night. So get your glam on and try {makeup} today";
        return new RadioTextMessageInfo(content, Random.Range(0.8f, 1.2f), Gender.FEMALE, Random.Range(0.2f, 0.8f), RadioChannel.FM100, RadioProgramType.Ads, 1);
    }

    private RadioTextMessageInfo Message_Boombox()
    {
        List<string> names = new List<string>();
        names.Add("Sonic Breeze");
        names.Add("Beat Blaster");
        names.Add("PowerBeat");
        names.Add("RockBox");
        names.Add("GrooveCube");
        names.Add("JamMaster");

        List<string> looks = new List<string>();
        looks.Add("high-quality sound and a portable design");
        looks.Add("clear acoustics and lightweight frame");
        looks.Add("dynamic range and sleek form factor");
        looks.Add("distortion-free playback and durable casing");
        looks.Add("rich tone and adjustable EQ");
        looks.Add("long battery life and convenient carrying handle");
        looks.Add("crisp audio and compact construction");


        string name = names[Random.Range(0, names.Count)];
        string look = looks[Random.Range(0, looks.Count)];

        string content = $"Do you want to be the life of the party? Then you need our new boombox, the {name}!" +
            $" With {look}, the {name} will have you grooving wherever you go." +
            $" So turn up the volume and get the party started with the {name}!";
        return new RadioTextMessageInfo(content, Random.Range(0.8f, 1.2f), Gender.MALE, Random.Range(0.2f, 0.8f), RadioChannel.FM100, RadioProgramType.Ads, 1);
    }

    private RadioTextMessageInfo Message_Game_Console()
    {
        List<string> names = new List<string>();
        names.Add("Game Master 9000");
        names.Add("PX1");
        names.Add("QuestArcade");
        names.Add("SuperDrive");
        names.Add("GameSphere");
        names.Add("FunMaster A20");

        List<string> looks = new List<string>();
        looks.Add("newest games and immersive gameplay");
        looks.Add("expandable memory and customizable settings");
        looks.Add("8-bit architecture and speedy processing");
        looks.Add("revolutionary technology and advanced features");
        looks.Add("intuitive controls and ergonomic design");
        looks.Add("long battery life and convenient carrying handle");
        looks.Add("multiplayer compatibility and head-to-head play");


        string name = names[Random.Range(0, names.Count)];
        string look = looks[Random.Range(0, looks.Count)];

        string content = $"Attention all gamers! Do you want to take your gaming experience to the next level? Then you need our new video game console, the {name}!" +
            $" With {look}, the {name} will have you immersed in virtual worlds like never before." +
            $" So power up and get your game on with the {name}!";
        return new RadioTextMessageInfo(content, Random.Range(0.8f, 1.2f), Gender.MALE, Random.Range(0.2f, 0.8f), RadioChannel.FM100, RadioProgramType.Ads, 1);
    }

    private RadioTextMessageInfo Message_Coffee_Machine()  //important feature
    {
        string content = "Are you tired of boring old coffee? Then you need our new coffee maker, the Java Jolt!" +
            " With programmable settings and a sleek design, the Java Jolt will brew the perfect cup every time. " +
            $"So wake up and smell the coffee with the Java Jolt!";
        return new RadioTextMessageInfo(content, Random.Range(0.8f, 1.2f), Gender.MALE, Random.Range(0.2f, 0.8f), RadioChannel.FM100, RadioProgramType.Ads, 1);
    }

    private RadioTextMessageInfo Message_Missing_Something()
    {
        string content = "Accroding to an informer, "
                         + GetRandomString("their child is missing. ", "her wallet got robbed. ",
                             "his pet dog is missing. ")
                         + "We're not entirely sure yet that this was those creatures did. But the possibility is extremely high.";
        return new RadioTextMessageInfo(content, Random.Range(0.8f, 1.2f), Gender.MALE, Random.Range(0.2f, 0.8f),RadioChannel.FM100,RadioProgramType.Announcement ,1);
    }

    private RadioTextMessageInfo Message_Weather_Report()
    {
        string content = "Weather report of tomorrow. "
                         + GetRandomString("In tomorrow morning ", "On tomorrow afternoon", "In tomorrow evening")
                         + "The temperature will be "
                         + GetRandomString("40", "41", "43", "44", "45", "47", "52", "57", "59", "62", "74", "76", "83", "88","91", "97")
                         + " Fahrenheit. Also, tomorrow day will be a "
                         +GetRandomString("rainy", "windy", "cloudy", "sunny", "snowy")
                         + " day.";
        return new RadioTextMessageInfo(content, Random.Range(0.8f, 1.2f), Gender.FEMALE, Random.Range(0.2f, 0.8f), RadioChannel.FM100,RadioProgramType.Announcement,1);
    }

    private RadioTextMessageInfo Message_Announcement_Of_President()
    {
        string content = "Our great President, Barack J Biden, announced that " +
                         GetRandomString(
                             "The whole country must raise the alert level.",
                             "Troops are maintaining order in our vital cities.",
                             "He fought an creature and won.",
                             "He is forcing Congress to pass the bill. He claimed that we have run out of time to hesitate.");
        return new RadioTextMessageInfo(content, Random.Range(0.8f, 1.2f), Gender.MALE, Random.Range(0.2f, 0.8f),RadioChannel.FM100,RadioProgramType.Announcement ,1);
    }

    private RadioTextMessageInfo Message_Hacked()
    {
        string content = "This is us. For human. Surrender then live. Otherwise, die.";
        return new RadioTextMessageInfo(content, 0.5f, Gender.MALE, 1f, RadioChannel.FM100,RadioProgramType.Announcement,4);
    }

    private RadioTextMessageInfo Message_Military()
    {
        string content =
            "Residents! This is Colonel Sanders! On behalf of the military, I request you not to go out in the middle of the night! ";
        content += "these creatures can camouflage as humans! You are likely to be in danger if you go out late at night. ";
        content += "We will often repeat this message to ensure safety of everyone.";
        return new RadioTextMessageInfo(content, 1f, Gender.MALE, 1f, RadioChannel.FM100,RadioProgramType.Announcement,1);
    }

    private RadioTextMessageInfo Message_Christian_Church()
    {
        string content =
            "Children! And all terrified people! I am the priest of the local church. Do not be afraid. We are praying together during this difficult time!";
        return new RadioTextMessageInfo(content, 0.9f, Gender.MALE, 1f, RadioChannel.FM100,RadioProgramType.Announcement,1);
    }

    private RadioTextMessageInfo Message_Research()
    {
        string content =
            "According to research, this mysterious creature have only a slight advantage over humans in terms of bodily functions. If you have a firearm in your home, make sure it is always ready to use.";
        return new RadioTextMessageInfo(content, 1.2f, Gender.MALE, 1f, RadioChannel.FM100,RadioProgramType.Announcement,1);
    }

    private RadioTextMessageInfo Message_Mayor() {
        string content =
            "The mayor held a press conference yesterday. He reiterate the urgent need to surpress recent infiltrations attempts. The government is dispatching more forces and couriers into the area";
        return new RadioTextMessageInfo(content, 1.2f, Gender.MALE, 0.5f, RadioChannel.FM100,RadioProgramType.Announcement,1);
    }

    private RadioTextMessageInfo Message_Suply() {
        string content =
            "If you are running low on your supplies during quarantine, no worries! The government along with many private organizations are providing you the most stable delivery service you can count on.";
        return new RadioTextMessageInfo(content, 1.2f, Gender.MALE, 0.8f,RadioChannel.FM100, RadioProgramType.Announcement,1);
    }

    private RadioTextMessageInfo Message_Newspaper()
    {
        string content =
            "Anxious about the infiltration in your area? Read the death report everyday to get the information that can keep you and your loved ones safe!";
        return new RadioTextMessageInfo(content, 1.2f, Gender.MALE, 0.8f, RadioChannel.FM100,RadioProgramType.Announcement,1);
    }

    private RadioTextMessageInfo Message_KFCVMe50() {
        DateTime today = this.GetSystem<GameTimeManager>().CurrentTime.Value;
        float chance = 0f;
        if (today.DayOfWeek == DayOfWeek.Thursday) {
            chance = 1;
        }

        string content = "Crazy Thursday is back! Call 233333 to get a V50 combo for half price!";
        return new RadioTextMessageInfo(content, 1.2f, Gender.MALE, chance, RadioChannel.FM100,RadioProgramType.Ads,1);
    }
    private void RegisterRadioMessages(Func<RadioTextMessageInfo> message, RadioProgramType programType) {
        if(!_radioMessages.ContainsKey(programType)) {
            _radioMessages.Add(programType, new List<Func<RadioTextMessageInfo>>());
        }
        _radioMessages[programType].Add(message);
    }

    public IArchitecture GetArchitecture() {
        return MainGame.Interface;
    }
}
