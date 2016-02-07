using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ModularBehaviour;

public class UserMenu : MonoBehaviour
{

    public enum UserMenuButtonType
    {
        Skill,
        Brain,
        Build,
        None
    }

    public enum AbilityType
    {
        Eat,
        Lure,
        Enslave,
        Spawn,
        Slow,
        Speedup
    }

    /// <summary>
    /// This event passes the player requested wind direction.
    /// </summary>
    public event System.Action<float> OnRequestWindDirection;

    [SerializeField] AbilityFader abilityFader;
    [SerializeField] Anemometer anemo;

    [SerializeField] RectTransform[] buttons;
    [SerializeField] UserAbilityButton[] abilityButtons;

    readonly float[] _activeButtonPositions = new float[3];
    public float InactiveButtonOffset;
    float _activeMenuPosition;
    public float InactiveMenuOffset;

    public Color AbilityActiveColor;
    public Color AbilityInactiveColor;

    const float StandardFadeTimeAbilities = 0.1f;
    const float StandardFadeTimeButtons = 0.25f;

    bool _abilityRectActive;
    UserMenuButtonType _activeButton = UserMenuButtonType.None;

    void Start()
    {
        for (var i = 1; i < buttons.Length; i++)
        {
            _activeButtonPositions[i-1] = buttons[i].anchoredPosition.x;
        }
        _activeMenuPosition = buttons[0].anchoredPosition.x;
        for (var i = 0; i < abilityButtons.Length; i++)
        {
            if (i > 1)
            {
                abilityButtons[i].SetActivatedState(false, AbilityInactiveColor);
            }
            else
            {
                abilityButtons[i].SetActivatedState(true, AbilityActiveColor);
            }
        }
        BlendOut();
        OnMenuInactive();
    }

    public void OnAnemoRequestsWindDirection(float degree)
    {
        if (OnRequestWindDirection == null) return;
        OnRequestWindDirection(degree);
    }   

    /// <summary>
    /// This opens the menu - TODO
    /// </summary>
    public void OnMenuSelected()
    {
        Debug.Log("Display the menu");
    }

    public void OnAbilitySelected(AbilityType type)
    {
        Debug.Log("Ability selected: " + type);
        ForceBlendOut();
        //implement handling here
    }

    public void OnBrainSelected()
    {
        Debug.Log("Brain selected");
        //implement handling here
    }

    public void OnBuildSelected()
    {
        Debug.Log("Build selected");
        //implement handling here
    }

    /// <summary>
    /// Use this to un/lock the ui ability buttons
    /// </summary>
    public void EnableOrDisableAbility(AbilityType type, bool state)
    {
        abilityButtons[(int) type].SetActivatedState(state, state ? AbilityActiveColor : AbilityInactiveColor);
    }

    /// <summary>
    /// Use this to set the ui anemometer
    /// </summary>
    /// <param name="degree"></param>
    public void SetAnemometerDirection(float degree)
    {
        anemo.SetOrientation(degree);
    }

    #region Fading

    void ForceBlendOut()
    {
        _activeButton = UserMenuButtonType.None;
        _abilityRectActive = false;
        BlendOut();
    }

    public void OnMenuButtonActive(UserMenuButtonType type)
    {
        _activeButton = type;
        BlendIn();
        if (type != UserMenuButtonType.Skill & !_abilityRectActive)
        {
            abilityFader.FadeOut(StandardFadeTimeAbilities);
        }
    }

    public void OnMenuButtonInactive(UserMenuButtonType type)
    {
        _activeButton = UserMenuButtonType.None;
        BlendOut();
    }

    public void OnAbilityRectActive()
    {
        _abilityRectActive = true;
        BlendIn();
    }

    public void OnAbilityRectInactive()
    {
        _abilityRectActive = false;
        BlendOut();
    }

    public void OnMenuActive()
    {
        if (menuFadeRoutine != null) StopCoroutine(menuFadeRoutine);
        menuFadeRoutine = StartCoroutine(FadeMenu(StandardFadeTimeButtons, true));
    }

    public void OnMenuInactive()
    {
        if (menuFadeRoutine != null) StopCoroutine(menuFadeRoutine);
        menuFadeRoutine = StartCoroutine(FadeMenu(StandardFadeTimeButtons, false));
    }    

    public void BlendOut()
    {
        if (delayedCheckRoutine != null) StopCoroutine(delayedCheckRoutine);
        delayedCheckRoutine = StartCoroutine(InactiveCheckTimer(0.25f));
    }

    public void BlendIn()
    {
        if (delayedCheckRoutine != null) StopCoroutine(delayedCheckRoutine);
        if (_activeButton == UserMenuButtonType.Skill)
        {
            abilityFader.FadeIn(StandardFadeTimeAbilities);
        }
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(FadeRoutine(StandardFadeTimeButtons, true));
    }

    Coroutine routine;
    Coroutine delayedCheckRoutine;
    Coroutine menuFadeRoutine;

    IEnumerator FadeMenu(float duration, bool inOut)
    {
        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            t = Mathf.Clamp01(t);
            var pos = buttons[0].anchoredPosition;
            pos.x = Mathf.Lerp(pos.x, inOut ? _activeMenuPosition : _activeMenuPosition + InactiveMenuOffset, t);
            buttons[0].anchoredPosition = pos;
            yield return null;
        }
    }

    IEnumerator FadeRoutine(float duration, bool inOut)
    {
        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime/duration;
            t = Mathf.Clamp01(t);
            for (var i = 1; i < buttons.Length; i++)
            {
                var pos = buttons[i].anchoredPosition;
                pos.x = Mathf.Lerp(pos.x, inOut ? _activeButtonPositions[i-1] : _activeButtonPositions[i-1] + InactiveButtonOffset, t);
                buttons[i].anchoredPosition = pos;
            }
            yield return null;
        }
    }

    IEnumerator InactiveCheckTimer(float duration)
    {
        while (duration > 0)
        {
            duration -= Time.deltaTime;
            yield return null;
        }
        if (_activeButton == UserMenuButtonType.Skill | _abilityRectActive)
        {
            yield break;
        }
        abilityFader.FadeOut(StandardFadeTimeAbilities);
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(FadeRoutine(StandardFadeTimeButtons, false));
    }
#endregion
}
