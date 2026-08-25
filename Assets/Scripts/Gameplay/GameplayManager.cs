using FootballWhackaMolePrototype.Event;
using FootballWhackaMolePrototype.Mole;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
        private MolePoolService _molePoolServiceObj;
        private EventBusService _eventBusServiceObj;

        private float _remainingTime;
        private float _moleSpawnTimer;
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

        public void Initialize(MolePoolService molePoolService, EventBusService eventBusService)
        {
            _molePoolServiceObj = molePoolService;
            _eventBusServiceObj = eventBusService;
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

            _gameplayRoutine = StartCoroutine(GameplayRoutine());
            _moleSpawnRoutine = StartCoroutine(MoleSpawnRoutine());
        }

        private IEnumerator GameplayRoutine()
        {
            float elapsedTime = 0f;

            while (elapsedTime < _sessionDuration)
            {
                elapsedTime += Time.deltaTime;

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
        }

        private MoleTypeEnum GetRandomMoleType()
        {
            return Random.value < _fastMoleChanceRate ? MoleTypeEnum.Normal : MoleTypeEnum.Fast;
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
            _molePoolServiceObj.ReturnMole(mole);
        }

        private void EndGame()
        {
            if (!_isPlaying) return;

            _isPlaying = false;

            if (_moleSpawnRoutine != null)
            {
                StopCoroutine(_moleSpawnRoutine);
                _moleSpawnRoutine = null;
            }

            _gameplayRoutine = null;
            Debug.Log("Game Over");
        }
    }
}
