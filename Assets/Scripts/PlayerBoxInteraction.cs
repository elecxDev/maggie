using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBoxInteraction : MonoBehaviour
{
    public float detectionRadius = 5f;
    public float pullForce = 10f;
    public float pushForce = 10f;
    public Image screenTint;
    public float fadeDuration = 1.5f;

    public Transform rightHand; // Blue beam (Attract)
    public Transform leftHand;  // Red beam (Repel)

    private bool isRepelling = false;
    private LineRenderer lineRenderer;
    private Transform detectedBox;
    private Coroutine fadeCoroutine;

    private Transform floatingBox = null;
    private Rigidbody floatingBoxRb = null;
    private float orbitRadius = 2f;
    private float orbitSpeed = 1.5f;
    private float hoverHeight = 1.5f;

    void Start()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.enabled = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            isRepelling = false;
            SetBeamColor(Color.blue);
            StartScreenTint(Color.blue);
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            isRepelling = true;
            SetBeamColor(Color.red);
            StartScreenTint(Color.red);
            ReleaseFloatingBox();
        }

        DetectAndApplyForce();

        if (floatingBox != null)
        {
            MakeBoxFloatAround();
        }

        UpdateBeam();
    }

    void StartScreenTint(Color tintColor)
    {
        if (screenTint != null)
        {
            tintColor.a = 0.25f;
            screenTint.color = tintColor;

            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeOutTint());
        }
    }

    IEnumerator FadeOutTint()
    {
        Color initialColor = screenTint.color;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(initialColor.a, 0f, elapsedTime / fadeDuration);
            screenTint.color = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);
            yield return null;
        }

        screenTint.color = new Color(initialColor.r, initialColor.g, initialColor.b, 0f);
    }

    void DetectAndApplyForce()
    {
        detectedBox = null;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius);

        foreach (Collider hit in hitColliders)
        {
            if (hit.CompareTag("Box"))
            {
                Rigidbody boxRigidbody = hit.GetComponent<Rigidbody>();

                if (boxRigidbody != null)
                {
                    if (isRepelling)
                    {
                        boxRigidbody.isKinematic = false;
                        Vector3 pushDirection = (hit.transform.position - transform.position).normalized;

                        // Check if the box is in a path where it might collide with a wall during repelling
                        RaycastHit raycastHit;
                        if (Physics.Raycast(hit.transform.position, pushDirection, out raycastHit, pushForce))
                        {
                            if (raycastHit.collider.CompareTag("Wall"))
                            {
                                return; // Stop if there's a wall in the way
                            }
                        }

                        boxRigidbody.AddForce(pushDirection * pushForce, ForceMode.Impulse);
                    }
                    else
                    {
                        AttachFloatingBox(hit.transform, boxRigidbody);
                    }

                    detectedBox = hit.transform;
                    break;
                }
            }
        }

        // Disconnect box if out of range
        if (detectedBox != null && Vector3.Distance(transform.position, detectedBox.position) > detectionRadius)
        {
            ReleaseFloatingBox();
        }
    }

    void AttachFloatingBox(Transform box, Rigidbody rb)
    {
        floatingBox = box;
        floatingBoxRb = rb;

        floatingBoxRb.isKinematic = true; // Disable physics
        floatingBoxRb.detectCollisions = false; // Ignore collisions while floating

        // Ignore collisions with Player 
        Physics.IgnoreCollision(floatingBox.GetComponent<Collider>(), GetComponent<Collider>(), true);

        // Ensure we don't ignore wall collisions when floating
        Collider[] colliders = floatingBox.GetComponents<Collider>();
        foreach (Collider collider in colliders)
        {
            Physics.IgnoreCollision(collider, GameObject.FindGameObjectWithTag("Wall").GetComponent<Collider>(), false);
        }
    }

    void MakeBoxFloatAround()
    {
        if (floatingBox == null) return;

        float orbitTime = Time.time * orbitSpeed;

        // Create smooth orbit around the player with a hover effect
        Vector3 targetPosition = transform.position + new Vector3(
            Mathf.Cos(orbitTime) * orbitRadius,
            hoverHeight + Mathf.Sin(orbitTime * 2) * 0.5f,
            Mathf.Sin(orbitTime) * orbitRadius
        );

        // Raycast to check for walls
        RaycastHit hit;
        if (Physics.Raycast(floatingBox.position, (targetPosition - floatingBox.position).normalized, out hit, (targetPosition - floatingBox.position).magnitude))
        {
            if (hit.collider.CompareTag("Wall"))
            {
                return; // Prevents passing through walls
            }
        }

        // Move the box smoothly, but respect the orbit
        floatingBox.position = Vector3.Lerp(floatingBox.position, targetPosition, pullForce * Time.deltaTime);

        // Rotate smoothly around its own axis
        floatingBox.Rotate(Vector3.up, 50f * Time.deltaTime);
    }

    void ReleaseFloatingBox()
    {
        if (floatingBox != null)
        {
            floatingBoxRb.isKinematic = false;
            floatingBoxRb.detectCollisions = true;

            Vector3 pushDirection = (floatingBox.position - transform.position).normalized;
            floatingBoxRb.AddForce(pushDirection * pushForce * 0.5f, ForceMode.Impulse);

            floatingBox = null;
            floatingBoxRb = null;
        }
    }

    void UpdateBeam()
    {
        if (detectedBox != null || floatingBox != null)
        {
            lineRenderer.enabled = true;
            Transform handTransform = isRepelling ? leftHand : rightHand;
            lineRenderer.SetPosition(0, handTransform.position);
            lineRenderer.SetPosition(1, (floatingBox != null ? floatingBox.position : detectedBox.position));
        }
        else
        {
            lineRenderer.enabled = false;
        }
    }

    void SetBeamColor(Color color)
    {
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
