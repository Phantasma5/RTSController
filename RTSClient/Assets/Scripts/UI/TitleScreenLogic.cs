using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TitleScreenLogic : MonoBehaviour {

    public Text server;
    public Text port;
    public RTSClient client;

    public void Connect()
    {
        Debug.Log("Button Pressed");
        client.ConnectToServer(server.text, int.Parse(port.text));
    }
}
