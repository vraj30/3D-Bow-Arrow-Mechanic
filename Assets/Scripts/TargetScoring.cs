using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

[RequireComponent(typeof(MeshCollider))]
public class TargetScoring : MonoBehaviour
{
    public int[] scoreZones = { 10, 8, 6, 4, 2, 1 };
    public float[] ringThresholds = { 0.1f, 0.2f, 0.35f, 0.5f, 0.7f, 1.0f };
    public GameObject impactCamera;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Arrow"))
        {
            Vector3 hitPosition = collision.contacts[0].point;
            Vector3 localHit = transform.InverseTransformPoint(hitPosition);
            float distance = new Vector2(-localHit.x, localHit.z).magnitude;
            int score = GetScoreFromDistance(distance);
            Debug.Log($"Arrow hit! Score: {score}");
            StartCoroutine(ShowScoreText(hitPosition, score));
            GlobalScore.Instance.AddScore(score);
        }
    }
    private IEnumerator ShowScoreText(Vector3 position, int score)
    {
       
        while (impactCamera != null && !impactCamera.activeSelf)
        {
            yield return null; // Wait for the next frame
        }

        // Show text once the impact camera is active
        DynamicTextManager.CreateText(position + new Vector3(0, 0, -0.5f), score.ToString(), DynamicTextManager.defaultData);
    }
    public int GetScoreFromDistance(float distance)
    {
        for (int i = 0; i < ringThresholds.Length; i++)
        {
            if (distance <= ringThresholds[i])
                return scoreZones[i];
        }
        return 0;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.color = Color.green;
        for (int i = 0; i < ringThresholds.Length; i++)
        {
            Handles.color = Color.magenta;
            Handles.DrawWireDisc(transform.position, transform.forward, ringThresholds[i] * transform.lossyScale.x);
            Handles.Label(transform.position + Vector3.up * (ringThresholds[i] + 0.05f), $"{scoreZones[i]} pts");
        }
    }
#endif
}
