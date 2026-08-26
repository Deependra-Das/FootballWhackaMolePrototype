using System.Collections;
using TMPro;
using UnityEngine;

namespace FootballWhackaMolePrototype.UI
{
    public class GameplayFeedbackUIManager : MonoBehaviour
    {
        [Header("World Space UI")]
        [SerializeField] private GameObject _hitImage;
        [SerializeField] private GameObject _missImage;

        [Header("Display")]
        [SerializeField] private float _verticalOffset = 0.5f;
        [SerializeField] private float _displayDuration = 0.5f;

        private Coroutine _hitRoutine;
        private Coroutine _missRoutine;

        public static GameplayFeedbackUIManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            HideImage(_hitImage);
            HideImage(_missImage);
        }

        public void ShowHitFeedback(Vector3 position)
        {
            if (_hitImage == null)
                return;

            if (_hitRoutine != null)
            {
                StopCoroutine(_hitRoutine);
            }

            _hitImage.transform.position = position + Vector3.up * _verticalOffset;

            _hitImage.gameObject.SetActive(true);
            _hitRoutine = StartCoroutine(HideImageAfterDelay( _hitImage,_displayDuration));
        }

        public void ShowMissFeedback(Vector3 position)
        {
            if (_missImage == null)
                return;

            if (_missRoutine != null)
            {
                StopCoroutine(_missRoutine);
            }

            _missImage.transform.position = position + Vector3.up * _verticalOffset;
            _missImage.gameObject.SetActive(true);
            _missRoutine = StartCoroutine(HideImageAfterDelay(_missImage,_displayDuration));
        }

        private IEnumerator HideImageAfterDelay(GameObject image,float duration)
        {
            yield return new WaitForSeconds(duration);
            HideImage(image);
        }

        private void HideImage(GameObject image)
        {
            if (image != null)
            {
                image.gameObject.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
    }
}