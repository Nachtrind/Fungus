using UnityEngine;
using System.Collections;

public class AbilityFader : MonoBehaviour
{
    [SerializeField] CanvasGroup fader;

    Coroutine routine;

    Vector2 startPos;

    public Vector2 inactiveOffset;

    RectTransform rt;

    void Start()
    {
        rt = transform as RectTransform;
        startPos = rt.anchoredPosition;
    }

    public void FadeIn(float duration)
    {
        if (routine != null)
        {
            StopCoroutine(routine);
        }
        routine = StartCoroutine(FadeRoutine(duration, 1f));
    }

    public void FadeOut(float duration)
    {
        if (routine != null)
        {
            StopCoroutine(routine);
        }
        routine = StartCoroutine(FadeRoutine(duration, 0f));
    }

    IEnumerator FadeRoutine(float duration, float targetAlpha)
    {
        targetAlpha = Mathf.Clamp01(targetAlpha);
        fader.blocksRaycasts = Mathf.Approximately(targetAlpha, 1f);
        while (!Mathf.Approximately(fader.alpha, targetAlpha))
        {
            fader.alpha = Mathf.Clamp01(Mathf.MoveTowards(fader.alpha, targetAlpha, Time.deltaTime/duration));
            rt.anchoredPosition = Vector2.Lerp(startPos + inactiveOffset, startPos, fader.alpha);
            yield return 0;
        }
    }
}
