using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DartsPhysics : MonoBehaviour
{
    private Rigidbody dartRb;
    public Transform flightTransform; // �H�̈ʒu���Q�Ƃ��邾��

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
            // ���������̂� Tip �� Collider ���ǂ����`�F�b�N
            if (collision.GetContact(0).thisCollider.name == "Tip")
            {
                StickToBoard(collision);
            }
        }
    }

    void StickToBoard(Collision collision)
    {
        // �Փ˖ʂɐ�[��������
        transform.rotation = Quaternion.LookRotation(-collision.contacts[0].normal);

        // �������~�߂ă{�[�h�ɌŒ�
        dartRb.isKinematic = true;
        dartRb.useGravity = false;
        Quaternion stickRotation = transform.rotation; // ���[���h��]��ۑ�
        transform.SetParent(collision.transform);      // �e��ݒ�
        transform.rotation = stickRotation;            // ��]��߂�
    }
}
