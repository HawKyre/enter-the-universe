
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

class PortalEntity : InteractableEntity
{
    private Vector3 initialPos;
    
    private Vector3Int nextZone;
    private Direction portalDirection;

    public Vector3Int NextZone { get => nextZone; set => nextZone = value; }
    public Direction PortalDirection { get => portalDirection; set => portalDirection = value; }

    private void Awake()
    {
        initialPos = this.transform.position;
    }

    private void Update()
    {
        MoveSine();
        if (keyToPress != KeyCode.None && Input.GetKeyDown(keyToPress) && CloseToPlayer())
        {
            OnInteract();
        }
    }

    private void MoveSine()
    {
        float y = Mathf.Sin(Time.time * 2 * Mathf.PI / 2) * 0.2f;
        this.transform.position = initialPos + new Vector3(0, y, 0);
    }

    public override void OnInteract()
    {
        ZoneLoader.GoToZone(nextZone, portalDirection);

        Debug.Log("Going to zone " + GameState.GetInstance()._PlayerState.currentZone);
    }
}