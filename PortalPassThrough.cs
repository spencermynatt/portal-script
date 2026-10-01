using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PortalPassThrough : MonoBehaviour
{
    private Rigidbody rb;
    private float lastTeleportTime;
    private const float teleportCooldown = 0.5f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // CRITICAL: This prevents the grenade from tunneling through the floor
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Time.time < lastTeleportTime + teleportCooldown) return;

        Portal entryPortal = other.GetComponent<Portal>();
        if (entryPortal != null)
        {
            Portal exitPortal = FindDestination(entryPortal);
            if (exitPortal != null)
            {
                Teleport(entryPortal.transform, exitPortal.transform);
            }
        }
    }

    private Portal FindDestination(Portal entryPortal)
    {
        Portal[] portals = Object.FindObjectsByType<Portal>(FindObjectsSortMode.None);
        foreach (var p in portals)
        {
            if (p.isBluePortal != entryPortal.isBluePortal) return p;
        }
        return null;
    }

    private void Teleport(Transform entry, Transform exit)
    {
        lastTeleportTime = Time.time;

        // 1. Preserve velocity before the jump
        Vector3 currentVelocity = rb.linearVelocity;
        Vector3 currentAngularVelocity = rb.angularVelocity;

        // 2. Position the grenade at the exit with a safe "floor clearance" offset
        Vector3 relativePos = entry.InverseTransformPoint(transform.position);
        Quaternion relativeRot = Quaternion.Inverse(entry.rotation) * transform.rotation;

        transform.position = exit.TransformPoint(relativePos) + (exit.forward * 0.8f);
        transform.rotation = exit.rotation * relativeRot;

        // 3. Re-apply velocity relative to the exit portal's facing direction
        rb.linearVelocity = exit.TransformDirection(entry.InverseTransformDirection(currentVelocity));
        rb.angularVelocity = exit.TransformDirection(entry.InverseTransformDirection(currentAngularVelocity));

        // 4. Force a physics update to prevent floor-clipping
        rb.WakeUp();

        Debug.Log("Grenade exited portal. Explosive timer remains active.");
    }
}