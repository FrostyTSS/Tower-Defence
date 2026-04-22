using UnityEngine;
[System.Serializable]
[CreateAssetMenu(fileName = "TowerAbilityVillage", menuName = "TowerInfo/TowerAbilityBase/TowerAbilityVillage", order = 1)]
public class VillageAbilityScript : TowerAbility // recharge everyones ability timers
{
    public override void OnUse(BaseTower TowerRef)
    {
        //SceneManager.LoadScene("MainMenuScene");
        /*
        if (SwordSwipeObj == null && TowerRef.GetComponent<SpinForSprite>())
        {
            SwordSwipeObj = TowerRef.GetComponent<SpinForSprite>().SwordSwingObj;
        }
        */
        if (TowerRef.GetComponent<VillageTowerScript>())
        {

           for (int i =0; i < TowerRef.GetComponent<VillageTowerScript>().CurrentlyAffectedTowers.Count; i++)
            {
                TowerRef.GetComponent<VillageTowerScript>().CurrentlyAffectedTowers[i].TowerAbilityCooldown /= 2;
            }
        }


        Debug.Log("Cooldown Erased!!");
        /*
        if (SwordSwipeObj)
        {
            Instantiate(SwordSwipeObj, TowerRef.transform.position, TowerRef.transform.rotation);
        }
        */
    }
}
