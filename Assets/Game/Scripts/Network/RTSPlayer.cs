using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSPlayer : NetworkBehaviour
{
   [SerializeField]  List<Unit> myUnits = new List<Unit>();
   
    #region server
    public override void OnStartServer()
    {
        base.OnStartServer();
        Unit.serverOnUnitSpanwed += ServerHandelUnitSpawned;
        Unit.serverOnUnitDespanwed += ServerHandelUnitDespawned;
    }
    public override void OnStopServer()
    {
        base.OnStopServer();
        Unit.serverOnUnitSpanwed -= ServerHandelUnitSpawned;
        Unit.serverOnUnitDespanwed -= ServerHandelUnitDespawned;

    }

    private void ServerHandelUnitSpawned(Unit unit)
    {
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId) return;
        myUnits.Add(unit);
    }
    private void ServerHandelUnitDespawned(Unit unit)
    {
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId) return;
        myUnits.Remove(unit);

    }
    #endregion

    #region Client

    public override void OnStartClient()
    {
        if (!isClientOnly) { return; }

        Unit.AuthorityOnUnitSpawned += AuthorityHandleUnitSpawned;
        Unit.AuthorityOnUnitDespawned += AuthorityHandleUnitDespawned;
    }

    public override void OnStopClient()
    {
        if (!isClientOnly) { return; }

        Unit.AuthorityOnUnitSpawned -= AuthorityHandleUnitSpawned;
        Unit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;
    }

    private void AuthorityHandleUnitSpawned(Unit unit)
    {
        if (!hasAuthority) { return; }

        myUnits.Add(unit);
    }

    private void AuthorityHandleUnitDespawned(Unit unit)
    {
        if (!hasAuthority) { return; }

        myUnits.Remove(unit);
    }

    #endregion
}

