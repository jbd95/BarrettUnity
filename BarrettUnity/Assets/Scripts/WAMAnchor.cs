using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System;

public class WAMAnchor : MonoBehaviour {

    public Transform tool;

    private void Start() {
        StartCoroutine(UDPSock());
    }

    IEnumerator UDPSock() {
        UdpClient client = new UdpClient(8000);
        IPEndPoint remoteIPEndPoint = new IPEndPoint(IPAddress.Any, 0);
        try {
            client.Connect("192.168.1.2", 8000);
            byte[] send = System.Text.Encoding.UTF8.GetBytes("init");
            client.Send(send, send.Length);
        } catch(Exception e) {
            Debug.Log(e);
            yield break;
        }

        while (true) {
            byte[] received;
            try {
                received = client.Receive(ref remoteIPEndPoint);
            } catch(Exception e) {
                Debug.Log(e);
                yield break;
            }
            string posString = System.Text.Encoding.ASCII.GetString(received);
            tool.localPosition = new Vector3(-float.Parse(posString.Substring(8, 8)), float.Parse(posString.Substring(16, 8)), float.Parse(posString.Substring(0, 8)));
            yield return null;
        }
    }
}
