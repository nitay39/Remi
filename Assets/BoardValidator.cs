using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BoardValidator : MonoBehaviour
{
    public Transform playerBoard; 
    public GameObject spacerPrefab; 

    public void ValidateAndSpaceBoard()
    {
        foreach (Transform row in playerBoard)
        {
            if (!row.CompareTag("BoardSlot")) continue;

            // נקה רווחים ישנים מהשורה
            foreach (Transform child in row)
            {
                if (child.CompareTag("Spacer")) Destroy(child.gameObject);
            }

            List<Tile> currentTiles = new List<Tile>();
            foreach (Transform child in row)
            {
                Tile t = child.GetComponent<Tile>();
                if (t != null) currentTiles.Add(t);
            }

            int i = 0;
            while (i < currentTiles.Count)
            {
                int validSetLength = FindValidSetLengthFromIndex(currentTiles, i);
                
                // התיקון: אם מצאנו סדרה חוקית נדלג מעליה, ואם לא - נדלג אריח אחד קדימה
                int jump = validSetLength >= 3 ? validSetLength : 1;
                i += jump;

                // אם עדיין לא הגענו לאריח האחרון בשורה, נכניס רווח
                if (i < currentTiles.Count)
                {
                    GameObject spacer = Instantiate(spacerPrefab, row);
                    spacer.SetActive(true);
                    // מכניס את הרווח בדיוק לפני האריח הבא בתור
                    spacer.transform.SetSiblingIndex(currentTiles[i].transform.GetSiblingIndex()); 
                }
            }
        }
    }

    private int FindValidSetLengthFromIndex(List<Tile> tiles, int startIndex)
    {
        for (int length = Mathf.Min(13, tiles.Count - startIndex); length >= 3; length--)
        {
            List<Tile> subList = tiles.GetRange(startIndex, length);
            if (IsValidGroup(subList) || IsValidRun(subList)) return length;
        }
        return 0;
    }

    private bool IsValidGroup(List<Tile> set)
    {
        if (set.Count > 4) return false;
        List<Tile> realTiles = set.Where(t => t.number != 0).ToList();
        if (realTiles.Count <= 1) return true; 

        int targetNum = realTiles[0].number;
        HashSet<string> colors = new HashSet<string>();

        foreach (Tile t in realTiles)
        {
            if (t.number != targetNum) return false; 
            if (colors.Contains(t.tileColor)) return false; 
            colors.Add(t.tileColor);
        }
        return true;
    }

    private bool IsValidRun(List<Tile> set)
    {
        List<Tile> realTiles = set.Where(t => t.number != 0).ToList();
        if (realTiles.Count == 0) return true; 

        string targetColor = realTiles[0].tileColor;
        foreach (Tile t in realTiles)
        {
            if (t.tileColor != targetColor) return false; 
        }

        int firstRealIndex = set.IndexOf(realTiles[0]);
        int startNumber = realTiles[0].number - firstRealIndex;

        if (startNumber < 1 || startNumber + set.Count - 1 > 13) return false; 

        for (int i = 0; i < set.Count; i++)
        {
            if (set[i].number != 0 && set[i].number != startNumber + i) return false;
        }
        return true;
    }

    public bool IsEntireBoardValid()
    {
        ValidateAndSpaceBoard(); 
        int totalTiles = 0;
        int validTilesInSets = 0;

        foreach (Transform row in playerBoard)
        {
            if (!row.CompareTag("BoardSlot")) continue;
            
            List<Tile> rowTiles = row.GetComponentsInChildren<Tile>().ToList();
            totalTiles += rowTiles.Count;
            
            int i = 0;
            while (i < rowTiles.Count)
            {
                int length = FindValidSetLengthFromIndex(rowTiles, i);
                if (length >= 3)
                {
                    validTilesInSets += length;
                    i += length;
                }
                else
                {
                    return false; 
                }
            }
        }
        return totalTiles == 14 && validTilesInSets == 14;
    }
}