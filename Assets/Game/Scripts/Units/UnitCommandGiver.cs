using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitCommandGiver : MonoBehaviour
{
    [SerializeField] UnitSelectionHandeler unitSelectionHandeler;
    [SerializeField] LayerMask layerMask = new LayerMask();
    Camera mainCamera;

    void Start()
    {
        mainCamera=Camera.main;
        //selectionHandeler.SelectedUnits;
    }

    void Update()
    {
        if(!Mouse.current.rightButton.wasPressedThisFrame)
        {
            return;
        }
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            return;
        }

        TryMove(hit.point);
    }

    private void TryMove(Vector3 point)
    {
        foreach (Unit unit in unitSelectionHandeler.SelectedUnits)
        {
            unit.GetUnitMovement().CmdMove(point);
        }
    }
}
