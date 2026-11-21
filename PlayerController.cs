using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace TopDownShooter
{
    /// <summary>
    /// Điều khiển Nhân Vật Chính (Player):
    /// - Di chuyển tự do 4 hướng (WASD / Phím mũi tên), xoay người theo hướng chuột.
    /// - Chỉ số: Tốc độ di chuyển (moveSpeed), Máu (maxHealth/currentHealth), Tấn công (attackDamage),
    ///   Tốc độ bắn (fireRate), Cấp độ (level), Kinh nghiệm (currentExp/expToNextLevel).
    /// - Hệ thống Nâng Cấp Súng: Mỗi cấp nâng cấp vũ khí sẽ đổi sang một khẩu súng hoàn toàn khác nhau
    ///   (Súng Lục USP -> Tiểu Liên MP5 -> Shotgun M1014 -> Súng Trường AK-47 -> Bắn Tỉa AWP -> Súng Máy 6 Nòng M134 -> Pháo Plasma)
    ///   với hình dáng súng trên tay nhân vật, đường đạn, tốc bắn và sát thương riêng biệt.
    /// - Khi lên cấp: Tăng máu, tấn công và tốc độ bắn.
    /// - Khi chết: Phát âm thanh chết và tự động bắt đầu lại game.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(CircleCollider2D))]
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController Instance { get; private set; }

        [Header("Chỉ số nhân vật (Player Stats)")]
        [Tooltip("Tốc độ di chuyển của nhân vật")]
        public float moveSpeed = 5.8f;

        [Tooltip("Máu tối đa của nhân vật")]
        public float maxHealth = 100f;

        [Tooltip("Máu hiện tại của nhân vật")]
        public float currentHealth = 100f;

        [Tooltip("Chỉ số tấn công cơ bản")]
        public float attackDamage = 25f;

        [Tooltip("Tốc độ bắn cơ bản (số đợt đạn bắn ra mỗi giây)")]
        public float fireRate = 3.2f;

        [Tooltip("Cấp độ hiện tại của nhân vật")]
        public int level = 1;

        [Tooltip("Điểm kinh nghiệm (EXP) hiện tại")]
        public int currentExp = 0;

        [Tooltip("Điểm EXP cần để lên cấp tiếp theo")]
        public int expToNextLevel = 100;

        [Header("Tăng chỉ số khi lên cấp (Level Up Upgrades)")]
        [Tooltip("Lượng máu tối đa tăng thêm mỗi khi lên cấp")]
        public float healthBonusPerLevel = 25f;

        [Tooltip("Sát thương tấn công tăng thêm mỗi khi lên cấp")]
        public float attackBonusPerLevel = 8f;

        [Tooltip("Tốc độ bắn tăng thêm mỗi khi lên cấp")]
        public float fireRateBonusPerLevel = 0.45f;

        [Tooltip("Hệ số tăng EXP yêu cầu cho cấp tiếp theo")]
        public float expScalePerLevel = 1.3f;

        [Header("Hệ Thống Súng & Nâng Cấp Vũ Khí (Weapon Arsenal)")]
        [Tooltip("Khẩu súng hiện tại đang cầm trên tay")]
        public WeaponType currentWeapon = WeaponType.Pistol_USP;

        [Tooltip("Prefab viên đạn (nếu để trống sẽ tự tạo)")]
        public Bullet bulletPrefab;

        [Tooltip("Điểm xuất phát của viên đạn ở đầu súng")]
        public Transform firePoint;

        [Tooltip("Tốc độ bay cơ bản của viên đạn")]
        public float bulletSpeed = 16f;

        [Tooltip("Cấp độ vũ khí hiện tại")]
        public int weaponLevel = 1;

        [Tooltip("Số tia đạn cộng thêm khi đã đạt súng tối đa")]
        public int projectilesPerShot = 1;

        [Tooltip("Góc tỏa giữa các tia đạn (độ)")]
        public float spreadAngle = 7.5f;

        [Tooltip("Số mục tiêu xuyên qua cộng thêm")]
        public int bulletPierce = 0;

        [Tooltip("Hệ số kích thước viên đạn")]
        public float bulletScale = 1.0f;

        public bool IsDead => _isDead;

        public event Action OnStatsChanged;
        public event Action<int> OnLevelUp;
        public event Action<string> OnSupplyPickedUp;

        private Rigidbody2D _rb;
        private SpriteRenderer _sr;
        private Camera _mainCamera;
        private Vector2 _moveInput;
        private float _nextFireTime;
        private bool _isDead;
        private int _bonusProjectiles;
        private int _bonusPierce;
        private Color _originalColor = Color.white;
        private Coroutine _hitFlashCoroutine;

        private void Awake()
        {
            Instance = this;

            _rb = GetComponent<Rigidbody2D>();
            _rb.gravityScale = 0f;
            _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            _rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            _rb.sleepMode = RigidbodySleepMode2D.NeverSleep;

            CircleCollider2D col = GetComponent<CircleCollider2D>();
            col.radius = 0.36f;
            col.isTrigger = false;

            _sr = GetComponent<SpriteRenderer>();
            _sr.sortingOrder = 5;
            _originalColor = _sr.color;

            if (firePoint == null)
            {
                Transform existingFp = transform.Find("FirePoint");
                if (existingFp != null)
                {
                    firePoint = existingFp;
                }
                else
                {
                    GameObject fp = new GameObject("FirePoint");
                    fp.transform.SetParent(transform, false);
                    fp.transform.localPosition = new Vector3(0.58f, -0.04f, 0f);
                    firePoint = fp.transform;
                }
            }
            else
            {
                firePoint.localPosition = new Vector3(0.58f, -0.04f, 0f);
            }

            currentHealth = maxHealth;
            EquipWeapon(currentWeapon, false);
        }

        private void Start()
        {
            _mainCamera = Camera.main;

            if (WorldMapManager.Instance == null || SupplySpawner.Instance == null || MinimapController.Instance == null)
            {
                GameBootstrapper.EnsureSceneSetup();
            }

            NotifyStatsChanged();
        }

        private void Update()
        {
            if (_isDead) return;

            ReadMovementInput();
            RotateTowardsMouse();
            HandleAutoShoot();
        }

        private void FixedUpdate()
        {
            if (_isDead)
            {
                _rb.linearVelocity = Vector2.zero;
                return;
            }

            Vector2 moveDir = _moveInput.sqrMagnitude > 1f ? _moveInput.normalized : _moveInput;
            _rb.linearVelocity = moveDir * moveSpeed;
        }

        /// <summary>
        /// Trang bị khẩu súng mới: Cập nhật hình ảnh người lính cầm đúng khẩu súng đó và đồng bộ thông số bắn.
        /// </summary>
        public void EquipWeapon(WeaponType newWeapon, bool notify = true)
        {
            currentWeapon = newWeapon;
            WeaponSpec spec = WeaponDatabase.GetSpec(currentWeapon);

            projectilesPerShot = Mathf.Clamp(spec.projectilesPerShot + _bonusProjectiles, 1, 9);
            spreadAngle = spec.spreadAngle;
            bulletPierce = spec.pierceCount + _bonusPierce;
            bulletSpeed = spec.bulletSpeed;
            bulletScale = spec.bulletScale;

            if (_sr == null) _sr = GetComponent<SpriteRenderer>();
            if (_sr != null)
            {
                _sr.sprite = SpriteGenerator.GetPlayerSpriteForWeapon(currentWeapon);
            }

            if (notify)
            {
                NotifyStatsChanged();
            }
        }

        private void ReadMovementInput()
        {
            Vector2 input = Vector2.zero;

#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null)
            {
                if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) input.y += 1f;
                if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) input.y -= 1f;
                if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) input.x += 1f;
                if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) input.x -= 1f;
            }

            if (input == Vector2.zero && Gamepad.current != null)
            {
                input = Gamepad.current.leftStick.ReadValue();
            }
#elif ENABLE_LEGACY_INPUT_MANAGER
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");
#endif

            _moveInput = input.normalized;
        }

        private void RotateTowardsMouse()
        {
            if (_mainCamera == null) _mainCamera = Camera.main;
            if (_mainCamera == null) return;

            Vector2 mouseScreenPos = Vector2.zero;
            bool hasMouse = false;

#if ENABLE_INPUT_SYSTEM
            if (Mouse.current != null)
            {
                mouseScreenPos = Mouse.current.position.ReadValue();
                hasMouse = true;
            }
#elif ENABLE_LEGACY_INPUT_MANAGER
            mouseScreenPos = Input.mousePosition;
            hasMouse = true;
#endif

            if (!hasMouse) return;

            Vector3 mouseWorldPos = _mainCamera.ScreenToWorldPoint(new Vector3(
                mouseScreenPos.x,
                mouseScreenPos.y,
                Mathf.Abs(_mainCamera.transform.position.z)));

            Vector2 lookDir = (Vector2)mouseWorldPos - (Vector2)transform.position;
            if (lookDir.sqrMagnitude > 0.0001f)
            {
                float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0f, 0f, angle);
            }
        }

        private void HandleAutoShoot()
        {
            if (Time.time < _nextFireTime) return;

            WeaponSpec spec = WeaponDatabase.GetSpec(currentWeapon);
            float effectiveFireRate = Mathf.Max(0.2f, fireRate * spec.fireRateMultiplier);
            float interval = 1f / effectiveFireRate;
            _nextFireTime = Time.time + interval;
            ShootForward(spec);
        }

        private void ShootForward(WeaponSpec spec)
        {
            Vector3 spawnPos = firePoint != null
                ? firePoint.position
                : transform.position + transform.right * 0.55f;

            int count = Mathf.Clamp(spec.projectilesPerShot + _bonusProjectiles, 1, 9);
            float currentSpread = spec.spreadAngle > 0.1f ? spec.spreadAngle : 5f;
            float baseAngle = transform.eulerAngles.z;

            // Thêm độ giật nhẹ cho súng liên thanh (MP5, Minigun) để cảm giác bắn chân thực
            if (count == 1 && spec.spreadAngle > 0.1f)
            {
                baseAngle += UnityEngine.Random.Range(-spec.spreadAngle * 0.5f, spec.spreadAngle * 0.5f);
            }

            float totalSpread = (count - 1) * currentSpread;
            float startAngle = baseAngle - totalSpread * 0.5f;

            float finalDamage = attackDamage * spec.damageMultiplier;
            int finalPierce = spec.pierceCount + _bonusPierce;
            float finalSpeed = spec.bulletSpeed;
            float finalScale = spec.bulletScale;
            Color bulletColor = spec.bulletColor;

            for (int i = 0; i < count; i++)
            {
                float angle = count == 1 ? baseAngle : startAngle + i * currentSpread;
                Quaternion rot = Quaternion.Euler(0f, 0f, angle);

                Bullet bulletInstance;
                if (bulletPrefab != null)
                {
                    bulletInstance = Instantiate(bulletPrefab, spawnPos, rot);
                    bulletInstance.gameObject.SetActive(true);
                }
                else
                {
                    GameObject bulletObj = new GameObject("Bullet");
                    bulletObj.transform.SetPositionAndRotation(spawnPos, rot);
                    bulletInstance = bulletObj.AddComponent<Bullet>();
                }

                bulletInstance.Initialize(finalDamage, finalSpeed, finalPierce, finalScale, bulletColor);
            }

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayShoot();
            }
        }

        public string GetWeaponDescription()
        {
            WeaponSpec spec = WeaponDatabase.GetSpec(currentWeapon);
            int totalShots = Mathf.Clamp(spec.projectilesPerShot + _bonusProjectiles, 1, 9);
            int totalPierce = spec.pierceCount + _bonusPierce;
            string pierceInfo = totalPierce > 0 ? $", Xuyên {totalPierce}" : "";
            return $"{spec.weaponName} [Cấp {weaponLevel} | {totalShots} Tia{pierceInfo}]";
        }

        /// <summary>
        /// Áp dụng nâng cấp khi nhặt Hòm Tiếp Tế:
        /// Mỗi lần nhặt nâng cấp vũ khí sẽ đổi sang một khẩu súng mới mạnh hơn (hoặc khẩu súng đặc biệt trong nhà),
        /// đồng thời cộng thêm chỉ số chiến đấu.
        /// </summary>
        public void ApplySupplyUpgrade(
            SupplyType type,
            string supplyTitle,
            bool hasSpecificWeapon = false,
            WeaponType specificWeapon = WeaponType.Pistol_USP)
        {
            if (_isDead) return;

            string detailMsg;
            switch (type)
            {
                case SupplyType.WeaponMultiShot:
                case SupplyType.WeaponPierce:
                case SupplyType.WeaponRapidFire:
                {
                    weaponLevel++;
                    WeaponType nextGun = DetermineNextUpgradeWeapon(hasSpecificWeapon, specificWeapon);
                    bool changedGun = nextGun != currentWeapon;

                    if (type == SupplyType.WeaponMultiShot)
                    {
                        attackDamage += 5f;
                        if (!changedGun && _bonusProjectiles < 4) _bonusProjectiles++;
                    }
                    else if (type == SupplyType.WeaponPierce)
                    {
                        attackDamage += 6f;
                        if (!changedGun && _bonusPierce < 4) _bonusPierce++;
                    }
                    else
                    {
                        fireRate += 0.45f;
                        attackDamage += 6f;
                    }

                    EquipWeapon(nextGun, false);
                    WeaponSpec spec = WeaponDatabase.GetSpec(currentWeapon);
                    detailMsg = $"TRANG BỊ SÚNG MỚI (Cấp {weaponLevel}): {spec.weaponName.ToUpper()} — {spec.description}!";
                    break;
                }

                case SupplyType.MedicalSupply:
                {
                    maxHealth += 20f;
                    float healAmount = Mathf.Max(45f, maxHealth * 0.5f);
                    currentHealth = Mathf.Min(maxHealth, currentHealth + healAmount);
                    detailMsg = $"TIẾP TẾ QUÂN Y: HỒI +{Mathf.RoundToInt(healAmount)} MÁU & +20 MÁU TỐI ĐA";
                    break;
                }

                case SupplyType.TacticalSupply:
                default:
                {
                    moveSpeed = Mathf.Min(10.5f, moveSpeed + 0.45f);
                    int bonusExp = Mathf.Max(45, Mathf.RoundToInt(expToNextLevel * 0.45f));
                    AddExp(bonusExp);
                    detailMsg = $"TIẾP TẾ CHIẾN THUẬT: +TỐC ĐỘ CHẠY & +{bonusExp} EXP";
                    break;
                }
            }

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayPickup();
            }

            OnSupplyPickedUp?.Invoke(detailMsg);
            NotifyStatsChanged();
        }

        private WeaponType DetermineNextUpgradeWeapon(bool hasSpecificWeapon, WeaponType specificWeapon)
        {
            int currentTier = WeaponDatabase.GetTierIndex(currentWeapon);
            if (hasSpecificWeapon)
            {
                int targetTier = WeaponDatabase.GetTierIndex(specificWeapon);
                if (targetTier > currentTier)
                {
                    return specificWeapon;
                }
            }

            int nextTier = currentTier + 1;
            if (nextTier < WeaponDatabase.TotalWeaponTypes)
            {
                return WeaponDatabase.GetWeaponByTierIndex(nextTier);
            }

            // Nếu đã ở bậc súng tối thượng (Pháo Plasma) nhưng nhặt hòm vũ khí đặc biệt khác thì cho phép đổi hoặc cường hóa
            return hasSpecificWeapon ? specificWeapon : currentWeapon;
        }

        public void AddExp(int amount)
        {
            if (_isDead || amount <= 0) return;

            currentExp += amount;

            while (currentExp >= expToNextLevel)
            {
                currentExp -= expToNextLevel;
                LevelUp();
            }

            NotifyStatsChanged();
        }

        private void LevelUp()
        {
            level++;
            expToNextLevel = Mathf.Max(50, Mathf.RoundToInt(expToNextLevel * expScalePerLevel));

            // Khi người chơi lên cấp sẽ tăng máu, tấn công và tốc độ bắn
            maxHealth += healthBonusPerLevel;
            currentHealth = maxHealth;
            attackDamage += attackBonusPerLevel;
            fireRate += fireRateBonusPerLevel;

            OnLevelUp?.Invoke(level);
            NotifyStatsChanged();
        }

        public void TakeDamage(float damage)
        {
            if (_isDead || damage <= 0f) return;

            currentHealth = Mathf.Max(0f, currentHealth - damage);
            NotifyStatsChanged();

            if (currentHealth > 0f)
            {
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayPlayerHit();
                }

                if (_hitFlashCoroutine != null) StopCoroutine(_hitFlashCoroutine);
                _hitFlashCoroutine = StartCoroutine(DamageFlashRoutine());
            }
            else
            {
                Die();
            }
        }

        private IEnumerator DamageFlashRoutine()
        {
            if (_sr != null)
            {
                _sr.color = new Color(1f, 0.25f, 0.25f, 1f);
                yield return new WaitForSeconds(0.1f);
                if (!_isDead && _sr != null)
                {
                    _sr.color = _originalColor;
                }
            }
        }

        private void Die()
        {
            if (_isDead) return;
            _isDead = true;
            _rb.linearVelocity = Vector2.zero;

            if (_sr != null)
            {
                _sr.color = new Color(0.5f, 0.1f, 0.1f, 0.6f);
            }

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayPlayerDeath();
            }

            StartCoroutine(RestartGameAfterDelay(0.45f));
        }

        private IEnumerator RestartGameAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            MainMenuManager.SkipMenuOnNextReload = true;
            Scene activeScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(activeScene.name);
        }

        private void NotifyStatsChanged()
        {
            OnStatsChanged?.Invoke();
            if (UIManager.Instance != null)
            {
                UIManager.Instance.RefreshUI(this);
            }
        }
    }
}
