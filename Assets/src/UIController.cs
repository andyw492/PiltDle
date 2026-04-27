using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;
using System.Linq;
using UnityEngine.Rendering;
using System.Collections.Generic;

public class UIController : MonoBehaviour
{
    public GameController gameController;
    public GameObject textPrefab;
    private TextMeshPro score;
    private Button[] buttons;

    void Start()
    {
        buttons = FindObjectsOfType<Button>();

        foreach (Button button in buttons)
        {
            if (button.name.Contains("guess"))
            {
                string displayName = button.GetComponentInChildren<TMP_Text>().text;
                if (displayName != "Random")
                {
                    displayName = displayName.Substring(1); // remove the @
                }
                button.onClick.AddListener(() => GuessClicked(displayName));
            }
        }

        Button reveal_timestamp_button = GameObject.Find("reveal_timestamp_button").GetComponent<Button>();
        reveal_timestamp_button.onClick.AddListener(RevealTimestampClicked);

        Button reveal_channel_button = GameObject.Find("reveal_channel_button").GetComponent<Button>();
        reveal_channel_button.onClick.AddListener(RevealChannelClicked);

        Button narrow_options_button = GameObject.Find("narrow_options_button").GetComponent<Button>();
        narrow_options_button.onClick.AddListener(NarrowOptionsClicked);

        Button restart_button = GameObject.Find("restart_button").GetComponent<Button>();
        restart_button.onClick.AddListener(RestartClicked);

        score = Instantiate(textPrefab).GetComponent<TextMeshPro>();
        score.transform.position = new Vector3(-5.9f, 4.8f, 0);
        score.text = "0";
        score.fontSize = 100;
        Color color;
        ColorUtility.TryParseHtmlString("#525252", out color);
        score.color = color;

        gameController.NewGame();
    }

    void GuessClicked(string displayName)
    {
        foreach (Button button in buttons)
        {
            button.interactable = true;
            button.GetComponentInChildren<TMP_Text>().fontSize = 18;
        }

        int points = gameController.NewGuess(displayName);
        score.text = (int.Parse(score.text) + points).ToString();

        EventSystem.current.SetSelectedGameObject(null);
    }

    void RevealTimestampClicked()
    {
        int cost = gameController.RevealTimestamp();
        score.text = (int.Parse(score.text) - cost).ToString();

        foreach (Button button in buttons)
        {
            if (button.name.Contains("timestamp"))
            {
                button.interactable = false;
            }
        }
    }

    void RevealChannelClicked()
    {
        int cost = gameController.RevealChannel();
        score.text = (int.Parse(score.text) - cost).ToString();

        foreach (Button button in buttons)
        {
            if (button.name.Contains("channel"))
            {
                button.interactable = false;
            }
        }
    }

    void NarrowOptionsClicked()
    {
        (int cost, string[] namesToRemove) = gameController.NarrowOptions();
        score.text = (int.Parse(score.text) - cost).ToString();

        foreach (Button button in buttons)
        {
            if (namesToRemove.Contains(button.GetComponentInChildren<TMP_Text>().text.Substring(1)))
            {
                button.GetComponentInChildren<TMP_Text>().fontSize = 0;
            }
        }

        foreach (Button button in buttons)
        {
            if (button.name.Contains("narrow"))
            {
                button.interactable = false;
            }
        }
    }

    void RestartClicked()
    {
        foreach (Button button in buttons)
        {
            button.interactable = true;
            button.GetComponentInChildren<TMP_Text>().fontSize = 18;
        }

        gameController.NewGame();
        score.text = "0";
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void GameFinished()
    {
        foreach (Button button in buttons)
        {
            if (!button.name.Contains("restart"))
            {
                button.interactable = false;
                button.GetComponentInChildren<TMP_Text>().fontSize = 0;
            }
        }
    }

    public void DisableButtons(bool disableRestart = true)
    {
        foreach (Button button in buttons)
        {
            if (button.GetComponentInChildren<TMP_Text>().text.Contains("restart") && !disableRestart)
            {
                continue;
            }
            button.interactable = false;
        }
    }

    public void EnableButtons()
    {
        foreach (Button button in buttons)
        {
            button.interactable = true;   
        }
    }
}
