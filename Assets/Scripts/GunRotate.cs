using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunRotate : MonoBehaviour
{
    public Grapple grapple;

    private Quaternion originalRotation;
    private Quaternion desiredRotation;
    private float rotationSpeed = 5f;

    private void Awake()
    {
        originalRotation = transform.localRotation;
    }
    void Update()
    {
        if (grapple.ropeRenderer.enabled == true)
        {
            transform.LookAt(grapple.ropeRenderer.GetPosition(1));
        }
        else {
            desiredRotation = originalRotation;
        }

        transform.localRotation = Quaternion.Lerp(transform.localRotation, desiredRotation, Time.deltaTime * rotationSpeed);
    }
}
