using UnityEngine;
using CsvHelper;
using System.Globalization;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class Message
{
    public string channel { get; set; }
    public string userid { get; set; }
    public string content { get; set; }
    public string type { get; set; }
    public string datetime { get; set; }
}

public class User
{
    public string id;
    public string displayName;
    public Sprite pfp;
    public string color;
    public List<Message> messages;

    public User(string id, string displayName, Sprite pfp, string color)
    {
        this.id = id;
        this.displayName = displayName;
        this.pfp = pfp;
        this.color = color;
        this.messages = new List<Message>();
    }
}

public class GameController : MonoBehaviour
{
    public UIController uiController;
    public SpriteController spriteController;
    private List<User> users;
    private int messageContentLimit = 40;
    private int maxEntries = 10;
    private int currentEntryIndex;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LoadUsers();
    }

    public void NewGame()
    {
        spriteController.Clear();
        (User user, Message message) = RandomEntry();
        spriteController.NewMessage(user, message);
        currentEntryIndex = 0;
    }

    public int NewGuess()
    {
        spriteController.NewGuess(users[0]);

        if (currentEntryIndex == maxEntries - 1)
        {
            uiController.GameFinished();
        }
        else
        {
            (User user, Message message) = RandomEntry();
            spriteController.NewMessage(user, message);
            currentEntryIndex += 1;
        }

        return 100;
    }

    private void LoadUsers()
    {
        users = new List<User>
        {
            new User(
            "n1trosquid",
            "Viktor",
            Resources.Load<Sprite>("viktor_pfp"),
            "#C40A16"
        ),
            new User(
            "metta241",
            "Vi",
            Resources.Load<Sprite>("vi_pfp"),
            "#8A1549"
        ),
            new User(
            "sunnnyfish",
            "sunny fish",
            Resources.Load<Sprite>("sunny fish_pfp"),
            "#E47D22"
        ),
            new User(
            "akuma_sc",
            "Akuma",
            Resources.Load<Sprite>("akuma_pfp"),
            "#5F0F11"
        ),
            new User(
            "asterdrake",
            "Ekko",
            Resources.Load<Sprite>("ekko_pfp"),
            "#3498DB"
        ),
            new User(
            "connivingkitten",
            "Zac",
            Resources.Load<Sprite>("zac_pfp"),
            "#2BB264"
        ),
            new User(
            "coleszn7",
            "Donger",
            Resources.Load<Sprite>("donger_pfp"),
            "#C4A012"
        )
        };

        List<Message> fileMessages = new List<Message>();
        using (var reader = new StreamReader("Assets/Resources/messages.csv"))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            IEnumerable<Message> fileMessagesEnumerator = csv.GetRecords<Message>();
            foreach (Message message in fileMessagesEnumerator)
            {
                fileMessages.Add(message);
            }
        }

        foreach (User user in users)
        {
            foreach (Message message in fileMessages)
            {
                if (message.userid == user.id)
                {
                    message.content = FormatContent(message.content);
                    message.datetime = FormatDatetime(message.datetime);
                    user.messages.Add(message);
                }
            }
        }
    }

    private string FormatContent(string content)
    {
        Dictionary<string, string> idToDisplayName = new Dictionary<string, string>
        {
            {"1012160821515780157", "100%BEEF"},
            {"173522188970885121", "dantheman"},
            {"747585119656607809", "Aksiin Storer"},
            {"756235879713996840", "EdenMurchison"},
            {"989736487849443429", "CalamityCroll"},
            {"190654139909275650", "Scammus"},
            {"1023053756344062102", "HannahRob"},
            {"1226751027466862672", "Saul Goodman"},
            {"1020185701637173248", "Ghost Girl"},
            {"341714626653716481", "OniReiv"},
            {"692122067792822293", "annoyydd"},
            {"181531467590008839", "Zac"},
            {"1016034344386707616", "Chiral/Jesse"},
            {"298051151507750912", "NotObsessed"},
            {"672606639492431872", "DBerg"},
            {"175828475507245056", "toni"},
            {"693907030104211466", "sunny fish"},
            {"644030368156090378", "crusty"},
            {"451202436183359490", "Willoh"},
            {"841884778877681694", "Clem"},
            {"331882150800392194", "Akuma"},
            {"414303137302577152", "Silco"},
            {"176489856711524352", "Ekko"},
            {"98577694433751040", "Viktor"},
            {"187738908203679755", "Jinx"},
            {"186584006458867713", "Donger"},
            {"187326704916758528", "Vi"}
        };

        Dictionary<string, string> roleToDisplayName = new Dictionary<string, string>
        {
            
        };

        if (content.Contains('\n'))
        {
            content = content.Substring(0, content.IndexOf('\n'));
        }
        foreach (KeyValuePair<string, string> kvp in idToDisplayName)
        {
            content = content.Replace(kvp.Key, $"{kvp.Value}");
        }
        foreach (KeyValuePair<string, string> kvp in roleToDisplayName)
        {
            content = content.Replace(kvp.Key, $"{kvp.Value}");
        }
        content = Regex.Replace(content, @"<[^:]*(:[^:]+:)\d+>", @"$1");
        content = Regex.Replace(content, @"<@([^>]+)>", @"$1");
        if (content.Length > messageContentLimit)
        {
            content = content.Substring(0, messageContentLimit) + " (...)";
        }
        
        return content;
    }

    private string FormatDatetime(string datetime)
    {
        // 2026-04-02 02-16-00
        string[] dateParts = datetime.Split(" ")[0].Split("-");
        string[] timeParts = datetime.Split(" ")[1].Split("-");

        string month = dateParts[1].TrimStart('0');
        string day = dateParts[2];
        string year = dateParts[0];
        string hour = int.Parse(timeParts[0]) > 12 ? (int.Parse(timeParts[0]) - 12).ToString() : timeParts[0];
        if (timeParts[0] == "0") { hour = "12"; }
        string minute = timeParts[1];

        return $"{month}/{day}/{year} {hour}:{minute} {(int.Parse(timeParts[0]) >= 12 ? "PM" : "AM")}";
    }

    private (User, Message) RandomEntry()
    {
        User user = users[UnityEngine.Random.Range(0, users.Count)];
        Message message = user.messages[UnityEngine.Random.Range(0, user.messages.Count)];
        return (user, message);
    }
}
