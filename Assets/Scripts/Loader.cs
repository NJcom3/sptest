using UnityEngine;

namespace GameSession
{
    public class Loader : MonoBehaviour
    {
        private static GameObject _canvas = null;
        private void Awake()
        {
            var canvas = transform.parent.gameObject;
            
            if (_canvas != null && _canvas != this)
            {
                Destroy(canvas);
                return;
            }
            
            DontDestroyOnLoad(canvas);
            _canvas = canvas;
        }
    }
}