using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StoryTextUI : MonoBehaviour
{
    public static StoryTextUI Instance { get; private set; }

    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI bodyText;

    private Queue<string> lines = new Queue<string>();
    private bool isShowing = false;

    public bool IsShowing => isShowing;

    private void Awake()
    {
        Instance = this;
        Hide();
    }

    public void ShowLines(params string[] newLines)
    {
        lines.Clear();

        foreach (string line in newLines)
            lines.Enqueue(line);

        panel.SetActive(true);
        isShowing = true;
        ShowNextLine();
    }

    public void ShowNextLine()
    {
        if (!isShowing)
            return;

        if (lines.Count == 0)
        {
            Hide();
            return;
        }

        bodyText.text = lines.Dequeue();
    }

    public void Hide()
    {
        panel.SetActive(false);
        isShowing = false;
        bodyText.text = "";
        lines.Clear();
    }
}