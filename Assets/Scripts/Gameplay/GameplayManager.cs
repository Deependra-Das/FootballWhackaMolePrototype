using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FootballWhackaMolePrototype.Event;
using FootballWhackaMolePrototype.Mole;
using FootballWhackaMolePrototype.Score;


namespace FootballWhackaMolePrototype.Gameplay
{
    public class GameplayManager : MonoBehaviour
    {
        [Header("Gameplay")]
        [SerializeField] private float _sessionDuration = 60f;

        [Header("Mole Spawn Points")]
        [SerializeField] private List<Transform> _moleSpawnPoints;

        [Header("Mole Spawn")]
        [SerializeField] private float _moleSpawnInterval = 2.5f;

        [SerializeField, Range(0f, 1f)]
        private float _fastMoleChanceRate = 0.25f;

        private Dictionary<Transform, bool> _spawnPointAvailability;
        private readonly List<BaseMole> _activeMoles = new();

        private MolePoolService _molePoolServiceObj;
        private EventBusService _eventBusServiceObj;
        private ScoreService _scoreServiceObj;

        private float _remainingTime;
        private bool _isPlaying;

        private Coroutine _gameplayRoutine;
        private Coroutine _moleSpawnRoutine;

        public static GameplayManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        private void SubscribeToEvents()
        {
            _eventBusServiceObj.Subscribe<RestartGameEvent>(HandleRestartGame);
        }

        private void UnsubscribeToEvents()
        {
            if (_eventBusServiceObj != null)
            {
                _eventBusServiceObj.Unsubscribe<RestartGameEvent>(HandleRestartGame);
            }
        }

        public void Initialize(MolePoolService molePoolService, ScoreService scoreService, EventBusService eventBusService)
        {
            _molePoolServiceObj = molePoolService;
            _scoreServiceObj = scoreService;
            _eventBusServiceObj = eventBusService;
            SubscribeToEvents();
            InitializeSpawnPoints();
        }

        private void InitializeSpawnPoints()
        {
            _spawnPointAvailability = new Dictionary<Transform, bool>();

            foreach (Transform spawnPoint in _moleSpawnPoints)
            {
                if (spawnPoint == null)
                    continue;

                _spawnPointAvailability.Add(spawnPoint, true);
            }
        }

        public void StartGameplay()
        {
            if (_isPlaying)
                return;

            _isPlaying = true;

            _scoreServiceObj.ResetScore();
            PlayerManager.Instance.SpawnPlayer();
            _gameplayRoutine = StartCoroutine(GameplayRoutine());
            _moleSpawnRoutine = StartCoroutine(MoleSpawnRoutine());
        }

        private IEnumerator GameplayRoutine()
        {
            _remainingTime = _sessionDuration;
            int lastDisplayedSecond = -1;

            while (_remainingTime > 0f)
            {
                _remainingTime -= Time.deltaTime;
                _remainingTime = Mathf.Max(_remainingTime, 0f);
                int displayedSecond = Mathf.CeilToInt(_remainingTime);

                if (displayedSecond != lastDisplayedSecond)
                {
                    lastDisplayedSecond = displayedSecond;
                    RaiseGameTimerUpdatedEvent(_remainingTime);
                }

                yield return null;
            }

            EndGame();
        }

        private IEnumerator MoleSpawnRoutine()
        {
            while (_isPlaying)
            {
                yield return new WaitForSeconds(_moleSpawnInterval);

                if (!_isPlaying)
                    yield break;

                SpawnMole();
            }
        }

        private void SpawnMole()
        {
            int spawnPointIndex = GetRandomAvailableSpawnPointIndex();

            if (spawnPointIndex == -1)
            {
                Debug.LogWarning("No available mole spawn point.");
                return;
            }

            MoleTypeEnum moleType = GetRandomMoleType();
            BaseMole mole = _molePoolServiceObj.GetMole(moleType);

            if (mole == null)
            {
                Debug.LogWarning("Failed to get mole from pool.");
                return;
            }

            Transform spawnPoint = _moleSpawnPoints[spawnPointIndex];
            _spawnPointAvailability[spawnPoint] = false;
            mole.transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
            mole.Initialize(this, spawnPointIndex);
            _activeMoles.Add(mole);
        }

        private MoleTypeEnum GetRandomMoleType()
        {
            return Random.value < _fastMoleChanceRate ? MoleTypeEnum.Fast : MoleTypeEnum.Normal;
        }

        private int GetRandomAvailableSpawnPointIndex()
        {
            List<int> availableIndices = new List<int>();

            for (int i = 0; i < _moleSpawnPoints.Count; i++)
            {
                Transform spawnPoint = _moleSpawnPoints[i];

                if (spawnPoint == null)
                    continue;

                if (_spawnPointAvailability[spawnPoint])
                {
                    availableIndices.Add(i);
                }
            }

            if (availableIndices.Count == 0) return -1;

            int randomIndex = Random.Range(0, availableIndices.Count);
            return availableIndices[randomIndex];
        }

        public void DespawnMole(BaseMole mole, int spawnPointIndex)
        {
            if (mole == null) return;

            if (spawnPointIndex < 0 || spawnPointIndex >= _moleSpawnPoints.Count)
            {
                Debug.LogWarning($"Invalid mole spawn point index: {spawnPointIndex}");
                return;
            }

            Transform spawnPoint = _moleSpawnPoints[spawnPointIndex];
            _spawnPointAvailability[spawnPoint] = true;
            _activeMoles.Remove(mole);
            _molePoolServiceObj.ReturnMole(mole);
        }

        public void HandleMoleHit(BaseMole mole, int spawnPointIndex)
        {
            if (mole == null)
                return;

            _scoreServiceObj.UpdateScore(mole.Score);
            DespawnMole(mole, spawnPointIndex);
            PlayerManager.Instance.HandleFootballHitMole();
        }

        private void EndGame()
        {
            if (!_isPlaying) return;

            _isPlaying = false;
      
            if (_gameplayRoutine != null)
            {
                StopCoroutine(_gameplayRoutine);
                _gameplayRoutine = null;
            }

            if (_moleSpawnRoutine != null)
            {
                StopCoroutine(_moleSpawnRoutine);
                _moleSpawnRoutine = null;
            }

            ReturnAllActiveMoles();
            ResetSpawnPoints();
            Debug.Log("Game Over");
        }

        private void HandleRestartGame(RestartGameEvent eventData)
        {
            RestartGameplay();
        }

        private void RestartGameplay()
        {
            EndGame();
            StartGameplay();
        }

        private void ReturnAllActiveMoles()
        {
            for (int i = _activeMoles.Count - 1; i >= 0; i--)
            {
                BaseMole mole = _activeMoles[i];

                if (mole == null)
                    continue;

                _molePoolServiceObj.ReturnMole(mole);
            }

            _activeMoles.Clear();
        }

        private void ResetSpawnPoints()
        {
            foreach (Transform spawnPoint in _moleSpawnPoints)
            {
                if (spawnPoint == null)
                    continue;

                _spawnPointAvailability[spawnPoint] = true;
            }
        }

        private void RaiseGameTimerUpdatedEvent(float remainingTime)
        {
            _eventBusServiceObj.Publish(new GameTimerUpdatedEvent(remainingTime));
        }

        private void OnDestroy()
        {
            UnsubscribeToEvents();
        }
    }
}
