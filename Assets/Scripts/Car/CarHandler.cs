using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarHandler : MonoBehaviour
{
    [SerializeField]
    Rigidbody rb;

    [SerializeField]
    Transform gameModel;

    [SerializeField]
    MeshRenderer carMeshRenderer;

    [SerializeField]
    ExplodeHandler explodeHandler;



    // [SerializeField]
    // [Tooltip("Subtle rotation angle when steering sideways")]
    // float maxTiltAngle = 10f;

    // Cache the model's original local rotation so we can tilt relative to it
    Quaternion modelBaseRotation;

    Quaternion rbBaseRotation;

    float maxSteerVelocity = 2;
    float maxForwardVelocity = 30;

    float accelerationMultiplier = 3;
    float brakeMultiplier = 15;
    float steeringMultiplier = 10;

    // [SerializeField, Range(0f, 0.5f)]
    // float inputDeadZone = 0.1f;

    Vector2 input = Vector2.zero;

    int _EmissionColor = Shader.PropertyToID("_EmissionColor");
    Color emmisiveColor = Color.white;
    float emmisiveColorMultiplier = 0f;

    bool isExploded = false;
    bool isPlayer = true;
    void Start()
    {
        // rbBaseRotation = rb.rotation;
        isPlayer = CompareTag("Player");
    }

    void Update()
    {
        // Reset game on R key press after exploding
        if (isExploded && isPlayer && Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
        {
            Time.timeScale = 1.0f; // Reset time scale
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
            return;
        }

        if(isExploded) return;
        // Keep visual rotation stable; steering is handled via lateral motion.
        gameModel.transform.rotation = Quaternion.Euler(0,rb.linearVelocity.x*5,0);
        if(carMeshRenderer != null)
        {
            float desiredCarEmmisiveColorMultiplier = 0f;
            if(input.y < 0) desiredCarEmmisiveColorMultiplier = 4.0f;
            emmisiveColorMultiplier = Mathf.Lerp(emmisiveColorMultiplier,desiredCarEmmisiveColorMultiplier,Time.deltaTime*4);
            carMeshRenderer.material.SetColor(_EmissionColor,emmisiveColor*emmisiveColorMultiplier);

        }
    }

    void FixedUpdate()
    {
        if(isExploded){
            rb.linearDamping = rb.linearVelocity.z * 0.1f;
            rb.linearDamping = Mathf.Clamp(rb.linearDamping,1.5f,10);
            rb.MovePosition(Vector3.Lerp(transform.position,new Vector3(0,0,transform.position.z),Time.deltaTime*0.5f));
            return;
        }
        // Always constrain movement to Z-axis (forward) and X-axis (sideways only)
        // Lock Y velocity to prevent flying
        // rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        
        // Keep rotation locked (no yaw feedback from sideways velocity)
        // rb.angularVelocity = Vector3.zero;
        // rb.rotation = rbBaseRotation;
        
        if (input.y > 0)
        {
            Accelarate();
        }
        else
        {
            // Don't add extra drag just because we're steering.
            rb.linearDamping = Mathf.Abs(input.x) > 0 ? 0f : 0.2f;
        }

        if(input.y < 0) Brake();
        Steer();
        if(rb.linearVelocity.z <= 0){
            rb.linearVelocity = Vector3.zero;
        }
    }

    void Accelarate()
    {
        rb.linearDamping = 0;
        if(rb.linearVelocity.z >= maxForwardVelocity)
            return;
        // Always push along world Z so the rickshaw moves frontward
        rb.AddForce(rb.transform.forward * accelerationMultiplier * input.y);
    }

    void Brake()
    {
        if(rb.linearVelocity.z <= 0)
        {
            return;
        }
         rb.AddForce(Vector3.forward * brakeMultiplier * input.y);
    }
    void Steer()
    {
        float speedBaseSteerLimit = Mathf.Clamp01(rb.linearVelocity.z / 5.0f);

        float currentX = rb.linearVelocity.x;

        if (Mathf.Abs(input.x) > 0)
        {
            float targetX = input.x * maxSteerVelocity * speedBaseSteerLimit;

            // If the player changes direction, don't allow an initial "wrong way" slide.
            if (Mathf.Abs(currentX) > 0.01f && Mathf.Sign(currentX) != Mathf.Sign(targetX))
            {
                currentX = 0f;
            }

            float newX = Mathf.MoveTowards(currentX, targetX, steeringMultiplier * Time.fixedDeltaTime);
            rb.linearVelocity = new Vector3(newX, 0, rb.linearVelocity.z);
        }
        else
        {
            float newX = Mathf.MoveTowards(currentX, 0f, 3f * Time.fixedDeltaTime);
            rb.linearVelocity = new Vector3(newX, 0, rb.linearVelocity.z);
        }

    }
    public void SetInput(Vector2 inputVector)
    {
        // Don't normalize - we want to keep the input magnitude.
        // Apply a deadzone so steering doesn't accidentally register as braking.
        // float x = Mathf.Abs(inputVector.x) < inputDeadZone ? 0f : inputVector.x;
        // float y = Mathf.Abs(inputVector.y) < inputDeadZone ? 0f : inputVector.y;
        // input = new Vector2(Mathf.Clamp(x, -1f, 1f), Mathf.Clamp(y, -1f, 1f));
        inputVector.Normalize();
        input = inputVector;
    }

    public void SetMaxSpeed(float newMaxSpeed){
        maxForwardVelocity = newMaxSpeed;
    }
    IEnumerator SlowDownTimeCO(){
        while (Time.timeScale > 0.2f){
            Time.timeScale -= Time.deltaTime* 2;
            yield return null; 
        }
        yield return new WaitForSeconds(0.5f);

        while(Time.timeScale <= 1.0f){
            Time.timeScale += Time.deltaTime;
            yield return null;
        }
        Time.timeScale = 1.0f;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (isExploded) return;
        if (explodeHandler == null) return;

        // Try to get tag from both the colliding object and its root
        GameObject other = collision.gameObject;
        string otherTag = other.tag;
        if (otherTag == "Untagged" && other.transform.root != null)
        {
            otherTag = other.transform.root.tag;
        }

        // Ignore collisions with ground/static objects
        if (otherTag == "Untagged" || otherTag == "Ground") return;

        // Explode on: Obstacle, or Player-AI collision only (no AI-AI collision)
        bool shouldExplode = false;

        if (otherTag == "Obstacle")
        {
            shouldExplode = true;
        }
        else if (isPlayer && otherTag == "CarAI")
        {
            shouldExplode = true; // Player hits AI car
        }
        else if (!isPlayer && otherTag == "Player")
        {
            shouldExplode = true; // AI car hits Player only (not other AI cars)
        }

        if (shouldExplode)
        {
            Vector3 velocity = rb.linearVelocity;
            explodeHandler.Explode(velocity * 45);
            isExploded = true;
            if (isPlayer)
            {
                StartCoroutine(SlowDownTimeCO());
            }
        }
    }
}
