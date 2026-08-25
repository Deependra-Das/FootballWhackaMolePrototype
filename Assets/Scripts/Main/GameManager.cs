using UnityEngine;
using FootballWhackaMolePrototype.Event;

namespace FootballWhackaMolePrototype.Main
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public ServiceLocator Services { get; private set; }
        private EventBusService _eventBusService;

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
        }

        private void InitializeServices()
        {
            Services = new ServiceLocator();
            _eventBusService = new EventBusService();
        }

        private void RegisterServices()
        {
            Services.Register(_eventBusService);
        }
    }
}