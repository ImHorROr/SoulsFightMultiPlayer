using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverHandeler : NetworkBehaviour
{
    [SerializeField] private List<UnitBase> bases = new List<UnitBase>();

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
        Debug.Log("spawn");

    }

    [Server]
    private void ServerHandleBaseDespawned(UnitBase unitBase)
    {
        bases.Remove(unitBase);
        Debug.Log("remove");


        if (bases.Count != 1) { return; }

        Debug.Log("Game Over");
    }
    #endregion

}
