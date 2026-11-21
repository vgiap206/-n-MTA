using UnityEngine;

namespace TopDownShooter
{
    /// <summary>
    /// Camera di chuyển bám theo nhân vật chính mượt mà.
    /// </summary>
    public class CameraFollow : MonoBehaviour
    {
        [Header("Cài đặt Camera bám theo nhân vật")]
        [Tooltip("Mục tiêu cần bám theo (tự động nhận Player nếu để trống)")]
        public Transform target;

        [Tooltip("Độ mượt khi di chuyển camera (càng cao càng bám sát)")]
        public float smoothSpeed = 10f;

        [Tooltip("Khoảng cách lệch trục Z của Camera 2D")]
        public Vector3 offset = new Vector3(0f, 0f, -10f);

        private void LateUpdate()
        {
            if (target == null)
            {
                if (PlayerController.Instance != null)
                {
                    target = PlayerController.Instance.transform;
                }
                else
                {
                    return;
                }
            }

            Vector3 desiredPosition = target.position + offset;
            desiredPosition.z = offset.z;

            transform.position = Vector3.Lerp(
                transform.position,
                desiredPosition,
                1f - Mathf.Exp(-smoothSpeed * Time.deltaTime));
        }
    }
}

