
using UnityEngine;

public class placeHolderColor : MonoBehaviour
{
    public GameObject[] objects;

    
    public Color col;

    public Material p;

    public void colidingColor() {

        foreach (GameObject obj in objects) {
            obj.GetComponent<MeshRenderer>().material.color = col;
        }
    }

    public void notColidingColor() {
    
        foreach (GameObject obj in objects) {
            obj.GetComponent<MeshRenderer>().material.color = p.color;
        }     
    }



}
