using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.BindableProperty;
using UnityEngine;

public enum RadioChannel {
    FM96,
    FM100,
    FM104,
    FM108,
    FM92,
    None,
    AllChannels
}



public struct OnRadioUnlocked {
    
}

[Serializable]
public class RadioChannelInfo {
    public RadioChannel Channel;
    public string ChannelName;
    //public string FMName;
    public RadioChannalRange Range;
    
    public RadioChannelInfo(RadioChannel channel, string channelName, RadioChannalRange range) {
        Channel = channel;
        ChannelName = channelName;
        Range = range;
    }
}
public class RadioModel : AbstractSavableModel {
    

    [field: ES3Serializable]
    public List<AlienDescriptionData> DescriptionDatas { get; } = new List<AlienDescriptionData>();

    [field: ES3Serializable]
    public bool HasDescriptionDatasToday { get; set; } = false;
    public Dictionary<RadioChannel, bool> IsSpeaking { get; private set; } = new Dictionary<RadioChannel, bool>();

    [field: ES3Serializable] public BindableProperty<RadioChannel> CurrentChannel { get; private set; } = new BindableProperty<RadioChannel>(RadioChannel.FM96);
    
    [field: ES3Serializable] public BindableProperty<float> RelativeVolume { get; } = new BindableProperty<float>(1); //0-1, if 0, then play noise
    [field: ES3Serializable] public BindableProperty<float> CurrentProgress { get; } = new BindableProperty<float>(0); //0-1
    [field: ES3Serializable] public Dictionary<RadioChannel, RadioChannelInfo> AllChannels { get; private set; } = null;

    [field: ES3Serializable] public BindableProperty<bool> IsOn { get; } = new BindableProperty<bool>(true);

    [field: ES3Serializable] private HashSet<RadioChannel> unlockedChannel = new HashSet<RadioChannel>() {
        RadioChannel.FM96,
        RadioChannel.FM100,
        RadioChannel.FM104,
        RadioChannel.FM92,
        RadioChannel.FM108
        
    };
    protected override void OnInit() {
        
    }

    public void ChangeProgress(float offset) {
        CurrentProgress.Value = Mathf.Clamp01(CurrentProgress.Value + offset);

        RadioChannelInfo channel = GetChannelByProgress(CurrentProgress.Value);
        if (channel != null) {
            float start = channel.Range.start;
            float end = channel.Range.end;
            //volume is determined by the progress, the closer to the middle, the louder
            RelativeVolume.Value = Mathf.Clamp01(1 - Mathf.Abs(CurrentProgress.Value - (start + end) / 2) / (end - start));
            CurrentChannel.Value = channel.Channel;
        }
        else {
            RelativeVolume.Value = 1;
            CurrentChannel.Value = RadioChannel.None;
        }
    }

    public void UnlockChannel(RadioChannel channel) {
        if (!unlockedChannel.Contains(channel)) {
            unlockedChannel.Add(channel);
            ChangeProgress(0);
            this.SendEvent<OnRadioUnlocked>();
        }
    }
    
    private RadioChannelInfo GetChannelByProgress(float progress) {
        foreach (var channel in AllChannels.Values) {
            if (progress >= channel.Range.start && progress <= channel.Range.end) {
                if (unlockedChannel.Contains(channel.Channel)) {
                    return channel;
                }

                return null;
            }
        }

        return null;
    }
    
    public void InitializeChannelInfos(List<RadioChannelInfo> infos) {
        IsSpeaking = new Dictionary<RadioChannel, bool>();
        foreach (RadioChannelInfo info in infos) {
            IsSpeaking.Add(info.Channel, false);
        }
        
        if(AllChannels!=null) return;
        AllChannels = new Dictionary<RadioChannel, RadioChannelInfo>();
       
        foreach (RadioChannelInfo info in infos) {
            AllChannels.Add(info.Channel, info);
        }
        
        

        CurrentProgress.Value = GetRangeCenter(CurrentChannel);
    }
    
    
    public RadioChannalRange GetRadioChannalRange(RadioChannel channel) {
        return AllChannels[channel].Range;
    }
    
    
    public void SetIsSpeaking(RadioChannel channel, bool isSpeaking) {
        if (IsSpeaking.ContainsKey(channel)) {
            IsSpeaking[channel] = isSpeaking;
        }
    }
    
    public bool GetIsSpeaking(RadioChannel channel) {
        if (IsSpeaking.ContainsKey(channel)) {
            return IsSpeaking[channel];
        }

        return false;
    }
    
    public RadioChannelInfo GetRadioChannelInfo(RadioChannel channel) {
        if (channel == RadioChannel.None) {
            return null;
        }
        return AllChannels[channel];
    }
    
    public float GetRangeCenter(RadioChannel channel) {
        RadioChannalRange range = GetRadioChannalRange(channel);
        return (range.start + range.end) / 2;
    }


}
