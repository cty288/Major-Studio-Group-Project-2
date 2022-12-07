using System;
using System.Collections;
using System.Collections.Generic;
using Mikrocosmos;
using MikroFramework;
using MikroFramework.Architecture;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class OpeningManager : MonoBehaviour {
    [SerializeField] private Speaker speaker;
    [SerializeField] private float rate = 1.2f;
    [SerializeField] private AudioMixerGroup mixer;
    private void Start() {
        ImageEffectController.Singleton.DisableAllFeatures();
        speaker.Speak("Attention! Recently, some citizens in the town have mysteriously disappeared, " +
                      "while there are some witnesses of unknown creatures that can disguise themselves as humans and attack humans." +
                      " According to observations, the creature mostly acts at night. We are currently investigating this incident." +
                      " Remember! All residents �� please do not go out or open the door at night! We will deliver food and other supplies for you, " +
                      "and we will knock on your door when delivering supplies. Please only open your door when you hear us knocking! " +
                      "Again, please do not open the door at night!", mixer, "Radio", OnFinished, rate);
        this.Delay(8f, () => {
            Architecture<MainGame>.ResetArchitecture();
        });
    }

    private void OnFinished() {
        SceneManager.LoadScene("MainGame");
    }
}
