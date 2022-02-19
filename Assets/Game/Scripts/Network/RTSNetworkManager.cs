using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RTSNetworkManager : NetworkManager
{
    public static event Action ClientOnConnected;
    public static event Action ClientOnDisconnected;
    [SerializeField] private GameObject unitBasePrefab = null;
    [SerializeField] GameOverHandeler gameOverHandlerPrefab = null;
    bool gameInProgress = false;

    public List<RTSPlayer> players { get; } = new List<RTSPlayer>();


    #region server
    public override void OnServerConnect(NetworkConnection conn)
    {
        if (!gameInProgress) return;
        conn.Disconnect();
    }
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        RTSPlayer player = conn.identity.GetComponent<RTSPlayer>();
        players.Remove(player);
    }
    public override void OnStopServer()
    {
        players.Clear();
        gameInProgress = false;
    }
    public void StartGame()
    {
        if (players.Count < 2) return;
        gameInProgress = true;
        //scene name to load when start game
        ServerChangeScene("Map1");

    }
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);
        RTSPlayer player = conn.identity.GetComponent<RTSPlayer>();
        players.Add(player);
        player.SetDisplayName("player" + players.Count);
        player.SetTeamColor(new Color(
            UnityEngine.Random.Range(0f, 1f),
            UnityEngine.Random.Range(0f, 1f),
            UnityEngine.Random.Range(0f, 1f)
        ));
        player.SetPartyOwner(players.Count == 1);

        
    }
    public override void OnServerSceneChanged(string sceneName)
    {
        if (SceneManager.GetActiveScene().name.StartsWith("Map1"))
        {
            GameOverHandeler gameOverHandlerInstance = Instantiate(gameOverHandlerPrefab);

            NetworkServer.Spawn(gameOverHandlerInstance.gameObject);
            foreach (RTSPlayer player in players)
            {
                GameObject baseInstance = Instantiate(unitBasePrefab, GetStartPosition().position, Quaternion.identity);
                NetworkServer.Spawn(baseInstance, player.connectionToClient);

            }
        }
    }

    #endregion


    #region client

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

        ClientOnConnected?.Invoke();
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);

        ClientOnDisconnected?.Invoke();
    }
    public override void OnStopClient()
    {
        players.Clear();
    }
    #endregion



}

