using UnityEngine;

namespace TopDownShooter
{
    public enum ObstacleKind
    {
        Tree,
        Bush,
        Rock,
        Crate,
        Pillar,
        Wall,
        House,
        ExplosiveBarrel,
        CrashedPlane,
        WreckedCar,
        BrokenMotorcycle
    }

    /// <summary>
    /// Vật cản / Chướng ngại vật môi trường trên bản đồ:
    /// Nhà cửa, Cây cối, Đá tảng, Thùng gỗ, Bình xăng nổ, Máy bay hỏng, Ô tô hỏng, Xe máy hỏng.
    /// </summary>
    public class EnvironmentObstacle : MonoBehaviour
    {
        public ObstacleKind kind = ObstacleKind.Rock;
        public bool blocksBullets = true;
        public Color minimapColor = new Color(0.45f, 0.50f, 0.55f, 1f);
        public float minimapRadius = 1.2f;
    }
}
