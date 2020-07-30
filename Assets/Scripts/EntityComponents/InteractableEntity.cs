
using UnityEngine;

public class InteractableEntity : MonoBehaviour
{
    public KeyCode keyToPress;

    public virtual void OnInteract()
    {
        print("Interacted with an entity!");
    }

    protected bool CloseToPlayer()
    {
        return Vector3.Distance(this.transform.position, GameState.GetInstance()._PlayerState.position) < 2;
    }
}