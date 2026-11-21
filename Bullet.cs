using System.Collections.Generic;
using UnityEngine;

namespace TopDownShooter
{
    /// <summary>
    /// Đạn của người chơi: bay thẳng về phía trước (transform.right), gây sát thương khi trúng Enemy,
    /// hỗ trợ xuyên mục tiêu (pierceCount) khi nâng cấp vũ khí và dừng lại khi va chạm vật cản cứng.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(CircleCollider2D))]
    public class Bullet : MonoBehaviour
    {
        [Header("Chỉ số viên đạn")]
        public float speed = 14f;
        public float damage = 25f;
        public float lifeTime = 2.5f;
        public int pierceCount = 0;

        private Rigidbody2D _rb;
        private SpriteRenderer _sr;
        private bool _isDepleted;
        private readonly HashSet<Enemy> _hitEnemies = new HashSet<Enemy>();

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _rb.gravityScale = 0f;
            _rb.bodyType = RigidbodyType2D.Kinematic;
            _rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            CircleCollider2D col = GetComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius = 0.14f;

            _sr = GetComponent<SpriteRenderer>();
            if (_sr.sprite == null)
            {
                _sr.sprite = SpriteGenerator.GetBulletSprite();
            }
            _sr.sortingOrder = 5;
        }

        private void Start()
        {
            Destroy(gameObject, lifeTime);
        }

        public void Initialize(float bulletDamage, float bulletSpeed, int pierce = 0, float scale = 1f, Color? tint = null)
        {
            damage = bulletDamage;
            speed = bulletSpeed;
            pierceCount = Mathf.Max(0, pierce);
            transform.localScale = Vector3.one * Mathf.Clamp(scale, 0.7f, 2.5f);

            if (_sr == null) _sr = GetComponent<SpriteRenderer>();
            if (_sr != null && tint.HasValue)
            {
                _sr.color = tint.Value;
            }
        }

        private void FixedUpdate()
        {
            if (_rb != null)
            {
                Vector2 nextPos = _rb.position + (Vector2)transform.right * (speed * Time.fixedDeltaTime);
                _rb.MovePosition(nextPos);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_isDepleted) return;

            // 1. Kiểm tra nếu bắn trúng Bình Xăng Nổ (ExplosiveBarrel)
            ExplosiveBarrel barrel = other.GetComponent<ExplosiveBarrel>();
            if (barrel != null)
            {
                _isDepleted = true;
                barrel.TakeDamage(damage);
                Destroy(gameObject);
                return;
            }

            // 2. Kiểm tra va chạm vật cản môi trường (Đá, Tường, Nhà cửa, Thùng, Máy bay, Ô tô, Xe máy, Thân cây)
            EnvironmentObstacle obstacle = other.GetComponent<EnvironmentObstacle>();
            if (obstacle != null && obstacle.blocksBullets)
            {
                _isDepleted = true;
                Destroy(gameObject);
                return;
            }

            // Kiểm tra va chạm Kẻ Địch
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null && !enemy.IsDead)
            {
                if (_hitEnemies.Contains(enemy)) return;
                _hitEnemies.Add(enemy);

                enemy.TakeDamage(damage);

                if (pierceCount > 0)
                {
                    pierceCount--;
                    damage *= 0.85f; // Giảm nhẹ sát thương sau mỗi lần xuyên
                }
                else
                {
                    _isDepleted = true;
                    Destroy(gameObject);
                }
            }
        }
    }
}
