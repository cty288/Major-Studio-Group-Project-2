using System.Collections;
using System.Collections.Generic;
using Crosstales.RTVoice;
using Crosstales.RTVoice.Model;
using Crosstales.RTVoice.Model.Enum;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VoiceTest : MonoBehaviour {
    public TMP_InputField text;
    public string VoiceName;
    public Speaker speaker;
    public void Speak() {

        speaker.Speak(text.text, null, "");
        speaker.Corrupt(5, () => {
            speaker.Stop();
        });
    }

 
}
