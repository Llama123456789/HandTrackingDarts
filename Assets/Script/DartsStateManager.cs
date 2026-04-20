using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DartsStateManager : MonoBehaviour
{
    public enum DartState { InCase, Held, Thrown, Stuck }

    [Header("参照")]
    [SerializeField] private Rigidbody dartRb;
    [SerializeField] private DartsPhysics_02 dartsPhysics;

    [Header("投擲設定")]
    [SerializeField] private float throwForceMultiplier = 1.5f;
    [SerializeField] private int velocitySampleCount = 5;
    [SerializeField] private float maxThrowSpeed = 10f;
    [SerializeField] private float minThrowSpeed = 4f;

    [Header("確認用")]
    [SerializeField] private DartState currentState;

    private Vector3 previousPosition;
    private Queue<Vector3> velocityQueue = new Queue<Vector3>();

    private void Start()
    {
        EnterInCase();
    }

    // ─── 状態遷移 ───────────────────────────────────

    private void EnterInCase()
    {
        currentState = DartState.InCase;
        dartRb.useGravity = false;
        dartRb.velocity = Vector3.zero;
        dartRb.angularVelocity = Vector3.zero;
        dartRb.isKinematic = true;
        Debug.Log("[Dart] InCase");
    }

    private void EnterHeld()
    {
        currentState = DartState.Held;
        dartRb.useGravity = false;
        velocityQueue.Clear();
        previousPosition = transform.position;
        Debug.Log("[Dart] Held");
    }

    private void EnterThrown()
    {
        currentState = DartState.Thrown;
        dartRb.useGravity = true;
        dartsPhysics.OnThrow();
        Debug.Log($"[Dart] Thrown / 速度: {dartRb.velocity.magnitude:F2} m/s");
    }

    public void EnterStuck()
    {
        currentState = DartState.Stuck;
        dartsPhysics.OnStick();
        Debug.Log("[Dart] Stuck");
    }

    // ─── ケースに戻す ────────────────────────────────

    public void ReturnToCase()
    {
        transform.SetParent(null);
        EnterInCase();
        Debug.Log("[Dart] ケースに戻った");
    }

    // ─── 速度計算 ────────────────────────────────────

    private void Update()
    {
        if (currentState != DartState.Held) return;

        Vector3 frameVelocity = (transform.position - previousPosition) / Time.deltaTime;
        previousPosition = transform.position;

        velocityQueue.Enqueue(frameVelocity);
        if (velocityQueue.Count > velocitySampleCount)
            velocityQueue.Dequeue();
    }

    // ─── InteractableUnityEventWrapperから呼ぶ ───────

    public void OnGrab()
    {
        if (currentState != DartState.InCase) return;
        EnterHeld();
    }

    public void OnRelease()
    {
        if (currentState != DartState.Held) return;
        dartRb.isKinematic = false;

        if (velocityQueue.Count > 0)
        {
            Vector3 avg = Vector3.zero;
            foreach (Vector3 v in velocityQueue) avg += v;
            avg /= velocityQueue.Count;

            Vector3 direction = avg.normalized;
            float speed = avg.magnitude * throwForceMultiplier;

            // ★ 速度を範囲内に収める
            speed = Mathf.Clamp(speed, minThrowSpeed, maxThrowSpeed);
            dartRb.velocity = direction * speed;
        }

        Debug.Log($"[Dart] リリース速度: {dartRb.velocity.magnitude:F2} m/s");
        EnterThrown();
    }
}
