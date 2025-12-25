using UnityEngine;
using Unity.Cinemachine;

public class HeliFollower : MonoBehaviour
{
    [Header("Cinemachine Camera Settings")]
    [SerializeField] private CinemachineCamera virtualCamera;
    [SerializeField] private Transform helicopterTarget;
    
    [Header("Follow Settings")]
    [SerializeField] private Vector3 followOffset = new Vector3(0, 3, -8);
    [SerializeField] private float followDamping = 1f;
    
    [Header("Dynamic Camera Settings")]
    [SerializeField] private float speedInfluence = 0.5f;
    [SerializeField] private float maxSpeedOffset = 3f;
    
    private CinemachineFollow followComponent;
    private CinemachineRotationComposer rotationComposer;
    private Rigidbody heliRigidbody;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetupCinemachineCamera();
    }

    void SetupCinemachineCamera()
    {
        // If no virtual camera is assigned, try to get it from this GameObject
        if (virtualCamera == null)
        {
            virtualCamera = GetComponent<CinemachineCamera>();
        }
        
        // If still null, create one
        if (virtualCamera == null)
        {
            virtualCamera = gameObject.AddComponent<CinemachineCamera>();
        }
        
        // Set the helicopter as the tracking target
        if (helicopterTarget != null)
        {
            virtualCamera.Target.TrackingTarget = helicopterTarget;
            virtualCamera.Target.LookAtTarget = helicopterTarget;
            
            // Get rigidbody for speed-based camera adjustments
            heliRigidbody = helicopterTarget.GetComponent<Rigidbody>();
        }
        
        // Add and configure CinemachineFollow component
        followComponent = virtualCamera.GetComponent<CinemachineFollow>();
        if (followComponent == null)
        {
            followComponent = virtualCamera.gameObject.AddComponent<CinemachineFollow>();
        }
        
        followComponent.FollowOffset = followOffset;
        followComponent.TrackerSettings.PositionDamping = new Vector3(followDamping, followDamping, followDamping);
        
        // Add and configure CinemachineRotationComposer for aim
        rotationComposer = virtualCamera.GetComponent<CinemachineRotationComposer>();
        if (rotationComposer == null)
        {
            rotationComposer = virtualCamera.gameObject.AddComponent<CinemachineRotationComposer>();
        }
        
        rotationComposer.Composition.ScreenPosition = new Vector2(0.5f, 0.5f);
        
        Debug.Log("Cinemachine Helicopter Camera Setup Complete");
    }

    // Update is called once per frame
    void Update()
    {
        AdjustCameraForSpeed();
    }
    
    void AdjustCameraForSpeed()
    {
        // Dynamically adjust camera offset based on helicopter speed
        if (heliRigidbody != null && followComponent != null)
        {
            float speed = heliRigidbody.linearVelocity.magnitude;
            float speedOffset = Mathf.Clamp(speed * speedInfluence, 0, maxSpeedOffset);
            
            // Push camera back when moving fast
            Vector3 dynamicOffset = followOffset + new Vector3(0, 0, -speedOffset);
            followComponent.FollowOffset = dynamicOffset;
        }
    }
    
    // Optional: Call this to set a new helicopter target at runtime
    public void SetHelicopterTarget(Transform newTarget)
    {
        helicopterTarget = newTarget;
        if (virtualCamera != null)
        {
            virtualCamera.Target.TrackingTarget = helicopterTarget;
            virtualCamera.Target.LookAtTarget = helicopterTarget;
            heliRigidbody = helicopterTarget.GetComponent<Rigidbody>();
        }
    }
}
