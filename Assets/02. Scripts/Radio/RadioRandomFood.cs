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

public class RadioRandomFood : MikroSingleton<RadioRandomFood>, IController
{
    //public int RandomRadioAverageTimeInterval = 30;
    public List<Type> foodlist = new List<Type>(){
        typeof(DryFood),
        typeof(CanFood),
        typeof(CrackerPackages),
    };
    
    private RadioRandomFood()
    {
        Init();
    }



    private List<Antlr.Runtime.Misc.Func<string, RadioMessage>> _radioMessages = new List<Antlr.Runtime.Misc.Func<string, RadioMessage>>();
    private List<Antlr.Runtime.Misc.Func<string, RadioMessage>> radioMessageCopies = new List<Antlr.Runtime.Misc.Func<string, RadioMessage>>();


    public RadioMessage GetNextRandomFoodRadio()
    {
        radioMessageCopies.CTShuffle();

        RadioMessage targetMessage = null;

        while (targetMessage == null)
        {
            Type nextFood = GetRandomFood();
            string displayName = "temp";


            targetMessage = radioMessageCopies[0](displayName);
            radioMessageCopies.RemoveAt(0);
            if (Random.Range(0f, 1f) > targetMessage.TriggerChance)
            {
                targetMessage = null;
            }

            if (radioMessageCopies.Count == 0)
            {
                radioMessageCopies.AddRange(_radioMessages);
            }
        }
        //record next upgraded food information in starvation && food system 
        return targetMessage;
    }

    private void Init()
    {
        RegisterRadioMessages(Message_KFCVMe50);
       
        radioMessageCopies.AddRange(_radioMessages);
    }

    private string GetRandomString(params string[] stringArray)
    {
        return stringArray[Random.Range(0, stringArray.Length)];
    }

    private Type GetRandomFood()
    {
        return foodlist[Random.Range(0, foodlist.Count)];
    }


    
    private RadioMessage Message_KFCVMe50(string displayName)
    {
        DateTime today = this.GetSystem<GameTimeManager>().CurrentTime.Value;
        float chance = 0f;
        if (today.DayOfWeek == DayOfWeek.Thursday)
        {
            chance = 1;
        }

        string content = $"my name is {displayName}";
        return new RadioMessage(content, 1.2f, Gender.MALE, chance, RadioChannel.GeneralNews, 1);
    }
    private void RegisterRadioMessages(Antlr.Runtime.Misc.Func<string, RadioMessage> message)
    {
        _radioMessages.Add(message);
    }

    public IArchitecture GetArchitecture()
    {
        return MainGame.Interface;
    }
}

