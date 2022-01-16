using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class Unit : NetworkBehaviour
{
    [SerializeField] private UnitMovement unitMovement = null;
    [SerializeField] private UnityEvent onSelected = null;
    [SerializeField] private UnityEvent onDeselected = null;
    [SerializeField] private Targeter targeter = null;

    public static event Action<Unit> serverOnUnitSpanwed; 
    public static event Action<Unit> serverOnUnitDespanwed; 
    public static event Action<Unit> AuthorityOnUnitSpawned; 
    public static event Action<Unit> AuthorityOnUnitDespawned;


    public UnitMovement GetUnitMovement()
    {
        return unitMovement;
    }
    public Targeter GetTargeter()
    {
        return targeter;
    }


    #region server
    public override void OnStartServer()
    {
        serverOnUnitSpanwed?.Invoke(this);
    }

    public override void OnStopServer()
    {
        serverOnUnitDespanwed?.Invoke(this);
    }

    #endregion

    #region Client

    [Client]
    public void Select()
    {
        if (!hasAuthority) { return; }

        onSelected?.Invoke();
    }

    [Client]
    public void Deselect()
    {
        if (!hasAuthority) { return; }

        onDeselected?.Invoke();
    }

    public override void OnStartClient()
    {
        if (!isClientOnly || !hasAuthority) { return; }

        AuthorityOnUnitSpawned?.Invoke(this);
    }

    public override void OnStopClient()
    {
        if (!isClientOnly || !hasAuthority) { return; }

        AuthorityOnUnitDespawned?.Invoke(this);
    }

    #endregion
}
