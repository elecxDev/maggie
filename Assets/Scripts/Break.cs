using UnityEngine;

public class Break : MonoBehaviour
{
    public Rigidbody rb;
    public float explosionForce = 25f;      // Explosion force
    public float explosionRadius = 2f;     // Explosion radius
    public float upwardModifier = 1.5f;    // Upward push for debris
    public float destroyDelayMin = 6f;     // Minimum time before destroying fragment
    public float destroyDelayMax = 10f;     // Maximum time before destroying fragment

    private bool isBroken = false;

    void Start()
    {
        rb.isKinematic = true; // Fragments should not move initially
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision detected with: " + collision.gameObject.name);

        if (collision.gameObject.CompareTag("Box") && !isBroken)
        {
            Debug.Log("Breaking wall piece...");
            BreakWall(); // Trigger breaking logic
        }
    }

    void BreakWall()
    {
        isBroken = true;

        rb.isKinematic = false; // Enable physics for fragment

        // Apply explosion force to scatter fragments
        Vector3 explosionPosition = transform.position;
        rb.AddExplosionForce(explosionForce, explosionPosition, explosionRadius, upwardModifier, ForceMode.Impulse);

        // Randomize destruction time for debris
        float destroyDelay = Random.Range(destroyDelayMin, destroyDelayMax);
        Invoke("DestroyPiece", destroyDelay);
    }

    void DestroyPiece()
    {
        Destroy(gameObject);
    }
}
