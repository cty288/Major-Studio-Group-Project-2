using System;
using System.Collections;
using System.Collections.Generic;
using Mikrocosmos;
using MikroFramework;
using MikroFramework.Architecture;
using MikroFramework.TimeSystem;
using MikroFramework.Utilities;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class OpeningManager : MonoBehaviour {
    [SerializeField] private Speaker speaker;
    [SerializeField] private float rate = 1.2f;
    [SerializeField] private AudioMixerGroup mixer;
    private bool canSkip = false;
    [SerializeField] private GameObject canSkipHint;
    private string content =
        "Recently, there are more and more reports indicating that some mysterious creature has been trying to disguise itself as humans and infiltrate our society. These strange incidents have aroused widespread panic among people. We don¡¯t know their target yet, but we are now taking actions against such infiltration operations." +
        " This creature has the ability to transform and mimic human behavior. According to witnesses, it can maintain its disguise for <color=yellow>up to three days</color> after killing and assuming the identity of its victim. Be sure to carefully check the death date on any reports you come across." +
        " The government has issued orders to encourage our courageous residents to report any susspicious activities. The area is also locked down until we have fully cleared the potential threats to our civilization." +
        " Be sure that you listen to the local government radio and read the newspaper at your doorstep everyday. We will update our citizens with recent incidents and their locations. Please be cautious or stay away from dangerous areas. <color=yellow>Radio may sometimes be inaccurate, as we are trying to broadcast every latest incidents to our audience. Newspaper is the most accurate source since we carefully select the actual scenes that has been thoroughly investigated.</color>" +
        " If you come across anyone acting strangely or suspiciously, do not hesitate to report the case to us. In case of emergency, you may use lethal force, but be sure to carefully consider your decision before acting.";

    private void Awake() {
        canSkip = false;
    }

    private void Start() {
        canSkip = false;
        SubtitleNotebookHightlightedTextRecorder.IsMouseOverUIRC = new SimpleRC();
        ImageEffectController.Singleton.DisableAllFeatures();
        /*
        speaker.Speak(content, mixer, "Radio", OnFinished, rate);
        this.Delay(3f, () => {
            Architecture<MainGame>.ResetArchitecture();
            canSkipHint.SetActive(true);
            canSkip = true;
        });*/
        Architecture<MainGame>.ResetArchitecture();
        //MainGame.Interface.ResetArchitecture();
       
        //((MainGame) (MainGame.Interface)).ClearSave();
        this.Delay(3f, () => {
           
            Architecture<MainGame>.ResetArchitecture();
            SceneManager.LoadScene("MainGame");
        });
        
    }

    private void Update() {
        
        if (Input.GetKeyDown(KeyCode.Space) && canSkip) {

            if (speaker.IsSpeaking)
            {
                speaker.Stop(true);
            }
        }
    }

    private void OnFinished() {
        
        // Architecture<MainGame>.ResetArchitecture();
        SceneManager.LoadScene("MainGame");
    }
}
