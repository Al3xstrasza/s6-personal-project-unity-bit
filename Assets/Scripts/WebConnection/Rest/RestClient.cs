using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using WebSocketSharp;

public class RestClient : MonoBehaviour
{
    public bool debugging = false;

    //public static RestClient _instance;
    [SerializeField]
    private WebConnectionSettings settings;

    public void Start()
    {
        //if (_instance == null)
        //{
        //    _instance = GetComponent<RestClient>();
        //}
    }

    public void MakeRestCall(string method, string uriAddition, Action<string> callback = null)
    {
        if (settings != null)
        {
            StartCoroutine(SendWithoutBody(method, uriAddition, callback));
        }
    }

    public void MakeRestCall(string method, string uriAddition, Dictionary<string, string> headers, Action<string> callback = null)
    {
        if (settings != null)
        {
            StartCoroutine(SendWithoutBody(method, uriAddition, callback, headers));
        }
    }

    public void MakeRestCall<T>(string method, T objToJson, string uriAddition, Action<string> callback = null)
    {
        if (settings != null)
        {
            StartCoroutine(SendWithBody(method, JsonUtility.ToJson(objToJson), uriAddition, callback));
        }
    }

    public void MakeRestCall<T>(string method, T objToJson, string uriAddition, Dictionary<string, string> headers, Action<string> callback = null)
    {
        if (settings != null)
        {
            StartCoroutine(SendWithBody(method, JsonUtility.ToJson(objToJson), uriAddition, callback, headers));
        }
    }

    IEnumerator SendWithoutBody(string method, string uri, Action<string> callback = null, Dictionary<string, string> headers = null)
    {
        if (debugging) Debug.Log(settings.RestEndpoint + uri);
        string reply = "";
        using (UnityWebRequest www = new UnityWebRequest(settings.RestEndpoint + uri, method))
        {
            if (headers != null && headers.Count > 0)
            {
                foreach (var keyPair in headers)
                {
                    if (debugging) Debug.Log("Adding header " + keyPair.Key + " " + keyPair.Value);
                    www.SetRequestHeader(keyPair.Key, keyPair.Value);
                }
            }
            www.downloadHandler = new DownloadHandlerBuffer();
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                if (debugging) Debug.Log("Upload complete!");

                reply = www.downloadHandler.text;
                if (reply == null)
                {
                    Debug.LogError("Response Null!");
                    yield break;
                }

                if (debugging) Debug.Log(reply + " < Raw reply");
            }

            if (callback != null)
            {
                callback.Invoke(reply);
            }
        }
    }

    IEnumerator SendWithBody(string method, string json, string uri, Action<string> callback = null, Dictionary<string, string> headers = null)
    {
        if (debugging) Debug.Log(settings.RestEndpoint + uri);
        string reply = "";
        if (debugging) Debug.Log(json);
        using (UnityWebRequest www = new UnityWebRequest(settings.RestEndpoint + uri, method))
        {
            if (headers != null && headers.Count > 0)
            {
                foreach (var keyPair in headers)
                {
                    if (debugging) Debug.Log(keyPair.Key + " " + keyPair.Value);
                    www.SetRequestHeader(keyPair.Key, keyPair.Value);
                }
            }
            //www.SetRequestHeader("UserId", settings.currentUserId.ToString());
            //www.SetRequestHeader("Content-Type", "multipart/form-data");
            www.SetRequestHeader("Content-Type", "application/json");
            www.uploadHandler = new UploadHandlerRaw(GetBytes(json));
            www.uploadHandler.contentType = "application/json";
            www.downloadHandler = new DownloadHandlerBuffer();
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                if (debugging) Debug.Log("Upload complete!");

                reply = www.downloadHandler.text;
                if (reply == null)
                {
                    Debug.LogError("Response Null!");
                    yield break;
                }

                if (debugging) Debug.Log(reply + " < Raw reply");
            }

            if (callback != null)
            {
                callback.Invoke(reply);
            }
        }
    }
    protected static byte[] GetBytes(string str)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(str);
        return bytes;
    }
}
