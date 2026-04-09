using UnityEngine;




[System.Serializable]
[CreateAssetMenu(fileName = "TowerAbilitySniper", menuName = "TowerInfo/TowerAbilityBase/TowerAbilitySniper", order = 1)]

public class SniperAbility : TowerAbility
{
    //private GameObject SwordSwipeObj;
    //public float Cooldown = 5f;
    //public Sprite AbilityImage;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float TotalRotAngles = 1800;
    public override void OnUse(BaseTower TowerRef)
    {
        //SceneManager.LoadScene("MainMenuScene");
        /*
        if (SwordSwipeObj == null && TowerRef.GetComponent<SpinForSprite>())
        {
            SwordSwipeObj = TowerRef.GetComponent<SpinForSprite>().SwordSwingObj;
        }
        */
        if (TowerRef.GetComponent<SniperTowerScript>())
        {
            
            TowerRef.GetComponent<SniperTowerScript>().AbilityCaller(); 
            
        }

       
        Debug.Log("Swipe!");
        /*
        if (SwordSwipeObj)
        {
            Instantiate(SwordSwipeObj, TowerRef.transform.position, TowerRef.transform.rotation);
        }
        */
    }
    
}
