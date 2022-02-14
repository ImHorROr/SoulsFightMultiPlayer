using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class Building : NetworkBehaviour
{
    [SerializeField] Sprite icon = null;
    [SerializeField] int price = 100;
    [SerializeField] int iD = -1;
    [SerializeField] GameObject buildingPreview;
    public static event Action<Building> ServerOnBuildingSpawned;
    public static event Action<Building> ServerOnBuildingDespawned;

    public static event Action<Building> AuthorityOnBuildingSpawned;
    public static event Action<Building> AuthorityOnBuildingDespawned;

    public GameObject GetBuildingPreview()
    {
        return buildingPreview;
    }
    public Sprite GetIcon()
    {
        return icon;
    }
    public int GetID()
    {
        return iD;
    }
    public int GetPrice()
    {
        return price;
    }
    #region server
    public override void OnStartServer()
    {
        ServerOnBuildingSpawned?.Invoke(this);
    }
    public override void OnStopServer()
    {
        ServerOnBuildingDespawned?.Invoke(this);
    }
    #endregion
    #region client

    public override void OnStartAuthority()
    {
        AuthorityOnBuildingSpawned?.Invoke(this);
    }
    public override void OnStopClient()
    {
        if (!hasAuthority) { return; }
        AuthorityOnBuildingDespawned?.Invoke(this);

    }


    #endregion
}
