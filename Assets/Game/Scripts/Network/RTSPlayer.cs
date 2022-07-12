using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RTSPlayer : NetworkBehaviour
{
    [SerializeField] LayerMask buildingBlockLayerMask = new LayerMask();
    [SerializeField] float buildingRangeLimit = 5;
    [SerializeField]  List<Unit> myUnits = new List<Unit>();
    [SerializeField]  List<Building> myBuildings = new List<Building>();
    [SerializeField]  Building[] buildings = new Building[0];
    [SerializeField] Transform camTrans;


    [SyncVar(hook =nameof(ClientHandleOnResourcesChange))]
    int resources = 100;
    [SyncVar (hook = nameof(AuthorityHandlePartyOwnerStatUpdated))]
    bool isPartyOwner = false;
    [SyncVar(hook = nameof(ClientHandleDisplayNameUpdated))]
    string displayName;

    public event Action<int> ClientOnResourcesChange;
    public static event Action<bool> AuthorityOnPartyOwnerStateUpdated;
    public static event Action ClientOnInfoUpdated;

    private Color teamColor = new Color();



    public string GetDisplayName()
    {
        return displayName;
    }
    public bool GetIsAPartyOwner()
    {
        return isPartyOwner;
    }
    public Transform GetCamTransform()
    {
        return camTrans;
    }
    public List<Building> GetmyBuildings()
    {
        return myBuildings;
    }
    public int GetResources()
    {
        return resources;
    }


    [Server]
    public void SetDisplayName(string displayName)
    {
        this.displayName = displayName;
    }
    [Server]
    public void SetResources(int newRes)
    {
        resources = newRes;
    }
    [Server]
    public void SetPartyOwner(bool state)
    {
        isPartyOwner = state;
    }


    public bool CanPlaceBuilding(BoxCollider buildingCollider, Vector3 pos)
    {
        if (Physics.CheckBox(pos + buildingCollider.center, buildingCollider.size / 2, Quaternion.identity, buildingBlockLayerMask))
        {
            //overlaping
            return false;
        }

        foreach (Building building1 in myBuildings)
        {
            if ((pos - building1.transform.position).sqrMagnitude <= buildingRangeLimit * buildingRangeLimit)
            {
                return true;
            }
        }
        return false;
    }

    #region server


    public override void OnStartServer()
    {
        base.OnStartServer();
        Unit.ServerOnUnitSpawned += ServerHandelUnitSpawned;
        Unit.ServerOnUnitDespawned += ServerHandelUnitDespawned;
        Building.ServerOnBuildingSpawned += ServerHandelBuildingSpawned;
        Building.ServerOnBuildingDespawned += ServerHandelBuildingDespawned;
        DontDestroyOnLoad(this);

    }
    public override void OnStopServer()
    {
        base.OnStopServer();
        Unit.ServerOnUnitSpawned -= ServerHandelUnitSpawned;
        Unit.ServerOnUnitDespawned -= ServerHandelUnitDespawned;
        Building.ServerOnBuildingSpawned -= ServerHandelBuildingSpawned;
        Building.ServerOnBuildingDespawned -= ServerHandelBuildingDespawned;
    }

    [Server]
    public void SetTeamColor(Color newTeamColor)
    {
        teamColor = newTeamColor;
    }
    public Color GetTeamColor()
    {
        return teamColor;
    }
    [Command]
    public void CmdTryPlaceBuilding(int buildingID , Vector3 pos)
    {
        Building buildingToPlace = null;
        foreach (Building building in buildings)
        {
            if(building.GetID() == buildingID)
            {
                buildingToPlace = building;
                break;
            }
        }

        if (buildingToPlace == null) return;
        if (resources < buildingToPlace.GetPrice()) return;
        BoxCollider collider = buildingToPlace.GetComponent<BoxCollider>();

        if (!CanPlaceBuilding(collider, pos)) return;
        GameObject buildingInstance =  Instantiate(buildingToPlace.gameObject, pos, buildingToPlace.transform.rotation);
        NetworkServer.Spawn(buildingInstance, connectionToClient);
        SetResources(resources - buildingToPlace.GetPrice());

    }
    [Command]
    public void CmdStartGame()
    {
        if (!isPartyOwner) return;
        ((RTSNetworkManager)NetworkManager.singleton).StartGame();
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
    private void ServerHandelBuildingSpawned(Building building)
    {
        if (building.connectionToClient.connectionId != connectionToClient.connectionId) return;
        myBuildings.Add(building);

    }

    private void ServerHandelBuildingDespawned(Building building)
    {
        if (building.connectionToClient.connectionId != connectionToClient.connectionId) return;
        myBuildings.Remove(building);

    }
    #endregion

    #region Client


    public override void OnStartClient()
    {
        if (NetworkServer.active) return;
        DontDestroyOnLoad(this);
        ((RTSNetworkManager)NetworkManager.singleton).players.Add(this);

    }
    public override void OnStartAuthority()
    {
        if (NetworkServer.active) { return; }

        Unit.AuthorityOnUnitSpawned += AuthorityHandleUnitSpawned;
        Unit.AuthorityOnUnitDespawned += AuthorityHandleUnitDespawned;
        Building.AuthorityOnBuildingSpawned += AuthorityHandleBuildingSpawned;
    Building.AuthorityOnBuildingDespawned += AuthorityHandleBuildingDespawned;
    }


    public override void OnStopClient()
    {
        ClientOnInfoUpdated?.Invoke();
        if (!isClientOnly) { return; }
                ((RTSNetworkManager)NetworkManager.singleton).players.Remove(this);
        if (!hasAuthority) { return; }
        
        Unit.AuthorityOnUnitSpawned -= AuthorityHandleUnitSpawned;
        Unit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;
        Building.AuthorityOnBuildingSpawned -= AuthorityHandleBuildingSpawned;

        Building.AuthorityOnBuildingDespawned -= AuthorityHandleBuildingDespawned;
    }
    public List<Unit> GetMyUnits()
    {
        return myUnits;
    }
    private void ClientHandleDisplayNameUpdated(string oldName, string newName)
    {
        ClientOnInfoUpdated?.Invoke();
    }
    private void AuthorityHandlePartyOwnerStatUpdated(bool oldState, bool newState)
    {
        if (!hasAuthority) return;
        AuthorityOnPartyOwnerStateUpdated?.Invoke(newState);
    }
    private void AuthorityHandleUnitSpawned(Unit unit)
    {
        myUnits.Add(unit);
    }

    private void AuthorityHandleUnitDespawned(Unit unit)
    {
        myUnits.Remove(unit);
    }

    private void AuthorityHandleBuildingSpawned(Building building)
    {
        myBuildings.Add(building);
    }
    private void AuthorityHandleBuildingDespawned(Building building)
    {
        myBuildings.Remove(building);
    }

    void ClientHandleOnResourcesChange(int oldResoruces, int newResources)
    {
        ClientOnResourcesChange?.Invoke(newResources);
    }
    #endregion
}

