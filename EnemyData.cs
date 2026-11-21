using System;
using UnityEngine;

namespace TopDownShooter
{
    /// <summary>
    /// Cấu hình chỉ số cho từng loại kẻ địch khác nhau:
    /// - Tốc độ di chuyển (moveSpeed)
    /// - Máu (maxHealth)
    /// - Tấn công (attackDamage)
    /// - Tốc độ đánh (attackSpeed)
    /// - Tỉ lệ xuất hiện (spawnRate)
    /// - EXP cộng cho người chơi khi bị tiêu diệt (expReward)
    /// </summary>
    [Serializable]
    public class EnemyTypeData
    {
        [Tooltip("Tên loại kẻ địch")]
        public string enemyName = "Kẻ Địch Thường";

        [Tooltip("Prefab riêng của loại kẻ địch này (nếu để trống sẽ tự tạo từ dữ liệu)")]
        public Enemy enemyPrefab;

        [Tooltip("Tốc độ di chuyển của kẻ địch")]
        public float moveSpeed = 2.5f;

        [Tooltip("Máu tối đa của kẻ địch")]
        public float maxHealth = 50f;

        [Tooltip("Sát thương gây ra cho người chơi mỗi đòn đánh")]
        public float attackDamage = 10f;

        [Tooltip("Tốc độ đánh (số lần gây sát thương mỗi giây khi chạm người chơi)")]
        public float attackSpeed = 1.0f;

        [Tooltip("Tỉ lệ xuất hiện (trọng số xác suất spawn)")]
        [Range(1f, 100f)]
        public float spawnRate = 50f;

        [Tooltip("Lượng EXP cộng cho người chơi khi tiêu diệt kẻ địch này")]
        public int expReward = 25;

        [Header("Hình ảnh hiển thị")]
        public Color color = new Color(0.35f, 0.85f, 0.35f, 1f);
        public float scale = 0.85f;
    }
}

