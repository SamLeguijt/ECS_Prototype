using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private Transform visualsParent = null;
    [SerializeField] private float speed;
    [SerializeField] private LayerMask layerMask = new LayerMask();

    private Vector3 stepDirection;
    private float stepSize;

    private Vector3 prevPosition;


    private void Update()
    {
        Move();
        CalculateCollision();
    }

    private void Move()
    {
        transform.Translate(speed * Time.deltaTime * transform.forward, Space.World);
    }

    public void Init(BulletData data)
    {
        this.speed = data.Speed;
        this.layerMask = data.CollisionLayers;
    }

    private void CalculateCollision()
    {
        stepDirection = transform.forward.normalized;
        stepSize = (transform.position - prevPosition).magnitude;

        // TODO: Fix rotation.
        visualsParent.rotation = Quaternion.Euler(new Vector3(90,0,0));

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
