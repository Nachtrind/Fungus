#if DEBUG
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using ModularBehaviour;

public class DebugHelper : MonoBehaviour
{
    float smoothedFps;
    List<float> lastFrames = new List<float>();
    float lastUpdate = 0;
    bool showDebug = false;
    bool showEntityDebug = false;
    bool restartConfirmation = false;
    Material groundMaterialCached;
    List<DisplayObject> displayCallbacks = new List<DisplayObject>();
    List<LogMessage> lastConsoleMessages = new List<LogMessage>();
    LogMessage activeDisplayedMessage;

    class LogMessage: IEquatable<LogMessage>
    {
        public readonly string msg = "None";
        public readonly string stackTrace = "Empty";
        public LogType type;
        public LogMessage(string msg, string stackTrace, LogType type)
        {
            this.msg = msg;
            this.stackTrace = stackTrace;
            this.type = type;
        }

        public bool Equals(LogMessage other)
        {
            return (other.type == type & other.msg.Length == msg.Length);
        }

        public override bool Equals(object obj)
        {
            LogMessage lm = obj as LogMessage;
            if (lm == null) { return false; }
            return Equals(lm);
        }

        public override int GetHashCode() //just to hide the error
        {
            return GetHashCode();
        }

    }

    class DisplayObject
    {
        public readonly string name;
        public readonly Func<string> callback;
        public readonly bool newLineAfterTitle = false;
        public DisplayObject(string name, Func<string> callback, bool newLineAfterTitle) { this.name = name;this.callback = callback; this.newLineAfterTitle = newLineAfterTitle; }
    }

    public void AddDebugValue(string name, Func<string> callback, bool newLineAfterTitle = false)
    {
        displayCallbacks.Add(new DisplayObject(name, callback, newLineAfterTitle));
    }

    #region Retriever functions

    void ConsoleLogCallback(string condition, string stackTrace, LogType type)
    {
        LogMessage msg = new LogMessage(condition, stackTrace, type);
        if (lastConsoleMessages.Count > 0 && msg.Equals(lastConsoleMessages[lastConsoleMessages.Count-1]))
        {
            return;
        }
        lastConsoleMessages.Add(msg);
        if (lastConsoleMessages.Count > 5)
        {
            lastConsoleMessages.RemoveAt(0);
        }
    }

    string GetFPS()
    {
        return smoothedFps.ToString();
    }

    string GetDeviceOrientation()
    {
        return Screen.orientation.ToString();
    }

    string GetNumHumans()
    {
        return GameWorld.Instance.Humans.Count.ToString();
    }

    string GetNumOfficers()
    {
        return GameWorld.Instance.Humans.Where(p => p.Behaviour.name.Contains("Police")).ToList().Count.ToString();
    }

    string GetNumPoliceCars()
    {
        return GameWorld.Instance.PoliceCars.Count.ToString();
    }

    string GetNumPoliceStations()
    {
        return GameWorld.Instance.PoliceStations.Count.ToString();
    }

    string GetNumNodes()
    {
        return GameWorld.Instance.Nodes.Count.ToString();
    }
    #endregion

    void OnLevelWasLoaded()
    {
        OnEnable();
    }

    void OnEnable()
    {
        DontDestroyOnLoad(gameObject);
        var slimeRenderer = FindObjectOfType<SlimeRenderer>();
        if (slimeRenderer)
        {
            groundMaterialCached = slimeRenderer.groundMaterial;
        }
        Application.logMessageReceived -= ConsoleLogCallback;
        Application.logMessageReceived += ConsoleLogCallback;
        displayCallbacks.Clear();
        RegisterCallbacks();
    }

    void OnDisable()
    {
        Application.logMessageReceived -= ConsoleLogCallback;
        showEntityDebug = false;
        showEntityDebug = false;
        Entity.showDebug = false;
        showDebug = false;
    }

	void Update ()
    {
        lastFrames.Add(1f / Time.deltaTime);
        if (Time.time-lastUpdate > 0.25f)
        {
            if (lastFrames.Count > 0)
            {
                for (int i = 0; i < lastFrames.Count; i++)
                {
                    smoothedFps += lastFrames[i];
                }
                smoothedFps /= lastFrames.Count;
                smoothedFps = Mathf.Round(smoothedFps * 10) * 0.1f;
                lastFrames.Clear();
            }
            else
            {
                smoothedFps = 0;
            }
            lastUpdate = Time.time;
        }
	}

    void RegisterCallbacks()
    {
        AddDebugValue("FPS", GetFPS);
        AddDebugValue("Screen Orientation", GetDeviceOrientation);
        AddDebugValue("Humans", GetNumHumans);
        AddDebugValue("Officers", GetNumOfficers);
        AddDebugValue("PoliceCars", GetNumPoliceCars);
        AddDebugValue("PoliceStations", GetNumPoliceStations);
        AddDebugValue("Nodes", GetNumNodes);
    }

    Vector2 detailScroll;
    void OnGUI()
    {
        Rect debugButtonRect = new Rect(Screen.width / 2 - 80, 10, 160, 40);
        if (GUI.Button(debugButtonRect, "Debug")) { showDebug = !showDebug; }
        if (!showDebug) { return; }
        Rect displayRect = new Rect(Screen.width - 320, 0, 300, Screen.height);
        GUILayout.BeginArea(displayRect);
        GUILayout.Space(20);
        bool showStates = GUILayout.Toggle(showEntityDebug, "Entity Debug Infos");
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Force Restart") && restartConfirmation)
        {
            GameWorld.Instance.RestartCurrentLevel();
            restartConfirmation = false;
        }
        GUILayout.Space(10);
        restartConfirmation = GUILayout.Toggle(restartConfirmation, "confirm");
        GUILayout.EndHorizontal();
        if (showStates != showEntityDebug)
        {
            Entity.showDebug = showStates;
            showEntityDebug = showStates;
        }
        for (int i = 0; i < displayCallbacks.Count; i++)
        {
            GUILayout.Label(string.Format("{0}:{1}{2}", displayCallbacks[i].name, displayCallbacks[i].newLineAfterTitle?"\n":" ", displayCallbacks[i].callback().ToString()));
        }
        if (groundMaterialCached)
        {
            GUILayout.Label("Slime Projection:");
            if (groundMaterialCached.mainTexture)
            {
                Rect r = GUILayoutUtility.GetRect(100, 100);
                r.width = r.height = 100;
                GUI.DrawTexture(r, groundMaterialCached.mainTexture);
            }
            else
            {
                GUILayout.Label("Texture empty!");
            }
        }
        GUILayout.Label("Console Output:");
        int count = Mathf.Min(3, lastConsoleMessages.Count);
        for (int i = 0; i < count; i++)
        {
            LogMessage lm = lastConsoleMessages[lastConsoleMessages.Count - count + i];
            string clampedMsg = string.Format("{0}:{1}", lm.type, lm.msg);
            if (clampedMsg.Length > 100) { clampedMsg = string.Format("{0}[..]", clampedMsg.Substring(0, 100)); }
            GUILayout.Label(clampedMsg);
            if (GUILayout.Button("Details"))
            {
                activeDisplayedMessage = lm;
            }
        }
        GUILayout.EndArea();
        if (activeDisplayedMessage != null)
        {
            GUILayout.BeginArea(new Rect(Screen.width / 2 - 200, Screen.height / 2 - 100, 400, 200), GUI.skin.box);
            detailScroll = GUILayout.BeginScrollView(detailScroll);
            GUILayout.Label(activeDisplayedMessage.stackTrace);
            GUILayout.EndScrollView();
            if (GUILayout.Button("Close"))
            {
                activeDisplayedMessage = null;
            }
            GUILayout.EndArea();
        }
    }
}
#endif