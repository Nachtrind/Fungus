using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AudioSource))]
class AudioSourcePresetLoader: DecoratorEditor
{
    public AudioSourcePresetLoader() : base("AudioSourceInspector") { }

    public enum SettingsType { Select, Entity, Ambience, Music }
    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        GUILayout.Label("Load Fungus base settings:");
        SettingsType st = (SettingsType)EditorGUILayout.EnumPopup("For:", SettingsType.Select);
        EditorGUILayout.EndVertical();
        switch (st)
        {
            case SettingsType.Select:
                break;
            case SettingsType.Entity:
                StandardGameSettings.Get.ApplyEntitySoundStandards(target as AudioSource);
                break;
            case SettingsType.Ambience:
                StandardGameSettings.Get.ApplyAmbienceSoundStandards(target as AudioSource);
                break;
            case SettingsType.Music:
                StandardGameSettings.Get.ApplyMusicStandards(target as AudioSource);
                break;
        }
        base.OnInspectorGUI();
    }
}

