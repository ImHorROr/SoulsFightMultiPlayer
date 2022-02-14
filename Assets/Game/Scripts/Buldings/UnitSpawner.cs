using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
{
    [SerializeField] private Health health = null;
    [SerializeField] private Unit unitPrefab = null;
    [SerializeField] private Transform unitSpawnPoint = null;
    [SerializeField] TMP_Text remaingUnitsText = null;
    [SerializeField] Image unitProgressImage;
    [SerializeField] int maxQueue = 5;
    [SerializeField] float spawnRange = 5;
    [SerializeField] float spawnDuartion = 5f;
    [SyncVar (hook =nameof(ClientHandleQueueIsUpdated))]
    int QueueUnit;
    [SyncVar]
    float unitTimer;

    float progressImageVelocity;

    private void Update()
    {
        if(isServer)
        {
            ProduceUnit();
        }
        if(isClient)
        {
            UpdateTimerDisplay();
        }

    }


    #region Server

    public override void OnStartServer()
    {
        health.serverOnDie += ServerHandleDie;
    }

    public override void OnStopServer()
    {
        health.serverOnDie -= ServerHandleDie;
    }
    void ProduceUnit()
    {
        if (QueueUnit == 0) return;
        unitTimer += Time.deltaTime;
        if(unitTimer < spawnDuartion)
        {
            return;
        }
        GameObject unitInstance = Instantiate(unitPrefab.gameObject,unitSpawnPoint.position,unitSpawnPoint.rotation);


        NetworkServer.Spawn(unitInstance, connectionToClient);
        Vector3 spawnOffset = UnityEngine.Random.insideUnitSphere * spawnRange;
        spawnOffset.y = unitSpawnPoint.position.y;
        UnitMovement unitMovement = unitInstance.GetComponent<UnitMovement>();

        unitMovement.ServerMove(unitSpawnPoint.position + spawnOffset);
        QueueUnit--;
        unitTimer = 0;

    }
    [Server]
    private void ServerHandleDie()
    {
        NetworkServer.Destroy(gameObject);
    }

    [Command]
    private void CmdSpawnUnit()
    {
        if (QueueUnit == maxQueue) return;
        RTSPlayer player = connectionToClient.identity.GetComponent<RTSPlayer>();
        if (player.GetResources() < unitPrefab.GetResCost()) return;
        QueueUnit++;
        player.SetResources(player.GetResources() - unitPrefab.GetResCost());

    }

    #endregion

    #region Client

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) { return; }

        if (!hasAuthority) { return; }

        CmdSpawnUnit();
    }
    void ClientHandleQueueIsUpdated(int oldUnits, int newUnits)
    {
        remaingUnitsText.text = $"{newUnits}";

    }
    void UpdateTimerDisplay()
    {
        float progress = unitTimer / spawnDuartion;
        if(progress <  unitProgressImage.fillAmount)
        {
            unitProgressImage.fillAmount = progress;
        }
        else
        {
            unitProgressImage.fillAmount = Mathf.SmoothDamp(unitProgressImage.fillAmount, progress, ref progressImageVelocity, 0.1f);
        }
    }
    #endregion
}
