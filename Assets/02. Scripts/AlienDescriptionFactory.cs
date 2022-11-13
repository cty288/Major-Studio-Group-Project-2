using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Random = UnityEngine.Random;

public static class AlienDescriptionFactory {

    public static List<Func<BodyInfo,float, string>> RadioDescriptions = new List<Func<BodyInfo, float, string>>();

    private static bool inited = false;

    private static DescriptionFormatter formatter = new DescriptionFormatter();
    public static void Init() {
        RegisterRadioDescription(TestRadioDescription);
    }

    public static string GetRadioDescription(BodyInfo bodyInfo, float reality) {
        if (!inited) {
            Init();
        }

       
        // bodyInfo = BodyInfo.GetRandomBodyInfo();
        return RadioDescriptions[Random.Range(0, RadioDescriptions.Count)](bodyInfo, reality);
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


    private static bool IsReal(float reality) {
        return Random.Range(0f, 1f) < reality;
    }
}
