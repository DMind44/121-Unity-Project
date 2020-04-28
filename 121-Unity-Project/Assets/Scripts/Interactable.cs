using UnityEngine;
using System;
using Mirror;

// Adapted from Unity docs:
// https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnMouseOver.html

[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(NetworkTransform))]
public class Interactable : NetworkBehaviour {
    private Color originalColor;
    private Color[] originalColors;
    [SerializeField] private Color hoverColor = Color.green;
    [SerializeField] private Color liftedColor = Color.blue;

    private Material originalMat;
    [SerializeField] private Material hoverMat;
    [SerializeField] private Material liftedMat;

    public bool lifting { get; internal set; }  // true while being lifted
    public bool lifted { get; internal set; }
    public bool flying { get; internal set; }  // true after thrown, false after collision
    [SerializeField] private float throwSpeed = 0;

    private MeshRenderer meshRenderer;
    private MeshRenderer[] rends;
    [SerializeField] private Rigidbody rb;

    [SerializeField] private Vector3 relativePos = Vector3.zero;
    public Transform playerT { get; internal set; }

    [SerializeField] private float cutoff_momentum = 10;

    private float dmgAmount = 0;

    // Start is called before the first frame update
    void Start() {
        meshRenderer = GetComponent<MeshRenderer>();
        rends = GetComponentsInChildren<MeshRenderer>();
        rb = GetComponent<Rigidbody>();

        // initializes the array of originalColors of the child objects
        originalColors = new Color[rends.Length];
        for (int i = 0; i < rends.Length; i++) {
            if ((rends[i] != null) && (rends[i].material != null)) {
                originalColors[i] = rends[i].material.color;
            }
        }
    }

    // Called every time another object is hit
    [ServerCallback] private void OnCollisionEnter(Collision other) {
        // Deal damage if a flying interactable collides with a Player
        if (flying && other.gameObject.CompareTag("Player")) {
            RecalculateDamage();
            RpcHitSomething();
            RpcDamageSomething(other.gameObject, dmgAmount);
        }
    }

    [ClientRpc] private void RpcDamageSomething(GameObject target, float amount) {
        target.GetComponent<PlayerController>().DamageMe(amount);
    }

    // Returns the amount of damage caused by this object
    [Server] public void RecalculateDamage() {
        float momentum = rb.velocity.magnitude * rb.mass;
        if (momentum < cutoff_momentum) {
            dmgAmount = 0f;
        }
        dmgAmount = (float)Math.Sqrt(momentum);
    }

    [ClientRpc] private void RpcHitSomething() {
        flying = false;
    }

    // Called when a mouse is hovering and is close enough
    // Only on client so that only client sees hovering
    [Client] public void BeginHover() {
        if (!lifted && !lifting) {
            for (int i = 0; i < rends.Length; i++) {
                if (rends[i] != null) {
                    rends[i].material.color = hoverColor;
                }
            }
        }
    }

    // Server-side request to grab this object
    [Server] public void Grab(Transform playerTransform) {
        if (!lifted && !lifting) {
            RpcGrab(playerTransform);
        }
    }

    // Update all clients on new owner of this Interactable
    [ClientRpc] private void RpcGrab(Transform playerTransform) {
        lifting = true;
        playerT = playerTransform;

        // iterate through all the children meshRenderers
        for (int i = 0; i < rends.Length; i++) {
            if (rends[i] != null) {
                rends[i].material.color = liftedColor;
            }
        }
        GetComponent<Rigidbody>().useGravity = false;
    }

    [Server] public void Throw() {
        RpcThrow();
    }

    // Throws the lifted object.
    // This function first does things that always happen, but only physically
    // throws the object if it was fully lifted, not if still lifting
    [ClientRpc] private void RpcThrow() {
        // Always reset colors and turn on gravity
        for (int i = 0; i < rends.Length; i++) {
            if (originalColors[i] != null) {
                rends[i].material.color = originalColors[i];
            }
        }
        GetComponent<Rigidbody>().useGravity = true;
        lifting = false;

        // If lifted, initiate throwing and flying sequence
        if (lifted) {
            flying = true;
            lifted = false;

            // Determine velocity based on what you're looking at
            Ray ray;
            RaycastHit hit;
            float throwAngle;
            Camera cam = playerT.gameObject.GetComponent<PlayerController>().GetCamera();
            ray = cam.ScreenPointToRay(Input.mousePosition);

            // Before raycasting, disable the collider for this object and the player.
            // That way, they will be ignored by the raycast.
            GetComponent<Collider>().enabled = false;
            playerT.gameObject.GetComponent<Collider>().enabled = false;

            // Fetch the strength multiplier
            float mult = playerT.gameObject.GetComponent<PlayerProperties>().strengthMult;

            if (Physics.Raycast(ray, out hit) &&
                    CalculateThrowAngle(transform.position, hit.point,
                    throwSpeed, out throwAngle)) {
                Debug.Log(hit.transform.name);
                Vector3 throwDirection = hit.point - transform.position;
                throwDirection.y = 0;
                throwDirection = Vector3.RotateTowards(throwDirection, Vector3.up, throwAngle, throwAngle).normalized;
                Debug.DrawRay(transform.position, throwDirection, Color.green, 5f);
                rb.velocity = throwDirection * throwSpeed * mult;
            } else {
                rb.velocity = cam.transform.forward * throwSpeed;
            }

            GetComponent<Collider>().enabled = true;
            playerT.gameObject.GetComponent<Collider>().enabled = true;
        }
    }

    // Calculates the necessary throw angle to hit a target
    //  Source: https://answers.unity.com/questions/49195/trajectory-of-a-projectile-formula-does-anyone-kno.html?_ga=2.240046149.757207726.1586110776-1944583397.1580664386
    bool CalculateThrowAngle(Vector3 from, Vector3 to, float speed, out float angle) {
        float xx = to.x - from.x;
        float xz = to.z - from.z;
        float x = Mathf.Sqrt(xx * xx + xz * xz);
        float y = from.y - to.y;

        float v = speed;
        float g = Physics.gravity.y;

        float sqrt = (v*v*v*v) - (g * (g * (x*x) + 2 * y * (v*v)));

        // Not enough range
        if (sqrt < 0) {
            angle = 0.0f;
            Debug.Log("Out of range");
            return false;
        }

        angle = -Mathf.Atan(((v*v) - Mathf.Sqrt(sqrt)) / (g*x));
        Debug.Log(angle);
        return true;
    }

    // On FixedUpdate, follow player if object is all the way lifted.
    // Rise slowly if object is still lifting.
    void FixedUpdate() {
        if (lifted) {
            rb.MovePosition(playerT.position + relativePos);
            rb.MoveRotation(playerT.rotation);
        } else if (lifting) {
            float targetMagnitude = 1 / rb.mass;
            Vector3 finalPos = playerT.position + relativePos;
            Vector3 targetPos = (finalPos - transform.position).normalized;
            targetPos *= targetMagnitude;
            targetPos += transform.position;

            // If the object is within two steps of the final position, the
            // lift is considered complete.
            if ((targetPos - finalPos).magnitude <= 2 * targetMagnitude) {
                lifting = false;
                lifted  = true;
                Debug.Log("Lift complete");
            } else {
                rb.MovePosition(targetPos);
                rb.MoveRotation(playerT.rotation);
            }
        }
    }

    // Return to original color when mouse leaves
    void OnMouseExit() {
        if (!lifted && !lifting) {
            for (int i = 0; i < rends.Length; i++) {
                if (originalColors[i] != null) {
                    rends[i].material.color = originalColors[i];
                }
            }
        }
    }
}
