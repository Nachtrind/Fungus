using System;
using System.Collections.Generic;
using UnityEngine;
using NodeAbilities;
using System.IO;

[Serializable]
public static class PlayerRecord
{

    static string name;
    public static string Name { get { return name; } }

    public static string LastLevel = "";

    static List<NodeAbility> availableAbilities = new List<NodeAbility>();

    static List<NodeAbility> unlockedAbilities = new List<NodeAbility>();
    public static List<NodeAbility> UnlockedAbilities { get { return unlockedAbilities; } }

    public static bool UnlockAbility(NodeAbility ability)
    {
        if (!unlockedAbilities.Contains(ability))
        {
            unlockedAbilities.Add(ability);
            return true;
        }
        return false;
    }

    public static void Initialize(string playerName)
    {
        name = playerName;
        LoadAvailableAbilities();
    }

    static void LoadAvailableAbilities()
    {
        availableAbilities.Clear();
        availableAbilities.AddRange(Resources.LoadAll<NodeAbility>("NodeAbilities"));
    }

    #region LoadingSaving
    const string gameIdentifier = "Fungus";
    const char separator = '#';
    public static void LoadSavedState()
    {
        string serializedRecord = PlayerPrefs.GetString(gameIdentifier + name);
        if (serializedRecord == string.Empty || string.IsNullOrEmpty(serializedRecord) || serializedRecord == "") { return; }
        using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(serializedRecord)))
        using (BinaryReader br = new BinaryReader(ms))
        {
            LastLevel = br.ReadString();
            int numUnlockedAbilities = br.ReadInt32();
            for (int i = 0; i < numUnlockedAbilities; i++)
            {
                string abilityName = br.ReadString();
                NodeAbility ability = GetAbility(abilityName);
                if (ability == null) { Debug.LogError("Couldn't load ability " + abilityName); }
                unlockedAbilities.Add(ability);
            }
        }
    }

    static NodeAbility GetAbility(string abilityidentifier)
    {
        for (int i = 0; i < availableAbilities.Count; i++)
        {
            AbilityIdentifier[] ai = availableAbilities[i].GetType().GetCustomAttributes(typeof(AbilityIdentifier), false) as AbilityIdentifier[];
            if (ai[0].identifier.Equals(abilityidentifier, StringComparison.OrdinalIgnoreCase))
            {
                return availableAbilities[i];
            }
        }
        return null;
    }

    public static void SaveCurrentState()
    {
        using (MemoryStream ms = new MemoryStream())
        using (BinaryWriter bw = new BinaryWriter(ms))
        {
            bw.Write(LastLevel);
            bw.Write(unlockedAbilities.Count);
            for (int i = 0; i < unlockedAbilities.Count; i++)
            {
                AbilityIdentifier[] ai = unlockedAbilities[i].GetType().GetCustomAttributes(typeof(AbilityIdentifier), false) as AbilityIdentifier[];
                bw.Write(ai[0].identifier);
            }
            PlayerPrefs.SetString(gameIdentifier + name, Convert.ToBase64String(ms.GetBuffer()));
        }
    }

    #endregion

}
