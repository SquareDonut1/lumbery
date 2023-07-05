using UnityEngine;

public class CheckColiders : MonoBehaviour
{
    public GameObject[] Coliders;
    public buildingManager bm;
    public int objs;


    private void Awake() {

        bm = GameObject.Find("World").GetComponent<buildingManager>();

    }

    public void Check(GameObject obj, bool delete) {

        foreach (GameObject j in Coliders) {
          
            if(j.name == "wall") {
               checkWall(j);
            }               


            if (Vector3.Distance(j.transform.position, obj.transform.position) < 0.2f) {

                if (delete) {
                 
                    j.GetComponent<colider>().ObjectP = null;
                    j.SetActive(true);

                } else {

                    j.GetComponent<colider>().ObjectP = obj;
                    j.SetActive(false);

                }      
            }
        }
    }


    public GameObject[] pillerWalls;

    void checkWall(GameObject wall) {

        if (bm.placedPrefabs.ContainsKey(wall.transform.GetChild(0).position)) {

            wall.SetActive(true);
               
        } else {

            wall.SetActive(false);
        
        }
    }
}
