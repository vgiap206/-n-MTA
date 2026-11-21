using System.Collections.Generic;
using UnityEngine;

namespace TopDownShooter
{
    public enum SupplyType
    {
        WeaponMultiShot = 0, // Hòm Vũ Khí: Nâng cấp lên súng mới / Đạn chùm
        WeaponPierce = 1,    // Hòm Vũ Khí: Nâng cấp lên súng mới / Xuyên thấu
        WeaponRapidFire = 2, // Hòm Vũ Khí: Nâng cấp lên súng mới / Liên thanh
        MedicalSupply = 3,   // Bình Máu Quân Y: Hồi máu & tăng máu tối đa
        TacticalSupply = 4   // Lõi Năng Lượng Chiến Thuật: Tăng tốc chạy & cộng EXP
    }

    /// <summary>
    /// Vật Phẩm Tiếp Tế (Bình Máu, Hòm Vũ Khí Nâng Cấp Súng, Lõi Năng Lượng) xuất hiện trên bản đồ hoặc rơi ra từ kẻ địch.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(CircleCollider2D))]
    public class SupplyDrop : MonoBehaviour
    {
        public static readonly List<SupplyDrop> ActiveSupplies = new List<SupplyDrop>();

        [Header("Loại Tiếp Tế / Vật Phẩm Rơi")]
        public SupplyType supplyType = SupplyType.WeaponMultiShot;
        public bool hasSpecificWeapon = false;
        public WeaponType grantedWeapon = WeaponType.SMG_MP5;
        public string displayName = "HÒM VŨ KHÍ NÂNG CẤP SÚNG";
        public float lifeTime = 95f;

        private SpriteRenderer _sr;
        private Transform _glowRing;
        private Vector3 _baseScale = Vector3.one * 0.96f;
        private float _spawnTime;
        private bool _collected;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStaticList()
        {
            ActiveSupplies.Clear();
        }

        private void Awake()
        {
            _sr = GetComponent<SpriteRenderer>();
            _sr.sortingOrder = 4;

            CircleCollider2D col = GetComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius = 0.55f;

            _spawnTime = Time.time;
            CreateGlowIndicator();
            ConfigureVisualByType(supplyType);
        }

        private void OnEnable()
        {
            if (!ActiveSupplies.Contains(this))
            {
                ActiveSupplies.Add(this);
            }
        }

        private void OnDisable()
        {
            ActiveSupplies.Remove(this);
        }

        private void Start()
        {
            if (lifeTime > 0f)
            {
                Destroy(gameObject, lifeTime);
            }
        }

        public void Setup(SupplyType type)
        {
            supplyType = type;
            hasSpecificWeapon = false;
            ConfigureVisualByType(type);
        }

        public void SetupWeaponCrate(WeaponType weapon, float customLifeTime = 0f)
        {
            supplyType = SupplyType.WeaponMultiShot;
            hasSpecificWeapon = true;
            grantedWeapon = weapon;
            if (customLifeTime >= 0f)
            {
                lifeTime = customLifeTime;
            }
            ConfigureVisualByType(supplyType);
        }

        private void ConfigureVisualByType(SupplyType type)
        {
            if (_sr == null) _sr = GetComponent<SpriteRenderer>();

            Color ringColor;
            if (hasSpecificWeapon &&
                (type == SupplyType.WeaponMultiShot || type == SupplyType.WeaponPierce || type == SupplyType.WeaponRapidFire))
            {
                WeaponSpec spec = WeaponDatabase.GetSpec(grantedWeapon);
                displayName = $"SÚNG MỚI: {spec.weaponName.ToUpper()} ({spec.description})";
                _sr.sprite = SpriteGenerator.GetSupplyWeaponSprite();
                _sr.color = Color.white;
                ringColor = new Color(spec.bulletColor.r, spec.bulletColor.g, spec.bulletColor.b, 0.50f);
            }
            else
            {
                switch (type)
                {
                    case SupplyType.WeaponMultiShot:
                        displayName = "HÒM VŨ KHÍ: NÂNG CẤP SÚNG MỚI (+TIA ĐẠN & +CÔNG)";
                        _sr.sprite = SpriteGenerator.GetSupplyWeaponSprite();
                        _sr.color = Color.white;
                        ringColor = new Color(1f, 0.80f, 0.18f, 0.50f);
                        break;

                    case SupplyType.WeaponPierce:
                        displayName = "HÒM VŨ KHÍ: NÂNG CẤP SÚNG MỚI (+XUYÊN PHÁ & +CÔNG)";
                        _sr.sprite = SpriteGenerator.GetSupplyWeaponSprite();
                        _sr.color = Color.white;
                        ringColor = new Color(1f, 0.52f, 0.18f, 0.50f);
                        break;

                    case SupplyType.WeaponRapidFire:
                        displayName = "HÒM VŨ KHÍ: NÂNG CẤP SÚNG MỚI (+LIÊN THANH & +CÔNG)";
                        _sr.sprite = SpriteGenerator.GetSupplyWeaponSprite();
                        _sr.color = Color.white;
                        ringColor = new Color(0.25f, 0.90f, 1f, 0.50f);
                        break;

                    case SupplyType.MedicalSupply:
                        displayName = "BÌNH MÁU QUÂN Y (+HỒI MÁU & +MÁU TỐI ĐA)";
                        _sr.sprite = SpriteGenerator.GetSupplyHealthSprite();
                        _sr.color = Color.white;
                        ringColor = new Color(1f, 0.25f, 0.38f, 0.52f);
                        break;

                    case SupplyType.TacticalSupply:
                    default:
                        displayName = "LÕI NĂNG LƯỢNG CHIẾN THUẬT (+TỐC CHẠY & +EXP)";
                        _sr.sprite = SpriteGenerator.GetSupplyTacticalSprite();
                        _sr.color = Color.white;
                        ringColor = new Color(0.20f, 0.88f, 1f, 0.52f);
                        break;
                }
            }

            if (_glowRing != null)
            {
                SpriteRenderer glowSr = _glowRing.GetComponent<SpriteRenderer>();
                if (glowSr != null)
                {
                    glowSr.sprite = SpriteGenerator.GetItemAuraSprite();
                    glowSr.color = ringColor;
                }
            }
        }

        private void CreateGlowIndicator()
        {
            Transform existing = transform.Find("GlowBeacon");
            if (existing != null)
            {
                _glowRing = existing;
                SpriteRenderer existingSr = _glowRing.GetComponent<SpriteRenderer>();
                if (existingSr != null) existingSr.sprite = SpriteGenerator.GetItemAuraSprite();
                return;
            }

            GameObject glowObj = new GameObject("GlowBeacon");
            glowObj.transform.SetParent(transform, false);
            glowObj.transform.localScale = new Vector3(1.35f, 1.35f, 1f);
            SpriteRenderer glowSr = glowObj.AddComponent<SpriteRenderer>();
            glowSr.sprite = SpriteGenerator.GetItemAuraSprite();
            glowSr.color = new Color(1f, 0.85f, 0.2f, 0.45f);
            glowSr.sortingOrder = 2;
            _glowRing = glowObj.transform;
        }

        private void Update()
        {
            float t = (Time.time - _spawnTime) * 3.0f;
            float pulse = 1f + Mathf.Sin(t) * 0.07f;
            transform.localScale = _baseScale * pulse;

            if (_glowRing != null)
            {
                float ringPulse = 1.25f + Mathf.Cos(t * 1.2f) * 0.14f;
                _glowRing.localScale = new Vector3(ringPulse, ringPulse, 1f);
            }
        }

        public Color GetMinimapColor()
        {
            switch (supplyType)
            {
                case SupplyType.WeaponMultiShot:
                case SupplyType.WeaponPierce:
                    return new Color(1f, 0.88f, 0.15f, 1f); // Vàng cam (Hòm Vũ Khí)
                case SupplyType.WeaponRapidFire:
                    return new Color(0.2f, 0.95f, 1f, 1f);   // Xanh ngọc (Vũ Khí Liên Thanh)
                case SupplyType.MedicalSupply:
                    return new Color(1f, 0.28f, 0.38f, 1f);  // Đỏ hồng (Bình Máu)
                default:
                    return new Color(0.35f, 0.85f, 1f, 1f);  // Xanh sáng (Lõi Năng Lượng)
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_collected) return;

            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null && !player.IsDead)
            {
                _collected = true;
                player.ApplySupplyUpgrade(supplyType, displayName, hasSpecificWeapon, grantedWeapon);
                Destroy(gameObject);
            }
        }
    }
}
