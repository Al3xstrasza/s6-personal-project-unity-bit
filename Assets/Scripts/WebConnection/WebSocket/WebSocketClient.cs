using System;
using System.Collections;
using System.Collections.Generic;
using StompHelper;
using UnityEngine;
using WebSocketSharp;

public class WebSocketClient : MonoBehaviour
{
    //public static WebSocketClient _instance;

    public bool debugging;

    [SerializeField]
    private WebConnectionSettings settings;

    public static List<StompMessage> responses = new List<StompMessage>();

    private bool shouldCloseConnection = false;
    private static WebSocket ws;
    private static StompMessageSerializer serializer = new StompMessageSerializer();

    public void EstablishWebsocketConnection()
    {
        if (settings == null) return;
        shouldCloseConnection = false;
        StartCoroutine(StartWebSocket());
    }

    public void DropWebsocketConnection()
    {
        shouldCloseConnection = true;
    }

    public bool SendMessage(string destination, string body)
    {
        if (debugging) Debug.Log("send message " + destination + " " + body);
        if (!IsSocketReady()) return false;

        var broad = new StompMessage("SEND", body);
        broad["destination"] = destination;
        ws.Send(serializer.Serialize(broad));
        return true;
    }

    public bool SubscribeTopic(string topic, string id)
    {
        if (!IsSocketReady()) return false;

        var sub = new StompMessage("SUBSCRIBE");
        sub["id"] = id;
        sub["destination"] = topic;
        ws.Send(serializer.Serialize(sub));
        return true;
    }

    public bool UnsubscribeTopic(string id)
    {
        if (!IsSocketReady()) return false;

        var unsub = new StompMessage("UNSUBSCRIBE");
        unsub["id"] = id;
        ws.Send(serializer.Serialize(unsub));
        return true;
    }

    private bool IsSocketReady()
    {
        if (ws == null) return false;
        if (ws.ReadyState != WebSocketState.Open) return false;
        return true;
    }

    #region websocket connection coroutine
    IEnumerator StartWebSocket()
    {
        if (settings != null)
        {
            using (ws = new WebSocket(settings.WebSocketEndpoint + "?access_token=" + PlayerManager._instance.GetPlayerId()))
            {
                //ws.Log.File = "C:/Users/sjors/Desktop/School/S6/Personal Project/Unity S6 Personal Project/websocket.log";
                //ws.Log.Level = LogLevel.Trace;
                ws.OnMessage += ws_OnMessage;
                ws.OnOpen += ws_OnOpen;
                ws.OnError += ws_OnError;
                ws.Connect();

                while (ws.ReadyState == WebSocketState.Connecting)
                {
                    Debug.Log("connecting");
                    yield return null;
                }

                if (ws.ReadyState == WebSocketState.Open)
                {
                    Debug.Log("opened");
                }

                var connect = new StompMessage("CONNECT");
                connect["accept-version"] = "1.2";
                connect["host"] = "";
                ws.Send(serializer.Serialize(connect));

                yield return new WaitForSeconds(1);

                // Subscribe to test topic
                //var sub = new StompMessage("SUBSCRIBE");
                //sub["id"] = "sub-0";
                //sub["destination"] = "/topic/greetings";
                //ws.Send(serializer.Serialize(sub));

                // Subscribe to test topic
                //var sub1 = new StompMessage("SUBSCRIBE");
                //sub1["id"] = "sub-1";
                //sub1["destination"] = "/queue/message-" + clientId;
                //ws.Send(serializer.Serialize(sub1));

                // Wait for subscribe
                //yield return new WaitForSeconds(1);

                // Sending test message
                //var content = new Content() { Subject = "Stomp client", Message = "Hello World!!" };
                //var broad = new StompMessage("SEND", JsonUtility.ToJson(content));
                //broad["content-type"] = "application/json";
                //var broad = new StompMessage("SEND", "Test");
                //broad["destination"] = "/prefix/hello";
                //ws.Send(serializer.Serialize(broad));

                Debug.Log("Websocket connected");

                while (!shouldCloseConnection || !ws.IsAlive)
                {
                    yield return null;
                }
                ws.Close();
            }
        }
    }

    static void ws_OnOpen(object sender, EventArgs e)
    {
        Debug.Log("ws_OnOpen says: " + e.ToString());
    }

    static void ws_OnMessage(object sender, MessageEventArgs e)
    {
        Debug.Log("message received");
        responses.Add(serializer.Deserialize(e.Data));
    }

    static void ws_OnError(object sender, ErrorEventArgs e)
    {
        Debug.Log("ws_OnError says: " + e.Exception + "\n" + e.Message);
    }
    #endregion
}