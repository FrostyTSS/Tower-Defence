using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShowHintOverlay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    //Currently set up as:
    //0 is name
    //1 is description
    //2 is cash cost if applicable


    // for sizes:
    //tower/upgrade: Desc 18 72, name 18 72, cash 18 72.
    //for description: 10, 72


    private void Start() //dogshit code
    {
        if (LevelManager.instance && PathHolder.instance.HintOverlay)
        {
            LevelInfoHint();
        }
    }

    //Detect if the Cursor starts to pass over the GameObject
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        if (CheckIfPrepared())
        {
            GameObject OverlayRef = PathHolder.instance.HintOverlay;
            OverlayRef.SetActive(true);
            // OverlayRef.GetComponent<RectTransform>().position = this.GetComponent<RectTransform>().position += new Vector3(0,0, 0);
           
            
             if (this.GetComponent<ApplyUpgrade>())
            {
                ApplyUpgrade UpgradeRef = this.GetComponent<ApplyUpgrade>();
                OverlayRef.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = UpgradeRef.UpgradeData.UpgradeName;
                OverlayRef.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = UpgradeRef.UpgradeData.UpgradeDescription;
                OverlayRef.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "$" + UpgradeRef.UpgradeData.Cost;

                OverlayRef.transform.GetChild(1).GetComponent<TextMeshProUGUI>().fontSizeMin = 18;
                OverlayRef.transform.GetChild(2).GetComponent<TextMeshProUGUI>().fontSizeMin = 18;
                // OverlayRef.GetComponent<RectTransform>().position = this.GetComponent<RectTransform>().position + new Vector3(0, 75, 0);

            }
            else if (this.GetComponent<TowerPlacement>())
            {
                TowerPlacement TowerInfoRef = this.GetComponent<TowerPlacement>();
                if (TowerInfoRef.TowerName != "")
                {
                    OverlayRef.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = TowerInfoRef.TowerName; // lazy, fix later
                }
                else
                {
                    OverlayRef.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = TowerInfoRef.TowerToSpawn.name; // lazy, fix later
                }
                   
                OverlayRef.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = TowerInfoRef.Description;
                OverlayRef.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "$" + TowerInfoRef.Cost;
                OverlayRef.transform.GetChild(1).GetComponent<TextMeshProUGUI>().fontSizeMin = 18;
                OverlayRef.transform.GetChild(2).GetComponent<TextMeshProUGUI>().fontSizeMin = 18;
                // i hate ui code
                // OverlayRef.GetComponent<RectTransform>().position = this.GetComponent<RectTransform>().position + new Vector3(25, 0, 0);
                //  OverlayRef.GetComponent<RectTransform>().anchoredPosition = new Vector2(this.GetComponent<RectTransform>().anchoredPosition.x + 50, this.GetComponent<RectTransform>().anchoredPosition.y + 15);
            }
             /*
            else if (LevelManager.instance)
            {
                LevelManager LevelMnger = LevelManager.instance;
                // GameObject OverlayRef = PathHolder.instance.HintOverlay;
                OverlayRef.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = LevelMnger.CurrentLevel.DisplayedLevelName;
                OverlayRef.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = LevelMnger.CurrentLevel.LevelDescription;
                OverlayRef.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Difficulty: " + LevelMnger.CurrentDifficulty;
            }
             */
            else
            {
                PathHolder.instance.HintOverlay.SetActive(false);
            }
            //OverlayRef.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text //probs bad coding doing children like this
        }
        //Output to console the GameObject's name and the following message
       // Debug.Log("Cursor Entering " + name + " GameObject");
    }

    //Detect when Cursor leaves the GameObject
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        if (PathHolder.instance.HintOverlay)
        {
            if (LevelManager.instance)
            {
                LevelInfoHint();
            }
            else
            {
                PathHolder.instance.HintOverlay.SetActive(false);
            }
        }
        //Output the following message with the GameObject's name
      //  Debug.Log("Cursor Exiting " + name + " GameObject");
    }

    void LevelInfoHint()
    {
        LevelManager LevelMnger = LevelManager.instance;
        GameObject OverlayRef = PathHolder.instance.HintOverlay;
        OverlayRef.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = LevelMnger.CurrentLevel.DisplayedLevelName;
        OverlayRef.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = LevelMnger.CurrentLevel.LevelDescription;
        OverlayRef.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Current Difficulty: " + LevelMnger.CurrentDifficulty;
        OverlayRef.transform.GetChild(1).GetComponent<TextMeshProUGUI>().fontSizeMin = 10;
        OverlayRef.transform.GetChild(2).GetComponent<TextMeshProUGUI>().fontSizeMin = 10;
    }

    public bool CheckIfPrepared()
    {
        if (PathHolder.instance.HintOverlay && this.GetComponent<ApplyUpgrade>() || PathHolder.instance.HintOverlay && this.GetComponent<TowerPlacement>() || LevelManager.instance && PathHolder.instance.HintOverlay)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
