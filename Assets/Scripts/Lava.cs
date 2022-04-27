using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lava : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player")) {
            try { FindObjectOfType<GameManager>().GameOver(); }
            catch (System.Exception) { }
        }
    }
}
