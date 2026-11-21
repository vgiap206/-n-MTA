using UnityEngine;

namespace TopDownShooter
{
    /// <summary>
    /// Danh sách 7 loại Súng khác nhau khi người chơi nhặt nâng cấp vũ khí:
    /// Mỗi loại súng có hình dáng riêng trên tay nhân vật, cơ chế đạn, tốc độ bắn, số tia và độ xuyên phá riêng.
    /// </summary>
    public enum WeaponType
    {
        Pistol_USP = 0,     // Cấp 1: Súng Lục Chiến Thuật USP
        SMG_MP5 = 1,        // Cấp 2: Súng Tiểu Liên MP5 (Bắn cực nhanh)
        Shotgun_M1014 = 2,  // Cấp 3: Súng Shotgun M1014 (Bắn chùm 5 tia rẻ quạt)
        Rifle_AK47 = 3,     // Cấp 4: Súng Trường Tấn Công AK-47 (2 tia sát thương lớn + xuyên 1)
        Sniper_AWP = 5,     // Cấp 5: Súng Bắn Tỉa AWP (Đạn siêu tốc, sát thương cực đại, xuyên 4 mục tiêu)
        Minigun_M134 = 6,   // Cấp 6: Súng Máy 6 Nòng M134 (Bão đạn 3 tia liên thanh)
        Plasma_Cannon = 7   // Cấp 7: Pháo Điện Từ Plasma (5 tia năng lượng xuyên 3 mục tiêu)
    }

    [System.Serializable]
    public struct WeaponSpec
    {
        public WeaponType type;
        public string weaponName;
        public string description;
        public float damageMultiplier;
        public float fireRateMultiplier;
        public int projectilesPerShot;
        public float spreadAngle;
        public int pierceCount;
        public float bulletSpeed;
        public float bulletScale;
        public Color bulletColor;
    }

    public static class WeaponDatabase
    {
        private static readonly WeaponType[] OrderedWeapons =
        {
            WeaponType.Pistol_USP,
            WeaponType.SMG_MP5,
            WeaponType.Shotgun_M1014,
            WeaponType.Rifle_AK47,
            WeaponType.Sniper_AWP,
            WeaponType.Minigun_M134,
            WeaponType.Plasma_Cannon
        };

        public static int TotalWeaponTypes => OrderedWeapons.Length;

        public static WeaponType GetWeaponByTierIndex(int index)
        {
            int clamped = Mathf.Clamp(index, 0, OrderedWeapons.Length - 1);
            return OrderedWeapons[clamped];
        }

        public static int GetTierIndex(WeaponType type)
        {
            for (int i = 0; i < OrderedWeapons.Length; i++)
            {
                if (OrderedWeapons[i] == type) return i;
            }
            return 0;
        }

        public static WeaponSpec GetSpec(WeaponType type)
        {
            switch (type)
            {
                case WeaponType.Pistol_USP:
                    return new WeaponSpec
                    {
                        type = WeaponType.Pistol_USP,
                        weaponName = "Súng Lục USP",
                        description = "Đơn tia chuẩn xác",
                        damageMultiplier = 1.0f,
                        fireRateMultiplier = 1.0f,
                        projectilesPerShot = 1,
                        spreadAngle = 0f,
                        pierceCount = 0,
                        bulletSpeed = 15f,
                        bulletScale = 0.95f,
                        bulletColor = new Color(1f, 0.95f, 0.45f, 1f)
                    };

                case WeaponType.SMG_MP5:
                    return new WeaponSpec
                    {
                        type = WeaponType.SMG_MP5,
                        weaponName = "Tiểu Liên MP5",
                        description = "Liên thanh tốc độ cao",
                        damageMultiplier = 0.95f,
                        fireRateMultiplier = 1.85f,
                        projectilesPerShot = 1,
                        spreadAngle = 4f,
                        pierceCount = 0,
                        bulletSpeed = 17.5f,
                        bulletScale = 0.9f,
                        bulletColor = new Color(1f, 0.82f, 0.25f, 1f)
                    };

                case WeaponType.Shotgun_M1014:
                    return new WeaponSpec
                    {
                        type = WeaponType.Shotgun_M1014,
                        weaponName = "Shotgun M1014",
                        description = "Đạn chùm 5 tia áp sát",
                        damageMultiplier = 0.85f,
                        fireRateMultiplier = 0.90f,
                        projectilesPerShot = 5,
                        spreadAngle = 7.5f,
                        pierceCount = 0,
                        bulletSpeed = 15.5f,
                        bulletScale = 1.05f,
                        bulletColor = new Color(1f, 0.58f, 0.18f, 1f)
                    };

                case WeaponType.Rifle_AK47:
                    return new WeaponSpec
                    {
                        type = WeaponType.Rifle_AK47,
                        weaponName = "Súng Trường AK-47",
                        description = "2 tia hạng nặng xuyên giáp",
                        damageMultiplier = 1.35f,
                        fireRateMultiplier = 1.35f,
                        projectilesPerShot = 2,
                        spreadAngle = 4.5f,
                        pierceCount = 1,
                        bulletSpeed = 18.5f,
                        bulletScale = 1.12f,
                        bulletColor = new Color(1f, 0.72f, 0.2f, 1f)
                    };

                case WeaponType.Sniper_AWP:
                    return new WeaponSpec
                    {
                        type = WeaponType.Sniper_AWP,
                        weaponName = "Bắn Tỉa AWP",
                        description = "Sát thương cực đại | Xuyên 4 mục tiêu",
                        damageMultiplier = 2.65f,
                        fireRateMultiplier = 0.85f,
                        projectilesPerShot = 1,
                        spreadAngle = 0f,
                        pierceCount = 4,
                        bulletSpeed = 26f,
                        bulletScale = 1.45f,
                        bulletColor = new Color(0.30f, 1f, 0.65f, 1f)
                    };

                case WeaponType.Minigun_M134:
                    return new WeaponSpec
                    {
                        type = WeaponType.Minigun_M134,
                        weaponName = "Súng Máy 6 Nòng M134",
                        description = "Bão đạn 3 tia siêu tốc | Xuyên 1",
                        damageMultiplier = 1.25f,
                        fireRateMultiplier = 2.15f,
                        projectilesPerShot = 3,
                        spreadAngle = 6.0f,
                        pierceCount = 1,
                        bulletSpeed = 20f,
                        bulletScale = 1.1f,
                        bulletColor = new Color(0.35f, 0.92f, 1f, 1f)
                    };

                case WeaponType.Plasma_Cannon:
                default:
                    return new WeaponSpec
                    {
                        type = WeaponType.Plasma_Cannon,
                        weaponName = "Pháo Điện Từ Plasma",
                        description = "5 Tia Plasma Tối Thượng | Xuyên 3",
                        damageMultiplier = 1.75f,
                        fireRateMultiplier = 1.65f,
                        projectilesPerShot = 5,
                        spreadAngle = 6.5f,
                        pierceCount = 3,
                        bulletSpeed = 22f,
                        bulletScale = 1.35f,
                        bulletColor = new Color(0.95f, 0.35f, 1f, 1f)
                    };
            }
        }
    }
}

