using UnityEngine;

namespace DefaultNamespace
{
    public class PlayerCamera : MonoBehaviour
    {
        public static Camera Instance { get; private set; }

        private void Awake()
        {
            Instance = GetComponent<Camera>();
        }
    }
}