using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Random = UnityEngine.Random;

public static class AlienDescriptionFactory {

    public static List<Func<BodyInfo,float, string>> RadioDescriptions = new List<Func<BodyInfo, float, string>>();

    private static bool inited = false;

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
        StringBuilder sb = new StringBuilder();
        sb.Append("We have a new alien in the area. ");

        float fatness = body.GetAverageFatness();
        if (!IsReal(reality)) {
            fatness = 1 - fatness;
        }
        
        if (fatness < 0.3f) {
            sb.Append("It is very thin. ");
        }
        else if (fatness < 0.6f) {
            sb.Append("It is average. ");
        }
        else {
            sb.Append("It is very fat. ");
        }

        float height = body.GetTotalHeight();
        if (!IsReal(reality)) {
            height = 3 - height;
        }
        
        if (height < 1.5f) {
            sb.Append("It is very short. ");
        }
        else if (height < 2f) {
            sb.Append("It is average. ");
        }
        else {
            sb.Append("It is very tall. ");
        }

        sb.Append("It was reported that it attacked a human this morning!");
        if (body.CheckContainTag<IClothTag>(out IClothTag cloth)) {
            sb.Append(" It was wearing " + cloth.GetRandomDescription(IsReal(reality)) + "!");
        }

        return sb.ToString();
    }


    private static bool IsReal(float reality) {
        return Random.Range(0f, 1f) < reality;
    }
}
