using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OpeningManager : MonoBehaviour {
    [SerializeField] private Speaker speaker;
    [SerializeField] private float rate = 1.2f;
    private void Start() {
        speaker.Speak("Attention! Recently, some citizens in the town have mysteriously disappeared, " +
                      "while there are some witnesses of unknown creatures that can disguise themselves as humans and attack humans." +
                      " According to the survey, the creature mostly acts at night. We are currently investigating this incident." +
                      " Remember! All residents ¡ª please do not go out or open the door at night! We will deliver food and other supplies for you, " +
                      "and we will knock on your door when delivering supplies. Please only open your door when you hear us knocking! " +
                      "Again, please do not open the door at night!", OnFinished, rate);
    }

    private void OnFinished() {
        SceneManager.LoadScene("MainGame");
    }
}
