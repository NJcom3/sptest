using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using MazeGeneration;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameSessionController : MonoBehaviour
    {
        [SerializeField] private CanvasGroup winPopup;
        
        [SerializeField] public MazeRenderer mazeRenderer;
        [SerializeField] private TMP_Text timerText;
        [SerializeField] private TMP_Text distanceText;
        [SerializeField] List<ParticleSystem> particles;

        private bool _pause = true;

        private float _timeFromStart;
        private float _distance;
        private void Start()
        {
            GameController.Instance.OnPlaySceneLoaded(this);
            _timeFromStart = 0f;
            _distance = 0f;
        }

        public void StartGame()
        {
            _pause = false;
        }

        private void Update()
        {
            if (_pause)
            {
                return;
            }
            
            _timeFromStart += Time.deltaTime;
            timerText.text = TimeSpan.FromSeconds(_timeFromStart).ToString(@"mm\:ss");
        }

        public void AddDistance(float distance)
        {
            if (_pause)
            {
                return;
            }
            
            _distance += distance;
            distanceText.text = _distance.ToString("F2");
        }

        public void ExitFounded()
        {
            _pause = true;
            SaveManager.SaveResult(_timeFromStart, _distance);
            GameFinFlow();
        }

        private async Task GameFinFlow()
        {
            foreach (var particle in particles)
            {
                particle.gameObject.SetActive(true);
                await Task.Delay(TimeSpan.FromSeconds(Random.Range(0.05f, 0.3f)));
            }
            
            await Task.Delay(TimeSpan.FromSeconds(0.3f));
            
            winPopup.alpha = 0;
            winPopup.gameObject.SetActive(true);
            winPopup.DOFade(1f, 0.5f);
        }

        public void GoToMenu()
        {
            SceneManager.LoadScene("MenuScene");
        }
    }
