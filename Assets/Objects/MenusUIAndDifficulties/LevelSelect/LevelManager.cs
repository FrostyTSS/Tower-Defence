using JetBrains.Annotations;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static LevelInfo;

//this is trash, don't make your HUD and main scripts into one script next time you idiot!
[Serializable]
public class LevelClearInfoSerializable
{
    public List<LevelInfo> SavedLevelInfo = new List<LevelInfo>();
}

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    public string MainMenuSceneName;
    public List<LevelInfo> GlobalLevelList = new List<LevelInfo>();
    public LevelInfo CurrentLevel;
    public DifficultyList ListOfDifficulties;
    public DifficultyInfo CurrentDifficulty;
    public GameObject LevelUIPrefab;
     List<GameObject> LevelUIPrefabList = new List<GameObject>();
    public GameObject MedalPrefab;
    public GameObject LevelSelectHolderObject;
    public SwapMenu MenuSwapScript;
    //public Image LevelDifficultyOverviewImage; // for difficulty select icon
    public GameObject ButtonTemplate;
   public List<GameObject> ButtonList = new List<GameObject>();
    //public RectTransform OriginalPosition;
    public int CurrentPage = 0; public int Columns = 5; public int Rows = 3;

    public List<AudioClip> MusicList;


    //save file stuff
    //redo
    public void Save(string SaveFileName)
    {
        
    }

    public void Load(string SaveFileName)
    {
        
    }



    void OnEnable()
    {
        //Tell our 'OnLevelFinishedLoading' function to start listening for a scene change as soon as this script is enabled.
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnDisable()
    {
        //Tell our 'OnLevelFinishedLoading' function to stop listening for a scene change as soon as this script is disabled. Remember to always have an unsubscription for every delegate you subscribe to!
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }


    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == MainMenuSceneName)
        {
            MenuSwapScript  = (SwapMenu)FindAnyObjectByType(typeof(SwapMenu));
            LevelSelectHolderObject = MenuSwapScript.MenuObjList[1];
        }
    }


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

        if (MainMenuSceneName == "")
        {
            MainMenuSceneName = SceneManager.GetActiveScene().name;
        }
        CurrentLevel = null;
        
       // ShowAllLevels();
        // Load("PlayerData");
    }
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

   
    public void QuitGame()
    {
        Application.Quit();
    }

    public void ClearDifficulty()
    {
        for (int i = 0; i < LevelManager.instance.CurrentLevel.DifficultiesCleared.Count; i++)
        {
            if (CurrentLevel.DifficultiesCleared[i].Difficulty == CurrentDifficulty)
            {

                //I like using structs for small mini classes that are just info, using them for stuff i edit in the editor. However, when you have to set variables for that, you get.. this.
                DifficultyClearInfo TempDifficulty = new DifficultyClearInfo();
                TempDifficulty.Difficulty = CurrentLevel.DifficultiesCleared[i].Difficulty;
                TempDifficulty.DifficultyClear = true;
                CurrentLevel.DifficultiesCleared[i] = TempDifficulty;
            }
        }
        Save("PlayerData");
    }


    public void ClearAssets()
    {
        for (int i = 0; i < LevelUIPrefabList.Count; i++)
        {
            Destroy(LevelUIPrefabList[i]);
        }
        LevelUIPrefabList.Clear();
        for (int i = 0; i < ButtonList.Count; i++)
        {
            Destroy(ButtonList[i]);
        }
        ButtonList.Clear();
    }

    public void ShowAllLevels()
    {
        ClearAssets();
        int ColumnCount = 0; int RowCount = 0; Vector3 StartingPosition = Vector3.zero;
        StartingPosition.x = -35;
        StartingPosition.y = 25.5f;
        for (int i = 0; i < GlobalLevelList.Count; i++)
        {
            LevelInfo CurrentlyCycledLevel = GlobalLevelList[i];

            Vector3 UIElementLocation = StartingPosition;
            UIElementLocation.x += (20 * ColumnCount);
            UIElementLocation.y -= (30 * RowCount);
            Debug.Log(UIElementLocation);


            GameObject LevelUIObj = Instantiate(LevelUIPrefab, Vector3.zero, Quaternion.identity, LevelSelectHolderObject.transform);
            LevelUIObj.GetComponent<RectTransform>().anchoredPosition = UIElementLocation;
            LevelUIPrefabList.Add(LevelUIObj);
            Button LevelUIButton = LevelUIObj.GetComponentInChildren<Button>();
            //  LevelUIButton.onClick.AddListener(delegate { LoadLevel(CurrentlyCycledLevel.NameToLoad); });
            LevelUIButton.onClick.AddListener(delegate { SelectLevelForDifficultyList(CurrentlyCycledLevel); });
            LevelUIObj.GetComponentInChildren<TextMeshProUGUI>().text = CurrentlyCycledLevel.DisplayedLevelName;
            LevelUIObj.GetComponentInChildren<Image>().sprite = CurrentlyCycledLevel.LevelIcon;
            for (int j = 0; j < CurrentlyCycledLevel.DifficultiesCleared.Count; j++)
            {
                Vector3 MedalPos = LevelUIObj.transform.position;
                MedalPos.x = -40 + (j * 20);
                MedalPos.y = 40;
                GameObject MedalRef = Instantiate(MedalPrefab, Vector3.zero, Quaternion.identity, LevelUIObj.transform);
                MedalRef.GetComponent<RectTransform>().anchoredPosition = MedalPos;
                if (CurrentlyCycledLevel.DifficultiesCleared[j].DifficultyClear)
                {
                   
                    MedalRef.GetComponent<Image>().color = CurrentlyCycledLevel.DifficultiesCleared[j].Difficulty.MedalColour;
                   
                }
                else
                {
                    MedalRef.GetComponent<Image>().color = Color.black;
                }
                LevelUIPrefabList.Add(MedalRef);

            }

            ColumnCount++;
            if (ColumnCount >= Columns)
            {
                ColumnCount = 0;
                RowCount++;
                if (RowCount >= Rows)
                {
                    Debug.Log("Time to add another page!");
                    break;
                }
            }
                //medal placement: top left is -40, 40

            }
        //for every level make new level panel
        //set image to iicon in level info, load difficultie completions

    }

    public void LoadLevel(string LevelName)
    {
        SceneManager.LoadScene(LevelName);
    }
    public void LoadLevel(DifficultyInfo Difficulty)
    {
        CurrentDifficulty = Difficulty;
        SceneManager.LoadScene(CurrentLevel.NameToLoad);
    }

    public void SelectLevelForDifficultyList(LevelInfo id)
    {
        Debug.Log("ID IS " + id);
        CurrentLevel = id;
        ShowAllDifficulties();
    }
    public void ShowAllDifficulties()
    {
        ClearAssets();
        if (MenuSwapScript)
        {
            MenuSwapScript.SwapShownMenu(2);

            //messy woo
            // MenuSwapScript.MenuObjList[2].GetComponentInChildren<TextMeshProUGUI>().text = CurrentLevel.DisplayedLevelName;
            Debug.Log(MenuSwapScript.MenuObjList[2].transform.GetChild(0).GetChild(1).name);
            MenuSwapScript.MenuObjList[2].transform.GetChild(0).transform.GetChild(2).GetComponentInChildren<TextMeshProUGUI>().text = CurrentLevel.DisplayedLevelName;
            MenuSwapScript.MenuObjList[2].transform.GetChild(0).transform.GetChild(3).GetComponentInChildren<TextMeshProUGUI>().text = CurrentLevel.LevelDescription;
            MenuSwapScript.MenuObjList[2].transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = CurrentLevel.LevelIcon; // VERY BAD PROGRAMMING. not to self: don't be an idiot and put your level loading and menu loading scripts in the same file.

            /*
            LevelUIButton.onClick.AddListener(delegate { SelectLevelForDifficultyList(i); });
            LevelUIObj.GetComponentInChildren<TextMeshProUGUI>().text = CurrentlyCycledLevel.DisplayedLevelName;

            Button LevelUIButton = LevelUIObj.GetComponentInChildren<Button>();
                //  LevelUIButton.onClick.AddListener(delegate { LoadLevel(CurrentlyCycledLevel.NameToLoad); });
                LevelUIButton.onClick.AddListener(delegate { SelectLevelForDifficultyList(i); });
                LevelUIObj.GetComponentInChildren<TextMeshProUGUI>().text = CurrentlyCycledLevel.DisplayedLevelName;
            */
                for (int j = 0; j < ListOfDifficulties.GlobalDifficultyList.Count; j++)
                {

                DifficultyInfo CurrentlyCycledDifficulty = ListOfDifficulties.GlobalDifficultyList[j];
                /*
                Vector3 DifficultyPos = new Vector3(26.5f, 35.5f, 0);
                //MedalPos.x = -40 + (j * 20);
                DifficultyPos.y += (-20 * j);
                */
                Vector3 DifficultyPos = new Vector3(-42.5f, -33.2f, 0);
                //MedalPos.x = -40 + (j * 20);
                DifficultyPos.x += (+22.5f * j);
                GameObject ButtonRef = Instantiate(ButtonTemplate, Vector3.zero, Quaternion.identity, MenuSwapScript.MenuObjList[2].transform);
               
                ButtonRef.GetComponent<RectTransform>().sizeDelta = new Vector2(65, 35);
                ButtonRef.GetComponent<RectTransform>().anchoredPosition = DifficultyPos;
                if (CurrentLevel.DifficultiesCleared.Count < j && CurrentLevel.DifficultiesCleared[j].DifficultyClear == true)
                {
                    ButtonRef.GetComponent<Image>().color = Color.green;
                    
                    
                       
                    
                }
                else
                {
                    ButtonRef.GetComponent<Image>().color = CurrentlyCycledDifficulty.MedalColour;
                }
                ButtonRef.GetComponentInChildren<TextMeshProUGUI>().text = CurrentlyCycledDifficulty.DifficultyName;
               
                ButtonRef.GetComponent<Button>().onClick.AddListener(delegate { LoadLevel(CurrentlyCycledDifficulty); }); // load via difficulty. idk why I have to do -1 when everything else works like that. Is it adding 1 somewhere??
                ButtonList.Add(ButtonRef);
              


            }

               
                //medal placement: top left is -40, 40

            
        }

    }
}
