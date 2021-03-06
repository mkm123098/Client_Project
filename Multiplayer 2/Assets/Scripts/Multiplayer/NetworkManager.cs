using System.Collections;
using System.Collections.Generic;
using RiptideNetworking;
using RiptideNetworking.Utils;
using UnityEngine;
using System;

public enum ServerToClientId : ushort
{
    playerSpawned = 1,
    playerMovement,
}

public enum ClientToServerId : ushort
{
    name = 1,
    input,
    playerCollision,
}

public class NetworkManager : MonoBehaviour
{
    private static NetworkManager singleton;

    public static NetworkManager Singleton
    {
        get => singleton;
        private set
        {
            if (singleton == null)
            {
                singleton = value;
            }
            else if (singleton != value)
            {
                Debug.Log($"{nameof(NetworkManager)} instance already exists");
                Destroy(value);
            }
        }
    }

    public Client Client { get; private set; }

    [SerializeField] private string ip;
    [SerializeField] private ushort port;

    private void Awake()
    {
        Singleton = this;
    }

    // Start is called before the first frame update
    private void Start()
    {
        RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);

        Client = new Client();
        Client.Connected += DidConnect;
        Client.ConnectionFailed += FailedToConnect;
        Client.ClientDisconnected += PlayerLeft;
        Client.Disconnected += DidDisconnect;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        Client.Tick();
    }

    private void OnApplicationQuit()
    {
        Client.Disconnect();
    }

    public void Connect()
    {
        Client.Connect($"{ip}:{port}");
    }

    private void DidConnect(object sender, EventArgs e)
    {
        UI_Manager.Singleton.SendName();
    }

    private void FailedToConnect(object sender, EventArgs e)
    {
        UI_Manager.Singleton.BackToMain();
    }

    private void PlayerLeft(object sender, ClientDisconnectedEventArgs e)
    {
        if (Player.list.TryGetValue(e.Id, out Player player))
            Destroy(player.gameObject);
    }

    private void DidDisconnect(object sender, EventArgs e)
    {
        UI_Manager.Singleton.BackToMain();
        foreach (Player player in Player.list.Values)
            Destroy(player.gameObject);
    }
}
