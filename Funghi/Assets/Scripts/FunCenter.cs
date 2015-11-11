using UnityEngine;
using System.Collections;

public class FunCenter : MonoBehaviour
{
    Vector3 target;
    bool reachedTarget;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetNewPosition(Vector3 _pos)
    {
        reachedTarget = false;

    }


    /*
    void OnMouseDrag()
    {
        float distance = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance));
    }*/

}
