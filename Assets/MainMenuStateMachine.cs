using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuStateMachine : MonoBehaviour
{
    public Dictionary<string, UIMainMenuState> states;
    public UIMainMenuState currentState;

    private void Awake()
    {
        
    }

    public void ChangeState(string newState)
    {
        if (currentState != null)
        {
            currentState.ExitState();
        }
        
        if (states.ContainsKey(newState))
        {
            states[newState].EnterState();
            currentState = states[newState];
        }
        else
        {
            currentState = null;
        }
    }
}
