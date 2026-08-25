using UnityEngine;
using UnityEngine.InputSystem;

namespace FootballWhackaMolePrototype.Player
{
    public class PlayerFootballController : MonoBehaviour
    {
        [Header("Input")]
        [SerializeField] private InputActionAsset _inputActionAsset;

        [Header("Swipe")]
        [SerializeField] private float _minSwipeDistance = 50f;
        [SerializeField] private float _maxSwipeDistance = 600f;

        private InputActionMap _playerActionMap;
        private InputAction _touchPressAction;
        private InputAction _touchPositionAction;

        private Vector2 _swipeStartPosition;

        private void Awake()
        {
            _playerActionMap = _inputActionAsset.FindActionMap("Player", true);
            _touchPressAction = _playerActionMap.FindAction("TouchPress", true);
            _touchPositionAction = _playerActionMap.FindAction("TouchPosition", true);
        }

        private void OnEnable()
        {
            _playerActionMap.Enable();

            _touchPressAction.started += HandleTouchStarted;
            _touchPressAction.canceled += HandleTouchReleased;
        }

        private void OnDisable()
        {
            _touchPressAction.started -= HandleTouchStarted;
            _touchPressAction.canceled -= HandleTouchReleased;

            _playerActionMap.Disable();
        }

        private void HandleTouchStarted(InputAction.CallbackContext context)
        {
            _swipeStartPosition = _touchPositionAction.ReadValue<Vector2>();
        }

        private void HandleTouchReleased(InputAction.CallbackContext context)
        {
            Vector2 swipeEndPosition = _touchPositionAction.ReadValue<Vector2>();
            Vector2 swipe = swipeEndPosition - _swipeStartPosition;

            if (swipe.magnitude < _minSwipeDistance)
            {
                Debug.Log("Swipe too short - ignored");
                return;
            }

            swipe = Vector2.ClampMagnitude(swipe, _maxSwipeDistance);

            if (swipe.y <= 0f)
            {
                Debug.Log("Swipe was not upward - ignored");
                return;
            }

            Debug.Log("Valid Swipe!");
        }
    }
}
