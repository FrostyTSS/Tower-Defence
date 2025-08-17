using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

//using static UnityEditor.Rendering.CameraUI;

public class TowerPlacement : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    Vector3 OriginalHUDPosition;
    Vector3 output = Vector2.zero;
    public Vector3 PlacedTowerRotation = new Vector3(0, 90, 0);
    public Canvas HolderCanvas;
    Transform HolderObject;
    public GameObject TowerToSpawn;
    public string Description;
    public int Cost;

    //noises for dragging.
    public AudioClip PickupSound;
    public AudioClip DropoffSound;
    public float SoundVolume = 0.75f;
    void Start()
    {

        if (!this.GetComponent<AudioSource>())
        {
            gameObject.AddComponent<AudioSource>();
            this.GetComponent<AudioSource>().volume = SoundVolume;
        }

        //to do: main menu, pause (yikes), test autoround and flame pin upgrades.. or pin at all tbh.

        OriginalHUDPosition = GetComponent<RectTransform>().localPosition;
        //OriginalHUDPosition.x += 100; // It spawns offset to the side for.. reasons. So We offset it BACK!
        transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().sprite = TowerToSpawn.GetComponentInChildren<SpriteRenderer>().sprite;
        HolderObject = this.transform.parent; //oh, you can't ignore the horizontal mask, jank time.


        //GetComponentInChildren<UnityEngine.UI.Image>().sprite = TowerToSpawn.GetComponentInChildren<SpriteRenderer>().sprite; // why doesnt this work :(
        //Debug.Log(TowerToSpawn.transform.GetChild(0).name);
    }

    // Update is called once per frame
    void Update()
    {

    }





    public void OnBeginDrag(PointerEventData eventData)
    {
        if (PathHolder.instance.Lives > 0)
        {
            this.transform.SetParent(HolderCanvas.transform, false);
            if (this.GetComponent<AudioSource>() && PickupSound)
            {
                //this.GetComponent<AudioSource>().clip = PickupSound;
                this.GetComponent<AudioSource>().PlayOneShot(PickupSound);
            }
            if (PathHolder.instance)
            {
                //overlay of range
                PathHolder.instance.RangeVisual.SetActive(true);
                PathHolder.instance.RangeVisual.layer = 5;
                PathHolder.instance.DraggingTower = true;
            }
        }
    }

    public void OnDrag(PointerEventData data)
    {

        RectTransformUtility.ScreenPointToWorldPointInRectangle(HolderCanvas.GetComponent<RectTransform>(), Input.mousePosition, HolderCanvas.GetComponent<Canvas>().worldCamera, out output);

        if (PathHolder.instance)
        {

            int TowerLayerID = LayerMask.NameToLayer("Tower");
            int WaterLayerID = LayerMask.NameToLayer("Water");

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // Construct a ray from the current mouse coordinates
            RaycastHit hit;
            // ray.origin = GetComponent<RectTransform>().position;


            //  if (Physics.Raycast(ray, out hit, 9999, GroundCheck))
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject.layer == TowerLayerID || hit.transform.gameObject.layer == WaterLayerID)
                {
                    GetComponent<Image>().color = Color.red;
                }
                else
                {
                    GetComponent<Image>().color = Color.white;
                }
            }
            else
            {
                GetComponent<Image>().color = Color.white;
            }


            PathHolder.instance.RangeVisual.transform.position = output;
            PathHolder.instance.RangeVisual.transform.localScale = Vector3.one;
            PathHolder.instance.RangeVisual.transform.localScale *= TowerToSpawn.GetComponent<BaseTower>().Range * 4; // diameter to range, multiplied due to UI scaling
            GetComponent<RectTransform>().position = output;
        }


    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GetComponent<Image>().color = Color.white;

        int TowerLayerID = LayerMask.NameToLayer("Tower");
        int WaterLayerID = LayerMask.NameToLayer("Water");
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // Construct a ray from the current mouse coordinates
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.gameObject.layer != TowerLayerID && hit.transform.gameObject.layer != WaterLayerID)
            {

                if (PathHolder.instance && PathHolder.instance.Money >= Cost)
                {
                    PathHolder.instance.RangeVisual.layer = 0;
                    PathHolder.instance.RangeVisual.SetActive(false);
                    if (this.GetComponent<AudioSource>() && DropoffSound)
                    {
                        //this.GetComponent<AudioSource>().clip = PickupSound;
                        this.GetComponent<AudioSource>().PlayOneShot(DropoffSound);
                    }

                    //try for orthographic
                    /*var scrPoint : Vector3 = Vector3(x,y, 0);
            var ray : Ray = Camera.main.ScreenPointToRay(scrPoint);
            var hit : RaycastHit;
            if (Physics.Raycast (ray, hit)) {
            var hitPoint : Vector3 = hit.point;
            }
                    */


                    // ray.origin = GetComponent<RectTransform>().position;


                    //  if (Physics.Raycast(ray, out hit, 9999, GroundCheck))

                    // Debug.Log(hit.transform.gameObject.layer + " + " + TowerLayerID);
                    Debug.DrawLine(ray.origin, hit.point, Color.red, 500);
                    //Debug.Log(hit.point);
                    //Instantiate (particle, hit.point, transform.rotation); // Create a particle if hit
                    PathHolder.instance.Money -= Cost;
                    PathHolder.instance.UpdateMoneyCounter();
                    GameObject TowerRef = Instantiate(TowerToSpawn, hit.point, Quaternion.Euler(PlacedTowerRotation));
                    if (!TowerRef.GetComponent<BaseTower>().LevelPath)
                    {
                        TowerRef.GetComponent<BaseTower>().LevelPath = PathHolder.instance; // get pathholder if missing
                    }
                    PathHolder.instance.PlacedTowers.Add(TowerRef.GetComponent<BaseTower>());


                    //   {

                }
                

            }
        }
        this.transform.SetParent(HolderObject, false);
        // this.transform.parent = HolderObject;
        GetComponent<RectTransform>().localPosition = OriginalHUDPosition;
        PathHolder.instance.RangeVisual.layer = 0;
        PathHolder.instance.RangeVisual.SetActive(false);
        PathHolder.instance.DraggingTower = false;
    }
}

    /*
    private void OnMouseDrag()
    {
        Debug.Log("DRAG");
          RectTransformUtility.ScreenPointToWorldPointInRectangle(HolderCanvas.GetComponent<RectTransform>(), Input.mousePosition, HolderCanvas.GetComponent<Canvas>().worldCamera, out output);
        GetComponent<RectTransform>().position = output;
    }

    private void OnMouseUp()
    {
        GetComponent<RectTransform>().position = OriginalHUDPosition;
    }
    */
    
