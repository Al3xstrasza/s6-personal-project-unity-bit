using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonHighlighter : MonoBehaviour
{
    [SerializeField]
    private List<Button> buttons = new List<Button>();

    public void ClickButton(Button button)
    {
        foreach (var _button in buttons)
        {
            _button.interactable = true;
            if (_button.Equals(button))
            {
                _button.interactable = false;
            }
        }
    }

    public void OnDisable()
    {
        ClickButton(buttons[0]);
    }
}
