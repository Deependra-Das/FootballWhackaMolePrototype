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

        [Header("Shooting")]
        [SerializeField] private float _minLaunchSpeed = 8f;
        [SerializeField] private float _maxLaunchSpeed = 15f;
        [SerializeField] private float _verticalLaunchSpeed = 10f;

        [Header("Football")]
        [SerializeField] private Rigidbody _football;

        [Header("Trajectory Preview")]
        [SerializeField] private LineRenderer _trajectoryLine;
        [SerializeField] private int _trajectoryPointCount = 50;
        [SerializeField] private float _trajectoryTimeStep = 0.05f;

        private InputActionMap _playerActionMap;
        private InputAction _touchPressAction;
        private InputAction _touchPositionAction;

        private Vector2 _swipeStartPosition;
        private bool _isAiming;

        private void Awake()
        {
            _football.useGravity = false;
            _playerActionMap =_inputActionAsset.FindActionMap("Player", true);
            _touchPressAction = _playerActionMap.FindAction("TouchPress", true);
            _touchPositionAction = _playerActionMap.FindAction("TouchPosition", true);
            _trajectoryLine.enabled = false;
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

        private void Update()
        {
            if (!_isAiming)
                return;

            UpdateTrajectoryPreview();
        }

        private void HandleTouchStarted(InputAction.CallbackContext context)
        {
            _swipeStartPosition = _touchPositionAction.ReadValue<Vector2>();
            _isAiming = true;
            _trajectoryLine.enabled = false;
        }

        private void HandleTouchReleased(InputAction.CallbackContext context)
        {
            _isAiming = false;
            _trajectoryLine.enabled = false;
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

            ShootFootball(swipe);
        }

        private void ShootFootball(Vector2 swipe)
        {
            Vector3 launchVelocity = CalculateLaunchVelocity(swipe);

            _football.linearVelocity = Vector3.zero;
            _football.angularVelocity = Vector3.zero;
            _football.useGravity = true;
            _football.linearVelocity = launchVelocity;
        }

        private Vector3 CalculateLaunchVelocity(Vector2 swipe)
        {
            Vector2 swipeDirection = swipe.normalized;

            float swipeStrength = Mathf.InverseLerp(_minSwipeDistance, _maxSwipeDistance, swipe.magnitude);
            Vector3 launchDirection =new Vector3(swipeDirection.x, 0f, swipeDirection.y).normalized;

            float launchSpeed = Mathf.Lerp( _minLaunchSpeed, _maxLaunchSpeed, swipeStrength);
            Vector3 launchVelocity = launchDirection * launchSpeed;
            launchVelocity.y = _verticalLaunchSpeed;

            return launchVelocity;
        }

        private void UpdateTrajectoryPreview()
        {
            Vector2 currentTouchPosition =_touchPositionAction.ReadValue<Vector2>();
            Vector2 swipe = currentTouchPosition - _swipeStartPosition;

            if (swipe.magnitude < _minSwipeDistance)
            {
                _trajectoryLine.enabled = false;
                return;
            }

            swipe = Vector2.ClampMagnitude(swipe, _maxSwipeDistance);

            if (swipe.y <= 0f)
            {
                _trajectoryLine.enabled = false;
                return;
            }

            Vector3 launchVelocity = CalculateLaunchVelocity(swipe);
            DrawTrajectory( _football.position, launchVelocity);
        }

        private void DrawTrajectory( Vector3 startPosition, Vector3 launchVelocity)
        {
            _trajectoryLine.enabled = true;
            _trajectoryLine.positionCount = _trajectoryPointCount;

            for (int i = 0; i < _trajectoryPointCount; i++)
            {
                float time = i * _trajectoryTimeStep;
                Vector3 point = CalculateTrajectoryPoint(startPosition, launchVelocity, time );
                _trajectoryLine.SetPosition(i, point );
            }
        }

        private Vector3 CalculateTrajectoryPoint(Vector3 startPosition, Vector3 launchVelocity, float time)
        {
            return startPosition + launchVelocity * time + 0.5f * Physics.gravity * time * time;
        }
    }
}
