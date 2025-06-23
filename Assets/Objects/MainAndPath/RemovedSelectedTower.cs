using UnityEngine;

public class RemovedSelectedTower : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnMouseDown()
    {
        Debug.Log("Goddamnit i got mouse down the wrong way around");
        if (this.gameObject && PathHolder.instance)
        {
            PathHolder.instance.SelectTower(this.gameObject);
        }
    }
}
