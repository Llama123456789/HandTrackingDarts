using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DartsManager : MonoBehaviour
{
    private Vector3 firstPosition;
    private Quaternion firstRotation;
    [SerializeField]
    private GameObject Dart;
    [SerializeField]
    private GameObject[] tryDartsBase = new GameObject[3];
    [SerializeField]
    private GameObject[] tryDarts = new GameObject[3];
    private Vector3[] tryDartsFirstPos = new Vector3[3];
    private Quaternion[] tryDartsFirstRot = new Quaternion[3];
    private Rigidbody[] tryDartsRb = new Rigidbody[3];
    [SerializeField]
    private GameObject[] tryDartsTip = new GameObject[3];
    void Start()
    {
        for(int i = 0; i < tryDarts.Length; i++)
        {
            tryDartsFirstPos[i] = tryDartsBase[i].transform.position;
            tryDartsFirstRot[i] = tryDartsBase[i].transform.rotation;
            tryDartsRb[i] = tryDarts[i].GetComponent<Rigidbody>();
        }
    }
    public void DartInitialize() //ダーツのリセット
    {
        for(int i = 0;i < tryDarts.Length; i++)
        {
            tryDartsRb[i].velocity = Vector3.zero;
            tryDartsRb[i].angularVelocity = Vector3.zero;
            tryDarts[i].transform.parent = null;
            tryDarts[i].transform.position = tryDartsFirstPos[i];
            tryDarts[i].transform.rotation = tryDartsFirstRot[i];
            //tryDartsRb[i].isKinematic = true;
            tryDartsRb[i].useGravity = true;
            tryDartsTip[i].SetActive(true);
        }
    }
    
}
