using System;
using System.Collections.Generic;
using UnityEngine;

public class LoginRestCommunication : MonoBehaviour
{
    private const string URI_LOGIN = "users/login";
    private const string URI_SIGNUP = "users/sign-up";
    private const string URI_GERUSERINFO = "users/getExtraUserInfo";

    [SerializeField]
    private RestClient client;

    public static LoginRestCommunication _instance;

    public void Start()
    {
        if (_instance == null)
        {
            _instance = GetComponent<LoginRestCommunication>();
        }
    }

    // Create Auction
    public void Signup(string username, string password, Action<string> callback)
    {
        if (client != null)
        {
            client.MakeRestCall("POST", new UsersEntity(username, password), URI_SIGNUP, callback);
        }
    }

    public void Login(string username, string password, Action<string> callback)
    {
        if (client != null)
        {
            client.MakeRestCall("POST", new UsersEntity(username, password), URI_LOGIN, callback);
        }
    }

    public void GetUserInfo(Action<string> callback)
    {
        if (client != null)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Authorization", "Bearer " + PlayerManager._instance.GetPlayerId());
            client.MakeRestCall("GET", URI_GERUSERINFO, headers, callback);
        }
    }
}

[System.Serializable]
public class UsersEntity
{
    public string username;
    public string password;

    public UsersEntity(string username, string password)
    {
        this.username = username;
        this.password = password;
    }
}