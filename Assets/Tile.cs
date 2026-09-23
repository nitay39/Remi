using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Tile : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler
{
    public int number; 
    public string tileColor; 

    public Text numberText; 
    public Image tileImage;
    
    private Outline outline;
    private CanvasGroup canvasGroup; 
    private GameObject placeholder; 

    void Start()
    {
        outline = GetComponent<Outline>();
        if (outline != null) outline.enabled = false;
        
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();

        UpdateVisuals();
    }

    public void UpdateVisuals()
    {
        if (number == 0)
        {
            numberText.text = "☻"; 
            numberText.color = Color.red; 
        }
        else
        {
            numberText.text = number.ToString();
            switch (tileColor)
            {
                case "Red": numberText.color = Color.red; break;
                case "Blue": numberText.color = Color.blue; break;
                case "Black": numberText.color = Color.black; break;
                case "Yellow": numberText.color = new Color(1f, 0.8f, 0f); break; 
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (GameManager.Instance.isGameOver) return; // חסימה אם ניצחנו
        string audioText = number == 0 ? "ג'וקר" : $"{number} {tileColor}";
        GameManager.Instance.PlayAudioAnnouncement(audioText);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (GameManager.Instance.isGameOver) return; // חסימה אם ניצחנו

        placeholder = new GameObject("Placeholder");
        LayoutElement le = placeholder.AddComponent<LayoutElement>();
        le.preferredWidth = 100;
        le.preferredHeight = 150;
        placeholder.transform.SetParent(transform.parent, false);
        placeholder.transform.SetSiblingIndex(transform.GetSiblingIndex());

        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        transform.localScale = new Vector3(1.2f, 1.2f, 1.2f); 

        if (outline != null) outline.enabled = true;
        canvasGroup.blocksRaycasts = false; 
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (GameManager.Instance.isGameOver) return; 

        transform.position = eventData.position;
        GameObject hoverObject = eventData.pointerCurrentRaycast.gameObject;

        bool overTrash = false;
        if (hoverObject != null)
        {
            Transform t = hoverObject.transform;
            while (t != null)
            {
                if (t.CompareTag("DiscardArea")) { overTrash = true; break; }
                t = t.parent;
            }
        }

        if (overTrash)
        {
            transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
        }
        else
        {
            transform.localScale = new Vector3(1.2f, 1.2f, 1.2f); 

            if (hoverObject != null)
            {
                Tile hoverTile = hoverObject.GetComponentInParent<Tile>();
                if (hoverTile != null && hoverTile.gameObject != this.gameObject)
                {
                    Transform row = hoverTile.transform.parent;
                    if (row != null && row.CompareTag("BoardSlot"))
                    {
                        placeholder.transform.SetParent(row);
                        
                        int hoverIndex = hoverTile.transform.GetSiblingIndex();
                        if (eventData.position.x < hoverTile.transform.position.x)
                        {
                            placeholder.transform.SetSiblingIndex(hoverIndex);
                        }
                        else
                        {
                            placeholder.transform.SetSiblingIndex(hoverIndex + 1);
                        }
                    }
                }
                else
                {
                    Transform hoverRow = hoverObject.transform;
                    while (hoverRow != null)
                    {
                        if (hoverRow.CompareTag("BoardSlot"))
                        {
                            if (placeholder.transform.parent != hoverRow)
                            {
                                placeholder.transform.SetParent(hoverRow);
                            }
                            break;
                        }
                        hoverRow = hoverRow.parent;
                    }
                }
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (GameManager.Instance.isGameOver) return; 

        transform.localScale = Vector3.one;
        if (outline != null) outline.enabled = false;
        canvasGroup.blocksRaycasts = true; 

        GameObject dropTarget = eventData.pointerCurrentRaycast.gameObject;
        bool droppedOnDiscard = false;
        if (dropTarget != null)
        {
            Transform t = dropTarget.transform;
            while (t != null)
            {
                if (t.CompareTag("DiscardArea")) { droppedOnDiscard = true; break; }
                t = t.parent;
            }
        }

        if (droppedOnDiscard)
        {
            if (placeholder != null) Destroy(placeholder);
            GameManager.Instance.DiscardTile(this.gameObject);
            return;
        }

        if (placeholder != null)
        {
            transform.SetParent(placeholder.transform.parent);
            transform.SetSiblingIndex(placeholder.transform.GetSiblingIndex());
            Destroy(placeholder);
        }

        GameManager.Instance.boardValidator.ValidateAndSpaceBoard();
    }
}