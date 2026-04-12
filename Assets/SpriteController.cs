using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class SpriteController : MonoBehaviour
{
    public GameObject spritePrefab;
    public GameObject textPrefab;

    
    public List<GameObject> objects;

    private Vector3 start_position = new Vector3(-5.6f, 2.85f, 0);

    public Font emojiFont;

    Sprite pfp;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pfp = Resources.Load<Sprite>("viktor_pfp");
        transform.position = start_position;
    }

    public void NewMessage()
    {
        Vector3 usernameOffset = new Vector3(0.5f, 0.255f, 0);
        Vector3 bodyOffset = new Vector3(0.5f, 0.005f, 0);
        Vector3 channelOffset = new Vector3(-2.5f, 0.125f, 0);
        Vector3 datetimeOffset = new Vector3(1.5f, 0.225f, 0);

        GameObject spriteRendererObject = Instantiate(spritePrefab);
        objects.Add(spriteRendererObject);
        SpriteRenderer spriteRenderer = spriteRendererObject.GetComponent<SpriteRenderer>();
        spriteRenderer.transform.position = transform.position;
        spriteRenderer.sprite = pfp;

        GameObject usernameTextObject = Instantiate(textPrefab);
        objects.Add(usernameTextObject);
        TextMeshPro usernameText = usernameTextObject.GetComponent<TextMeshPro>();
        usernameText.transform.position = transform.position + usernameOffset;
        usernameText.text = "Viktor";
        Color color;
        ColorUtility.TryParseHtmlString("#C40A16", out color);
        usernameText.color = color;

        GameObject bodyTextObject = Instantiate(textPrefab);
        objects.Add(bodyTextObject);
        TextMeshPro bodyText = bodyTextObject.GetComponent<TextMeshPro>();
        bodyText.transform.position = transform.position + bodyOffset;
        bodyText.text = "This is a real Tik Tok Ad I’ve just been presented with 🤣 🐌 👑";

        GameObject channelTextObject = Instantiate(textPrefab);
        objects.Add(channelTextObject);
        TextMeshPro channelText = channelTextObject.GetComponent<TextMeshPro>();
        channelText.transform.position = transform.position + channelOffset;
        channelText.text = "# lumo-and-marshmarlow";
        ColorUtility.TryParseHtmlString("#81828A", out color);
        channelText.color = color;

        GameObject datetimeTextObject = Instantiate(textPrefab);
        objects.Add(datetimeTextObject);
        TextMeshPro datetimeText = datetimeTextObject.GetComponent<TextMeshPro>();
        datetimeText.transform.position = transform.position + datetimeOffset;
        datetimeText.text = "4/11/2026 2:38 PM";
        ColorUtility.TryParseHtmlString("#81828A", out color);
        datetimeText.color = color;
        datetimeText.fontSize = 14;

        transform.position = transform.position + new Vector3(0, -0.75f, 0);
    }

    public void Clear()
    {
        foreach(GameObject gameObject in objects)
        {
            Destroy(gameObject);
        }

        transform.position = start_position;
    }
}
