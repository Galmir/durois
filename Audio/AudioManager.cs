using Godot;
using System.Collections.Generic;
using System.Linq;

public enum AudioType {
    Music,
    SFX
}
public partial class AudioManager : Node {
    private float audioRange = 0.2f;
    private List<AudioStreamPlayer> children = [];

    private static AudioManager _instance;
    public static AudioManager Instance => _instance;

    public override void _Ready() {
        if (_instance != null) {
            _instance.QueueFree(); // The Singletone is already loaded, kill this instance
        }
        _instance = this;
    }





    public void Play(AudioType audioType, AudioStream audio = null, bool single = false, bool randomPitch = false) {
        if (children.Count == 0) CreateChild(audioType);
        if (single) Stop();
        bool freeAudioChannel = false;
        foreach (AudioStreamPlayer child in children.Where(child => !child.IsPlaying())) {
            child.Stream = audio;
            child.Bus = new StringName(nameof(audioType));
            if (randomPitch) child.PitchScale = (float)GD.RandRange(1 - audioRange, 1 + audioRange);
            child.Play();
            freeAudioChannel = true;
            break;
        }
        if (!freeAudioChannel) CreateChild(audioType, audio, single, randomPitch);
    }

    private void CreateChild(AudioType audioType, AudioStream audio = null, bool single = false,
        bool randomPitch = false) {
        AudioStreamPlayer player = new AudioStreamPlayer();
        player.Bus = new StringName(nameof(audioType));
        if (audio != null) {
            if (single && children.Count != 0) Stop();
            player.Stream = audio;
            if (randomPitch) player.PitchScale = (float)GD.RandRange(1 - audioRange, 1 + audioRange);
        }
        AddChild(player);
        children.Add(player);
        if (audio != null) player.Play();
    }

    private void Stop() {
        foreach (AudioStreamPlayer child in children) child.Stop();
    }
}
