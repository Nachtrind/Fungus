using System;
using System.Collections;
using UnityEngine;

public class UserMenu : MonoBehaviour
{
    public enum AbilityType
    {
        Eat,
        Lure,
        Enslave,
        Spawn,
        Slow,
        Speedup
    }

    public enum UserMenuButtonType
    {
        Menu,
        Brain,
        Skill,
        Build,
        Wind,
        None
    }

    public static UserMenu current;

    [SerializeField, ReadOnlyInInspector] UserMenuButtonType _lastButton = UserMenuButtonType.None;

    public Color AbilityActiveColor;
    [SerializeField] UserAbilityButton[] abilityButtons;

    [SerializeField] AbilityFader abilityFader;
    public Color AbilityInactiveColor;
    [SerializeField] Anemometer anemo;

    Transform audioAnchor;

    [SerializeField] UserMenuButton[] buttons;
    public AudioClip interactionSound;

    [SerializeField] MainMenu mainMenu;

    /// <summary>
    ///     This event passes the player requested wind direction.
    /// </summary>
    public static event Action<float> OnRequestWindDirection;

    void Start()
    {
        current = this;
        audioAnchor = Camera.main.transform.FindChild("Listener");
        Wind.OnWind += SetAnemometerDirection;

        FungusResources.Instance.beatneat.isUnlocked = true;
        FungusResources.Instance.attract.isUnlocked = true;
        FungusResources.Instance.growth.isUnlocked = false;
        FungusResources.Instance.zombies.isUnlocked = false;
        FungusResources.Instance.slowdown.isUnlocked = false;
        FungusResources.Instance.speedup.isUnlocked = false;

        abilityFader.FadeOut(0.05f);
        UpdateEnabledAbilities();
    }

    public void UpdateEnabledAbilities()
    {
        EnableOrDisableAbility(AbilityType.Eat, FungusResources.Instance.beatneat.isUnlocked);
        EnableOrDisableAbility(AbilityType.Lure, FungusResources.Instance.attract.isUnlocked);
        EnableOrDisableAbility(AbilityType.Spawn, FungusResources.Instance.growth.isUnlocked);
        EnableOrDisableAbility(AbilityType.Enslave, FungusResources.Instance.zombies.isUnlocked);
        EnableOrDisableAbility(AbilityType.Slow, FungusResources.Instance.slowdown.isUnlocked);
        EnableOrDisableAbility(AbilityType.Speedup, FungusResources.Instance.speedup.isUnlocked);
    }

    void OnDestroy()
    {
        Wind.OnWind -= SetAnemometerDirection;
    }

    public void OnAnemoRequestsWindDirection(float degree)
    {
        if (OnRequestWindDirection == null)
            return;
        OnRequestWindDirection(degree);
    }

    public void OnAbilitySelected(AbilityType type)
    {
        if (GameWorld.Instance.IsPaused)
            return;
        GameInput.Instance.SelectSkill(type);
        HighlightSkill(type);
        GameInput.Instance.ActivateMode(UserMenuButtonType.Skill);
        AudioSource.PlayClipAtPoint(interactionSound, audioAnchor.position);
    }

    public void OnMenuButtonClicked(UserMenuButtonType type)
    {
        if (GameWorld.Instance.IsPaused)
            return;
        _lastButton = type;
        if (type == UserMenuButtonType.Menu)
        {
            HighlightButtons(UserMenuButtonType.None);
            mainMenu.OpenMenu();
            return;
        }
        HighlightButtons(type);
        AudioSource.PlayClipAtPoint(interactionSound, audioAnchor.position);
        if (type != UserMenuButtonType.Skill)
        {
            abilityFader.FadeOut(0.1f);
            GameInput.Instance.ActivateMode(type);
        }
        else
        {
            abilityFader.FadeIn(0.1f);
            GameInput.Instance.SelectSkill(AbilityType.Eat);
            HighlightSkill(AbilityType.Eat);
        }
    }

    void HighlightButtons(UserMenuButtonType type)
    {
        for (var i = 0; i < buttons.Length; i++)
        {
            if (i == (int)type)
            {
                buttons[i].OnActive(null);
            }
            else
            {
                buttons[i].OnInactive(null);
            }
        }
    }

    void HighlightSkill(AbilityType type)
    {
        for (var i = 0; i < abilityButtons.Length; i++)
        {
            abilityButtons[i].IsSelected = i == (int)type;
        }
    }

    public void StartChangingWind()
    {
        if (GameWorld.Instance.IsPaused) return;
        HighlightButtons(UserMenuButtonType.None);
        GameInput.Instance.ActivateMode(UserMenuButtonType.Wind);
    }

    public void StopChangingWind()
    {
        GameInput.Instance.ActivateMode(UserMenuButtonType.None);
    }

    /// <summary>
    ///     Use this to un/lock the ui ability buttons
    /// </summary>
    public void EnableOrDisableAbility(AbilityType type, bool state)
    {
        abilityButtons[(int) type].SetActivatedState(state, state ? AbilityActiveColor : AbilityInactiveColor);
    }

    /// <summary>
    ///     Use this to set the ui anemometer
    /// </summary>
    /// <param name="degree"></param>
    public void SetAnemometerDirection(float degree)
    {
        anemo.SetOrientation(degree);
    }

    public void InputModeCallback(UserMenuButtonType type)
    {
        if (type != _lastButton)
        {
            _lastButton = UserMenuButtonType.None;
            HighlightButtons(_lastButton);
            abilityFader.FadeOut(0.1f);
            GameInput.Instance.ActivateMode(_lastButton);
        }
    }

    #region Fading

    Coroutine pingRoutine;

    public void PingSkillButton()
    {
        if (pingRoutine != null)
        {
            StopCoroutine(pingRoutine);
        }
        pingRoutine = StartCoroutine(PingSkill());
    }

    IEnumerator PingSkill()
    {
        var timer = 0.0f;
        var duration = .5f;
        var startSize = buttons[2].transform.localScale;
        while (timer < 1.0f)
        {
            buttons[2].transform.localScale = Mathf.Lerp(1.0f, 1.5f, Mathf.SmoothStep(0f, 1f, timer))*startSize;
            timer += Time.deltaTime/duration;
            yield return null;
        }

        while (timer > 0f)
        {
            buttons[2].transform.localScale = Mathf.Lerp(1.0f, 1.5f, Mathf.SmoothStep(0f, 1f, timer))*startSize;
            timer -= Time.deltaTime/duration;
            yield return null;
        }
    }

    #endregion
}