using UnityEngine;
using FootballWhackaMolePrototype.Event;
using FootballWhackaMolePrototype.Gameplay;
using FootballWhackaMolePrototype.Mole;
using FootballWhackaMolePrototype.UI;
using FootballWhackaMolePrototype.Score;

namespace FootballWhackaMolePrototype.Main
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private MoleData_SO _moleData_SO;
        [SerializeField] private Transform _poolContainer;
        public static GameManager Instance { get; private set; }

        public ServiceLocator Services { get; private set; }
        private EventBusService _eventBusService;
        private MolePoolService _molePoolService;
        private ScoreService _scoreService;

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

        private void Start()
        {
            InitializeServices();
            RegisterServices();
            UIManager.Instance.Initialize(_eventBusService);
            GameplayManager.Instance.Initialize(_molePoolService, _scoreService, _eventBusService);
            GameplayManager.Instance.StartGameplay();
        }

        private void InitializeServices()
        {
            Services = new ServiceLocator();
            _eventBusService = new EventBusService();
            _molePoolService = new MolePoolService(_moleData_SO, _poolContainer);
            _scoreService = new ScoreService(_eventBusService);
        }

        private void RegisterServices()
        {
            Services.Register(_eventBusService);
            Services.Register(_molePoolService);
            Services.Register(_scoreService);
        }
    }
}