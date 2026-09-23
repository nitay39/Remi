using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject tilePrefab;
    public Transform playerBoard; 
    public BoardValidator boardValidator;
    
    public Text statusText; 
    public Button drawButton; 
    public GameObject restartButtonObj; 

    private List<TileData> deck = new List<TileData>();
    private bool hasDrawnThisTurn = false;
    public bool isGameOver = false; 

    public struct TileData { public int number; public string color; }

    void Awake() { Instance = this; }

    void Start()
    {
        boardValidator = GetComponent<BoardValidator>();
        
        if (drawButton != null)
        {
            drawButton.onClick.RemoveAllListeners();
            drawButton.onClick.AddListener(OnDrawButtonClicked);
        }
        
        InitializeGame();
    }

    private string FixHebrew(string text)
    {
        string[] lines = text.Split('\n');
        for (int i = 0; i < lines.Length; i++)
        {
            char[] chars = lines[i].ToCharArray();
            System.Array.Reverse(chars);
            lines[i] = new string(chars);
            lines[i] = lines[i].Replace('(', '\x00').Replace(')', '(').Replace('\x00', ')');
        }
        return string.Join("\n", lines);
    }

    void InitializeGame()
    {
        isGameOver = false;
        deck.Clear();
        string[] colors = { "Red", "Blue", "Black", "Yellow" };
        
        for (int set = 0; set < 2; set++)
            foreach (string color in colors)
                for (int num = 1; num <= 13; num++)
                    deck.Add(new TileData { number = num, color = color });

        deck.Add(new TileData { number = 0, color = "Joker" });
        deck.Add(new TileData { number = 0, color = "Joker" });

        for (int i = 0; i < deck.Count; i++)
        {
            TileData temp = deck[i];
            int randomIndex = Random.Range(i, deck.Count);
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }

        for(int i=0; i<14; i++) DrawTile(true);
        
        StartNewTurn();
    }

    public void StartNewTurn()
    {
        hasDrawnThisTurn = false;
        drawButton.interactable = true;
        statusText.text = FixHebrew("משוך אריח חדש מהקופה להתחלת התור");
        boardValidator.ValidateAndSpaceBoard();
    }

    public void OnDrawButtonClicked()
    {
        if (hasDrawnThisTurn || isGameOver) return;
        DrawTile(false);
        hasDrawnThisTurn = true;
        drawButton.interactable = false;
        statusText.text = FixHebrew("סדר את האריחים. זרוק אריח לסיום התור.");
    }

    private void DrawTile(bool initialDeal)
    {
        if (deck.Count == 0) return;
        TileData data = deck[0];
        deck.RemoveAt(0);

        Transform targetRow = playerBoard.GetChild(0); 
        if (targetRow.childCount >= 10 && playerBoard.childCount > 1) 
        {
            targetRow = playerBoard.GetChild(1);
        }

        GameObject newTile = Instantiate(tilePrefab, targetRow);
        newTile.SetActive(true); 
        
        Tile t = newTile.GetComponent<Tile>();
        t.number = data.number;
        t.tileColor = data.color;
        t.UpdateVisuals();
        
        if(!initialDeal) boardValidator.ValidateAndSpaceBoard();
    }

    public void DiscardTile(GameObject tileObj)
    {
        if (!hasDrawnThisTurn)
        {
            statusText.text = FixHebrew("עליך למשוך אריח לפני שתוכל לזרוק!");
            return;
        }
        Destroy(tileObj); 
        Invoke("CheckWinCondition", 0.2f); 
    }

    private void CheckWinCondition()
    {
        if (boardValidator.IsEntireBoardValid())
        {
            isGameOver = true;
            statusText.text = FixHebrew("כל הכבוד! ניצחת! הלוח חוקי לחלוטין.");
            PlayAudioAnnouncement("ניצחת במשחק!");
        }
        else
        {
            StartNewTurn();
        }
    }

    public void RestartGame()
    {
        // מחיקת כל האריחים והרווחים מהלוח
        foreach (Transform row in playerBoard)
        {
            if (row.CompareTag("BoardSlot"))
            {
                foreach (Transform child in row)
                {
                    Destroy(child.gameObject);
                }
            }
        }
        
        Invoke("InitializeGame", 0.1f);
    }

    public void PlayAudioAnnouncement(string msg)
    {
        Debug.Log("Audio Voiceover: " + msg);
    }
}