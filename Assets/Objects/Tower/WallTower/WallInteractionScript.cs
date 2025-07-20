using UnityEngine;
using System.Collections;
public class WallInteractionScript : MonoBehaviour
{
    public BaseTower OwningTower;
    int EnemyLayerID;
    public int StartingUses = 10;
    public int RemainingUses = 10;
    public float RechargeTime = 2.5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RemainingUses = StartingUses;
        EnemyLayerID = LayerMask.NameToLayer("Enemy");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
         if (other.gameObject.layer == EnemyLayerID && other.gameObject.GetComponent<BaseEnemy>().CurrentSpeed >= other.gameObject.GetComponent<BaseEnemy>().MaxSpeed && RemainingUses > 0)
     //   if (other.gameObject.layer == EnemyLayerID)
        {
            other.gameObject.GetComponent<BaseEnemy>().TakeDamage(1, OwningTower);
            //  other.gameObject.GetComponent<BaseEnemy>().Slowdown(-1.0f);
            other.gameObject.GetComponent<BaseEnemy>().StartCoroutine("Slowdown", 0.25f * other.gameObject.GetComponent<BaseEnemy>().MaxSpeed);
            RemainingUses -= 1;
            Debug.Log("Hit!");
            if (RemainingUses <= 0)
            {
                StartCoroutine(ShieldRecharge());
            }
        }
    }


    public IEnumerator ShieldRecharge()
    {
        this.GetComponent<MeshRenderer>().enabled = false;
        this.GetComponent<BoxCollider>().enabled = false;
        yield return new WaitForSeconds(RechargeTime);
        RemainingUses = StartingUses;
        this.GetComponent<MeshRenderer>().enabled = true;
        this.GetComponent<BoxCollider>().enabled = true;
    }

}
