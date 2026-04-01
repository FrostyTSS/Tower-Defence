using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;




[System.Serializable]
[CreateAssetMenu(fileName = "TowerAbilityPin", menuName = "TowerInfo/TowerAbilityBase/TowerAbilityPin", order = 2)]

public class PinAbilityScript : TowerAbility
{
    //private GameObject SwordSwipeObj;
    //public float Cooldown = 5f;
    //public Sprite AbilityImage;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject AbilityProjType;
    public int AbilityAmount;
    public float TimeToLastFor = 3.5f;
    public float NewDelay = 0.2f;
    

    public override void OnUse(BaseTower TowerRef)
    {
        Debug.Log(1 % 1);
        Debug.Log(1.5 % 1);
        Debug.Log(2 % 1);
        
        if (TowerRef is PinTowerScript)
        {
            PinTowerScript PinRef = (PinTowerScript)TowerRef;
            PinRef.AbilityCaller();
            //  PinRef.PinShoot(AbilityProjType, AbilityAmount);
        }
        
    }

    
}
