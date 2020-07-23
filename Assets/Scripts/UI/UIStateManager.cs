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
            case UIIngameState.GAME:
                if (Input.GetKeyDown(KeyCode.E))
                {
                    GameState.GetInstance()._CurrentUIState = UIIngameState.INVENTORY;
                    inventoryManager.RefreshInventory();
                    InventoryUI.SetActive(true);
                }
                else if (Input.GetKeyDown(KeyCode.Escape))
                {
                    GameState.GetInstance()._CurrentUIState = UIIngameState.PAUSE;
                    PauseUI.SetActive(true);
                }
                break;
            case UIIngameState.INVENTORY:
                if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Escape))
                {
                    GameState.GetInstance()._CurrentUIState = UIIngameState.GAME;
                    InventoryUI.SetActive(false);
                }
                break;
            case UIIngameState.PAUSE:
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    GameState.GetInstance()._CurrentUIState = UIIngameState.GAME;
                    PauseUI.SetActive(false);
                }
                break;
        }
    }

}

public enum UIIngameState
{
    GAME,
    PAUSE,
    INVENTORY
}