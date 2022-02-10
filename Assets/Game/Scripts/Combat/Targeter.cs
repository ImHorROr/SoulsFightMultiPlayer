using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targeter : NetworkBehaviour
{
    Targetable target;
    public Targetable GetTarget()
    {
        return target;
    }
    [Command]
    public void CmdSetTarger(GameObject targetGameObject)
    {
        if (!targetGameObject.TryGetComponent<Targetable>(out Targetable target)) return;
        this.target = target;
    }

    [Server]
    public void ClearTargets()
    {
        target = null;
    }
    public override void OnStartServer()
    {
        GameOverHandeler.serverOnGameOver += serverHandelOnGameOver;
    }

    public override void OnStopServer()
    {
        GameOverHandeler.serverOnGameOver -= serverHandelOnGameOver;
    }
    [Server]
    void serverHandelOnGameOver()
    {
        ClearTargets();
    }
}