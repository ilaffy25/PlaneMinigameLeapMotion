using UnityEngine;

/// <summary>
/// Represents an airborne checkpoint that awards fuel when the plane flies through it.
/// </summary>
[RequireComponent(typeof(Collider))]
public class FuelCheckpoint : MonoBehaviour
{
    [Tooltip("Fuel amount granted to the player when collected.")]
    public float fuelReward = 10f;

    [Tooltip("Optional visual effect spawned on collection.")]
    public GameObject collectEffect;

    [Tooltip("Time before the checkpoint respawns. Leave zero for single use.")]
    public float respawnDelay = 0f;

    private Collider checkpointCollider;
    private Renderer[] renderers;
    private float respawnTimer;

    private void Awake()
    {
        checkpointCollider = GetComponent<Collider>();
        checkpointCollider.isTrigger = true;
        renderers = GetComponentsInChildren<Renderer>();
    }

    private void Update()
    {
        if (respawnDelay <= 0f || checkpointCollider.enabled)
        {
            return;
        }

        respawnTimer -= Time.deltaTime;
        if (respawnTimer <= 0f)
        {
            ToggleCheckpoint(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerController controller = other.GetComponentInParent<PlayerController>();
        if (!controller || !checkpointCollider.enabled)
        {
            return;
        }

        controller.ReceiveFuel(fuelReward);
        controller.gameObject.GetComponent<Rigidbody>()?.AddForce(Vector3.up * 15f, ForceMode.Impulse);

        if (collectEffect)
        {
            Instantiate(collectEffect, transform.localPosition, Quaternion.identity, transform);
        }

        ToggleCheckpoint(false);
    }

    private void ToggleCheckpoint(bool state)
    {
        checkpointCollider.enabled = state;
        foreach (Renderer r in renderers)
        {
            r.enabled = state;
        }

        if (!state && respawnDelay > 0f)
        {
            respawnTimer = respawnDelay;
        }
    }
}
