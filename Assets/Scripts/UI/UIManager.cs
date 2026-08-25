using TMPro;
using UnityEngine;
using UnityEngine.UI;
using FootballWhackaMolePrototype.Event;

namespace FootballWhackaMolePrototype.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("Timer")]
        [SerializeField] private TMP_Text _timerText;

        [Header("Game")]
        [SerializeField] private Button _restartButton;

        private EventBusService _eventBusServiceObj;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void SubscribeToEvents()
        {
            _restartButton.onClick.AddListener(OnRestartButtonClicked);
            _eventBusServiceObj.Subscribe<GameTimerUpdatedEvent>(HandleTimerUpdated);
        }

        private void UnsubscribeToEvents()
        {
            _restartButton.onClick.RemoveListener(OnRestartButtonClicked);

            if (_eventBusServiceObj != null)
            {
                _eventBusServiceObj.Unsubscribe<GameTimerUpdatedEvent>(HandleTimerUpdated);
            }
        }

        public void Initialize(EventBusService eventBusService)
        {
            _eventBusServiceObj = eventBusService;
            SubscribeToEvents();
        }

        private void HandleTimerUpdated(GameTimerUpdatedEvent eventData)
        {
            int seconds = Mathf.CeilToInt(eventData.RemainingTime);
            _timerText.text = seconds.ToString();
        }

        private void OnRestartButtonClicked()
        {
            _eventBusServiceObj.Publish(new RestartGameEvent());
        }

        private void OnDestroy()
        {
            if (Instance != this)
                return;

            UnsubscribeToEvents();
            Instance = null;
        }
    }
}