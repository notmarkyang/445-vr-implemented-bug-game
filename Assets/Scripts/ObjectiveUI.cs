using TMPro;
using UnityEngine;

public class ObjectiveUI : MonoBehaviour
{
    public static ObjectiveUI Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI objectiveText;

    private void Awake()
    {
        Instance = this;
    }

    public void SetObjective(string text)
    {
        if (objectiveText != null)
            objectiveText.text = text;
    }
}