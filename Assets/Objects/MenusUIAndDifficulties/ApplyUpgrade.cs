using UnityEngine;

public class ApplyUpgrade : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public UpgradeInfo UpgradeData;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

  public  void UpgradeApply()
    {
       
        if (PathHolder.instance.Money >= UpgradeData.Cost && PathHolder.instance.Lives > 0)
        {
            PathHolder.instance.SelectedTower.ApplyUpgrade(UpgradeData, false);
            //jank cleanup lol
            PathHolder.instance.Money -= UpgradeData.Cost;
            PathHolder.instance.UpdateMoneyCounter();
            PathHolder.instance.ShowUpgradeUI(false);
            PathHolder.instance.InitRangeVisual(PathHolder.instance.SelectedTower.gameObject);
            PathHolder.instance.ShowUpgradeUI(true);
        }
    }
}
