using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class NewsTicker : MonoBehaviour
{

    static NewsTicker instance;

    class TransformTrack
    {
        public readonly RectTransform t;
        public bool triggered;
        public  TransformTrack(RectTransform rt)
        {
            t = rt;
            triggered = false;
        }
    }

    [SerializeField] NewsTickerDataCollection tickerData;

    [SerializeField] CanvasGroup fader;
    public float fadeSpeed = 1f;

    float fadeTarget = 0;

    [SerializeField] RectTransform upperPanel;
    [SerializeField] RectTransform lowerPanel;

    public float upperPanelSpeed = 1f;
    public float lowerPanelSpeed = 1f;

    public Font upperPanelFont;
    public Font lowerPanelFont;

    public int upperPanelFontSize = 10;
    public int lowerPanelFontSize = 14;

    List<TransformTrack> lowerPanelItems = new List<TransformTrack>();
    List<TransformTrack> upperPanelItems = new List<TransformTrack>();
    Coroutine scrollRoutine;
    Coroutine fadeRoutine;

    void Awake()
    {
        instance = this;
        fader.alpha = 0;
        InvokeRepeating("RegularCheck", 1f, 1f);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            NewsTickerCategory rnd = (NewsTickerCategory) Random.Range(0, 8);
            Trigger(rnd);
        }
    }

    Dictionary<NewsTickerCategory, Queue<NewsTickerDataCollection.TickerEntry>> queue =
        new Dictionary<NewsTickerCategory, Queue<NewsTickerDataCollection.TickerEntry>>();

    public static int ItemsInQueue
    {
        get
        {
            if (instance == null)
            {
                return -1;
            }
            return instance.GetItemsInQueue();
        }
    }

    int GetItemsInQueue()
    {
        int count = 0;
        foreach (var tdata in queue.Values)
        {
            count += tdata.Count;
        }
        return count;
    }

    public static void Trigger(NewsTickerCategory category)
    {
        var ticker = instance;
        if (!instance)
        {
            return;
        }
        var data = ticker.tickerData.GetTickerEntry(category);
        ticker.Enqueue(data, category);
    }

    void Enqueue(NewsTickerDataCollection.TickerEntry entry, NewsTickerCategory category)
    {
        if (!queue.ContainsKey(category))
        {
            queue.Add(category, new Queue<NewsTickerDataCollection.TickerEntry>());
        }
        queue[category].Enqueue(entry);
    }

    bool GetNextQueueItem(NewsTickerCategory category, out string text)
    {
        if (!queue.ContainsKey(category))
        {
            text = string.Empty;
            return false;
        }
        if (queue[category].Count > 0)
        {
            text = queue[category].Dequeue().text;
            return true;
        }
        text = string.Empty;
        return false;
    }

    void DisplayNextItem(RectTransform panel)
    {
        //higher to lower priority
        string txt;
        if (panel == lowerPanel)
        {
            for (int i = 8; i-- > 4;)
            {
                if (GetNextQueueItem((NewsTickerCategory) i, out txt))
                {
                    lowerPanelItems.Add(CreateTextPlate(txt, lowerPanel, lowerPanelFontSize, lowerPanelFont));
                    StartRoutineIfNotRunning();
                    return;
                }
            }
        }
        if (panel == upperPanel)
        {
            for (int i = 3; i-- > 0;)
            {
                if (GetNextQueueItem((NewsTickerCategory) i, out txt))
                {
                    upperPanelItems.Add(CreateTextPlate(txt, upperPanel, upperPanelFontSize, upperPanelFont));
                    StartRoutineIfNotRunning();
                    return;
                }
            }
        }
    }

    void StartRoutineIfNotRunning()
    {
        if (scrollRoutine == null)
        {
            scrollRoutine = StartCoroutine(ScrollPlates());
        }
        FadeTo(1f);
    }

    TransformTrack CreateTextPlate(string txt, RectTransform panel, int fontSize, Font font)
    {
        var txtPlate = new GameObject("TickerEntry");
        txtPlate.transform.parent = panel;
        var t = txtPlate.AddComponent<Text>();
        t.text = string.Format("{0}  - ", txt);
        t.fontSize = fontSize;
        t.fontStyle = FontStyle.Bold;
        t.font = font;
        t.alignment = TextAnchor.MiddleLeft;
        t.horizontalOverflow = HorizontalWrapMode.Overflow;
        t.verticalOverflow = VerticalWrapMode.Overflow;
        var rt = txtPlate.transform as RectTransform;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        var csf = txtPlate.AddComponent<ContentSizeFitter>();
        csf.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        rt.localScale = Vector3.one;
        Canvas.ForceUpdateCanvases();
        var right = panel.rect.x + (panel.rect.width*0.5f);
        right += (rt.rect.width) * 0.5f;
        rt.anchoredPosition = new Vector2(right, 0);
        return new TransformTrack(rt);
    }

    IEnumerator ScrollPlates()
    {
        while (upperPanelItems.Count > 0 | lowerPanelItems.Count > 0)
        {
            for (int i = upperPanelItems.Count; i-- > 0;)
            {
                TransformTrack upItem = upperPanelItems[i];
                upItem.t.anchoredPosition = new Vector2(upItem.t.anchoredPosition.x - upperPanelSpeed, upItem.t.anchoredPosition.y);
                //_______________________________________________________________________________
                if (!upItem.triggered && upItem.t.anchoredPosition.x+upItem.t.rect.width*0.5f < upperPanel.rect.x+upperPanel.rect.width*0.5f)
                {
                    upItem.triggered = true;
                    DisplayNextItem(upperPanel);
                }
                //_______________________________________________________________________________
                if (upperPanel.anchoredPosition.x-(upperPanel.rect.width*0.5f) > upItem.t.anchoredPosition.x+upItem.t.rect.width*0.5f)
                {
                    Destroy(upItem.t.gameObject);
                    upperPanelItems.RemoveAt(i);
                }
            }
            for (int i = lowerPanelItems.Count; i-- > 0;)
            {
                TransformTrack lowItem = lowerPanelItems[i];
                lowItem.t.anchoredPosition = new Vector2(lowItem.t.anchoredPosition.x - lowerPanelSpeed, lowItem.t.anchoredPosition.y);
                //_______________________________________________________________________________
                if (!lowItem.triggered && lowItem.t.anchoredPosition.x+lowItem.t.rect.width*0.5f < lowerPanel.rect.x+lowerPanel.rect.width*0.5f)
                {
                    lowItem.triggered = true;
                    DisplayNextItem(lowerPanel);
                }
                //_______________________________________________________________________________
                if (lowerPanel.anchoredPosition.x-(lowerPanel.rect.width*0.5f) > lowItem.t.anchoredPosition.x+lowItem.t.rect.width*0.5f)
                {
                    Destroy(lowItem.t.gameObject);
                    lowerPanelItems.RemoveAt(i);
                }
            }
            yield return null;
        }
        scrollRoutine = null;
        FadeTo(0f);
    }

    void FadeTo(float value)
    {
        if (fadeRoutine != null)
        {
            StopCoroutine(fadeRoutine);
        }
        fadeTarget = value;
        fadeRoutine = StartCoroutine(Fade());
    }

    IEnumerator Fade()
    {
        float t = 0;
        while (fader.alpha != fadeTarget)
        {
            fader.alpha = Mathf.MoveTowards(fader.alpha, fadeTarget, t);
            t += Time.deltaTime*fadeSpeed;
            yield return null;
        }
        fadeRoutine = null;
    }

    void RegularCheck()
    {
        if (upperPanelItems.Count == 0)
        {
            DisplayNextItem(upperPanel);
        }
        if (lowerPanelItems.Count == 0)
        {
            DisplayNextItem(lowerPanel);
        }
    }

}
