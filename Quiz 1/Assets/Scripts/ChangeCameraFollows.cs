using UnityEngine;
using System.Collections;
public class ChangeCameraFollows : MonoBehaviour
{
    public GameObject target; // the target that the camera should look at
    void Start()
    {
        if (target == null)
        {
            target = this.gameObject;
            Debug.Log("ChangeCameraFollows target not specified. Defaulting to parent GameObject");
        }
    }
    void OnMouseDown()
    {
        // change the target of the LookAtTarget script to be this gameobject.
        CameraFollows.target = target;
        // change the field of view on the perspective camera based on the distance from center of world, clamp it to a reasonable field of view
        float v = Mathf.Clamp(60 * target.transform.localScale.x, 1, 100);
        Camera.main.fieldOfView = v;
    }
}