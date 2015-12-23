using ModularBehaviour;
using UnityEditor;
using UnityEngine;
using System.Reflection;
using System;
using System.Linq;
using System.Collections.Generic;

public class IntelligenceEditorWindow : EditorWindow
{

    [MenuItem("Edit/Behaviours/Editor")]
    public static void Open()
    {
        GetWindow<IntelligenceEditorWindow>("IntelligenceEditor");
    }

    List<Type> oneShotActions = new List<Type>();
    string[] oneShotActionNames = new string[0];
    List<Type> regularActions = new List<Type>();
    string[] regularActionNames = new string[0];
    List<Type> triggerActions = new List<Type>();
    string[] triggerActionNames = new string[0];

    public Intelligence target;

    void CacheUsableTypes()
    {
        string defaultIndicator = "Add";
        oneShotActions = Assembly.GetAssembly(typeof(Intelligence)).GetTypes().Where(i => i.IsClass && !i.IsAbstract && i.IsSubclassOf(typeof(OneShotAction))).ToList();
        oneShotActionNames = new string[oneShotActions.Count + 1];
        oneShotActionNames[0] = defaultIndicator;
        for (int i = 0; i < oneShotActions.Count; i++)
        {
            oneShotActionNames[i + 1] = oneShotActions[i].Name;
        }
        regularActions = Assembly.GetAssembly(typeof(Intelligence)).GetTypes().Where(i => i.IsClass && !i.IsAbstract && i.IsSubclassOf(typeof(AIAction))).ToList();
        regularActionNames = new string[regularActions.Count + 1];
        regularActionNames[0] = defaultIndicator;
        for (int i = 0; i < regularActions.Count; i++)
        {
            regularActionNames[i + 1] = regularActions[i].Name;
        }
        triggerActions = Assembly.GetAssembly(typeof(Intelligence)).GetTypes().Where(i => i.IsClass && !i.IsAbstract && i.IsSubclassOf(typeof(TriggerAction))).ToList();
        triggerActionNames = new string[triggerActions.Count + 1];
        triggerActionNames[0] = defaultIndicator;
        for (int i = 0; i < triggerActions.Count; i++)
        {
            triggerActionNames[i + 1] = triggerActions[i].Name;
        }
    }

    Type OneShotPopup()
    {
        int i = EditorGUILayout.Popup(0, oneShotActionNames);
        if (i > 0)
        {
            return oneShotActions[i - 1];
        }
        return null;
    }

    Type RegularActionPopup()
    {
        int i = EditorGUILayout.Popup(0, regularActionNames);
        if (i > 0)
        {
            return regularActions[i - 1];
        }
        return null;
    }

    Type TriggerActionPopup()
    {
        int i = EditorGUILayout.Popup(0, triggerActionNames);
        if (i > 0)
        {
            return triggerActions[i - 1];
        }
        return null;
    }

    CallbackCollection callbackCollection;

    void OnEnable()
    {
        callbackCollection = new CallbackCollection();
        callbackCollection.AddAsset = AddToAsset;
        callbackCollection.RemoveAsset = RemoveAsset;
        callbackCollection.OneShotPopup = OneShotPopup;
        callbackCollection.RegularActionPopup = RegularActionPopup;
        callbackCollection.TriggerActionPopup = TriggerActionPopup;
        CacheUsableTypes();
    }

    void OnDisable()
    {
        if (target)
        {
            EditorUtility.SetDirty(target);
        }
    }

    void AddToAsset(ScriptableObject instance)
    {
        instance.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
        AssetDatabase.AddObjectToAsset(instance, target);
    }

    void RemoveAsset(ScriptableObject instance)
    {
        DestroyImmediate(instance, true);
    }

    const string downSymbol = "˅";
    const string upSymbol = "˄";
    const int symbolWidth = 20;
    Vector2 scrollPos;

    void OnGUI()
    {
        scrollPos = GUILayout.BeginScrollView(scrollPos);
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.BeginVertical(GUILayout.Width(200));
        GUILayout.Space(4);
        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox, GUILayout.Width(200));
        GUILayout.Label("Target:", GUILayout.Width(45));
        target = EditorGUILayout.ObjectField(target, typeof(Intelligence), false, GUILayout.Width(160)) as Intelligence;
        EditorGUILayout.EndHorizontal();
        if (!target) { return; }
        Intelligence intel = target;
        EditorGUILayout.BeginHorizontal();
        if (intel.states.Count > 0)
        {
            SerializedObject serializedObject = new SerializedObject(intel);
            SerializedProperty startState = serializedObject.FindProperty("activeState");
            if (startState != null)
            {
                GUI.backgroundColor = Color.gray;
                GUILayout.BeginHorizontal(EditorStyles.helpBox);
                GUI.backgroundColor = Color.white;
                GUILayout.Label("Start state:", GUILayout.Width(70));
                if (startState.objectReferenceValue != null)
                {
                    GUILayout.Label(startState.objectReferenceValue.name);
                    if (GUILayout.Button("change", EditorStyles.miniButton, GUILayout.Width(50)))
                    {
                        startState.objectReferenceValue = null;
                        serializedObject.ApplyModifiedProperties();
                    }
                }
                else
                {
                    List<string> stateNames = new List<string>();
                    stateNames.Add("Select");
                    for (int i = 0; i < intel.states.Count; i++)
                    {
                        stateNames.Add(intel.states[i].name);
                    }
                    int selectedStartState = EditorGUILayout.Popup(0, stateNames.ToArray());
                    if (selectedStartState > 0)
                    {
                        startState.objectReferenceValue = intel.states[selectedStartState - 1];
                        serializedObject.ApplyModifiedProperties();
                    }
                }
                GUILayout.EndHorizontal();
            }
        }
        EditorGUILayout.EndHorizontal();
        if (GUILayout.Button("Add new State", EditorStyles.miniButton))
        {
            IntelligenceState iState = CreateInstance<IntelligenceState>();
            iState.name = "New State " + intel.states.Count;
            intel.states.Add(iState);
            AddToAsset(iState);
        }
        if (intel.states.Count > 0)
        {
            GUILayout.Label("Triggers:");
            for (int i = 0; i < intel.triggers.Count; i++)
            {
                bool delete = false;
                GUI.backgroundColor = Color.gray;
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                GUI.backgroundColor = Color.white;
                EditorGUILayout.BeginHorizontal();
                intel.triggers[i].trigger = EditorGUILayout.TextField(intel.triggers[i].trigger);
                if (GUILayout.Button("x", EditorStyles.miniButton, GUILayout.Width(20)))
                {
                    delete = true;
                }
                EditorGUILayout.EndHorizontal();
                if (intel.triggers[i].action != null)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(intel.triggers[i].action.name);
                    if (GUILayout.Button("x", EditorStyles.miniButton, GUILayout.Width(20)))
                    {
                        RemoveAsset(intel.triggers[i].action);
                        intel.triggers[i].action.OnDelete(callbackCollection);
                        intel.triggers[i].action = null;
                    }
                    GUILayout.EndHorizontal();
                    if (intel.triggers[i].action != null)
                    {
                        intel.triggers[i].action.DrawGUI(intel, callbackCollection);
                    }
                }
                else
                {
                    Type res = TriggerActionPopup();
                    if (res != null)
                    {
                        TriggerAction os = CreateInstance(res) as TriggerAction;
                        os.name = res.Name;
                        intel.triggers[i].action = os;
                        AddToAsset(os);
                    }
                }
                EditorGUILayout.EndVertical();
                if (delete)
                {
                    if (intel.triggers[i].action != null)
                    {
                        RemoveAsset(intel.triggers[i].action);
                        intel.triggers[i].action.OnDelete(callbackCollection);
                    }
                    intel.triggers.RemoveAt(i);
                }
            }
            if (GUILayout.Button("Add Trigger"))
            {
                intel.triggers.Add(new Intelligence.ActionTrigger("Trigger "+intel.triggers.Count.ToString()));
            }
        }
        EditorGUILayout.EndVertical();

        GUILayout.BeginHorizontal();
        for (int i = 0; i < intel.states.Count; i++)
        {
            bool delete = false;

            GUI.backgroundColor = intel.IsActiveState(intel.states[i].name) ? Color.black : Color.gray;
            EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(150));
            GUI.backgroundColor = Color.white;

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(i.ToString(), GUILayout.Width(20));
            intel.states[i].name = GUILayout.TextField(intel.states[i].name, 80);
            if (GUILayout.Button("x", EditorStyles.miniButton, GUILayout.Width(22)))
            {
                delete = true;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("On Enter:", EditorStyles.boldLabel);
            for (int oe = 0; oe < intel.states[i].enterActions.Count; oe++)
            {
                bool removeAction = false;
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(intel.states[i].enterActions[oe].name);
                //moving
                if (oe > 0)
                {
                    if (GUILayout.Button(upSymbol, EditorStyles.miniButton, GUILayout.Width(symbolWidth)))
                    {
                        OneShotAction os = intel.states[i].enterActions[oe];
                        intel.states[i].enterActions[oe] = intel.states[i].enterActions[oe - 1];
                        intel.states[i].enterActions[oe - 1] = os;
                    }
                }
                if (oe < intel.states[i].enterActions.Count - 1)
                {
                    if (GUILayout.Button(downSymbol, EditorStyles.miniButton, GUILayout.Width(symbolWidth)))
                    {
                        OneShotAction os = intel.states[i].enterActions[oe];
                        intel.states[i].enterActions[oe] = intel.states[i].enterActions[oe + 1];
                        intel.states[i].enterActions[oe + 1] = os;
                    }
                }
                //end moving
                if (GUILayout.Button("x", EditorStyles.miniButton, GUILayout.Width(22)))
                {
                    removeAction = true;
                }
                EditorGUILayout.EndHorizontal();
                intel.states[i].enterActions[oe].DrawGUI(intel.states[i], intel, callbackCollection);
                EditorGUILayout.EndVertical();
                if (removeAction)
                {
                    RemoveAsset(intel.states[i].enterActions[oe]);
                    intel.states[i].enterActions[oe].OnDelete(callbackCollection);
                    intel.states[i].enterActions.RemoveAt(oe);
                }
            }
            int selectedEnter = EditorGUILayout.Popup(0, oneShotActionNames);
            if (selectedEnter > 0)
            {
                OneShotAction os = CreateInstance(oneShotActions[selectedEnter - 1]) as OneShotAction;
                os.name = oneShotActions[selectedEnter - 1].Name;
                intel.states[i].enterActions.Add(os);
                AddToAsset(os);
            }

            GUILayout.Label("On Update:", EditorStyles.boldLabel);
            for (int ou = 0; ou < intel.states[i].updateActions.Count; ou++)
            {
                bool removeAction = false;
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(intel.states[i].updateActions[ou].name);
                //moving
                if (ou > 0)
                {
                    if (GUILayout.Button(upSymbol, EditorStyles.miniButton, GUILayout.Width(symbolWidth)))
                    {
                        AIAction os = intel.states[i].updateActions[ou];
                        intel.states[i].updateActions[ou] = intel.states[i].updateActions[ou - 1];
                        intel.states[i].updateActions[ou - 1] = os;
                    }
                }
                if (ou < intel.states[i].updateActions.Count - 1)
                {
                    if (GUILayout.Button(downSymbol, EditorStyles.miniButton, GUILayout.Width(symbolWidth)))
                    {
                        AIAction os = intel.states[i].updateActions[ou];
                        intel.states[i].updateActions[ou] = intel.states[i].updateActions[ou + 1];
                        intel.states[i].updateActions[ou + 1] = os;
                    }
                }
                //end moving
                if (GUILayout.Button("x", EditorStyles.miniButton, GUILayout.Width(22)))
                {
                    removeAction = true;
                }
                EditorGUILayout.EndHorizontal();
                intel.states[i].updateActions[ou].DrawGUI(intel.states[i], intel, callbackCollection);
                EditorGUILayout.EndVertical();
                if (removeAction)
                {
                    RemoveAsset(intel.states[i].updateActions[ou]);
                    intel.states[i].updateActions[ou].OnDelete(callbackCollection);
                    intel.states[i].updateActions.RemoveAt(ou);
                }
            }
            int selectedUpdate = EditorGUILayout.Popup(0, regularActionNames);
            if (selectedUpdate > 0)
            {
                AIAction rs = CreateInstance(regularActions[selectedUpdate - 1]) as AIAction;
                rs.name = regularActions[selectedUpdate - 1].Name;
                intel.states[i].updateActions.Add(rs);
                AddToAsset(rs);
            }

            GUILayout.Label("On Exit:", EditorStyles.boldLabel);
            for (int oe = 0; oe < intel.states[i].exitActions.Count; oe++)
            {
                bool removeAction = false;
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(intel.states[i].exitActions[oe].name);
                //moving
                if (oe > 0)
                {
                    if (GUILayout.Button(upSymbol, EditorStyles.miniButton, GUILayout.Width(symbolWidth)))
                    {
                        OneShotAction os = intel.states[i].exitActions[oe];
                        intel.states[i].exitActions[oe] = intel.states[i].exitActions[oe - 1];
                        intel.states[i].exitActions[oe - 1] = os;
                    }
                }
                if (oe < intel.states[i].exitActions.Count - 1)
                {
                    if (GUILayout.Button(downSymbol, EditorStyles.miniButton, GUILayout.Width(symbolWidth)))
                    {
                        OneShotAction os = intel.states[i].exitActions[oe];
                        intel.states[i].exitActions[oe] = intel.states[i].exitActions[oe + 1];
                        intel.states[i].exitActions[oe + 1] = os;
                    }
                }
                //end moving
                if (GUILayout.Button("x", EditorStyles.miniButton, GUILayout.Width(22)))
                {
                    removeAction = true;
                }
                EditorGUILayout.EndHorizontal();
                intel.states[i].exitActions[oe].DrawGUI(intel.states[i], intel, callbackCollection);
                EditorGUILayout.EndVertical();   
                if (removeAction)
                {
                    RemoveAsset(intel.states[i].exitActions[oe]);
                    intel.states[i].exitActions[oe].OnDelete(callbackCollection);
                    intel.states[i].exitActions.RemoveAt(oe);
                }   
            }
            int selectedExit = EditorGUILayout.Popup(0, oneShotActionNames);
            if (selectedExit > 0)
            {
                OneShotAction os = CreateInstance(oneShotActions[selectedExit - 1]) as OneShotAction;
                os.name = oneShotActions[selectedExit - 1].Name;
                intel.states[i].exitActions.Add(os);
                AddToAsset(os);
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndVertical();

            if (delete)
            {
                RemoveAsset(intel.states[i]);
                intel.states[i].OnDelete(callbackCollection);
                intel.states.RemoveAt(i);
                break;
            }
        }
        GUILayout.EndHorizontal();

        EditorGUILayout.EndHorizontal();
        GUILayout.EndScrollView();
    }
}
