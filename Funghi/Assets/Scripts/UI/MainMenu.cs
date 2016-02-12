using System.Collections;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] CanvasGroup canvasGroup;

    Coroutine fadeRoutine;

    public void OnPlayButtonClick()
    {  
        CloseMenu();
    }

    public void OpenMenu()
    {
        GameWorld.Instance.IsPaused = true;
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(FadeIn());
    }

    void CloseMenu()
    {
        GameWorld.Instance.IsPaused = false;
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(FadeOut());
    }

    public void OnRestartButtonClick()
    {
        GameWorld.Instance.RestartCurrentLevel();
        CloseMenu();
    }

    IEnumerator FadeOut()
    {
        var t = Time.realtimeSinceStartup;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        while (Time.realtimeSinceStartup - t < 1f)
        {
            canvasGroup.alpha = 1-Mathf.Clamp01(Time.realtimeSinceStartup - t);
            yield return null;
        }
        canvasGroup.alpha = 0;
    }

    IEnumerator FadeIn()
    {
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        var t = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup - t < 1f)
        {
            canvasGroup.alpha = Mathf.Clamp01(Time.realtimeSinceStartup - t);
            yield return null;
        }
        canvasGroup.alpha = 1;
    }
}