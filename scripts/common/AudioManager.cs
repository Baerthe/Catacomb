namespace Common;

using Godot;
using System.Collections.Generic;
using System.Linq;
/// <summary>
/// Manages audio playback with two channels and volume control.
/// </summary>
public sealed class AudioManager
{
    private readonly AudioStreamPlayer _audioPlayerChannel1;
    private readonly AudioStreamPlayer _audioPlayerChannel2;
    private readonly AudioStreamPlayer _audioPlayerChannelMusic;
    private AudioEvent[] _audioClips = [];
    private AudioEvent[] _musicClips = [];
    private float _channel1Volume = 1.0f;
    private float _channel2Volume = 1.0f;
    private float _channelMusicVolume = 1.0f;
    private bool _isChannel1Playing = false;
    private bool _isChannel1AllowedToPlay = true;
    private bool _isChannel2Playing = false;
    private bool _isChannel2AllowedToPlay = true;
    private bool _isChannelMusicPlaying = false;
    private bool _isChannelMusicAllowedToPlay = true;
    public AudioManager(AudioStreamPlayer audioPlayerChannel1, AudioStreamPlayer audioPlayerChannel2, AudioStreamPlayer audioPlayerChannelMusic)
    {
        _audioPlayerChannel1 = audioPlayerChannel1;
        _audioPlayerChannel2 = audioPlayerChannel2;
        _audioPlayerChannelMusic = audioPlayerChannelMusic;
        BuildAudioCache();
        GD.Print("AudioManager: Initialized");
    }
    /// <summary>
    /// Plays an audio clip by name on the specified channel.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="channel"></param>
    public void PlayAudioClip(AudioEvent audioEvent, int channel = 1)
    {
        if (_audioClips.Contains(audioEvent))
        {
            if (channel == 1 && _isChannel1AllowedToPlay)
            {
                _audioPlayerChannel1.Stream = audioEvent.Clip;
                _audioPlayerChannel1.VolumeDb = LinearToDb(_channel1Volume);
                _audioPlayerChannel1.Play();
                _isChannel1Playing = true;
            } else if (channel == 2 && _isChannel2AllowedToPlay)
            {
                _audioPlayerChannel2.Stream = audioEvent.Clip;
                _audioPlayerChannel2.VolumeDb = LinearToDb(_channel2Volume);
                _audioPlayerChannel2.Play();
                _isChannel2Playing = true;
            }
        }
    }
    /// <summary>
    /// Plays a music track by name on the music channel.
    /// </summary>
    /// <param name="name"></param>
    public void PlayMusicTrack(AudioEvent audioEvent)
    {
        if (_musicClips.Contains(audioEvent) && _isChannelMusicAllowedToPlay)
        {
            _audioPlayerChannelMusic.Stream = audioEvent.Clip;
            _audioPlayerChannelMusic.VolumeDb = LinearToDb(_channelMusicVolume);
            _audioPlayerChannelMusic.Play();
            _isChannelMusicPlaying = true;
        }
    }
    /// <summary>
    /// Mutes or unmutes the specified audio channel. 0 = all, 1 = channel 1, 2 = channel 2, 3 = music.
    /// </summary>
    /// <param name="channel">int of channel</param>
    /// <param name="allowed">bool</param>
    public void SetAudioAllowed(int channel = 0, bool b = true)
    {
        if (channel == 0)
        {
            _isChannel1AllowedToPlay = b;
            _isChannel2AllowedToPlay = b;
            _isChannelMusicAllowedToPlay = b;
            return;
        }
        if (channel == 1)
            _isChannel1AllowedToPlay = b;
        else if (channel == 2)
            _isChannel2AllowedToPlay = b;
        else if (channel == 3)
            _isChannelMusicAllowedToPlay = b;
    }
    /// <summary>
    /// Sets the volume for the specified channel.
    /// </summary>
    /// <param name="channel">The channel number (1, 2, or 3 - for music).</param>
    /// <param name="volume">The volume level (0.0 to 1.0).</param>
    public void SetChannelVolume(int channel, float volume)
    {
        volume = Mathf.Clamp(volume, 0.0f, 1.0f);
        if (channel == 1)
        {
            _channel1Volume = volume;
            if (_isChannel1Playing)
                _audioPlayerChannel1.VolumeDb = LinearToDb(_channel1Volume);
        } else if (channel == 2)
        {
            _channel2Volume = volume;
            if (_isChannel2Playing)
                _audioPlayerChannel2.VolumeDb = LinearToDb(_channel2Volume);
        } else if (channel == 3)
        {
            _channelMusicVolume = volume;
            if (_isChannelMusicPlaying)
                _audioPlayerChannelMusic.VolumeDb = LinearToDb(_channelMusicVolume);
        }
    }
    /// <summary>
    /// Stops playback on the specified channel. 1 , 2, or 3 (music).
    /// </summary>
    /// <param name="channel"></param>
    public void StopChannel(int channel)
    {
        if (channel == 1 && _isChannel1Playing)
        {
            _audioPlayerChannel1.Stop();
            _isChannel1Playing = false;
        } else if (channel == 2 && _isChannel2Playing)
        {
            _audioPlayerChannel2.Stop();
            _isChannel2Playing = false;
        } else if (channel == 3 && _isChannelMusicPlaying)
        {
            _audioPlayerChannelMusic.Stop();
            _isChannelMusicPlaying = false;
        }
    }
    /// <summary>
    /// Converts a linear volume value (0.0 to 1.0) to decibels.
    /// </summary>
    /// <param name="linear"></param>
    /// <returns>float</returns>
    private static float LinearToDb(float linear) => linear <= 0.0f ? -80.0f : 20.0f * Mathf.Log(linear);
    /// <summary>
    /// Builds the audio cache from resources.
    /// </summary>
    private void BuildAudioCache()
    {
        string hardPath = "res://assets/resources/audio";
        var dir = DirAccess.Open(hardPath);
        List<AudioEvent> audioTemp = [];
        List<AudioEvent> musicTemp = [];
        if (dir != null)
        {
            dir.ListDirBegin();
            string fileName = dir.GetNext();
            while (fileName != "")
            {
                if (fileName.EndsWith(".tres"))
                {
                    var clipPath = $"{hardPath}/{fileName}";
                    AudioEvent audioEvent = GD.Load<AudioEvent>(clipPath);
                    if (audioEvent.IsMusic)
                        musicTemp.Add(audioEvent);
                    else
                        audioTemp.Add(audioEvent);
                    GD.Print($"AudioManager: Cached audio clip from {clipPath}");
                }
                fileName = dir.GetNext();
            }
            dir.ListDirEnd();
            _audioClips = [.. audioTemp];
            _musicClips = [..musicTemp];
        } else
            GD.PrintErr("AudioManager: Failed to open audio resources directory.");
    }
}