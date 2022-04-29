using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Grapple : MonoBehaviour
{
    
    public Camera mainCamera;
    public Transform ropeOrigin;
    public Vector3 tetherPoint;
    public LayerMask grapplableLayers;
    public float fireDuration;
    public float maxLength;
    public float minLength;
    public GameObject claw;
    private Vector3 clawOrigin;
    private Quaternion clawOriginRot;
    public float currentLength { get; private set; }

    [HideInInspector]
    public PlayerMovement player;
    public bool returning { get; private set; }
    public bool attached { get; private set; }
    public bool buffering { get; private set; }
    public LineRenderer ropeRenderer { get; private set; }
    public SpringJoint joint { get; private set; }
    private void Awake()
    {
        ropeRenderer = gameObject.GetComponent<LineRenderer>();
        player = gameObject.GetComponent<PlayerMovement>();
        joint = null;
        clawOrigin = claw.transform.localPosition;
        clawOriginRot = claw.transform.localRotation;
    }

    public void Update()
    {
        ropeRenderer.SetPosition(0, ropeOrigin.position);
        if (player.enabled)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (!returning)
                {
                    StartCoroutine("CR_FireRope");
                }
                else
                {
                    buffering = true;
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (!returning)
                {
                    StartCoroutine("CR_ReturnRope");
                }
                else
                {
                    buffering = false;
                }
            }
        }
    }
    public void Extend(float extensionAmount) {
        if (!attached) { return; }
        currentLength += extensionAmount;
        currentLength = Mathf.Clamp(currentLength, minLength, maxLength);
        if (joint != null) {
            joint.maxDistance = currentLength;
        }
    }
    public IEnumerator CR_FireRope() {
        if (!attached)
        {
            Vector3 origin = ropeOrigin.position;
            Vector3 targetPoint;
            RaycastHit hit;
            if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit, maxLength, grapplableLayers))
            {
                targetPoint = hit.point;
            }
            else {
                targetPoint = mainCamera.transform.position + mainCamera.transform.forward * maxLength;
            }
            ropeRenderer.enabled = true;
            for (float time = 0; time < fireDuration; time += Time.deltaTime) {
                ropeRenderer.SetPosition(1, Vector3.Lerp(origin, targetPoint, time/fireDuration));
                claw.transform.position = ropeRenderer.GetPosition(1);
                claw.transform.LookAt(ropeRenderer.GetPosition(1));
                if (!Input.GetMouseButton(0)) {
                    StartCoroutine("CR_ReturnRope");
                    yield break;
                }
                yield return null;
            }
            ropeRenderer.SetPosition(1, targetPoint);
            claw.transform.position = targetPoint;
            if (hit.collider != null && player.enabled)
            {
                tetherPoint = targetPoint;
                joint = player.gameObject.AddComponent<SpringJoint>();
                joint.autoConfigureConnectedAnchor = false;
                joint.connectedAnchor = tetherPoint;
                attached = true;

                float distanceFromPoint = Vector3.Distance(player.transform.position, tetherPoint);
                currentLength = Mathf.Max(distanceFromPoint * 0.8f, minLength*1.1f);
                joint.maxDistance = currentLength;
                joint.minDistance = minLength;
                joint.spring = 1000f;
                joint.damper = 900f;
                joint.massScale = 4.5f;
            }
            else {
                StartCoroutine("CR_ReturnRope");
            }
        }
        else {
            yield return null;
        }
    }
    public IEnumerator CR_ReturnRope() {
        if (!returning)
        {
            returning = true;
            attached = false;
            if (joint != null) {
                Destroy(joint);
                joint = null;
            }
            if (ropeRenderer.enabled) {
                Vector3 startPoint = ropeRenderer.GetPosition(ropeRenderer.positionCount - 1);
                for (float time = 0; time < fireDuration * 0.35f; time += Time.deltaTime)
                {
                    ropeRenderer.SetPosition(1, Vector3.Lerp(startPoint, ropeOrigin.position, time / (0.35f * fireDuration)));
                    claw.transform.position = ropeRenderer.GetPosition(1);
                    yield return null;
                }
                ropeRenderer.enabled = false;
            }
            claw.transform.localPosition = clawOrigin;
            claw.transform.localRotation = clawOriginRot;
            returning = false;
            if (buffering) {
                buffering = false;
                StartCoroutine("CR_FireRope");
            }
        }
        else
        {
            yield return null;
        }
    }
    public void Reset()
    {
        StopAllCoroutines();
        tetherPoint = Vector3.zero;
        ropeRenderer.enabled = false;
        returning = false;
        attached = false;
        buffering = false;
        if (joint != null)
        {
            Destroy(joint);
            joint = null;
        }
        claw.transform.localPosition = clawOrigin;
        claw.transform.localRotation = clawOriginRot;
    }
}
