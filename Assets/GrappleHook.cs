using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class GrappleHook : MonoBehaviour
{
    [SerializeField] private float grappleSpeed = 40f;
    [SerializeField] private float grappleRadius = 0.25f;
    [SerializeField] private float grappleRange = 15;
    [SerializeField] private float grappleDelay = 0.5f;

    private float grappleTime;

    [SerializeField] private float pullForce;

    private Vector2 grappleVelocity;
    private Vector2 grappleDirection;

    private bool isGrappling;

    private Vector2 previousHookPosition;
    Transform _hookTransform;
    GameObject _hookedObject;
    private float distanceTravelled;

    private Rigidbody2D rb;

    private void Awake()
    {
        _hookTransform = new GameObject($"{gameObject.name} HookTransform").transform;
        rb = GetComponent<Rigidbody2D>();
    }

    public void Fire(Vector2 direction)
    {
        if (grappleTime >= Time.time) return;

        grappleTime = Time.time + grappleDelay;
        isGrappling = true;
        grappleDirection = direction.normalized;
        grappleVelocity = grappleDirection * grappleSpeed;
        previousHookPosition = transform.position;
        _hookTransform.position = transform.position;
        distanceTravelled = 0;
    }

    public void Pull(RaycastHit2D hit)
    {
        Debug.Log($"pulling {hit.collider.gameObject.name}");
        if(hit.collider.TryGetComponent(out Rigidbody2D hitRb))
        {
            Vector2 pullDirection = (rb.position - hitRb.position).normalized;
            Vector2 force = pullDirection * pullForce;
            hitRb.AddForce(force, ForceMode2D.Impulse);
        }
        else
        {
            Vector2 pullDirection = (hit.point - rb.position).normalized;
            Vector2 force = pullDirection * pullForce;
            rb.velocity = force;
        }
        isGrappling = false;
    }

    private void FixedUpdate()
    {
        if (!isGrappling) return;

        _hookTransform.position += (Vector3)grappleVelocity * Time.fixedDeltaTime;
        float distanceTravelledThisFrame = Vector2.Distance(_hookTransform.position, previousHookPosition);
        distanceTravelled += distanceTravelledThisFrame;
        if(distanceTravelled > grappleRange)
        {
            float extraDistance = distanceTravelled - grappleRange;
            distanceTravelledThisFrame -= extraDistance;
            _hookTransform.position -= (Vector3)grappleDirection * extraDistance;
            isGrappling = false;
        }
        RaycastHit2D hit = Physics2D.CircleCast(previousHookPosition, grappleRadius, grappleDirection, distanceTravelledThisFrame);

        if (hit && hit.collider.gameObject != gameObject)
        {
            Pull(hit);
        }
        previousHookPosition = _hookTransform.position;
    }

    private void OnDrawGizmos()
    {
        if(Application.isPlaying) 
            Gizmos.DrawWireSphere(_hookTransform.position, grappleRadius);
    }
}
