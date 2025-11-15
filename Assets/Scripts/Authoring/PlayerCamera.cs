using UnityEngine;

namespace RookieCartingClub.Authoring
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