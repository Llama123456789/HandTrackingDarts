using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DartsPhysics : MonoBehaviour
{
    private Rigidbody dartRb;
    //public Transform flightTransform; // 羽の位置を参照するだけ

    [Range(0f, 0.1f)]
    public float dragCoefficient = 0.02f;
    [SerializeField]
    private GameObject Tip;
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip se_Dartsthrow;

    [SerializeField] private HandGrabInteractable handGrabInteractable; // Inspectorで設定

    private bool isThrown = false;
    private void Start()
    {
        dartRb = GetComponent<Rigidbody>();

        if (handGrabInteractable != null)
        {
            handGrabInteractable.WhenSelectingInteractorViewRemoved +=
                OnReleased;
        }
    }

    private void OnDestroy()
    {
        if (handGrabInteractable != null)
        {
            handGrabInteractable.WhenSelectingInteractorViewRemoved -=
                OnReleased;
        }
    }

    private void OnReleased(IInteractorView interactor)
    {
        dartRb.isKinematic = false;
        isThrown = true;
        Debug.Log($"[Darts] Released! velocity={dartRb.velocity.magnitude:F2}");
    }

    private void FixedUpdate()
    {
        // ★ 投擲後かつ速度がある時だけ姿勢制御
        if (!isThrown) return;
        if (dartRb.isKinematic) return;

        if (dartRb.velocity.magnitude > 0.1f)
        {
            // 速度方向にダーツを向ける
            Quaternion targetRot = Quaternion.LookRotation(dartRb.velocity.normalized);
            dartRb.MoveRotation(Quaternion.Slerp(dartRb.rotation, targetRot, Time.deltaTime * 5f));

            Vector3 vel = dartRb.velocity;
            Vector3 dragForce = -vel.normalized * vel.sqrMagnitude * dragCoefficient;
            dartRb.AddForce(dragForce);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Hand")
        {
            dartRb.isKinematic = false;
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
                audioSource.PlayOneShot(se_Dartsthrow);
            }
        }
    }

    


    void StickToBoard(Collision collision)
    {
        isThrown = false;
        dartRb.isKinematic = true;
        dartRb.useGravity = false;
        Quaternion stickRotation = transform.rotation; // ワールド回転を保存
        transform.SetParent(collision.transform,true);      // 親を設定
        transform.rotation = stickRotation;            // 回転を戻す
    }
}
