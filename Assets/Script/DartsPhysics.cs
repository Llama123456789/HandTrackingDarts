using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DartsPhysics : MonoBehaviour
{
    private Rigidbody dartRb;
    public Transform flightTransform; // 羽の位置を参照するだけ

    [Range(0f, 0.1f)]
    public float dragCoefficient = 0.02f;
    private void Start()
    {
        dartRb = GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        if (dartRb.velocity.magnitude > 0.1f)
        {
            Quaternion targetRot = Quaternion.LookRotation(dartRb.velocity.normalized);
            dartRb.MoveRotation(Quaternion.Slerp(dartRb.rotation, targetRot, Time.deltaTime * 5f));

            Vector3 vel = dartRb.velocity;
            Vector3 dragForce = -vel.normalized * vel.sqrMagnitude * dragCoefficient;
            dartRb.AddForceAtPosition(dragForce, flightTransform.position);
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("DartBoard"))
        {
            // 当たったのが Tip の Collider かどうかチェック
            if (collision.GetContact(0).thisCollider.name == "Tip")
            {
                StickToBoard(collision);
            }
        }
    }

    void StickToBoard(Collision collision)
    {
        // 衝突面に先端を向ける
        transform.rotation = Quaternion.LookRotation(-collision.contacts[0].normal);

        // 物理を止めてボードに固定
        dartRb.isKinematic = true;
        dartRb.useGravity = false;
        Quaternion stickRotation = transform.rotation; // ワールド回転を保存
        transform.SetParent(collision.transform);      // 親を設定
        transform.rotation = stickRotation;            // 回転を戻す
    }
}
