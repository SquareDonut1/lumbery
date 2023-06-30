using UnityEngine;


public class CheckColiders : MonoBehaviour
{
    public GameObject[] Coliders;
    public GameObject h;
    public buildingManager bm;
    public int objs;






    public void Check(GameObject obj, bool delete) {

        foreach (GameObject j in Coliders) {
            if (Vector3.Distance(j.transform.position, obj.transform.position) < 1f) {

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

}
