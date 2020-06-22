using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoginHandler : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField username;
    [SerializeField]
    private TMP_Dropdown usernameDropdown;
    [SerializeField]
    private TMP_InputField password;
    [SerializeField]
    private GameObject window;

    [SerializeField]
    private PlayerManager playerManager;

    public void PerformLogin()
    {
        playerManager.PerformLogin(username.text, password.text);
    }

    public void PerformSignup()
    {
        playerManager.PerformSignup(username.text, password.text, SignupCallback);
    }

    private void SignupCallback(string response)
    {
        if (response.Equals("User created"))
        {
            PerformLogin();
        }
    }


    public void PerformLogout()
    {
        playerManager.Logout();
    }
}
