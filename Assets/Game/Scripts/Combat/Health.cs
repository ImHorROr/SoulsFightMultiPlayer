using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : NetworkBehaviour
{
    [SerializeField] int maxHealth = 100;
    [SyncVar(hook = nameof(HandelHealthChange))]
    int currntHealth;

    public event Action serverOnDie;
    public event Action<int,int> ClientOnHealthChange;
    #region server

    public override void OnStartServer()
    {
        currntHealth = maxHealth;
        UnitBase.serverOnPlayerDie += ServerHandelPlayerDie;

    }
    public override void OnStopServer()
    {
        UnitBase.serverOnPlayerDie -= ServerHandelPlayerDie;
    }
    [Server]
    public void DealDamage(int damageAmount)
    {
        if (currntHealth == 0) return;

        currntHealth = Mathf.Max(currntHealth - damageAmount, 0);
        if(currntHealth != 0)
        {
            return;
        }
        serverOnDie?.Invoke();
        Debug.Log("ded");
    }
    [Server]
    public void ServerHandelPlayerDie(int con)
    {
        if(connectionToClient.connectionId != con)
        {
            return;
        }
        DealDamage(currntHealth);

    }
    #endregion

    #region client

    void HandelHealthChange(int oldHealth, int newHealth)
    {
        ClientOnHealthChange?.Invoke(currntHealth, maxHealth);
    }

    #endregion
}
