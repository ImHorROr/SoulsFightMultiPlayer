using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverHandeler : NetworkBehaviour
{
    public static event Action<string> clientOnGameOver;
    public static event Action serverOnGameOver;

    private List<UnitBase> bases = new List<UnitBase>();

    #region Server

    public override void OnStartServer()
    {
        UnitBase.ServerOnBaseSpawned += ServerHandleBaseSpawned;
        UnitBase.ServerOnBaseDespawned += ServerHandleBaseDespawned;
    }

    public override void OnStopServer()
    {
        UnitBase.ServerOnBaseSpawned -= ServerHandleBaseSpawned;
        UnitBase.ServerOnBaseDespawned -= ServerHandleBaseDespawned;
    }

    [Server]
    private void ServerHandleBaseSpawned(UnitBase unitBase)
    {
        bases.Add(unitBase);
    }

    [Server]
    private void ServerHandleBaseDespawned(UnitBase unitBase)
    {
        bases.Remove(unitBase);

        if (bases.Count != 1) { return; }

        Debug.Log("Game Over");

        int playerID = bases[0].connectionToClient.connectionId;
        RpcGameOver($"{playerID}");
        serverOnGameOver?.Invoke();
    }

    #endregion

    #region Client

    [ClientRpc]
    void RpcGameOver(string winner)
    {
        clientOnGameOver?.Invoke(winner);
    }
    #endregion
}
