using System;
using System.Collections;
using System.Collections.Generic;
using Crosstales.RTVoice.Model.Enum;
using DG.Tweening;
using MikroFramework.Architecture;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class Speaker :  AbstractMikroController<MainGame> {
    private AudioSource audioSource;
    private AudioMixer audioMixer;

    private Queue<string> sentenceQueues = new Queue<string>();
    [SerializeField] private Subtitle subtitile;
    private void Awake() {
        audioSource = GetComponent<AudioSource>();
        audioMixer = audioSource.outputAudioMixerGroup.audioMixer;
        Crosstales.RTVoice.Speaker.Instance.OnSpeakCompleted.AddListener(OnSpeakCompleted);
    }

    private void OnDestroy() {
        Crosstales.RTVoice.Speaker.Instance.OnSpeakCompleted.RemoveListener(OnSpeakCompleted);
    }

    private void OnSpeakCompleted(string s) {
        if (sentenceQueues.Count > 0) {
            string nextSentence = sentenceQueues.Dequeue();
            SpeakSentence(nextSentence);
        }
        else {
            Stop();
        }
    }

    public void Speak(string sentence) {
        bool needSpeak = sentenceQueues.Count == 0;
        List<string> splitedSentences = VoiceTextSpliter.Split(sentence);
        foreach (string splitedSentence in splitedSentences) {
            sentenceQueues.Enqueue(splitedSentence);
        }

        if (needSpeak) {
            string text = sentenceQueues.Dequeue();
            SpeakSentence(text);
        }
    }

    public void Corrupt(float time, Action onFinished) {
        //change cutoff freq
        audioMixer.DOSetFloat("cutoffFreq", 0, time).OnComplete(() => {
            onFinished?.Invoke();
        });
        audioMixer.DOSetFloat("resonance", 10, time);
    }

    private void SpeakSentence(string text) {
        Crosstales.RTVoice.Speaker.Instance.Speak(text, audioSource, Crosstales.RTVoice.Speaker.Instance.VoiceForGender(Gender.MALE));
        if (subtitile) {
            subtitile.OnSpeakStart(text);
        }
     
    }

    public void Stop() {
        sentenceQueues.Clear();
        if (subtitile) {
            subtitile.OnSpeakStart("");
        }

        audioMixer.SetFloat("cutoffFreq", 4600);
        audioMixer.SetFloat("resonance", 1);
        audioSource.Stop();
    }
}
