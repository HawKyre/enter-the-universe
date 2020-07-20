using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIStateManager : MonoBehaviour
{
    [SerializeField] private UIInventoryManager inventoryManager;

    [SerializeField] private GameObject InventoryUI;
    [SerializeField] private GameObject PauseUI;

    private void Update()
    {
        switch (GameState.GetInstance()._CurrentUIState)
        {
            case UIState.GAME:
                if (Input.GetKeyDown(KeyCode.E))
                {
                    GameState.GetInstance()._CurrentUIState = UIState.INVENTORY;
                    inventoryManager.RefreshInventory();
                    InventoryUI.SetActive(true);
                }
                else if (Input.GetKeyDown(KeyCode.Escape))
                {
                    GameState.GetInstance()._CurrentUIState = UIState.PAUSE;
                    PauseUI.SetActive(true);
                }
                break;
            case UIState.INVENTORY:
                if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Escape))
                {
                    GameState.GetInstance()._CurrentUIState = UIState.GAME;
                    InventoryUI.SetActive(false);
                }
                break;
            case UIState.PAUSE:
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    GameState.GetInstance()._CurrentUIState = UIState.GAME;
                    PauseUI.SetActive(false);
                }
                break;
        }
    }

}

public enum UIState
{
    GAME,
    PAUSE,
    INVENTORY
}