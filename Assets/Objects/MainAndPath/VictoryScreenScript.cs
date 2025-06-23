using TMPro;
//using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static LevelInfo;

public class VictoryScreenScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject MedalIcon;
    public TextMeshProUGUI VictoryText;
    public void VictoryShow()
    {
        if (LevelManager.instance.CurrentDifficulty)
        { 
        MedalIcon.GetComponent<Image>().color = LevelManager.instance.CurrentDifficulty.MedalColour;
            if (LevelManager.instance.CurrentLevel)
            {
                /*
                for (int i = 0; i < LevelManager.instance.CurrentLevel.DifficultiesCleared.Count; i++)
                {
                    if (LevelManager.instance.CurrentLevel.DifficultiesCleared[i].Difficulty == PathHolder.instance.CurrentDifficulty)
                    {

                        //I like using structs for small mini classes that are just info, using them for stuff i edit in the editor. However, when you have to set variables for that, you get.. this.
                        DifficultyClearInfo TempDifficulty =  new DifficultyClearInfo();
                        TempDifficulty.Difficulty = LevelManager.instance.CurrentLevel.DifficultiesCleared[i].Difficulty;
                        TempDifficulty.DifficultyClear = true;
                        LevelManager.instance.CurrentLevel.DifficultiesCleared[i] = TempDifficulty;
                    }
                }
                */
                LevelManager.instance.ClearDifficulty(); // double check !
            }
           
            if (LevelManager.instance.CurrentLevel)
            {
                VictoryText.text = "Beat:" + LevelManager.instance.CurrentLevel.DisplayedLevelName + " on: " + LevelManager.instance.CurrentDifficulty.DifficultyName;
            }
            else //failsafe using the scene name and not designated level name
            {
                VictoryText.text = "Beat:" + SceneManager.GetActiveScene().name + " on: " + LevelManager.instance.CurrentDifficulty.DifficultyName;
            }
           
         }
        else
        {
            Debug.Log("NO DIFFICULTY");
        }

    }
}
