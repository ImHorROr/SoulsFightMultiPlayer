using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class ResourcesGenerator : NetworkBehaviour
{
    [SerializeField] Health health = null;
    [SerializeField] int resPerInterval = 10;
    [SerializeField] float Interval = 2f;
    float timer;
    RTSPlayer player;

    [ServerCallback]
    private void Update()
    {
        timer -= Time.deltaTime;
        if(timer <= 0)
        {
            // add res
            player.SetResources(player.GetResources() + resPerInterval);
            timer += Interval;

        }
    }
    public override void OnStartServer()
    {
        timer = Interval;
        player = connectionToClient.identity.GetComponent<RTSPlayer>();


        health.serverOnDie += ServerHandleDie;
        GameOverHandeler.serverOnGameOver += ServerHandleGameOver;

        base.OnStartServer();
    }
    public override void OnStopServer()
    {
        health.serverOnDie -= ServerHandleDie;
        GameOverHandeler.serverOnGameOver -= ServerHandleGameOver;
    }
    void ServerHandleDie()
    {
        NetworkServer.Destroy(gameObject);
    }
    void ServerHandleGameOver()
    {
        enabled = false;
    }
}
