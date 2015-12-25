using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu]
public class StandardGameSettings:ScriptableObject
{

    static StandardGameSettings cachedSettings;
    public static StandardGameSettings Get
    {
        get
        {
            if (cachedSettings == null) { cachedSettings = Resources.Load<StandardGameSettings>("StandardGameSettings"); }
            return cachedSettings;
        }
    }

    [Header("Sound")]
    public AudioMixer masterMixer;
    public AudioMixerGroup entityGroup;
    public AudioMixerGroup ambienceGroup;
    public AudioMixerGroup musicGroup;
    [Range(0, 0.5f)]
    public float entityPitchRandomization = 0.05f;
    [Header("AudioSource Presets")]
    public float baseMaxDistance = 10;
    [Range(0, 1f)]
    public float baseVolume = 0.25f;

    public void ApplyEntitySoundStandards(AudioSource aus)
    {
        aus.rolloffMode = AudioRolloffMode.Logarithmic;
        aus.outputAudioMixerGroup = entityGroup;
        aus.volume = baseVolume;
        aus.maxDistance = baseMaxDistance;
        aus.minDistance = 1;
        aus.spread = 0;
        aus.spatialBlend = 0.25f;
        aus.reverbZoneMix = 1;
        aus.loop = true;
        aus.playOnAwake = false;
        aus.pitch = 1;
        aus.dopplerLevel = 1;
    }

    public void ApplyAmbienceSoundStandards(AudioSource aus)
    {
        aus.rolloffMode = AudioRolloffMode.Logarithmic;
        aus.outputAudioMixerGroup = ambienceGroup;
        aus.volume = baseVolume;
        aus.maxDistance = baseMaxDistance;
        aus.minDistance = 1;
        aus.spread = 0;
        aus.spatialBlend = 0f;
        aus.reverbZoneMix = 0;
        aus.loop = true;
        aus.playOnAwake = true;
        aus.pitch = 1;
        aus.dopplerLevel = 1;
    }

    public void ApplyMusicStandards(AudioSource aus)
    {
        aus.rolloffMode = AudioRolloffMode.Linear;
        aus.outputAudioMixerGroup = musicGroup;
        aus.volume = baseVolume;
        aus.maxDistance = baseMaxDistance;
        aus.minDistance = 1;
        aus.spread = 0;
        aus.spatialBlend = 0f;
        aus.reverbZoneMix = 0;
        aus.loop = true;
        aus.playOnAwake = true;
        aus.pitch = 1;
        aus.dopplerLevel = 1;
    }
}
