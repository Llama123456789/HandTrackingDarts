using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DartsManager : MonoBehaviour
{
    private Vector3 firstPosition;
    private Quaternion firstRotation;
    private Rigidbody dartRb;
    void Start()
    {
        dartRb = GetComponent<Rigidbody>();
        firstPosition = transform.position;
        firstRotation = transform.rotation;
    }
    public void ResetDarts()
    {
        transform.position = firstPosition;
        transform.rotation = firstRotation;
        dartRb.isKinematic = false;
        dartRb.useGravity = true;
        transform.parent = null;
    }
}
