using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class SpriteController : MonoBehaviour
{
    public UIController uiController;
    public GameObject spritePrefab;
    public GameObject textPrefab;
    public Sprite defaultPfp;
    public string defaultColor = "#5865F2";
    public string scoreGreen = "#32A852";
    public string scoreRed = "#A83232";
    
    public List<GameObject> objects;

    private Vector3 start_position = new Vector3(-5.9f, 2.85f, 0);

    void Start()
    {
        transform.position = start_position;
    }

    public void NewEntry(Entry entry)
    {
        Vector3 usernameOffset = new Vector3(0.4f, 0.255f, 0);
        Vector3 bodyOffset = new Vector3(0.4f, 0.005f, 0);
        Vector3 channelOffset = new Vector3(-2.5f, 0.125f, 0);
        Vector3 datetimeOffset = new Vector3(1.4f, 0.225f, 0);

        GameObject spriteRendererObject = Instantiate(spritePrefab);
        objects.Add(spriteRendererObject);
        SpriteRenderer spriteRenderer = spriteRendererObject.GetComponent<SpriteRenderer>();
        spriteRenderer.transform.position = transform.position;
        spriteRenderer.sprite = entry.userRevealed ? entry.user.pfp : defaultPfp;

        GameObject usernameTextObject = Instantiate(textPrefab);
        objects.Add(usernameTextObject);
        TextMeshPro usernameText = usernameTextObject.GetComponent<TextMeshPro>();
        usernameText.transform.position = transform.position + usernameOffset;
        usernameText.text = entry.userRevealed ? entry.user.displayName : "@???";
        Color color;
        ColorUtility.TryParseHtmlString(entry.userRevealed ? entry.user.color : defaultColor, out color);
        usernameText.color = color;

        GameObject bodyTextObject = Instantiate(textPrefab);
        objects.Add(bodyTextObject);
        TextMeshPro bodyText = bodyTextObject.GetComponent<TextMeshPro>();
        bodyText.transform.position = transform.position + bodyOffset;
        bodyText.text = entry.message.content;

        GameObject channelTextObject = Instantiate(textPrefab);
        objects.Add(channelTextObject);
        TextMeshPro channelText = channelTextObject.GetComponent<TextMeshPro>();
        channelText.transform.position = transform.position + channelOffset;
        channelText.text = entry.channelRevealed ? $"# {entry.message.channel}" : "# ???";
        ColorUtility.TryParseHtmlString("#81828A", out color);
        channelText.color = color;

        GameObject datetimeTextObject = Instantiate(textPrefab);
        objects.Add(datetimeTextObject);
        TextMeshPro datetimeText = datetimeTextObject.GetComponent<TextMeshPro>();
        datetimeText.transform.position = transform.position + datetimeOffset;
        datetimeText.text = entry.datetimeRevealed ? entry.message.datetime : "???";
        ColorUtility.TryParseHtmlString("#81828A", out color);
        datetimeText.color = color;
        datetimeText.fontSize = 14;
    }

    public void ReplaceEntry(Entry entry)
    {
        int removeCount = 5;
        for (int objectIndex = objects.Count - 1; objectIndex > objects.Count - 1 - removeCount; objectIndex--)
        {
            Destroy(objects[objectIndex]);
        }
        int indexToRemove = objects.Count - removeCount;
        for (int _ = 0; _ < removeCount; _++)
        {
            objects.RemoveAt(indexToRemove);
        }

        NewEntry(entry);
    }

    public void NewGuess(User user, int guessScore, bool random)
    {
        Vector3 leftGuessOffset = new Vector3(4.1f, 0.005f, 0);
        Vector3 rightGuessOffset = new Vector3(5.095f, 0.005f, 0);
        Vector3 guessScoreOffset = new Vector3(6.035f, 0.005f, 0);

        GameObject leftGuessTextObject = Instantiate(textPrefab);
        objects.Add(leftGuessTextObject);
        TextMeshPro leftGuessText = leftGuessTextObject.GetComponent<TextMeshPro>();
        leftGuessText.transform.position = transform.position + leftGuessOffset;
        leftGuessText.text = $"You {(random ? "gamba'd" : "guessed")}:";

        GameObject rightGuessTextObject = Instantiate(textPrefab);
        objects.Add(rightGuessTextObject);
        TextMeshPro rightGuessText = rightGuessTextObject.GetComponent<TextMeshPro>();
        rightGuessText.transform.position = transform.position + rightGuessOffset;
        rightGuessText.text = $"@{user.displayName}";
        Color color;
        ColorUtility.TryParseHtmlString(user.color, out color);
        rightGuessText.color = color;

        GameObject guessScoreTextObject = Instantiate(textPrefab);
        objects.Add(guessScoreTextObject);
        TextMeshPro guessScoreText = guessScoreTextObject.GetComponent<TextMeshPro>();
        guessScoreText.transform.position = transform.position + guessScoreOffset;
        guessScoreText.text = $"({(guessScore >= 0 ? "+" : "")}{guessScore})";
        ColorUtility.TryParseHtmlString(guessScore > 0 ? scoreGreen : scoreRed, out color);
        guessScoreText.color = color;

        transform.position = transform.position + new Vector3(0, -0.75f, 0);
    }

    public void Clear()
    {
        foreach (GameObject gameObject in objects)
        {
            Destroy(gameObject);
        }

        transform.position = start_position;
    }
}
