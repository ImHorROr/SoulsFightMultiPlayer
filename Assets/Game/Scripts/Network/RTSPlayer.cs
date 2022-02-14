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

    [SyncVar(hook =nameof(ClientHandleOnResourcesChange))]
    int resources = 100;
    public event Action<int> ClientOnResourcesChange;

    public List<Building> GetmyBuildings()
    {
        return myBuildings;
    }
    public int GetResources()
    {
        return resources;
    }
    [Server]
    public void SetResources(int newRes)
    {
        resources = newRes;
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

    }
    public override void OnStopServer()
    {
        base.OnStopServer();
        Unit.ServerOnUnitSpawned -= ServerHandelUnitSpawned;
        Unit.ServerOnUnitDespawned -= ServerHandelUnitDespawned;
        Building.ServerOnBuildingSpawned -= ServerHandelBuildingSpawned;
        Building.ServerOnBuildingDespawned -= ServerHandelBuildingDespawned;
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
        if (!isClientOnly || !hasAuthority) { return; }

        Unit.AuthorityOnUnitSpawned -= AuthorityHandleUnitSpawned;
        Unit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;
        Building.AuthorityOnBuildingSpawned -= AuthorityHandleBuildingSpawned;

        Building.AuthorityOnBuildingDespawned -= AuthorityHandleBuildingDespawned;
    }
    public List<Unit> GetMyUnits()
    {
        return myUnits;
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

