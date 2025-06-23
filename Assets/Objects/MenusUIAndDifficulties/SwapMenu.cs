using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SwapMenu : MonoBehaviour
{
    public List<GameObject> MenuObjList = new List<GameObject>();
    public int CurrentMenuID = 0;


    public void SwapShownMenu(int MenuID)
    {

        if (LevelManager.instance)
        {
            LevelManager.instance.ClearAssets();
        }

        if (MenuID < MenuObjList.Count)
        {

            MenuObjList[CurrentMenuID].SetActive(false);
            if (MenuID >= 0)
            {
                MenuObjList[MenuID].SetActive(true);
                CurrentMenuID = MenuID;
                if (MenuID == 1)
                {
                    LevelManager.instance.ShowAllLevels();
                }
                /*
                if (MenuID == 2)
                {
                    LevelManager.instance.ShowAllDifficulties();
                }
                */

            }
            
        }
        else
        {
            Debug.Log("No menu by that ID!");
        }

    }

   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < MenuObjList.Count; i++)
        {
            MenuObjList[i].SetActive(false);
        }

        SwapShownMenu(0);
    }

    public void QuitPassthrough()
    {
        if (LevelManager.instance)
        {
            LevelManager.instance.QuitGame();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
