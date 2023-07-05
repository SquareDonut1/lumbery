using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buildingManager : MonoBehaviour
{
    public int Reach;
    public Buildables[] buildables;
    GameObject PlaceHolder;
    public int SelectedBuildable = 0;
    int lastSelectedBuildable = 0;
    public bool isColiding = false;
    Collider[] c;
    public LayerMask objectLayer;
    public LayerMask deleteLayer;

    public Dictionary<Vector3,GameObject> placedPrefabs = new Dictionary<Vector3, GameObject>();


    // Update is called once per frame
    private void Update()
    {
        int objectLayerint = LayerMask.NameToLayer(buildables[SelectedBuildable].prefab.name);
        objectLayer = 1 << objectLayerint;


        GetImputs();
                          
    }
    public void GetImputs() {


        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

        RaycastHit hit;

        if (Input.GetKeyDown(KeyCode.Alpha1))
            SelectedBuildable = 0;

        if (Input.GetKeyDown(KeyCode.Alpha2))
            SelectedBuildable = 1;

        if (Input.GetKeyDown(KeyCode.Alpha3))
            SelectedBuildable = 2;

        if (Input.GetMouseButtonDown(0) && !isColiding) {                                                

            if (Physics.Raycast(ray, out hit, Reach) && hit.transform.CompareTag("ground")) {

                placeObjectG(buildables[SelectedBuildable].prefab, hit.point);  

            }  else if (Physics.Raycast(ray, out hit, Reach, objectLayer) && buildables[SelectedBuildable].prefab.name == "wall") {

                placeObject(buildables[SelectedBuildable].prefab, hit.transform.position, hit.transform.rotation);

            }  else if (Physics.Raycast(ray, out hit, Reach, objectLayer)) {

                placeObject(buildables[SelectedBuildable].prefab, hit.transform.position, Quaternion.identity);
          
            }      
        }


        if (Input.GetMouseButtonDown(1)){
      
            if (Physics.Raycast(ray, out hit, Reach, deleteLayer)) {
                deleteObject(hit.transform.gameObject);
            }
        }


        if (lastSelectedBuildable != SelectedBuildable) {

            Destroy(PlaceHolder);
            print("deleted");
        }


        //placing placHolder
        if (Physics.Raycast(ray, out hit, Reach, objectLayer)) {
            
            

            if (hit.transform.CompareTag("colider") && buildables[SelectedBuildable].prefab.name == "wall") {

                UpdatePlaceHolder(hit.transform.position, buildables[SelectedBuildable].placeHolder, hit.transform.rotation);

            } else if (hit.transform.CompareTag("colider")) {

                UpdatePlaceHolder(hit.transform.position, buildables[SelectedBuildable].placeHolder, Quaternion.identity);

            }    

        } else if (Physics.Raycast(ray, out hit, Reach)) {
       
            if (hit.transform.CompareTag("ground") && buildables[SelectedBuildable].canPlaceOnGround) {
         
                UpdatePlaceHolder(hit.point, buildables[SelectedBuildable].placeHolder, Quaternion.identity);

            }  
            
        } else {

            Destroy(PlaceHolder);
      
        }

        lastSelectedBuildable = SelectedBuildable;

    }


    public void deleteObject(GameObject obj) {

        foreach (GameObject j in obj.GetComponent<CheckColiders>().Coliders) {

            if (placedPrefabs.ContainsKey(RoundedPos(j.transform.position))) {

                placedPrefabs[RoundedPos(j.transform.position)].GetComponent<CheckColiders>().Check(obj,true);
                obj.GetComponent<CheckColiders>().Check(j,true);

            }
        }

        placedPrefabs.Remove(obj.transform.position);
        Destroy(obj);


    }
    public void UpdatePlaceHolder(Vector3 pos,GameObject obj, Quaternion rot) {
       
      
        c = null;

        if (PlaceHolder != null) {

            Destroy(PlaceHolder);
        }
        

        if (placedPrefabs.ContainsKey(RoundedPos(pos)))
            return;

        if (buildables[SelectedBuildable].prefab.name == "piller") {
            //c = Physics.OverlapBox(pos, buildables[SelectedBuildable].prefab.GetComponent<BoxCollider>().size * 2f - new Vector3(0,0.5f,0));
        } else if(buildables[SelectedBuildable].prefab.name == "platform") {

            c = Physics.OverlapBox(pos, buildables[SelectedBuildable].prefab.GetComponent<BoxCollider>().size * 0.98f);
       
        }


        isColiding = false;

        if (c != null) {
  
            foreach (Collider col in c) {

            
                if (col.CompareTag("placedObject")) {
                  
                    isColiding = true;
                }
            }
        }

        if (isColiding) {
            PlaceHolder = Instantiate(obj, RoundedPos(pos), rot, this.transform);
            PlaceHolder.GetComponent<placeHolderColor>().colidingColor();
        } else {
            PlaceHolder = Instantiate(obj, RoundedPos(pos), rot, this.transform);
            PlaceHolder.GetComponent<placeHolderColor>().notColidingColor();
        }


        

    }
    public void placeObjectG(GameObject obj, Vector3 pos) {


        GameObject temp = Instantiate(obj, RoundedPos(pos), Quaternion.identity, this.transform);
        placedPrefabs.Add(temp.transform.position, temp);
        Destroy(PlaceHolder);

    }
    public void placeObject(GameObject obj, Vector3 pos , Quaternion rot) {
       
        if (placedPrefabs.ContainsKey(RoundedPos(pos))) {

            return;
        }
      
        GameObject temp = Instantiate(obj, RoundedPos(pos), rot, this.transform);

        placedPrefabs.Add(RoundedPos(temp.transform.position), temp);

       

          foreach (GameObject j in temp.GetComponent<CheckColiders>().Coliders) {
      
              if (placedPrefabs.ContainsKey(RoundedPos(j.transform.position))) {
                placedPrefabs[RoundedPos(j.transform.position)].GetComponent<CheckColiders>().Check(temp,false);
                temp.GetComponent<CheckColiders>().Check(j,false);

              }   
          }
        Destroy(PlaceHolder);
    }

    public Vector3 RoundedPos(Vector3 pos) {
        Vector3 roundedPos;
        roundedPos = new Vector3(Mathf.Round(pos.x * 10.0f) * 0.1f, Mathf.Round(pos.y * 10.0f) * 0.1f, Mathf.Round(pos.z * 10.0f) * 0.1f);
        return roundedPos;
    }

}

[System.Serializable]
public class Buildables {
   
    public GameObject prefab;
    public GameObject placeHolder;
    public bool canPlaceOnGround;
}
