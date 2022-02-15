using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class TeamColorSetter : NetworkBehaviour
{
    [SerializeField] Renderer[] colorRenderers = new Renderer[0];
    [SyncVar (hook =nameof(HandleTeamColorUpdated))] Color teamColor = new Color();


    #region server
    public override void OnStartServer()
    {
        RTSPlayer player = connectionToClient.identity.GetComponent<RTSPlayer>();
        teamColor = player.GetTeamColor();
        print(teamColor);
    }


    #endregion

    #region client

    void HandleTeamColorUpdated(Color oldColor , Color newColor)
    {
        foreach (Renderer renderer in colorRenderers)
        {
            renderer.material.SetColor("_Color", newColor);
            print(renderer + "changed to " + newColor);

        }
    }


    #endregion
}
