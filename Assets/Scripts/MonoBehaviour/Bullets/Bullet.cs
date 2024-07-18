using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed;
    public float x;

    private Vector3 stepDirection;
    private float stepSize;

    private Vector3 prevPosition;

    [SerializeField] private LayerMask layerMask = new LayerMask();

    private void Update()
    {
        Move();
        CalculateCollision();
    }

    private void Move()
    {
        transform.Translate(speed * Time.deltaTime * transform.forward, Space.World);
    }


    private void CalculateCollision()
    {
        stepDirection = transform.forward.normalized;
        stepSize = (transform.position - prevPosition).magnitude;

#if UNITY_EDITOR || UNITY_DEBUG
        Debug.DrawRay(prevPosition, stepDirection * stepSize, Color.red);
#endif
        if (Physics.Raycast(prevPosition, stepDirection, out RaycastHit hitInfo, stepSize, layerMask, QueryTriggerInteraction.Ignore))
        {
            OnHit(hitInfo);
        }
        else
        {
            prevPosition = transform.position;
        }
    }

    private void OnHit(RaycastHit hitInfo)
    {
        Debug.Log("Bullet hit");
        
        Destroy(gameObject);
    }
}
