using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResourcesDisplay : MonoBehaviour
{
    [SerializeField] TMP_Text resorucesText;
    RTSPlayer player;

    void Update()
    {
        if(player == null)
        {
            player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
            if(player!= null)
            {
                ClientHandelOnResourcesChange(player.GetResources());
                player.ClientOnResourcesChange += ClientHandelOnResourcesChange;
            }
        }
    }
    private void OnDestroy()
    {
        player.ClientOnResourcesChange -= ClientHandelOnResourcesChange;

    }
    void ClientHandelOnResourcesChange(int resoruces)
    {
        resorucesText.text = $"resources = {resoruces}";
    }
}
