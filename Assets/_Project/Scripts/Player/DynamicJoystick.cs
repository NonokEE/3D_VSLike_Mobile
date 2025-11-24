using UnityEngine;
using UnityEngine.EventSystems;

namespace VSLike.Player
{
    /// <summary>
    /// Dynamic joystick that appears at touch position<br/>
    /// Provides normalized input direction for player movement<br/>
    /// Optimized for mobile touch input<br/>
    /// </summary>
    public class DynamicJoystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        [Header("Joystick Settings")]
        [SerializeField] private float dragRadius = 50f;
        
        [Header("UI References")]
        [SerializeField] private RectTransform joystickBackground;
        [SerializeField] private RectTransform joystickHandle;
        
        [Header("Visual Settings")]
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private float fadeSpeed = 10f;
        
        // Input state
        private Vector2 inputDirection = Vector2.zero;
        private Vector2 joystickCenter = Vector2.zero;
        private bool isActive = false;
        
        // Canvas reference
        private Canvas canvas;
        private Camera mainCamera;

        private void Awake()
        {
            // Get canvas
            canvas = GetComponentInParent<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("[DynamicJoystick] Must be child of Canvas!");
            }
            
            // Cache main camera
            mainCamera = Camera.main;
            
            // Initialize canvas group if not assigned
            if (canvasGroup == null)
            {
                canvasGroup = GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                {
                    canvasGroup = gameObject.AddComponent<CanvasGroup>();
                }
            }
            
            // Start hidden
            Hide();
        }

        /// <summary>
        /// Get normalized input direction<br/>
        /// Called by PlayerController<br/>
        /// </summary>
        public Vector2 GetInputDirection()
        {
            return inputDirection;
        }

        /// <summary>
        /// Handle touch/pointer down event<br/>
        /// Shows joystick at touch position<br/>
        /// </summary>
        public void OnPointerDown(PointerEventData eventData)
        {
            // Set joystick position to touch position
            joystickCenter = eventData.position;
            joystickBackground.position = joystickCenter;
            joystickHandle.position = joystickCenter;
            
            isActive = true;
            Show();
        }

        /// <summary>
        /// Handle touch/pointer up event<br/>
        /// Hides joystick and resets input<br/>
        /// </summary>
        public void OnPointerUp(PointerEventData eventData)
        {
            isActive = false;
            inputDirection = Vector2.zero;
            Hide();
        }

        /// <summary>
        /// Handle drag event<br/>
        /// Updates handle position and calculates input direction<br/>
        /// </summary>
        public void OnDrag(PointerEventData eventData)
        {
            if (!isActive)
                return;
            
            // Calculate offset from joystick center
            Vector2 offset = eventData.position - joystickCenter;
            
            // Clamp to drag radius
            if (offset.magnitude > dragRadius)
            {
                offset = offset.normalized * dragRadius;
            }
            
            // Update handle position
            joystickHandle.position = joystickCenter + offset;
            
            // Calculate normalized input direction
            inputDirection = offset / dragRadius;
        }

        /// <summary>
        /// Show joystick with fade
        /// </summary>
        private void Show()
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        /// <summary>
        /// Hide joystick with fade
        /// </summary>
        private void Hide()
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            
            // Reset handle position
            if (joystickHandle != null)
            {
                joystickHandle.anchoredPosition = Vector2.zero;
            }
        }

#if UNITY_EDITOR
        // Debug visualization
        private void OnValidate()
        {
            if (joystickBackground == null)
            {
                joystickBackground = transform.Find("Background")?.GetComponent<RectTransform>();
            }
            
            if (joystickHandle == null)
            {
                joystickHandle = transform.Find("Background/Handle")?.GetComponent<RectTransform>();
            }
        }
#endif
    }
}
