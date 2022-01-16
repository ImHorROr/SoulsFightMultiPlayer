using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targetable : NetworkBehaviour
{
    [SerializeField] Transform aimAtPoint = null;

    public Transform GetAimAtPoint()
    {
        return aimAtPoint;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
