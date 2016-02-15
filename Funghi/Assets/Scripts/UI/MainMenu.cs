using System.Collections;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] CanvasGroup canvasGroup;

    Coroutine fadeRoutine;

    [SerializeField] RectTransform[] states;


    public void OnPlayButtonClick()
    {  
        CloseMenu();
    }

    public void OpenMenu(int stateID = 0)
    {
        for (var i = 0; i < states.Length; i++)
        {
            if (i == stateID)
            {
                states[i].gameObject.SetActive(true);
            }
            else
            {
                states[i].gameObject.SetActive(false);
            }
        }
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