using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TopDownShooter
{
    /// <summary>
    /// Hệ thống sinh Kẻ Địch & Quản lý Đợt Tấn Công (Wave & Difficulty Scaling System):
    /// - Sinh ngẫu nhiên nhiều loại kẻ địch xung quanh ngoài tầm nhìn Camera của người chơi.
    /// - Chia theo từng Đợt (Wave 1, Wave 2, Wave 3...): sau mỗi đợt, quái tăng mạnh Máu, Tấn công,
    ///   Tốc độ di chuyển, Tốc độ đánh, EXP thưởng và xuất hiện nhiều quái tinh anh / Boss đợt hơn.
    /// - Kẻ địch spawn nhanh dần theo thời gian và cấp độ đợt.
    /// </summary>
    public class EnemySpawner : MonoBehaviour
    {
        public static EnemySpawner Instance { get; private set; }

        [Header("Hệ thống Đợt Tấn Công & Tăng Độ Khó (Wave System)")]
        [Tooltip("Đợt (Wave) hiện tại")]
        public int currentWave = 1;

        [Tooltip("Thời gian tối đa của mỗi đợt (giây) trước khi sang đợt mới khó hơn")]
        public float waveDuration = 22f;

        [Tooltip("Thời gian còn lại của đợt hiện tại")]
        public float waveTimeRemaining = 22f;

        [Tooltip("Số lượng quái đã tiêu diệt trong đợt hiện tại")]
        public int killsInCurrentWave = 0;

        [Tooltip("Số quái tiêu diệt để vượt đợt sớm")]
        public int targetKillsForWave = 10;

        [Header("Tăng chỉ số độ khó của Quái sau mỗi Đợt (Per-Wave Difficulty Scaling)")]
        [Tooltip("Tỉ lệ tăng Máu của quái sau mỗi đợt (+30% mỗi đợt)")]
        public float healthBonusPerWave = 0.30f;

        [Tooltip("Tỉ lệ tăng Tấn Công của quái sau mỗi đợt (+22% mỗi đợt)")]
        public float damageBonusPerWave = 0.22f;

        [Tooltip("Tỉ lệ tăng Tốc Độ Chạy của quái sau mỗi đợt (+8% mỗi đợt)")]
        public float speedBonusPerWave = 0.08f;

        [Tooltip("Tỉ lệ tăng Tốc Độ Đánh của quái sau mỗi đợt (+10% mỗi đợt)")]
        public float attackSpeedBonusPerWave = 0.10f;

        [Tooltip("Tỉ lệ tăng EXP thưởng của quái sau mỗi đợt (+25% mỗi đợt)")]
        public float expBonusPerWave = 0.25f;

        [Header("Cài đặt thời gian Spawn (Nhanh dần theo thời gian & theo Đợt)")]
        [Tooltip("Khoảng thời gian giữa 2 lần spawn lúc mới bắt đầu (giây)")]
        public float initialSpawnInterval = 1.75f;

        [Tooltip("Khoảng thời gian spawn ngắn nhất (tốc độ spawn tối đa)")]
        public float minSpawnInterval = 0.22f;

        [Tooltip("Tốc độ giảm thời gian chờ spawn mỗi giây trôi qua")]
        public float spawnAccelerationPerSecond = 0.015f;

        [Tooltip("Mức giảm thời gian chờ spawn thêm sau mỗi Đợt")]
        public float spawnReductionPerWave = 0.12f;

        [Tooltip("Khoảng thời gian chờ spawn hiện tại")]
        public float currentSpawnInterval = 1.75f;

        [Header("Khoảng cách Spawn ngoài tầm nhìn Camera")]
        [Tooltip("Khoảng cách tối thiểu tính từ mép ngoài màn hình Camera")]
        public float minOutsideMargin = 1.5f;

        [Tooltip("Khoảng cách tối đa tính từ mép ngoài màn hình Camera")]
        public float maxOutsideMargin = 3.5f;

        [Header("Danh sách các loại kẻ địch (Nhiều loại với chỉ số khác nhau)")]
        public List<EnemyTypeData> enemyTypes = new List<EnemyTypeData>();

        public event Action<int, string> OnWaveChanged;

        public float CurrentHealthMultiplier => 1f + (currentWave - 1) * healthBonusPerWave;
        public float CurrentDamageMultiplier => 1f + (currentWave - 1) * damageBonusPerWave;
        public float CurrentSpeedMultiplier => Mathf.Min(1.9f, 1f + (currentWave - 1) * speedBonusPerWave);
        public float CurrentAttackSpeedMultiplier => 1f + (currentWave - 1) * attackSpeedBonusPerWave;
        public float CurrentExpMultiplier => 1f + (currentWave - 1) * expBonusPerWave;

        private float _spawnTimer;
        private float _elapsedTime;
        private Camera _mainCamera;

        private void Reset()
        {
            EnsureDefaultEnemyTypes();
        }

        private void Awake()
        {
            Instance = this;
            EnsureDefaultEnemyTypes();
            currentSpawnInterval = initialSpawnInterval;
            waveTimeRemaining = waveDuration;
        }

        private void Start()
        {
            _mainCamera = Camera.main;
            _spawnTimer = 0.4f;
            StartWave(1, false);
        }

        public void EnsureDefaultEnemyTypes()
        {
            if (enemyTypes != null && enemyTypes.Count > 0) return;

            enemyTypes = new List<EnemyTypeData>
            {
                new EnemyTypeData
                {
                    enemyName = "Zombie Thường (Normal)",
                    moveSpeed = 2.4f,
                    maxHealth = 50f,
                    attackDamage = 10f,
                    attackSpeed = 1.0f,
                    spawnRate = 52f,
                    expReward = 30,
                    color = new Color(0.35f, 0.85f, 0.35f, 1f),
                    scale = 0.85f
                },
                new EnemyTypeData
                {
                    enemyName = "Quái Tốc Độ (Fast Runner)",
                    moveSpeed = 4.2f,
                    maxHealth = 32f,
                    attackDamage = 8f,
                    attackSpeed = 1.8f,
                    spawnRate = 28f,
                    expReward = 42,
                    color = new Color(1.0f, 0.65f, 0.15f, 1f),
                    scale = 0.72f
                },
                new EnemyTypeData
                {
                    enemyName = "Quái Khổng Lồ (Tank Brute)",
                    moveSpeed = 1.55f,
                    maxHealth = 145f,
                    attackDamage = 22f,
                    attackSpeed = 0.7f,
                    spawnRate = 14f,
                    expReward = 85,
                    color = new Color(0.68f, 0.32f, 0.92f, 1f),
                    scale = 1.25f
                },
                new EnemyTypeData
                {
                    enemyName = "Thủ Lĩnh Tinh Anh (Elite)",
                    moveSpeed = 2.85f,
                    maxHealth = 230f,
                    attackDamage = 32f,
                    attackSpeed = 1.2f,
                    spawnRate = 6f,
                    expReward = 155,
                    color = new Color(0.95f, 0.18f, 0.25f, 1f),
                    scale = 1.45f
                }
            };
        }

        private void Update()
        {
            if (PlayerController.Instance == null || PlayerController.Instance.IsDead)
                return;

            _elapsedTime += Time.deltaTime;
            waveTimeRemaining -= Time.deltaTime;

            if (waveTimeRemaining <= 0f || killsInCurrentWave >= targetKillsForWave)
            {
                StartWave(currentWave + 1, true);
            }

            // Kẻ địch spawn nhanh dần theo thời gian VÀ nhanh hơn sau mỗi Đợt
            float waveReduction = (currentWave - 1) * spawnReductionPerWave;
            currentSpawnInterval = Mathf.Max(
                minSpawnInterval,
                initialSpawnInterval - (_elapsedTime * spawnAccelerationPerSecond) - waveReduction);

            _spawnTimer -= Time.deltaTime;
            if (_spawnTimer <= 0f)
            {
                SpawnRandomEnemyOutsideView(false);
                _spawnTimer = currentSpawnInterval;
            }
        }

        public void NotifyEnemyKilled()
        {
            killsInCurrentWave++;
            if (UIManager.Instance != null && PlayerController.Instance != null)
            {
                UIManager.Instance.RefreshUI(PlayerController.Instance);
            }
        }

        public void StartWave(int newWave, bool announceBanner)
        {
            currentWave = Mathf.Max(1, newWave);
            waveTimeRemaining = waveDuration;
            killsInCurrentWave = 0;
            targetKillsForWave = 8 + currentWave * 4;

            // Sinh ngay một tốp quái đột kích khi bước sang đợt mới
            int burstCount = currentWave == 1 ? 3 : Mathf.Min(12, 2 + currentWave * 2);
            for (int i = 0; i < burstCount; i++)
            {
                SpawnRandomEnemyOutsideView(false);
            }

            // Từ Đợt 2 trở đi: xuất hiện thêm 1 Siêu Thủ Lĩnh Đợt (Wave Boss)
            if (currentWave >= 2)
            {
                SpawnRandomEnemyOutsideView(true);
            }

            // Thả thêm hòm tiếp tế hỗ trợ khi sang đợt mới
            if (currentWave >= 2 && SupplySpawner.Instance != null)
            {
                SupplySpawner.Instance.SpawnSupplyNearPlayer();
            }

            string summary = GetWaveDifficultySummary();
            OnWaveChanged?.Invoke(currentWave, summary);

            if (announceBanner && UIManager.Instance != null)
            {
                string bossNotice = currentWave >= 2 ? "\n[!] CẢNH BÁO: BOSS ĐỢT ĐÃ XUẤT HIỆN!" : "";
                UIManager.Instance.ShowBanner(
                    $"ĐỢT QUÁI THỨ {currentWave} BẮT ĐẦU!\nĐộ khó: Máu x{CurrentHealthMultiplier:0.00} | Công x{CurrentDamageMultiplier:0.00} | Tốc độ x{CurrentSpeedMultiplier:0.00}{bossNotice}",
                    new Color(1f, 0.52f, 0.18f, 1f),
                    3.0f);
            }

            if (UIManager.Instance != null && PlayerController.Instance != null)
            {
                UIManager.Instance.RefreshUI(PlayerController.Instance);
            }
        }

        public string GetWaveDifficultySummary()
        {
            return $"ĐỢT {currentWave} (Độ khó x{CurrentHealthMultiplier:0.0} | Hạ: {killsInCurrentWave}/{targetKillsForWave} | {Mathf.CeilToInt(Mathf.Max(0f, waveTimeRemaining))}s)";
        }

        private void SpawnRandomEnemyOutsideView(bool asWaveBoss)
        {
            EnemyTypeData selectedType = asWaveBoss
                ? enemyTypes[enemyTypes.Count - 1]
                : ChooseEnemyTypeBySpawnRate();
            if (selectedType == null) return;

            Vector3 spawnPos = GetRandomPositionOutsideCameraView();

            Enemy spawnedEnemy;
            if (selectedType.enemyPrefab != null)
            {
                spawnedEnemy = Instantiate(selectedType.enemyPrefab, spawnPos, Quaternion.identity);
                spawnedEnemy.gameObject.SetActive(true);
            }
            else
            {
                GameObject enemyObj = new GameObject($"Enemy_{selectedType.enemyName}");
                enemyObj.transform.position = spawnPos;
                spawnedEnemy = enemyObj.AddComponent<Enemy>();
            }

            spawnedEnemy.SetupFromData(selectedType);
            spawnedEnemy.ApplyWaveDifficulty(
                currentWave,
                CurrentHealthMultiplier,
                CurrentDamageMultiplier,
                CurrentSpeedMultiplier,
                CurrentAttackSpeedMultiplier,
                CurrentExpMultiplier,
                asWaveBoss);
        }

        private EnemyTypeData ChooseEnemyTypeBySpawnRate()
        {
            if (enemyTypes == null || enemyTypes.Count == 0) return null;

            float totalWeight = 0f;
            for (int i = 0; i < enemyTypes.Count; i++)
            {
                totalWeight += GetWaveAdjustedWeight(i, enemyTypes[i].spawnRate);
            }

            if (totalWeight <= 0f) return enemyTypes[0];

            float roll = Random.Range(0f, totalWeight);
            float cumulative = 0f;

            for (int i = 0; i < enemyTypes.Count; i++)
            {
                cumulative += GetWaveAdjustedWeight(i, enemyTypes[i].spawnRate);
                if (roll <= cumulative)
                {
                    return enemyTypes[i];
                }
            }

            return enemyTypes[enemyTypes.Count - 1];
        }

        /// <summary>
        /// Càng về các đợt sau, tỉ lệ xuất hiện của Quái Tốc Độ, Quái Khổng Lồ và Thủ Lĩnh Tinh Anh càng cao.
        /// </summary>
        private float GetWaveAdjustedWeight(int typeIndex, float baseWeight)
        {
            float w = Mathf.Max(0f, baseWeight);
            int waveBonus = Mathf.Max(0, currentWave - 1);
            switch (typeIndex)
            {
                case 0: // Zombie Thường
                    return w;
                case 1: // Quái Tốc Độ
                    return w + waveBonus * 4f;
                case 2: // Quái Khổng Lồ
                    return w + waveBonus * 5f;
                default: // Thủ Lĩnh Tinh Anh
                    return w + waveBonus * 3.5f;
            }
        }

        private Vector3 GetRandomPositionOutsideCameraView()
        {
            if (_mainCamera == null) _mainCamera = Camera.main;

            Vector3 center = PlayerController.Instance != null
                ? PlayerController.Instance.transform.position
                : Vector3.zero;

            float halfHeight = 5f;
            float halfWidth = 9f;

            if (_mainCamera != null && _mainCamera.orthographic)
            {
                center = _mainCamera.transform.position;
                halfHeight = _mainCamera.orthographicSize;
                halfWidth = halfHeight * _mainCamera.aspect;
            }

            float maxBound = WorldMapManager.Instance != null
                ? WorldMapManager.Instance.halfMapSize - 2.5f
                : 60f;

            for (int attempt = 0; attempt < 8; attempt++)
            {
                float margin = Random.Range(minOutsideMargin, maxOutsideMargin);
                int side = (attempt + Random.Range(0, 4)) % 4;

                float x = center.x;
                float y = center.y;

                switch (side)
                {
                    case 0:
                        x = center.x + Random.Range(-halfWidth - margin, halfWidth + margin);
                        y = center.y + halfHeight + margin;
                        break;
                    case 1:
                        x = center.x + Random.Range(-halfWidth - margin, halfWidth + margin);
                        y = center.y - halfHeight - margin;
                        break;
                    case 2:
                        x = center.x - halfWidth - margin;
                        y = center.y + Random.Range(-halfHeight - margin, halfHeight + margin);
                        break;
                    case 3:
                        x = center.x + halfWidth + margin;
                        y = center.y + Random.Range(-halfHeight - margin, halfHeight + margin);
                        break;
                }

                if (Mathf.Abs(x) <= maxBound && Mathf.Abs(y) <= maxBound)
                {
                    return new Vector3(x, y, 0f);
                }
            }

            float clampedX = Mathf.Clamp(center.x + Random.Range(-halfWidth - 3f, halfWidth + 3f), -maxBound, maxBound);
            float clampedY = Mathf.Clamp(center.y + Random.Range(-halfHeight - 3f, halfHeight + 3f), -maxBound, maxBound);
            return new Vector3(clampedX, clampedY, 0f);
        }
    }
}
