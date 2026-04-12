using UnityEngine;
using CsvHelper;
using System.Globalization;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class MessageData
{
    public string channel { get; set; }
    public string userid { get; set; }
    public string message { get; set; }
    public string type { get; set; }
    public string datetime { get; set; }
}

public class User
{
    public string id;
    public Sprite pfp;

    public User(string id, Sprite pfp)
    {
        this.id = id;
        this.pfp = pfp;
    }
}

public class GameController : MonoBehaviour
{
    public SpriteController spriteController;
    private List<User> users;
    private List<MessageData> messages;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(LateStart());

        messages = new List<MessageData>();
        using (var reader = new StreamReader("Assets/Resources/2026-04-05 23-45-55.csv"))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            IEnumerable<MessageData> fileMessages = csv.GetRecords<MessageData>();

            foreach (MessageData message in fileMessages)
            {
                messages.Add(message);

                if (message.message.Contains("In terms of just straight content"))
                {
                    Debug.Log("datetime:");
                    Debug.Log(message.datetime);
                    Debug.Log(message.userid);
                }
            }
        }

        List<MessageData> processed = new List<MessageData>();
        List<string> userList = new List<string>{"n1trosquid", "metta241", "sunnnyfish", "akuma_sc", "asterdrake", "connivingkitten", "coleszn7"};

        foreach (string user in userList)
        {
            Debug.Log(user);
        }
        Debug.Log(userList);
        
        foreach (MessageData message in messages)
        {
            if (userList.Contains(message.userid) && !message.message.Contains("valle") && !message.message.Contains("Valle"))
            {
                processed.Add(message);
            }
        }

        Debug.Log(processed.Count);

        using (var writer = new StreamWriter("Assets/Resources/messages.csv"))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(processed);
        }
    }

    IEnumerator LateStart()
    {
        yield return null;
        NewMessage();
    }

    public void NewMessage()
    {
        spriteController.NewMessage();
    }

    public void Restart()
    {
        spriteController.Clear();
    }
}
