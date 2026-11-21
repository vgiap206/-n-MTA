using UnityEngine;

namespace TopDownShooter
{
    /// <summary>
    /// Quản lý việc thả Vật Phẩm (Bình Máu, Hòm Vũ Khí Nâng Cấp Súng, Lõi Năng Lượng)
    /// xung quanh người chơi và rơi ra khi tiêu diệt kẻ địch.
    /// </summary>
    public class SupplySpawner : MonoBehaviour
    {
        public static SupplySpawner Instance { get; private set; }

        [Header("Cấu hình thả Vật Phẩm & Súng Mới")]
        [Tooltip("Thời gian giữa mỗi đợt vật phẩm mới (giây)")]
        public float spawnInterval = 8.0f;

        [Tooltip("Số lượng vật phẩm tối đa trên bản đồ cùng lúc")]
        public int maxActiveSupplies = 16;

        [Tooltip("Tỉ lệ rơi vật phẩm (Bình Máu / Hòm Vũ Khí / Lõi Năng Lượng) khi tiêu diệt kẻ địch")]
        [Range(0f, 1f)]
        public float enemyDropChance = 0.28f;

        public SupplyDrop supplyPrefab;

        private float _timer;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            SpawnInitialZoneSupplies();
        }

        private void Update()
        {
            if (PlayerController.Instance == null || PlayerController.Instance.IsDead)
                return;

            _timer += Time.deltaTime;
            if (_timer >= spawnInterval)
            {
                _timer = 0f;
                if (SupplyDrop.ActiveSupplies.Count < maxActiveSupplies)
                {
                    SpawnSupplyNearPlayer();
                }
            }
        }

        private void SpawnInitialZoneSupplies()
        {
            // Đặt sẵn Bình Máu, Lõi Năng Lượng và các Hòm Vũ Khí xung quanh nhân vật để dễ nhìn và nhặt ngay
            SpawnSupplyAt(new Vector2(-4.5f, 2.5f), SupplyType.MedicalSupply);         // Bình Máu Quân Y
            SpawnSupplyAt(new Vector2(4.5f, 2.5f), SupplyType.TacticalSupply);         // Lõi Năng Lượng
            SpawnWeaponCrateAt(new Vector2(0f, 5.5f), WeaponType.SMG_MP5);             // Súng Tiểu Liên MP5
            SpawnWeaponCrateAt(new Vector2(-6.5f, -4.0f), WeaponType.Shotgun_M1014);   // Súng Shotgun M1014
            SpawnWeaponCrateAt(new Vector2(6.5f, -4.0f), WeaponType.Rifle_AK47);       // Súng Trường AK-47
            SpawnWeaponCrateAt(new Vector2(-11f, 8f), WeaponType.Sniper_AWP);          // Súng Bắn Tỉa AWP
            SpawnWeaponCrateAt(new Vector2(11f, 8f), WeaponType.Minigun_M134);         // Súng Máy 6 Nòng M134
            SpawnWeaponCrateAt(new Vector2(0f, -9.5f), WeaponType.Plasma_Cannon);      // Pháo Điện Từ Plasma
            SpawnSupplyAt(new Vector2(8.5f, 0f), SupplyType.MedicalSupply);            // Bình Máu Quân Y
        }

        public void SpawnSupplyNearPlayer()
        {
            if (PlayerController.Instance == null) return;

            Vector2 playerPos = PlayerController.Instance.transform.position;
            float angle = Random.Range(0f, Mathf.PI * 2f);
            float dist = Random.Range(7f, 18f);
            Vector2 pos = playerPos + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * dist;

            float maxBound = WorldMapManager.Instance != null ? WorldMapManager.Instance.halfMapSize - 8f : 310f;
            pos.x = Mathf.Clamp(pos.x, -maxBound, maxBound);
            pos.y = Mathf.Clamp(pos.y, -maxBound, maxBound);

            SupplyType randomType = PickWeightedSupplyType();
            SpawnSupplyAt(pos, randomType);
        }

        public void TryDropFromEnemy(Vector3 enemyPosition)
        {
            if (SupplyDrop.ActiveSupplies.Count >= maxActiveSupplies + 6) return;

            if (Random.value <= enemyDropChance)
            {
                SupplyType type = PickWeightedSupplyType();
                SpawnSupplyAt(enemyPosition, type);
            }
        }

        private SupplyType PickWeightedSupplyType()
        {
            float r = Random.value;
            if (r < 0.34f) return SupplyType.MedicalSupply;     // Bình máu
            if (r < 0.56f) return SupplyType.WeaponMultiShot;   // Hòm vũ khí
            if (r < 0.72f) return SupplyType.WeaponPierce;      // Hòm vũ khí xuyên phá
            if (r < 0.86f) return SupplyType.WeaponRapidFire;   // Hòm vũ khí liên thanh
            return SupplyType.TacticalSupply;                   // Lõi năng lượng tốc chạy
        }

        public SupplyDrop SpawnWeaponCrateAt(Vector2 position, WeaponType weapon)
        {
            SupplyDrop drop = CreateDropInstance(position, $"WeaponCrate_{weapon}");
            drop.SetupWeaponCrate(weapon, 0f);
            return drop;
        }

        public SupplyDrop SpawnSupplyAt(Vector2 position, SupplyType type)
        {
            SupplyDrop drop = CreateDropInstance(position, $"Supply_{type}");
            drop.Setup(type);
            return drop;
        }

        private SupplyDrop CreateDropInstance(Vector2 position, string objectName)
        {
            SupplyDrop drop;
            if (supplyPrefab != null)
            {
                drop = Instantiate(supplyPrefab, position, Quaternion.identity);
                drop.name = objectName;
                drop.gameObject.SetActive(true);
            }
            else
            {
                GameObject obj = new GameObject(objectName);
                obj.transform.position = new Vector3(position.x, position.y, 0f);
                obj.AddComponent<SpriteRenderer>();
                obj.AddComponent<CircleCollider2D>();
                drop = obj.AddComponent<SupplyDrop>();
            }
            return drop;
        }
    }
}
