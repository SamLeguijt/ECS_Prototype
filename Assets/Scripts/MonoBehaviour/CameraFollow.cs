using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Camera cam = null;
    [SerializeField] private Transform followTarget = null;
    [SerializeField] private float camSmoothSpeed = .1f;

    private Vector3 startPos;

    // Start is called before the first frame update
    void Start()
    {
        if (followTarget == null || cam == null)
            this.enabled = false;

        startPos = transform.position;  
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetPos = new Vector3(followTarget.position.x, startPos.y, followTarget.position.z);

        Vector3 smoothedPosition = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * camSmoothSpeed);
        transform.position = smoothedPosition;
    }
}
