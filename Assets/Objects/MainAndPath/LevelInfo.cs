using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using static BuffAttributes;
using static LevelInfo;




[CreateAssetMenu(fileName = "LevelInfo", menuName = "MapGameplayInfo/LevelInfo", order = 1)]
public class LevelInfo : ScriptableObject
{
    public Sprite LevelIcon;
    public string DisplayedLevelName = "Temp";
    public string NameToLoad = "Temp";
    public string LevelDescription = "Temp";
    public DifficultyList ListOfDifficulties;

    public List<DifficultyClearInfo> DifficultiesCleared;

    public int StartingLobbyTheme;
    public int StartingRoundTheme;
    public int StartingChatTheme;

    [Serializable]
    public struct DifficultyClearInfo
    {
        public DifficultyInfo Difficulty;
        public bool DifficultyClear;
    }


   

    private void OnValidate()
    {
        if (ListOfDifficulties)
        {
            for (int i = 0; i < ListOfDifficulties.GlobalDifficultyList.Count; i++) // populate list with difficulties from this list
            {

                bool AlreadyAdded = false;
                for (int j = 0; j < DifficultiesCleared.Count; j++)
                {
                    if (DifficultiesCleared[j].Difficulty == ListOfDifficulties.GlobalDifficultyList[i])
                    {
                        AlreadyAdded = true;
                    }
                }

                if (AlreadyAdded == false)
                {
                    DifficultyClearInfo CurrentInfo = new DifficultyClearInfo();
                    CurrentInfo.Difficulty = ListOfDifficulties.GlobalDifficultyList[i];

                    DifficultiesCleared.Add(CurrentInfo);
                }
               // DifficultiesCleared[0].DifficultyClear = true;
               // DifficultiesCleared[i].Difficulty = DifficultyList.instance.GlobalDifficultyList[i];
            }    


        }
    }

}
