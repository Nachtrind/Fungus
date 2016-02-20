using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UserAbilityButton : MonoBehaviour
{
    [SerializeField] UserMenu menu;
    public UserMenu.AbilityType Type;
    [SerializeField] Text txt;
    public bool IsActive;
    bool isSelected;
    int baseFontSize;
    public int fontSizeSelected = 14;
    Color previousColor;
    public Color SelectedColor = Color.green;

    void Start()
    {
        txt = GetComponent<Text>();
        previousColor = txt.color;
        baseFontSize = txt.fontSize;
    }

    public bool IsSelected
    {
        get { return isSelected; }
        set
        {
            isSelected = value;
            if (isSelected)
            {
                txt.color = SelectedColor;
                txt.fontStyle = FontStyle.Bold;
                txt.fontSize = fontSizeSelected;
            }
            else
            {
                txt.color = previousColor;
                txt.fontStyle = FontStyle.Normal;
                txt.fontSize = baseFontSize;
            }
        }
    }

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
        previousColor = stateColor;
        text.color = stateColor;
        IsActive = state;
    }
}
