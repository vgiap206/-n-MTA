using UnityEngine;

namespace TopDownShooter
{
    /// <summary>
    /// Giữ nền mặt đất cơ sở luôn lặp vô tận phía dưới cùng xung quanh nhân vật khi Camera di chuyển.
    /// Sử dụng kích thước tối ưu (48x48 với scale 2x) để không bao giờ vượt giới hạn 65535 đỉnh của SpriteRenderer.Tiled.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class ArenaFloorFollow : MonoBehaviour
    {
        private void Awake()
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            sr.sprite = SpriteGenerator.GetCentralFloorSprite();
            sr.drawMode = SpriteDrawMode.Tiled;
            transform.localScale = new Vector3(2f, 2f, 1f);
            sr.size = new Vector2(40f, 40f);
            sr.sortingOrder = -30;
        }

        private void LateUpdate()
        {
            if (PlayerController.Instance == null) return;

            Vector3 p = PlayerController.Instance.transform.position;
            // Snap theo từng ô 2 đơn vị (tương ứng localScale = 2) để hoạ tiết mặt đất đứng yên trong không gian thế giới
            float sx = Mathf.Floor(p.x * 0.5f) * 2f;
            float sy = Mathf.Floor(p.y * 0.5f) * 2f;
            transform.position = new Vector3(sx, sy, 0f);
        }
    }
}
