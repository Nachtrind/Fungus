using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Spawner.Modules;

[CustomEditor(typeof(EntitySpawner))]
public class EnemySpawnerEditor : Editor
{

    Dictionary<Type, List<ModuleInfo>> cachedModuleTypes = new Dictionary<Type, List<ModuleInfo>>();

    class ModuleInfo
    {
        public Type type;
        public string typeName;
        public ModuleInfo(Type type, string name)
        {
            this.type = type;
            typeName = name;
        }
    }

    void OnEnable()
    {
        cachedModuleTypes.Clear();
        CacheModules(typeof(SpawnModule));
        CacheModules(typeof(PositionModule));
        CacheModules(typeof(BehaviourModule));
        CacheModules(typeof(EventModule));
    }

    void CacheModules(Type baseType)
    {
        List<Type> moduleTypes = Assembly.GetAssembly(typeof(EntitySpawner)).GetTypes().Where(i => i.IsClass && i.IsSubclassOf(baseType)).ToList();
        List<ModuleInfo> infos = new List<ModuleInfo>();
        for (int i = 0; i < moduleTypes.Count; i++)
        {
            infos.Add(new ModuleInfo(moduleTypes[i], moduleTypes[i].Name));
        }
        cachedModuleTypes.Add(baseType, infos);
    }

    string GetModuleDescription(Type moduleType)
    {
        object[] attributes = moduleType.GetCustomAttributes(typeof(ModuleDescription), false);
        if (attributes.Length > 0)
        {
            return ((ModuleDescription)attributes[0]).description;
        }
        return string.Empty;
    }

    public override void OnInspectorGUI()
    {
        EntitySpawner esp = target as EntitySpawner;
        GUILayout.BeginVertical(EditorStyles.helpBox);
        esp.autoActivateOnStart = EditorGUILayout.Toggle("AutoStart", esp.autoActivateOnStart);
        if (!esp.autoActivateOnStart)
        {
            EditorGUILayout.HelpBox("Activate() must be called manually", MessageType.Info);
        }
        if (DrawModule("spawnModule", "Spawn", typeof(SpawnModule)))
        {
            DrawModule("positionModule", "Position", typeof(PositionModule));
            DrawModule("behaviourModule", "Behaviour", typeof(BehaviourModule));
            DrawModule("eventModule", "Event", typeof(EventModule));
        }
        else
        {
            ResetModule("positionModule");
            ResetModule("behaviourModule");
            ResetModule("eventModule");
        }
        GUILayout.EndVertical();
    }

    void OnSceneGUI()
    {
        SerializedProperty module = serializedObject.FindProperty("positionModule");
        if (module != null)
        {
            PositionModule pm = module.objectReferenceValue as PositionModule;
            if (pm != null)
            {
                pm.DrawHandles();
            }
        }
    }

    void ResetModule(string propertyName)
    {
        SerializedProperty module = serializedObject.FindProperty(propertyName);
        if (module != null)
        {
            if (module.objectReferenceValue != null)
            {
                DestroyImmediate(module.objectReferenceValue, false);
            }
            module.serializedObject.ApplyModifiedProperties();
        }
    }

    bool DrawModule(string propertyName, string label, Type baseType)
    {
        SerializedProperty module = serializedObject.FindProperty(propertyName);
        if (module == null) { EditorGUILayout.HelpBox("Script property misconfiguration", MessageType.Error); return false; }
        GUILayout.Label(label, EditorStyles.boldLabel);
        if (module.objectReferenceValue != null)
        {
            bool ret = true;
            SerializedObject so = new SerializedObject(module.objectReferenceValue);
            GUI.backgroundColor = Color.grey;
            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUI.backgroundColor = Color.white;
            GUILayout.BeginHorizontal(EditorStyles.helpBox);
            GUILayout.Label(so.targetObject.GetType().Name);
            if (GUILayout.Button("x", GUILayout.Width(20)))
            {
                ret = false;
            }
            GUILayout.EndHorizontal();
            string desc = GetModuleDescription(so.targetObject.GetType());
            if (desc != string.Empty)
            {
                EditorGUILayout.HelpBox(desc, MessageType.None);
            }
            SerializedProperty sp = so.GetIterator();
            sp.NextVisible(true);
            if (sp != null)
            {
                while (sp.NextVisible(false))
                {
                    if (sp.isArray & sp.propertyType != SerializedPropertyType.String)
                    {
                        GUILayout.Label(sp.displayName, GUILayout.ExpandWidth(true));
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(12);
                        sp.isExpanded = EditorGUILayout.Foldout(sp.isExpanded, "");
                        GUILayout.EndHorizontal();
                        if (sp.isExpanded)
                        {
                            for (int i = 0; i < sp.arraySize; i++)
                            {
                                bool deleteEntry = false;
                                GUILayout.BeginHorizontal();
                                EditorGUILayout.PropertyField(sp.GetArrayElementAtIndex(i));
                                if (GUILayout.Button("x", EditorStyles.miniButton))
                                {
                                    deleteEntry = true;
                                }
                                GUILayout.EndHorizontal();
                                if (deleteEntry)
                                {
                                    sp.DeleteArrayElementAtIndex(i);
                                    break;
                                }
                            }
                        }
                        if (GUILayout.Button("Add"))
                        {
                            sp.InsertArrayElementAtIndex(sp.arraySize);
                        }
                    }
                    else
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(sp.displayName, GUILayout.ExpandWidth(true));
                        EditorGUILayout.PropertyField(sp, GUIContent.none);
                        GUILayout.EndHorizontal();
                    }
                }
            }
            GUILayout.EndVertical();
            so.ApplyModifiedProperties();
            if (ret == false)
            {
                DestroyImmediate(module.objectReferenceValue);
            }
        }
        else
        {
            List<ModuleInfo> modules;
            if (cachedModuleTypes.TryGetValue(baseType, out modules))
            {
                string[] names = new string[modules.Count + 1];
                names[0] = "Select";
                for (int i = 0; i < modules.Count; i++)
                {
                    names[i+1] = modules[i].typeName;
                }
                int selected = EditorGUILayout.Popup(0, names);
                if (selected > 0)
                {
                    module.objectReferenceValue = CreateInstance(modules[selected - 1].type);
                    module.serializedObject.ApplyModifiedProperties();
                }
            }
        }
        return module.objectReferenceValue != null;
    }
}
