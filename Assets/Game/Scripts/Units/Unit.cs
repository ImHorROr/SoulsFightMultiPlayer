using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Unit : NetworkBehaviour
{
    [SerializeField] UnityEvent onSelected;
    [SerializeField] UnityEvent onDeselected;
    [SerializeField] UnitMovement unitMovement;

    public UnitMovement GetUnitMovement()
    {
        return unitMovement;
    }


    [Client]

    public void Select()
    {
        if (!hasAuthority) return;
        onSelected?.Invoke();
    }
    [Client]

    public void Deselect()
    {
        if (!hasAuthority) return;
        onDeselected?.Invoke();

    }
}
