using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSelector : MonoBehaviour
{
    private GameWindow activeWindow;

    [SerializeField]
    private List<GameWindow> windows = new List<GameWindow>();

    public void OnEnable()
    {
        foreach (var window in windows)
        {
            window.gameObject.SetActive(false);
        }
        if (windows.Count > 0)
        {
            SelectWindow(windows[0]);
        }
    }

    public void OnDisable()
    {
        foreach (var VARIABLE in windows)
        {
            VARIABLE.DoBeforeDisable();
        }
    }

    public void SelectWindow(GameWindow window)
    {
        if (window.Equals(activeWindow)) return;

        // Should only happen once when starting
        if (activeWindow != null)
        {
            activeWindow.DoBeforeDisable();
            activeWindow.gameObject.SetActive(false);
        }

        window.gameObject.SetActive(true);
        activeWindow = window;
    }
}
