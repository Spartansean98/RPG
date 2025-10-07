using UnityEngine;


namespace RPG.Core
{
    public class FollowCamera : MonoBehaviour
    {
        [SerializeField] GameObject target;
        [SerializeField] float minFov = 15f;
        [SerializeField] float maxFov  = 90f;
        [SerializeField] float sensitivity = 20f;

        void LateUpdate()
        {
            gameObject.transform.position = target.transform.position;
        }


        void Update()
        {
            ChangeFov();
        }

        private void ChangeFov()
        {
            float fov = Camera.main.fieldOfView;
            fov -= Input.GetAxis("Mouse ScrollWheel") * sensitivity;
            fov = Mathf.Clamp(fov, minFov, maxFov);
            Camera.main.fieldOfView = fov;
        }
    }
}