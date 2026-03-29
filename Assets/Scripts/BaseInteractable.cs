using UnityEngine;

public abstract class BaseInteractable : MonoBehaviour
{
    [SerializeField] protected string promptText = "Interact";

    public virtual string GetPromptText()
    {
        return promptText;
    }

    public abstract void Interact();
}