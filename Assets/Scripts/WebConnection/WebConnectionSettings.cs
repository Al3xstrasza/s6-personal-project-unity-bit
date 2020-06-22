using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Web Settings/Connection Settings")]
public class WebConnectionSettings : ScriptableObject
{
    public string WebSocketEndpoint = "";
    public string RestEndpoint = "";
}
