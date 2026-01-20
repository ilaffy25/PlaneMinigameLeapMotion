using UnityEngine;

/// <summary>
/// Handles simple arcade style airplane controls using a Rigidbody.
/// The aircraft continuously moves forward while the player steers using pitch, yaw and roll inputs.
/// Fuel is drained every frame by informing the <see cref="GameManager"/>.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Flight Settings")]
    [Tooltip("Base forward thrust applied every physics step.")]
    public float forwardThrust = 50f;

    [Tooltip("Maximum linear speed allowed for the aircraft.")]
    public float maxSpeed = 75f;

    [Tooltip("Torque strength applied when rolling the aircraft.")]
    public float rollTorque = 45f;

    [Tooltip("Torque strength applied when pitching the aircraft.")]
    public float pitchTorque = 35f;

    [Tooltip("Torque strength applied when yawing the aircraft.")]
    public float yawTorque = 20f;

    [Header("Fuel Consumption")]
    [Tooltip("Base fuel consumed per second while the plane is active.")]
    public float idleFuelDrain = 1.2f;

    [Tooltip("Additional fuel multiplier applied based on the player's steering input.")]
    public float maneuverFuelMultiplier = 1.4f;

    [Header("Audio")]
    [Tooltip("Optional looping audio source used for the engine.")]
    public AudioSource engineAudio;

    [Header("Input")]
    public FlightInputRouter inputRouter; // 👈 NUEVO

    private Rigidbody rb;
    private GameManager gameManager;
    public HandFlightInput handFlightInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        gameManager = FindObjectOfType<GameManager>();

        if (rb)
        {
            // Add a little angular drag to stabilise the plane.
            rb.angularDamping = 2f;
            rb.linearDamping = 0.2f;
        }
    }

    private void Update()
    {
        if (!gameManager || !gameManager.IsPlaying)
        {
            SetEngineAudio(false);
            return;
        }

        float maneuverIntensity =
            Mathf.Clamp01(
                Mathf.Abs(inputRouter.Pitch) +
                Mathf.Abs(inputRouter.Roll)
            );

        float drain =
            (idleFuelDrain + maneuverIntensity * maneuverFuelMultiplier)
            * Time.deltaTime;

        gameManager.ConsumeFuel(drain);
        SetEngineAudio(true);
    }

    private void FixedUpdate()
    {
        if (!gameManager || !gameManager.IsPlaying)
        {
            return;
        }

        ApplyForwardMotion();
        ApplySteering();
        ClampVelocity();
    }

    /// <summary>
    /// Applies constant forward force.
    /// </summary>
    private void ApplyForwardMotion()
    {
        rb.AddForce(transform.forward * forwardThrust, ForceMode.Acceleration);
    }

    /// <summary>
    /// Applies torque based on standard Unity input axes.
    /// </summary>
    private void ApplySteering()
    {
        Vector3 torque = new Vector3(
            inputRouter.Pitch * pitchTorque,
            inputRouter.Yaw * yawTorque,
            inputRouter.Roll * rollTorque
        );

        rb.AddRelativeTorque(torque, ForceMode.Acceleration);
    }

    /// <summary>
    /// Prevents the plane from accelerating indefinitely.
    /// </summary>
    private void ClampVelocity()
    {
        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
    }

    /// <summary>
    /// Called by fuel checkpoints when collected.
    /// </summary>
    /// <param name="amount">Fuel amount received.</param>
    public void ReceiveFuel(float amount)
    {
        if (!gameManager)
        {
            return;
        }

        gameManager.AddFuel(amount);
    }

    /// <summary>
    /// Called by hazards to apply penalties.
    /// </summary>
    /// <param name="fuelPenalty">Fuel removed immediately.</param>
    public void ApplyFuelPenalty(float fuelPenalty)
    {
        if (!gameManager)
        {
            return;
        }

        gameManager.ConsumeFuel(fuelPenalty);
    }

    /// <summary>
    /// Optional helper to toggle engine audio.
    /// </summary>
    private void SetEngineAudio(bool active)
    {
        if (!engineAudio)
        {
            return;
        }

        if (active && !engineAudio.isPlaying)
        {
            engineAudio.Play();
        }
        else if (!active && engineAudio.isPlaying)
        {
            engineAudio.Pause();
        }

        // Tie pitch to the forward speed for feedback.
        if (active)
        {
            engineAudio.pitch = 0.9f + rb.linearVelocity.magnitude / maxSpeed;
        }
    }

    private void OnDrawGizmos()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (!rb) return;

        Gizmos.color = Color.red;
        Vector3 worldCOM = rb.worldCenterOfMass;
        Gizmos.DrawSphere(worldCOM, 0.15f);
    }
}
