using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lava : MonoBehaviour
{
    private Vector3 originalPos;
    public float riseVelocity = 5;
    public float initialMultiplier = 2.5f;
    private float currentVelocity;
    public float riseAccel = 0.03f;
    private bool initialRise;
    public bool rising { get; private set; }
    public Transform player;
    private void Awake()
    {
        originalPos = transform.position;
    }
    private void Update()
    {
        if (!rising) { return; }
        Vector3 tempVector = transform.position;
        if (initialRise && DistToPlayer() > 50)
        {
            tempVector.y += currentVelocity * initialMultiplier * Time.deltaTime;
            if (DistToPlayer() < 50)
            {
                initialRise = false;
            }
        }
        else {
            tempVector.y += currentVelocity * Time.deltaTime;
        }
        transform.position = tempVector;
        currentVelocity += riseAccel * Time.deltaTime;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player")) {
            try { FindObjectOfType<GameManager>().GameOver(); }
            catch (System.Exception) { }
        }
    }
    public void Reset()
    {
        transform.position = originalPos;
        currentVelocity = riseVelocity;
        initialRise = false;
        rising = false;
    }
    public void StartRise() {
        initialRise = true;
        rising = true;
    }
    public void StopRise() {
        rising = false;
    }
    public float DistToPlayer() {
        return player.transform.position.y - transform.position.y;
    }
}
