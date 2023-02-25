using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.BindableProperty;
using UnityEngine;

public enum RadioChannel {
    DeadNews,
    GeneralNews,
    None
}
public class RadioModel : AbstractSavableModel {

    [field: ES3Serializable]
    public List<AlienDescriptionData> DescriptionDatas { get; } = new List<AlienDescriptionData>();
    [field: ES3Serializable]
    public bool IsSpeaking { get; set; } = false;

    [field: ES3Serializable] public BindableProperty<RadioChannel> CurrentChannel { get; private set; } = new BindableProperty<RadioChannel>(RadioChannel.DeadNews);
    [field: ES3Serializable] public BindableProperty<float> Volume { get; } = new BindableProperty<float>(1); //0-1, if 0, then play noise
    [field: ES3Serializable] public BindableProperty<float> CurrentProgress { get; } = new BindableProperty<float>(0); //0-1
    [field: ES3Serializable] public Dictionary<RadioChannel, RadioChannalRange> ChannelRanges { get; private set; } = null;
    protected override void OnInit() {
        
    }

    public void ChangeProgress(float offset) {
        CurrentProgress.Value = Mathf.Clamp01(CurrentProgress.Value + offset);

        RadioChannalRange channel = GetChannelByProgress(CurrentProgress.Value);
        if (channel != null) {
            float start = channel.start;
            float end = channel.end;
            //volume is determined by the progress, the closer to the middle, the louder
            Volume.Value = Mathf.Clamp01(1 - Mathf.Abs(CurrentProgress.Value - (start + end) / 2) / (end - start));
            CurrentChannel.Value = channel.channel;
        }
        else {
            Volume.Value = 0;
            CurrentChannel.Value = RadioChannel.None;
        }
        

    }
    
    private RadioChannalRange GetChannelByProgress(float progress) {
        foreach (var channelRange in ChannelRanges) {
            if (progress >= channelRange.Value.start && progress <= channelRange.Value.end) {
                return channelRange.Value;
            }
        }

        return null;
    }
    
    public void InitializeChannelRanges(List<RadioChannalRange> ranges) {
        if(ChannelRanges!=null) return;
        ChannelRanges = new Dictionary<RadioChannel, RadioChannalRange>();
        ChannelRanges.Clear();
        foreach (RadioChannalRange range in ranges) {
            ChannelRanges.Add(range.channel, range);
        }

        CurrentProgress.Value = GetRangeCenter(CurrentChannel);
    }
    
    
    public RadioChannalRange GetRadioChannalRange(RadioChannel channel) {
        return ChannelRanges[channel];
    }
    
    public float GetRangeCenter(RadioChannel channel) {
        RadioChannalRange range = GetRadioChannalRange(channel);
        return (range.start + range.end) / 2;
    }


}
