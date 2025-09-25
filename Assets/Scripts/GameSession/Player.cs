using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameSession
{
    public class Player : MonoBehaviour
    {
        private Vector3 _moveVector;
        private readonly float speed = 5f;
        private Camera _camera;
        private Action<float> _addDistanceAction;
        private Action _exitFoundedAction;
        private Vector3 _startFramePosition;
  
        public void SetAddDistanceAction(Action<float> action)
        {
            _addDistanceAction = action;
        }

        public void SetExitFoundedAction(Action action)
        {
            _exitFoundedAction = action;
        }

        private void Start()
        {
            if (!_camera)
            {
                _camera = Camera.main;
            }
        }

        public void OnMove(InputValue value)
        {
            _moveVector = value.Get<Vector2>();
        }


        private void OnTriggerEnter2D(Collider2D other)
        {
            // always exit 4 now
        
            _exitFoundedAction?.Invoke();
            enabled = false;
        }

        private void FixedUpdate()
        {
            if (!_camera)
            {
                return;
            }
        
            var distance = _moveVector.normalized * (Time.deltaTime * speed);
            _startFramePosition = transform.position;
            transform.position += distance;
        
            var cameraTransform = _camera.transform;
        
            cameraTransform.position = new Vector3(
                transform.position.x,
                transform.position.y, 
                cameraTransform.position.z
            );
        }

        private void Update()
        {
            _addDistanceAction?.Invoke((_startFramePosition - transform.position).magnitude);
        }
    }
}
