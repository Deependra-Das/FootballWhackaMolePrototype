using TMPro;
using UnityEngine;
using UnityEngine.UI;
using FootballWhackaMolePrototype.Event;

namespace FootballWhackaMolePrototype.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [SerializeField] private TMP_Text _timerText;
        [SerializeField] private TMP_Text _scoreText;
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
            _eventBusServiceObj.Subscribe<ScoreUpdatedEvent>(HandleScoreUpdated);
        }

        private void UnsubscribeToEvents()
        {
            _restartButton.onClick.RemoveListener(OnRestartButtonClicked);

            if (_eventBusServiceObj != null)
            {
                _eventBusServiceObj.Unsubscribe<GameTimerUpdatedEvent>(HandleTimerUpdated);
                _eventBusServiceObj.Unsubscribe<ScoreUpdatedEvent>(HandleScoreUpdated);
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

        private void HandleScoreUpdated(ScoreUpdatedEvent eventData)
        {
            int currentScore = eventData.CurrentScore;
            _scoreText.text = currentScore.ToString();
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