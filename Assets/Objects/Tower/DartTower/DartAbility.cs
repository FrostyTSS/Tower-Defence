using UnityEngine;



[System.Serializable]
[CreateAssetMenu(fileName = "TowerAbilityDart", menuName = "TowerInfo/TowerAbilityBase/TowerAbilityDart", order = 1)]

public class DartAbility : TowerAbility
{
    public GameObject SwordSwipeObj;
    //public float Cooldown = 5f;
    //public Sprite AbilityImage;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void OnUse(BaseTower TowerRef)
    {
        Debug.Log("Swipe!");
        if (SwordSwipeObj)
        {
            Instantiate(SwordSwipeObj, TowerRef.transform.position, TowerRef.transform.rotation);
        }
    }
    
}
