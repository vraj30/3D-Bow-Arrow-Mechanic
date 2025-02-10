﻿using UnityEngine;

public class Arrow : MonoBehaviour
{
    private Rigidbody rb;
    private bool hasHit = false;
    private bool cameraReturned = false;
    private CameraSwitcher cameraSwitcher;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    public void SetCameraSwitcher(CameraSwitcher switcher)
    {
        cameraSwitcher = switcher;
    }

    public void LaunchArrow(float power, Vector3 shootDirection)
    {
        rb.isKinematic = false;
        rb.useGravity = true;

        Quaternion customRotation = Quaternion.LookRotation(shootDirection.normalized);
        customRotation *= Quaternion.Euler(90, 0, 0);
        transform.rotation = customRotation;

        rb.AddForce(shootDirection.normalized * power, ForceMode.Impulse);

        Invoke("DestroyProj", 4f); // Fallback if arrow misses everything
    }

    void DestroyProj()
    {
        if (!cameraReturned)
        {
            cameraReturned = true;
            if (cameraSwitcher != null)
            {
                cameraSwitcher.ReturnToPlayerView(() => Destroy(gameObject));  //  Wait for camera to switch, then destroy
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    void FixedUpdate()
    {
        if (!hasHit && rb.linearVelocity.sqrMagnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(rb.linearVelocity.normalized);
            targetRotation *= Quaternion.Euler(90, 0, 0);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 8f);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!hasHit)
        {
            hasHit = true;

            rb.isKinematic = true;
            rb.useGravity = false;

            ContactPoint contact = collision.contacts[0];
            transform.position = contact.point;
            transform.rotation = Quaternion.LookRotation(-contact.normal);
            transform.Rotate(90, 0, 0);

            transform.SetParent(collision.transform);

            CancelInvoke("DestroyProj");  //  Cancel auto-destroy if collision happens

            Invoke("HandlePostCollision", 2f); //  Wait 2 seconds before returning to player
        }
    }

    void HandlePostCollision()
    {
        if (!cameraReturned)
        {
            cameraReturned = true;
            if (cameraSwitcher != null)
            {
                cameraSwitcher.ReturnToPlayerView(() => Destroy(gameObject));  // Wait for camera to switch back, then destroy
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
