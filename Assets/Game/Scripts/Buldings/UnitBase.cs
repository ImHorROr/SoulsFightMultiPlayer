using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBase : NetworkBehaviour
{
    [SerializeField] private Health health = null;

    public static event Action<UnitBase> ServerOnBaseSpawned;
    public static event Action<UnitBase> ServerOnBaseDespawned;
    public static event Action<int> serverOnPlayerDie;

    #region Server

    public override void OnStartServer()
    {
        health.serverOnDie += ServerHandleDie;

        ServerOnBaseSpawned?.Invoke(this);
    }

    public override void OnStopServer()
    {
        ServerOnBaseDespawned?.Invoke(this);

        health.serverOnDie -= ServerHandleDie;
    }
    
    [Server]
    private void ServerHandleDie()
    {
        serverOnPlayerDie?.Invoke(connectionToClient.connectionId);
        NetworkServer.Destroy(gameObject);
    }

    #endregion

    #region Client

    #endregion
}