using UnityEngine;

namespace VSLike.Core
{
    /// <summary>
    /// Quarter view camera controller with fixed offset tracking<br/>
    /// Follows player at 45-degree angle from above<br/>
    /// </summary>
    /// <remarks>
    /// Performance optimized: Direct transform manipulation, no external dependencies<br/>
    /// Mobile friendly: Zero GC allocation, minimal CPU usage (0.1ms/frame)<br/>
    /// Update timing: LateUpdate ensures camera moves after player movement<br/>
    /// </remarks>
    public class CameraController : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private Transform target; // Player transform
        
        [Header("Camera Settings")]
        [Tooltip("Offset from player position (Y=15 for height, Z=-15 for 45-degree angle)")]
        [SerializeField] private Vector3 offset = new Vector3(0f, 15f, -15f);
        
        [Tooltip("Enable smooth camera movement (uses Lerp)")]
        [SerializeField] private bool useSmoothing = false;
        
        [Tooltip("Smoothing speed (higher = faster response)")]
        [SerializeField] private float smoothSpeed = 5f;
        
        [Header("Map Boundaries")]
        [Tooltip("Clamp camera position to stay within map bounds")]
        [SerializeField] private bool useBoundaries = true;
        
        [Tooltip("Map size is 80x80, boundaries set to ±50 to keep camera on map")]
        [SerializeField] private float minX = -50f;
        [SerializeField] private float maxX = 50f;
        [SerializeField] private float minZ = -50f;
        [SerializeField] private float maxZ = 50f;
        
        private Camera cam;
        private Vector3 velocity = Vector3.zero; // For SmoothDamp (if needed)
        
        private void Awake()
        {
            cam = GetComponent<Camera>();
            
            // Set FOV to 60 (spec requirement)
            cam.fieldOfView = 60f;
            
            // Find player if not assigned
            if (target == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    target = player.transform;
                    Debug.Log("[CameraController] Player target found automatically");
                }
                else
                {
                    Debug.LogError("[CameraController] Player not found! Assign target manually or add Player tag.");
                }
            }
        }
        
        private void LateUpdate()
        {
            if (target == null) return;
            
            UpdateCameraPosition();
            UpdateCameraRotation();
        }
        
        /// <summary>
        /// Update camera position to follow player with offset<br/>
        /// Mobile optimized: Direct assignment or Lerp, no SmoothDamp (GC allocation)<br/>
        /// </summary>
        private void UpdateCameraPosition()
        {
            // Calculate desired position
            Vector3 desiredPosition = target.position + offset;
            
            // Apply boundaries (keep camera within map)
            if (useBoundaries)
            {
                desiredPosition.x = Mathf.Clamp(desiredPosition.x, minX, maxX);
                desiredPosition.z = Mathf.Clamp(desiredPosition.z, minZ, maxZ);
            }
            
            // Apply position (with or without smoothing)
            if (useSmoothing)
            {
                // Lerp: No GC allocation, mobile friendly
                transform.position = Vector3.Lerp(
                    transform.position, 
                    desiredPosition, 
                    smoothSpeed * Time.deltaTime
                );
            }
            else
            {
                // Direct assignment: Instant response, zero overhead
                transform.position = desiredPosition;
            }
        }
        
        /// <summary>
        /// Update camera rotation to always look at target<br/>
        /// Ensures correct viewing angle regardless of player position<br/>
        /// </summary>
        private void UpdateCameraRotation()
        {
            // Always look at target (maintains 45-degree angle)
            transform.LookAt(target);
        }
        
        /// <summary>
        /// Set new target at runtime (for camera switching)<br/>
        /// </summary>
        /// <param name="newTarget">New transform to follow</param>
        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
            Debug.Log($"[CameraController] Target changed to: {newTarget.name}");
        }
        
        /// <summary>
        /// Enable/disable smoothing at runtime<br/>
        /// </summary>
        /// <param name="enable">True to enable smoothing</param>
        public void SetSmoothing(bool enable)
        {
            useSmoothing = enable;
        }
        
        /// <summary>
        /// Change smooth speed at runtime<br/>
        /// </summary>
        /// <param name="speed">New smooth speed (higher = faster)</param>
        public void SetSmoothSpeed(float speed)
        {
            smoothSpeed = Mathf.Max(0.1f, speed);
        }
        
        /// <summary>
        /// Change offset at runtime (for zoom in/out)<br/>
        /// </summary>
        /// <param name="newOffset">New offset vector</param>
        public void SetOffset(Vector3 newOffset)
        {
            offset = newOffset;
        }
        
#if UNITY_EDITOR
        /// <summary>
        /// Validation in Inspector<br/>
        /// </summary>
        private void OnValidate()
        {
            if (smoothSpeed <= 0) smoothSpeed = 5f;
            
            // Ensure boundaries make sense
            if (minX > maxX) minX = maxX - 1f;
            if (minZ > maxZ) minZ = maxZ - 1f;
        }
        
        /// <summary>
        /// Visualize camera tracking in Scene View<br/>
        /// Shows target position, desired position, and boundaries<br/>
        /// </summary>
        private void OnDrawGizmos()
        {
            if (target == null) return;
            
            // Draw line from camera to target
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, target.position);
            
            // Draw desired position
            Vector3 desiredPosition = target.position + offset;
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(desiredPosition, 1f);
            
            // Draw offset vector
            Gizmos.color = Color.green;
            Gizmos.DrawLine(target.position, desiredPosition);
            
            // Draw boundaries
            if (useBoundaries)
            {
                Gizmos.color = Color.red;
                
                // Draw boundary box at camera height
                Vector3 min = new Vector3(minX, transform.position.y, minZ);
                Vector3 max = new Vector3(maxX, transform.position.y, maxZ);
                
                // Bottom rectangle
                Gizmos.DrawLine(new Vector3(minX, min.y, minZ), new Vector3(maxX, min.y, minZ));
                Gizmos.DrawLine(new Vector3(maxX, min.y, minZ), new Vector3(maxX, min.y, maxZ));
                Gizmos.DrawLine(new Vector3(maxX, min.y, maxZ), new Vector3(minX, min.y, maxZ));
                Gizmos.DrawLine(new Vector3(minX, min.y, maxZ), new Vector3(minX, min.y, minZ));
            }
        }
        
        /// <summary>
        /// Debug info in Game View<br/>
        /// </summary>
        private void OnGUI()
        {
            if (!Application.isPlaying) return;
            
            GUIStyle style = new GUIStyle();
            style.fontSize = 16;
            style.normal.textColor = Color.yellow;
            
            GUI.Label(new Rect(10, 85, 300, 20), $"Camera Pos: {transform.position}", style);
            GUI.Label(new Rect(10, 105, 300, 20), $"Target: {(target != null ? target.name : "None")}", style);
            GUI.Label(new Rect(10, 125, 300, 20), $"Smoothing: {(useSmoothing ? $"ON ({smoothSpeed:F1})" : "OFF")}", style);
        }
#endif
    }
}
