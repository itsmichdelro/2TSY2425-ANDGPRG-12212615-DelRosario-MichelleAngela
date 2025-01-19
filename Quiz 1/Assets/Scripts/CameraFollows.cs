using UnityEngine;
using System.Collections;
public class CameraFollows : MonoBehaviour
{
    static public GameObject target; // the target that the camera should look at
    void Start()
    {
        if (target == null)
        {
            target = this.gameObject;
            Debug.Log("CameraFollows target not specified. Defaulting to parent GameObject");
        }
    }
    void Update()
    {
        if (target)
            transform.LookAt(target.transform);
    }
}