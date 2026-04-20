using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DartsPhysics_02 : MonoBehaviour
{
    private Rigidbody dartRb;

    [Header("姿勢制御")]
    [SerializeField] private float minTrackingSpeed = 2f;  // 遅い時の追従速度
    [SerializeField] private float maxTrackingSpeed = 10f; // 速い時の追従速度
    [SerializeField] private float maxSpeedReference = 10f;// 最大追従になる速度(m/s)

    [Header("空気抵抗")]
    [Range(0f, 0.1f)]
    [SerializeField] private float dragCoefficient = 0.02f;

    [Header("音声")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip se_Dartsthrow;

    [Header("当たり判定")]
    [SerializeField] private GameObject Tip;
    [SerializeField] private float tipOffset = 0.05f;

    // 状態管理
    private bool isThrown = false;

    private void Start()
    {
        dartRb = GetComponent<Rigidbody>();
    }

    // 外部から投擲開始を通知するメソッド
    public void OnThrow()
    {
        isThrown = true;
        Debug.Log($"投げた！速度: {dartRb.velocity.magnitude:F2} m/s");
    }

    // 外部から着地を通知するメソッド
    public void OnStick()
    {
        isThrown = false;
    }

    private void FixedUpdate()
    {
        if (!isThrown) return;

        float speed = dartRb.velocity.magnitude;
        if (speed < 0.1f) return; // ほぼ止まってたら何もしない

        OrientToVelocity(speed);
        ApplyDrag(speed);
    }

    private void OrientToVelocity(float speed)
    {
        // 速度方向を向く
        Quaternion targetRot = Quaternion.LookRotation(dartRb.velocity.normalized);

        // 速度が速いほど強く追従
        float t = Mathf.Clamp01(speed / maxSpeedReference);
        float trackingSpeed = Mathf.Lerp(minTrackingSpeed, maxTrackingSpeed, t);

        dartRb.MoveRotation(
            Quaternion.Slerp(dartRb.rotation, targetRot, Time.fixedDeltaTime * trackingSpeed)
        );
    }

    private void ApplyDrag(float speed)
    {
        Vector3 vel = dartRb.velocity;
        Vector3 dragForce = -vel.normalized * vel.sqrMagnitude * dragCoefficient;
        dartRb.AddForce(dragForce);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("DartBoard")) return;

        ContactPoint contact = collision.GetContact(0);

        // Tipのコライダーで当たったか確認
        if (contact.thisCollider.gameObject != Tip) return;

        StickToBoard(collision, contact);
        audioSource.PlayOneShot(se_Dartsthrow);
    }

    void StickToBoard(Collision collision, ContactPoint contact)
    {
        // ① 物理を止める
        dartRb.isKinematic = true;
        dartRb.useGravity = false;
        dartRb.velocity = Vector3.zero;
        dartRb.angularVelocity = Vector3.zero;

        // ② 衝突面の法線を使ってボードに垂直に刺さる向きに補正
        // contact.normal = ボード表面の法線（ボードから外向き）
        // ダーツはその逆方向（ボードに向かって）を向くべき
        Quaternion stickRotation = Quaternion.LookRotation(-contact.normal);

        // ③ 親子関係の設定
        transform.SetParent(collision.transform, true);
        transform.rotation = stickRotation;

        // ④ 刺さった位置をTipの接触点に合わせる（めり込み防止）
        transform.position = contact.point - stickRotation * Vector3.forward * tipOffset;

        isThrown = false;
        GetComponent<DartsStateManager>().EnterStuck();
    }
}
