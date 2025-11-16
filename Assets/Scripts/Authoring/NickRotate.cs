using UnityEngine;

//todo
namespace RookieCartingClub.Authoring
{
    public class NickRotate : MonoBehaviour
    {
        public GameObject Camera;
        public CartHandleAuthoring cartHandleAuthoring;
        public bool NickSetup;
        private RectTransform rectTransform;

        public void Start()
        {
            Camera = GameObject.Find("Camera");
            rectTransform = gameObject.GetComponent<RectTransform>();
        }

        // Update is called once per frame
        private void Update()
        {
            // var angle = Quaternion.LookRotation(transform.position - Camera.transform.position);
            // rectTransform.rotation = angle;
            //
            // if (!NickSetup)
            //     if (!string.IsNullOrEmpty(CartHandle.Nickname.Value.Value))
            //     {
            //         NickSetup = true;
            //         GetComponent<TMP_Text>().text = CartHandle.Nickname.Value.Value;
            //     }
        }
    }
}