using FootballWhackaMolePrototype.Player;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [Header("Football")]
    [SerializeField] private PlayerFootballController _playerFootballPrefab;
    [SerializeField] private Transform _footballSpawnPoint;

    private PlayerFootballController _playerFootball;

    public static PlayerManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void SpawnPlayer()
    {
        if (_playerFootball == null)
        {
            _playerFootball = Instantiate(_playerFootballPrefab, _footballSpawnPoint.position, _footballSpawnPoint.rotation);
        }

        _playerFootball.ResetFootball();
    }

    public void RespawnPlayer()
    {
        if (_playerFootball == null)
        {
            Debug.LogWarning("Cannot respawn player because player has not been spawned.");
            return;
        }

        _playerFootball.transform.SetPositionAndRotation(_footballSpawnPoint.position, _footballSpawnPoint.rotation);
        _playerFootball.ResetFootball();
    }
}
