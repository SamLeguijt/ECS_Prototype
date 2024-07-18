using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Camera cam = null;
    [SerializeField] private Transform target = null;

    [SerializeField] private KeyCode followKey = KeyCode.Space;

    private Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        offset = target.position - transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(followKey))
        {
            transform.position = target.position - offset;
        }
    }
}
