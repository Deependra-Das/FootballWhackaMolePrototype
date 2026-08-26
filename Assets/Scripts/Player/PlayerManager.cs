using FootballWhackaMolePrototype.Player;
using FootballWhackaMolePrototype.UI;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [Header("Football")]
    [SerializeField] private PlayerFootballController _playerFootballPrefab;
    [SerializeField] private Transform _footballSpawnPoint;

    private PlayerFootballController _playerFootball;

    private bool _isFootballShotInProgress;

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
        _isFootballShotInProgress = false;

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

        _isFootballShotInProgress = false;
        _playerFootball.transform.SetPositionAndRotation(_footballSpawnPoint.position, _footballSpawnPoint.rotation);
        _playerFootball.ResetFootball();
    }

    public void RegisterShot()
    {
        if (_playerFootball == null)
            return;

        _isFootballShotInProgress = true;
    }

    public void HandleFootballHitMole()
    {
        if (!_isFootballShotInProgress)
            return;

        RespawnPlayer();
    }

    public void HandleFootballMiss(Vector3 missPosition)
    {
        if (!_isFootballShotInProgress)
            return;

        _isFootballShotInProgress = false;
        GameplayFeedbackUIManager.Instance.ShowMissFeedback(missPosition);
        RespawnPlayer();
    }

    public void CleanupPlayer()
    {
        _isFootballShotInProgress = false;

        if (_playerFootball == null)
            return;

        Destroy(_playerFootball.gameObject);
        _playerFootball = null;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
