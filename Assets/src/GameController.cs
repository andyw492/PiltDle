using UnityEngine;
using CsvHelper;
using System.Globalization;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Collections;
using UnityEngine.UI;

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

public class Entry
{
    public User user;
    public Message message;
    public bool userRevealed = false;
    public bool channelRevealed = false;
    public bool datetimeRevealed = false;
    public int score = 0;

    public Entry(User user, Message message)
    {
        this.user = user;
        this.message = message;
    }

    public void RevealAll()
    {
        userRevealed = true;
        channelRevealed = true;
        datetimeRevealed = true;
    }

}

public class GameController : MonoBehaviour
{
    public UIController uiController;
    public SpriteController spriteController;
    private List<User> users;
    private Dictionary<string, User> usersByDisplayName;
    private int messageContentLimit = 40;
    private int maxEntries = 10;
    private int currentEntryIndex;
    private Entry currentEntry;
    private int timestampCost = 5;
    private int channelCost = 5;
    private int narrowCost = 30;

    /*
    typing game:
    assign 5 letters with their corresponding scrabble point value
    each round has three word options and you type one of the three words
    gain points from each assigned letter that was contained in the word
    repeat rounds for 60 seconds
    */

    void Start()
    {
        LoadUsers();
    }

    public void NewGame()
    {
        spriteController.Clear();
        (User user, Message message) = RandomUserMessage();
        currentEntry = new Entry(user, message);
        spriteController.NewEntry(currentEntry);
        currentEntryIndex = 0;
    }

    public int NewGuess(string displayName)
    {
        bool random = displayName == "Random";
        if (random)
        {
            displayName = users[Random.Range(0, users.Count)].displayName;
        }

        // total score with reveal costs is passed to spriteController.NewGuess, but this function only returns 0 or 100

        currentEntry.RevealAll();
        spriteController.ReplaceEntry(currentEntry);
        bool correctGuess = displayName == currentEntry.user.displayName;
        int guessScore = currentEntry.score + (correctGuess ? 100 : 0);
        spriteController.NewGuess(UserByDisplayName(displayName), guessScore, random);

        if (currentEntryIndex == maxEntries - 1)
        {
            uiController.GameFinished();
        }
        else
        {
            StartCoroutine(NewEntryDelayed(0.5f));
        }

        return correctGuess ? 100 : 0;
    }

    public int RevealTimestamp()
    {
        currentEntry.datetimeRevealed = true;
        currentEntry.score -= timestampCost;
        spriteController.ReplaceEntry(currentEntry);
        return timestampCost;
    }

    public int RevealChannel()
    {
        currentEntry.channelRevealed = true;
        currentEntry.score -= channelCost;
        spriteController.ReplaceEntry(currentEntry);
        return channelCost;
    }

    public (int, string[]) NarrowOptions()
    {
        string[] namesToRemove = new string[3];

        List<User> usersCopy = new List<User>(users);
        foreach (User user in users)
        {
            if (user == currentEntry.user)
            {
                usersCopy.Remove(user);
            }
        }

        for (int removeCount = 0; removeCount < namesToRemove.Length; removeCount++)
        {
            int removeIndex = Random.Range(0, usersCopy.Count);
            namesToRemove[removeCount] = usersCopy[removeIndex].displayName;
            usersCopy.RemoveAt(removeIndex);
        }

        currentEntry.score -= narrowCost;

        return (narrowCost, namesToRemove);
    }

    private void LoadUsers()
    {
        users = new List<User>
        {
            new User(
            "metta241",
            "Vi",
            Resources.Load<Sprite>("vi_pfp"),
            "#8A1549"
        ),
            new User(
            "asterdrake",
            "Ekko",
            Resources.Load<Sprite>("ekko_pfp"),
            "#3498DB"
        ),
            new User(
            "n1trosquid",
            "Viktor",
            Resources.Load<Sprite>("viktor_pfp"),
            "#C40A16"
        ),
            new User(
            "connivingkitten",
            "Zac",
            Resources.Load<Sprite>("zac_pfp"),
            "#2BB264"
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
            "coleszn7",
            "Donger",
            Resources.Load<Sprite>("donger_pfp"),
            "#C4A012"
        )
        };

        List<Message> fileMessages = new List<Message>();
        using (var reader = new StreamReader($"{Application.streamingAssetsPath}/messages.csv"))
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

    private User UserByDisplayName(string displayName)
    {
        foreach (User user in users)
        {
            if (user.displayName == displayName)
            {
                return user;
            }
        }
        return null;
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

        Dictionary<string, string> idToRole = new Dictionary<string, string>
        {
            {"&790758099522813974", "Vi"},
            {"&790758737546706966", "Viktor"},
            {"&790759604492566579", "Jinx"},
            {"&790759609068683306", "Heimerdinger"},
            {"&790760194959081492", "Ekko"},
            {"&791923745509343232", "The Villain"},
            {"&915475832934391908", "Silco"},
            {"&915473945006837811", "Zaunites"},
            {"&943334759348723733", "THE Aram Andy"},
            {"&1119858900380954644", "Douma"},
            {"&933559622499971102", "Aram Andies"},
            {"&974534049374806026", "Lux"},
            {"&951218519402496020", "Yuumi Abuser"},
            {"&1187113272508420129", "NQN"},
            {"&1190356852047888454", "Server Booster"},
            {"&1199845181567017011", "Blitzcrank"},
            {"&1226835425893548044", "Fake and Fraud"},
            {"&1299212780775276556", "FlaviBot"},
            {"&1316107204356608020", "Gnar"},
            {"&1319893416489914392", "Dice Maiden"},
            {"&1378954315904847954", "Thresh"},
            {"&1405086865173909564", "The Council"},
            {"&1434718367054303233", "test"}
        };

        if (content.Contains('\n'))
        {
            content = content.Substring(0, content.IndexOf('\n'));
        }
        foreach (KeyValuePair<string, string> kvp in idToDisplayName)
        {
            content = content.Replace(kvp.Key, $"{kvp.Value}");
        }
        foreach (KeyValuePair<string, string> kvp in idToRole)
        {
            content = content.Replace(kvp.Key, $"{kvp.Value}");
        }

        content = Regex.Replace(content, @"<[^:]*(:[^:]+:)\d+>", @"$1");
        content = Regex.Replace(content, @"<@([^>]+)>", @"@$1");
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
        string hour = (int.Parse(timeParts[0]) > 12 ? (int.Parse(timeParts[0]) - 12).ToString() : timeParts[0]).TrimStart('0');
        if (int.Parse(timeParts[0]) == 0) { hour = "12"; }
        string minute = timeParts[1];

        return $"{month}/{day}/{year} {hour}:{minute} {(int.Parse(timeParts[0]) >= 12 ? "PM" : "AM")}";
    }

    private (User, Message) RandomUserMessage()
    {
        User user = users[Random.Range(0, users.Count)];
        Message message = user.messages[Random.Range(0, user.messages.Count)];
        return (user, message);
    }

    private IEnumerator NewEntryDelayed(float seconds)
    {
        uiController.DisableButtons();
        yield return new WaitForSeconds(seconds);
        uiController.EnableButtons();

        (User user, Message message) = RandomUserMessage();
        currentEntry = new Entry(user, message);
        spriteController.NewEntry(currentEntry);
        currentEntryIndex += 1;
    }
}
