using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class UIController : MonoBehaviour
{
    public GameController gameController;
    public GameObject textPrefab;
    private TextMeshPro score;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void BtnClicked()
    {
        gameController.NewMessage();
        score.text = (int.Parse(score.text) + 100).ToString();

        EventSystem.current.SetSelectedGameObject(null);
    }

    void RestartClicked()
    {
        gameController.Restart();
        score.text = "0";
        EventSystem.current.SetSelectedGameObject(null);
    }
}
