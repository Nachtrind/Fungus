using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SoundSet: ScriptableObject
{
    public List<Sound> sounds = new List<Sound>();

    public AudioClip GetClip(ClipType type, out ClipPlayType playType)
    {
        for (int i = 0; i < sounds.Count; i++)
        {
            if (sounds[i].type == type)
            {
                playType = sounds[i].playType;
                return sounds[i].clip;
            }
        }
        playType = ClipPlayType.OneShot;
        return null;
    }

    public enum ClipType
    {
        Idle,
        Moving,
        ReceiveDamage,
        DangerSighted,
        Aggroed,
        Alarmed,
        Attack,
        Death
    }

    public enum ClipPlayType
    {
        OneShot,
        Continuous
    }

    [System.Serializable]
    public class Sound
    {
        public ClipType type;
        public ClipPlayType playType;
        public AudioClip clip;
    }
}
