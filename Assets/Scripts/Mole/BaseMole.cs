using System.Collections;
using UnityEngine;
using FootballWhackaMolePrototype.Gameplay;

namespace FootballWhackaMolePrototype.Mole
{
    public abstract class BaseMole : MonoBehaviour
    {
        [SerializeField] private MoleTypeEnum _moleType;

        [Header("Movement")]
        [SerializeField] private float _popUpHeight = 3f;
        [SerializeField] private float _popUpDuration = 0.15f;
        [SerializeField] private float _popDownDuration = 0.15f;

        protected bool _isVisible;
        public bool IsVisible => _isVisible;

        private Coroutine _moleRoutine;
        private int _spawnPointIndex;
        private Vector3 _hiddenPosition;
        private Vector3 _visiblePosition;

        private GameplayManager _gameplayManager;

        public abstract int Score { get; }

        protected abstract float VisibleDuration { get; }

        public virtual void Initialize(GameplayManager gameplayManager, int spawnPointIndex)
        {
            _gameplayManager = gameplayManager;
            _spawnPointIndex = spawnPointIndex;
            _isVisible = false;
            _hiddenPosition = transform.localPosition;
            _visiblePosition = _hiddenPosition + Vector3.up * _popUpHeight;

            _moleRoutine = StartCoroutine(MoleRoutine());
        }

        private IEnumerator MoleRoutine()
        {
            _isVisible = true;
            yield return MoveMole(_hiddenPosition, _visiblePosition, _popUpDuration);

            yield return new WaitForSeconds(VisibleDuration);

            yield return MoveMole(_visiblePosition, _hiddenPosition, _popDownDuration);
            _isVisible = false;

            _moleRoutine = null;
            _gameplayManager.DespawnMole(this, _spawnPointIndex);
        }

        private IEnumerator MoveMole(Vector3 start, Vector3 target, float duration)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;

                float timer = Mathf.Clamp01(elapsed / duration);
                timer = Mathf.SmoothStep(0f, 1f, timer);
                transform.localPosition = Vector3.Lerp(start, target, timer);

                yield return null;
            }

            transform.localPosition = target;
        }

        protected virtual void OnDisable()
        {
            if (_moleRoutine != null)
            {
                StopCoroutine(_moleRoutine);
                _moleRoutine = null;
            }

            _isVisible = false;
        }
    }
}
