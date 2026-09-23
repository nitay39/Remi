using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameSetupMenu : EditorWindow
{
    [MenuItem("Rummikub/Setup Game Environment")]
    public static void SetupGame()
    {
        AddTag("BoardSlot");
        AddTag("DiscardArea");
        AddTag("Spacer");

        if (FindObjectOfType<EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
        }

        GameObject canvasObj = new GameObject("Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasObj.AddComponent<GraphicRaycaster>();

        GameObject playerBoard = new GameObject("PlayerBoard");
        playerBoard.transform.SetParent(canvasObj.transform, false);
        RectTransform pbRect = playerBoard.AddComponent<RectTransform>();
        pbRect.anchorMin = new Vector2(0.05f, 0.05f);
        pbRect.anchorMax = new Vector2(0.95f, 0.45f);
        pbRect.offsetMin = Vector2.zero; pbRect.offsetMax = Vector2.zero;
        Image pbImage = playerBoard.AddComponent<Image>();
        pbImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f); 
        
        VerticalLayoutGroup vlg = playerBoard.AddComponent<VerticalLayoutGroup>();
        vlg.childControlWidth = true; 
        vlg.childControlHeight = true;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = true;
        vlg.spacing = 10f;
        vlg.padding = new RectOffset(10, 10, 10, 10);

        for (int i = 1; i <= 2; i++)
        {
            GameObject row = new GameObject("Row" + i);
            row.transform.SetParent(playerBoard.transform, false);
            row.tag = "BoardSlot"; 
            HorizontalLayoutGroup hlg = row.AddComponent<HorizontalLayoutGroup>();
            hlg.childControlWidth = true; 
            hlg.childControlHeight = true;
            hlg.childForceExpandWidth = false; 
            hlg.childForceExpandHeight = false;
            hlg.childAlignment = TextAnchor.MiddleCenter;
            hlg.spacing = 10f;
        }

        GameObject discardArea = new GameObject("DiscardArea");
        discardArea.transform.SetParent(canvasObj.transform, false);
        discardArea.tag = "DiscardArea";
        RectTransform discardRect = discardArea.AddComponent<RectTransform>();
        discardRect.anchorMin = new Vector2(0.7f, 0.5f);
        discardRect.anchorMax = new Vector2(0.9f, 0.8f);
        discardRect.offsetMin = Vector2.zero; discardRect.offsetMax = Vector2.zero;
        Image discardImage = discardArea.AddComponent<Image>();
        discardImage.color = new Color(0.8f, 0.2f, 0.2f, 0.8f);
        
        GameObject discardTextObj = new GameObject("Text");
        discardTextObj.transform.SetParent(discardArea.transform, false);
        Text discardText = discardTextObj.AddComponent<Text>();
        discardText.text = "ןאכל רורג\nקורזל ידכ";
        discardText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        discardText.alignment = TextAnchor.MiddleCenter;
        discardText.color = Color.white;
        discardText.fontSize = 40;
        RectTransform dtRect = discardText.GetComponent<RectTransform>();
        dtRect.anchorMin = Vector2.zero; dtRect.anchorMax = Vector2.one;
        dtRect.offsetMin = Vector2.zero; dtRect.offsetMax = Vector2.zero;

        // טקסט הסטטוס זז מעט שמאלה כדי לפנות מקום לכפתור
        GameObject statusObj = new GameObject("StatusText");
        statusObj.transform.SetParent(canvasObj.transform, false);
        RectTransform statusRect = statusObj.AddComponent<RectTransform>();
        statusRect.anchorMin = new Vector2(0.1f, 0.85f);
        statusRect.anchorMax = new Vector2(0.7f, 0.95f); // שונה מ-0.9 ל-0.7
        statusRect.offsetMin = Vector2.zero; statusRect.offsetMax = Vector2.zero;
        Text statusText = statusObj.AddComponent<Text>();
        statusText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        statusText.alignment = TextAnchor.MiddleCenter;
        statusText.fontSize = 50;
        statusText.color = Color.black;

        GameObject drawBtnObj = new GameObject("DrawButton");
        drawBtnObj.transform.SetParent(canvasObj.transform, false);
        RectTransform drawRect = drawBtnObj.AddComponent<RectTransform>();
        drawRect.anchorMin = new Vector2(0.1f, 0.5f);
        drawRect.anchorMax = new Vector2(0.3f, 0.8f);
        drawRect.offsetMin = Vector2.zero; drawRect.offsetMax = Vector2.zero;
        Image drawBtnImage = drawBtnObj.AddComponent<Image>();
        drawBtnImage.color = new Color(0.2f, 0.6f, 0.8f);
        Button drawBtn = drawBtnObj.AddComponent<Button>();
        
        GameObject drawBtnTextObj = new GameObject("Text");
        drawBtnTextObj.transform.SetParent(drawBtnObj.transform, false);
        Text drawBtnText = drawBtnTextObj.AddComponent<Text>();
        drawBtnText.text = "הפוק\nחירא ךושמ";
        drawBtnText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        drawBtnText.alignment = TextAnchor.MiddleCenter;
        drawBtnText.fontSize = 40;
        drawBtnText.color = Color.white;
        RectTransform dbtRect = drawBtnText.GetComponent<RectTransform>();
        dbtRect.anchorMin = Vector2.zero; dbtRect.anchorMax = Vector2.one;
        dbtRect.offsetMin = Vector2.zero; dbtRect.offsetMax = Vector2.zero;

        // --- כפתור הריסטרט במיקום החדש ---
        GameObject restartBtnObj = new GameObject("RestartButton");
        restartBtnObj.transform.SetParent(canvasObj.transform, false);
        RectTransform rstRect = restartBtnObj.AddComponent<RectTransform>();
        rstRect.anchorMin = new Vector2(0.75f, 0.85f); // פינה ימנית עליונה
        rstRect.anchorMax = new Vector2(0.95f, 0.95f);
        rstRect.offsetMin = Vector2.zero; rstRect.offsetMax = Vector2.zero;
        Image rstImg = restartBtnObj.AddComponent<Image>();
        rstImg.color = new Color(0.2f, 0.8f, 0.2f); 
        Button rstBtn = restartBtnObj.AddComponent<Button>();

        GameObject rstTxtObj = new GameObject("Text");
        rstTxtObj.transform.SetParent(restartBtnObj.transform, false);
        Text rstTxt = rstTxtObj.AddComponent<Text>();
        rstTxt.text = "שדחמ לחתה"; 
        rstTxt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        rstTxt.alignment = TextAnchor.MiddleCenter;
        rstTxt.fontSize = 35; // הקטנתי מעט כדי שייכנס בצורה אסתטית
        rstTxt.color = Color.white;
        RectTransform rstTxtRect = rstTxt.GetComponent<RectTransform>();
        rstTxtRect.anchorMin = Vector2.zero; rstTxtRect.anchorMax = Vector2.one;
        rstTxtRect.offsetMin = Vector2.zero; rstTxtRect.offsetMax = Vector2.zero;
        
        GameObject tilePrefab = new GameObject("TilePrefab");
        tilePrefab.SetActive(false);
        RectTransform tileRect = tilePrefab.AddComponent<RectTransform>();
        tileRect.sizeDelta = new Vector2(100, 150); 
        
        LayoutElement tileLayout = tilePrefab.AddComponent<LayoutElement>();
        tileLayout.minWidth = 100; tileLayout.minHeight = 150;
        tileLayout.preferredWidth = 100; tileLayout.preferredHeight = 150;

        Image tileImage = tilePrefab.AddComponent<Image>();
        tileImage.color = new Color(1f, 0.98f, 0.9f); 
        
        Outline tileOutline = tilePrefab.AddComponent<Outline>();
        tileOutline.effectColor = Color.yellow;
        tileOutline.effectDistance = new Vector2(6, -6);
        tileOutline.enabled = false;
        
        Tile tileScript = tilePrefab.AddComponent<Tile>();
        tilePrefab.AddComponent<AudioSource>();
        
        GameObject tileTextObj = new GameObject("NumberText");
        tileTextObj.transform.SetParent(tilePrefab.transform, false);
        RectTransform textRect = tileTextObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero; textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero; textRect.offsetMax = Vector2.zero;
        Text tileText = tileTextObj.AddComponent<Text>();
        tileText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        tileText.alignment = TextAnchor.MiddleCenter;
        tileText.fontSize = 80;
        Outline textOutline = tileTextObj.AddComponent<Outline>(); 
        textOutline.effectColor = Color.white;
        textOutline.effectDistance = new Vector2(3, -3);
        
        tileScript.numberText = tileText;
        tileScript.tileImage = tileImage;

        GameObject spacerPrefab = new GameObject("SpacerPrefab");
        spacerPrefab.SetActive(false);
        spacerPrefab.tag = "Spacer";
        LayoutElement spacerLayout = spacerPrefab.AddComponent<LayoutElement>();
        spacerLayout.minWidth = 40; 

        GameObject gmObj = new GameObject("GameManager");
        GameManager gm = gmObj.AddComponent<GameManager>();
        BoardValidator bv = gmObj.AddComponent<BoardValidator>();
        
        gm.tilePrefab = tilePrefab;
        gm.playerBoard = playerBoard.transform;
        gm.boardValidator = bv;
        gm.statusText = statusText;
        gm.drawButton = drawBtn;
        gm.restartButtonObj = restartBtnObj; 
        
        bv.playerBoard = playerBoard.transform;
        bv.spacerPrefab = spacerPrefab;

        UnityEditor.Events.UnityEventTools.AddPersistentListener(rstBtn.onClick, gm.RestartGame);

        Debug.Log("Game Setup Complete! Check your scene.");
    }

    private static void AddTag(string tag)
    {
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty tagsProp = tagManager.FindProperty("tags");
        for (int i = 0; i < tagsProp.arraySize; i++)
        {
            if (tagsProp.GetArrayElementAtIndex(i).stringValue.Equals(tag)) return;
        }
        tagsProp.InsertArrayElementAtIndex(tagsProp.arraySize);
        tagsProp.GetArrayElementAtIndex(tagsProp.arraySize - 1).stringValue = tag;
        tagManager.ApplyModifiedProperties();
    }
}