using UnityEngine;
using System.Collections.Generic;
using FootballWhackaMolePrototype.Event;
using FootballWhackaMolePrototype.Mole;


namespace FootballWhackaMolePrototype.Gameplay
{
    public class GameplayManager : MonoBehaviour
    {
        [Header("Mole Spawn Points")]
        [SerializeField] private List<Transform> _moleSpawnPoints;

        private Dictionary<Transform, bool> _spawnPointAvailability;
        private MolePoolService _molePoolServiceObj;
        private EventBusService _eventBusServiceObj;
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
            SpawnMole();
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
            return Random.value < 0.5f ? MoleTypeEnum.Normal : MoleTypeEnum.Fast;
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
    }
}
