using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UserAbilityButton : MonoBehaviour
{
    [SerializeField] UserMenu menu;
    public UserMenu.AbilityType Type;
    public bool IsActive;

    Text text;

    void Awake()
    {
        text = GetComponent<Text>();
    }

    public void OnSelect()
    {
        if (IsActive)
        {
            menu.OnAbilitySelected(Type);
        }
    }

    public void SetActivatedState(bool state, Color stateColor)
    {
        text.color = stateColor;
        IsActive = state;
    }
}
