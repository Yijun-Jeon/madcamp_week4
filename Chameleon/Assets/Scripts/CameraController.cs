using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Vector3 offset = new Vector3(0f, 0f, -10f);
    [SerializeField] private FieldOfView fieldOfView;
    private float smoothTime = 0.25f;
    private Vector3 velocity = Vector3.zero;
    public Transform target = null;
    

    private void Update()
    {
        if(target != null)
        {
            Vector3 targetPosition = target.position + offset;
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
            fieldOfView.SetOrigin(target.position);
        }
    }
}