using UnityEngine;

/// <summary>
/// Simple hazard representing turbulent clouds or balloons that sap fuel on contact.
/// </summary>
[RequireComponent(typeof(Collider))]
public class TurbulenceHazard : MonoBehaviour
{
    [Tooltip("Fuel penalty applied when the player hits the hazard.")]
    public float fuelPenalty = 8f;

    [Tooltip("Impulse applied to the plane to push it away from the hazard.")]
    public float knockbackForce = 15f;

    [Tooltip("Optional particle effect spawned when hit.")]
    public GameObject hitEffect;

    private void Awake()
    {
        Collider c = GetComponent<Collider>();
        c.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerController controller = other.GetComponentInParent<PlayerController>();
        if (!controller)
        {
            return;
        }

        controller.ApplyFuelPenalty(fuelPenalty);

        Rigidbody rb = controller.GetComponent<Rigidbody>();
        if (rb)
        {
            Vector3 pushDirection = (controller.transform.position - transform.position).normalized;
            rb.AddForce(pushDirection * knockbackForce, ForceMode.Impulse);
        }

        if (hitEffect)
        {
            Instantiate(hitEffect, other.transform.position, Quaternion.identity);
        }
    }
}
