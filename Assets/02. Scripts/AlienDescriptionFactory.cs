using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Random = UnityEngine.Random;

public static class AlienDescriptionFactory {

    public static List<Func<BodyInfo,float, string>> RadioDescriptions = new List<Func<BodyInfo, float, string>>();

    private static bool inited = false;

    private static DescriptionFormatter formatter = new DescriptionFormatter();

    public static DescriptionFormatter Formatter => formatter;
    public static void Init() {
        //RegisterRadioDescription(TestRadioDescription);
        RegisterRadioDescription(Radio0);
        RegisterRadioDescription(Radio1);
        RegisterRadioDescription(Radio2);
        RegisterRadioDescription(Radio3);
        RegisterRadioDescription(Radio4);
        RegisterRadioDescription(Radio5);
    }

    public static string GetRadioDescription(BodyInfo bodyInfo, float reality) {
        if (!inited) {
            Init();
        }

       
        // bodyInfo = BodyInfo.GetRandomBodyInfo();
        return RadioDescriptions[Random.Range(0, RadioDescriptions.Count)](bodyInfo, reality).TrimEnd();
    }

    public static void RegisterRadioDescription(Func<BodyInfo, float, string> description) {
        RadioDescriptions.Add(description);
    }


    private static string TestRadioDescription(BodyInfo body, float reality) {
        DescriptionFormatter.Reality = reality;
        StringBuilder sb = new StringBuilder();
        
        sb.AppendFormat(formatter, "We have a new alien in the area. {0:fat} It {0:height}", body);
        sb.Append("It was reported that it attacked a human this morning!");
        sb.AppendFormat(formatter, "{0:clothb} {0:clothl}", body);

        return sb.ToString();
    }

    private static string Radio0(BodyInfo body, float reality)
    {
        DescriptionFormatter.Reality = reality;
        StringBuilder sb = new StringBuilder();

        sb.AppendFormat("We have just got the latest information of the newly discovered corpse!");
        sb.AppendFormat(formatter, "{0:height} and {0:clothb} Also, {0:hair}", body);
        return sb.ToString();
    }

    private static string Radio1(BodyInfo body, float reality)
    {
        DescriptionFormatter.Reality = reality;
        StringBuilder sb = new StringBuilder();

        sb.AppendFormat("Welcome back, we have some updated information about the dead body we found this morning.");
        if (body.CheckContainTag<IAccessoryTag>(out var accessoryTag)) {
            sb.AppendFormat(formatter, "According to our source, {0:acc} and it is also believed that, {0:clothb}", body);
        }
        else {
            sb.AppendFormat(formatter, "According to our source, {0:clothb}", body);
        }
       
        return sb.ToString();
    }

    private static string Radio2(BodyInfo body, float reality)
    {
        DescriptionFormatter.Reality = reality;
        StringBuilder sb = new StringBuilder();

        sb.AppendFormat("This is Radio MK, and I’m your host. New reports indicate that a newly found dead body yesterday has the following trait: ");
        sb.AppendFormat(formatter, "{0:height} and {0:voice}", body);
        sb.AppendFormat("Since the creature can disguise itself into human bodies, we highly recommend you to be aware of anyone who looks like this dead body.");
        return sb.ToString();
    }

    private static string Radio3(BodyInfo body, float reality)
    {
        DescriptionFormatter.Reality = reality;
        StringBuilder sb = new StringBuilder();

        sb.AppendFormat("This just in. A resident was reported misssing since yesterday morning.");
        if (body.CheckContainTag<IAccessoryTag>(out var accessoryTag)) {
            sb.AppendFormat(formatter, "{0:voice} Other sources have shown what it was wearing. {0:acc} {0:clothl}", body);
        }
        else {
            sb.AppendFormat(formatter, "{0:voice} Other sources have shown what it was wearing. {0:clothl}", body);
        }
       
        return sb.ToString();
    }

    private static string Radio4(BodyInfo body, float reality)
    {
        DescriptionFormatter.Reality = reality;
        StringBuilder sb = new StringBuilder();

        sb.AppendFormat("From recent reports, a resident went missing since two days ago.");
        if (body.CheckContainTag<IAccessoryTag>(out var accessoryTag)) {
            sb.AppendFormat(formatter, "{0:hair} and {0:acc} In addition, {0:clothl}", body);
        }
        else {
            sb.AppendFormat(formatter, "{0:hair}. In addition, {0:clothl}", body);
        }
       
        sb.AppendFormat("Please be aware of those who have the similar traits.");
        return sb.ToString();
    }

    private static string Radio5(BodyInfo body, float reality)
    {
        DescriptionFormatter.Reality = reality;
        StringBuilder sb = new StringBuilder();

        sb.AppendFormat(formatter,
            "We’ve got some news for you coming right up. One of the delivery couriers went missing yesterday according to our source.");
        sb.AppendFormat(formatter, "{0:height}  {0:clothb} and {0:hair}", body);
        return sb.ToString();
    }


    private static bool IsReal(float reality) {
        return Random.Range(0f, 1f) < reality;
    }
}
