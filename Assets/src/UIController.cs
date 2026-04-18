using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;

public class UIController : MonoBehaviour
{
    public GameController gameController;
    public GameObject textPrefab;
    private TextMeshPro score;
    private int maxEntries = 10;
    private int currentEntryIndex;

    void Start()
    {
        Button btn = GameObject.Find("button1").GetComponent<Button>();
        btn.onClick.AddListener(BtnClicked);

        Button button_restart = GameObject.Find("button_restart").GetComponent<Button>();
        button_restart.onClick.AddListener(RestartClicked);

        score = Instantiate(textPrefab).GetComponent<TextMeshPro>();
        score.transform.position = new Vector3(-5.9f, 4.8f, 0);
        score.text = "0";
        score.fontSize = 100;
        Color color;
        ColorUtility.TryParseHtmlString("#525252", out color);
        score.color = color;

        StartCoroutine(LateStart());
    }

    IEnumerator LateStart()
    {
        yield return null;
        gameController.NewGame();
        currentEntryIndex = 0;
    }

    void BtnClicked()
    {
        Debug.Log("clicked");
        int points = gameController.NewGuess();
        score.text = (int.Parse(score.text) + points).ToString();

        EventSystem.current.SetSelectedGameObject(null);
    }

    void RestartClicked()
    {
        gameController.NewGame();
        score.text = "0";
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void GameFinished()
    {
        Debug.Log("game finished");
    }
}
