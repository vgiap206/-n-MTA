using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TopDownShooter
{
    /// <summary>
    /// Hành vi và chỉ số của Kẻ Địch (Enemy):
    /// - Di chuyển về phía người chơi, gây sát thương khi chạm vào theo tốc độ đánh.
    /// - Có các chỉ số: tốc độ di chuyển, máu, tấn công, tốc độ đánh, tỉ lệ xuất hiện, exp thưởng.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(CircleCollider2D))]
    public class Enemy : MonoBehaviour
    {
        public static readonly List<Enemy> ActiveEnemies = new List<Enemy>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStaticEnemies()
        {
            ActiveEnemies.Clear();
        }

        [Header("Thông tin loại kẻ địch")]
        public string enemyTypeName = "Zombie Thường";

        [Header("Các chỉ số của kẻ địch (Enemy Stats)")]
        [Tooltip("Tốc độ di chuyển")]
        public float moveSpeed = 2.5f;

        [Tooltip("Máu tối đa")]
        public float maxHealth = 50f;

        [Tooltip("Máu hiện tại")]
        public float currentHealth = 50f;

        [Tooltip("Chỉ số tấn công (sát thương mỗi lần đánh)")]
        public float attackDamage = 10f;

        [Tooltip("Tốc độ đánh (số lần gây sát thương / giây khi chạm người chơi)")]
        public float attackSpeed = 1.0f;

        [Tooltip("Tỉ lệ xuất hiện (Weight / %)")]
        public float spawnRate = 50f;

        [Tooltip("EXP cộng cho người chơi khi bị tiêu diệt")]
        public int expReward = 25;

        [Header("Hiển thị & Độ Khó Đợt (Wave)")]
        public Color baseColor = new Color(0.35f, 0.85f, 0.35f, 1f);
        public int waveLevel = 1;
        public bool isWaveBoss = false;

        public bool IsDead => _isDead;

        private Rigidbody2D _rb;
        private SpriteRenderer _sr;
        private CircleCollider2D _collider;
        private float _nextAttackTime;
        private bool _isDead;
        private Coroutine _flashRoutine;

        // Thanh máu nhỏ phía trên đầu kẻ địch
        private Transform _hpBarFill;
        private GameObject _hpBarRoot;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _rb.gravityScale = 0f;
            _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            _rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            _rb.sleepMode = RigidbodySleepMode2D.NeverSleep;

            _collider = GetComponent<CircleCollider2D>();
            _collider.radius = 0.34f;
            _collider.isTrigger = false;

            _sr = GetComponent<SpriteRenderer>();
            if (_sr.sprite == null || _sr.sprite.rect.width < 90f)
            {
                _sr.sprite = SpriteGenerator.GetEnemySprite();
            }
            _sr.color = baseColor;
            _sr.sortingOrder = 3;

            currentHealth = maxHealth;
            CreateOverheadHealthBar();
        }

        private void OnEnable()
        {
            if (!ActiveEnemies.Contains(this))
            {
                ActiveEnemies.Add(this);
            }
        }

        private void OnDisable()
        {
            ActiveEnemies.Remove(this);
        }

        public void SetupFromData(EnemyTypeData data)
        {
            if (data == null) return;

            enemyTypeName = data.enemyName;
            moveSpeed = data.moveSpeed;
            maxHealth = data.maxHealth;
            currentHealth = data.maxHealth;
            attackDamage = data.attackDamage;
            attackSpeed = data.attackSpeed;
            spawnRate = data.spawnRate;
            expReward = data.expReward;
            baseColor = data.color;

            transform.localScale = Vector3.one * data.scale;

            if (_sr == null) _sr = GetComponent<SpriteRenderer>();
            if (_sr != null)
            {
                if (_sr.sprite == null || _sr.sprite.rect.width < 90f) _sr.sprite = SpriteGenerator.GetEnemySprite();
                _sr.color = baseColor;
            }
        }

        /// <summary>
        /// Tăng độ khó của quái theo từng Đợt (Wave): tăng Máu, Tấn công, Tốc độ chạy, Tốc độ đánh và EXP thưởng.
        /// </summary>
        public void ApplyWaveDifficulty(
            int wave,
            float healthMultiplier,
            float damageMultiplier,
            float speedMultiplier,
            float attackSpeedMultiplier,
            float expMultiplier,
            bool makeWaveBoss = false)
        {
            waveLevel = Mathf.Max(1, wave);
            isWaveBoss = makeWaveBoss;

            maxHealth = Mathf.Round(maxHealth * healthMultiplier);
            currentHealth = maxHealth;
            attackDamage = Mathf.Round(attackDamage * damageMultiplier);
            moveSpeed *= speedMultiplier;
            attackSpeed *= attackSpeedMultiplier;
            expReward = Mathf.Max(10, Mathf.RoundToInt(expReward * expMultiplier));

            if (isWaveBoss)
            {
                enemyTypeName = $"[BOSS ĐỢT {wave}] {enemyTypeName}";
                maxHealth *= 2.4f;
                currentHealth = maxHealth;
                attackDamage *= 1.5f;
                expReward *= 3;
                transform.localScale *= 1.38f;
                baseColor = new Color(1f, 0.15f, 0.35f, 1f);
                if (_sr != null) _sr.color = baseColor;
            }

            UpdateOverheadHealthBar();
        }

        private void FixedUpdate()
        {
            if (_isDead) return;

            PlayerController player = PlayerController.Instance;
            if (player == null || player.IsDead)
            {
                _rb.linearVelocity = Vector2.zero;
                return;
            }

            Vector2 toPlayer = (Vector2)player.transform.position - _rb.position;
            float distance = toPlayer.magnitude;

            if (distance > 0.05f)
            {
                Vector2 dir = toPlayer / distance;
                _rb.linearVelocity = dir * moveSpeed;

                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0f, 0f, angle);
            }
            else
            {
                _rb.linearVelocity = Vector2.zero;
            }

            // Giữ thanh máu luôn nằm ngang phía trên kẻ địch
            if (_hpBarRoot != null)
            {
                _hpBarRoot.transform.rotation = Quaternion.identity;
                _hpBarRoot.transform.position = transform.position + new Vector3(0f, 0.55f * transform.localScale.y, 0f);
            }

            // Kiểm tra khoảng cách chạm trực tiếp để đảm bảo tấn công mượt mà
            float contactRange = (0.38f * transform.localScale.x) + 0.42f;
            if (distance <= contactRange)
            {
                TryAttackPlayer(player);
            }
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            if (_isDead) return;
            PlayerController player = collision.collider.GetComponent<PlayerController>();
            if (player != null)
            {
                TryAttackPlayer(player);
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (_isDead) return;
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                TryAttackPlayer(player);
            }
        }

        private void TryAttackPlayer(PlayerController player)
        {
            if (player == null || player.IsDead) return;

            if (Time.time >= _nextAttackTime)
            {
                float interval = 1f / Mathf.Max(0.1f, attackSpeed);
                _nextAttackTime = Time.time + interval;
                player.TakeDamage(attackDamage);
            }
        }

        public void TakeDamage(float amount)
        {
            if (_isDead) return;

            currentHealth -= amount;
            UpdateOverheadHealthBar();

            if (currentHealth > 0f)
            {
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayEnemyHit();
                }

                if (_flashRoutine != null) StopCoroutine(_flashRoutine);
                _flashRoutine = StartCoroutine(HitFlashRoutine());
            }
            else
            {
                Die();
            }
        }

        private IEnumerator HitFlashRoutine()
        {
            if (_sr != null)
            {
                _sr.color = Color.white;
                yield return new WaitForSeconds(0.06f);
                if (!_isDead && _sr != null)
                {
                    _sr.color = baseColor;
                }
            }
        }

        private void Die()
        {
            if (_isDead) return;
            _isDead = true;

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayEnemyDeath();
            }

            if (PlayerController.Instance != null && !PlayerController.Instance.IsDead)
            {
                PlayerController.Instance.AddExp(expReward);
            }

            if (SupplySpawner.Instance != null)
            {
                if (isWaveBoss)
                {
                    SupplyType bossReward = (SupplyType)Random.Range(0, 3); // Chắc chắn rơi nâng cấp Vũ Khí
                    SupplySpawner.Instance.SpawnSupplyAt(transform.position, bossReward);
                }
                else
                {
                    SupplySpawner.Instance.TryDropFromEnemy(transform.position);
                }
            }

            if (EnemySpawner.Instance != null)
            {
                EnemySpawner.Instance.NotifyEnemyKilled();
            }

            Destroy(gameObject);
        }

        private void CreateOverheadHealthBar()
        {
            _hpBarRoot = new GameObject("EnemyHPBar");
            _hpBarRoot.transform.SetParent(transform);
            _hpBarRoot.transform.localPosition = new Vector3(0f, 0.55f, 0f);

            GameObject bg = new GameObject("BG");
            bg.transform.SetParent(_hpBarRoot.transform, false);
            bg.transform.localScale = new Vector3(0.7f, 0.09f, 1f);
            SpriteRenderer bgSr = bg.AddComponent<SpriteRenderer>();
            bgSr.sprite = SpriteGenerator.GetWhiteSquareSprite();
            bgSr.color = new Color(0.1f, 0.1f, 0.1f, 0.85f);
            bgSr.sortingOrder = 6;

            GameObject fill = new GameObject("Fill");
            fill.transform.SetParent(_hpBarRoot.transform, false);
            fill.transform.localScale = new Vector3(0.66f, 0.06f, 1f);
            SpriteRenderer fillSr = fill.AddComponent<SpriteRenderer>();
            fillSr.sprite = SpriteGenerator.GetWhiteSquareSprite();
            fillSr.color = new Color(0.95f, 0.2f, 0.2f, 0.95f);
            fillSr.sortingOrder = 7;

            _hpBarFill = fill.transform;
            _hpBarRoot.SetActive(false); // Chỉ hiện khi bị mất máu
        }

        private void UpdateOverheadHealthBar()
        {
            if (_hpBarRoot == null || _hpBarFill == null) return;

            float ratio = Mathf.Clamp01(currentHealth / Mathf.Max(1f, maxHealth));
            _hpBarRoot.SetActive(ratio < 0.999f && ratio > 0f);
            _hpBarFill.localScale = new Vector3(0.66f * ratio, 0.06f, 1f);
            _hpBarFill.localPosition = new Vector3(-0.33f * (1f - ratio), 0f, 0f);
        }
    }
}

