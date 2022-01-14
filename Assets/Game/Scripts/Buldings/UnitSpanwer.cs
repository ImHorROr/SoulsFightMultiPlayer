using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitSpanwer : NetworkBehaviour,IPointerClickHandler
{
    [SerializeField] GameObject unitPrefab;
    [SerializeField] Transform unitSpwanLocation;


    #region server
    [Command] void CmdSpawnUnit()
    {
        GameObject unitInstance = Instantiate(unitPrefab, unitSpwanLocation.position, unitSpwanLocation.rotation);
        NetworkServer.Spawn(unitInstance, connectionToClient);
    }

    #endregion

    #region client

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        if (!hasAuthority) return;
        CmdSpawnUnit();
    }

    #endregion
}
