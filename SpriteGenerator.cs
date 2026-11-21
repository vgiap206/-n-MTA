using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace TopDownShooter
{
    /// <summary>
    /// Tạo các Sprite 2D chân thực theo góc nhìn từ trên xuống (Top-Down Shooter giống CS2D):
    /// - Nhân vật (Player): Chiến binh nhìn từ đỉnh đầu cầm 7 loại súng khác nhau (USP, MP5, Shotgun, AK-47, AWP, Minigun, Plasma).
    /// - Kẻ địch (Enemy): Zombie / Quái vật nhìn từ đỉnh đầu với đầu, vai, 2 cánh tay vươn về phía trước (+X).
    /// - Ngôi Nhà & Môi trường: Sàn gỗ nhà, tường gạch nhà, mái ngói đỏ, nền đất cát/đá tự nhiên, tường rêu, thùng gỗ chữ X, cây rừng.
    /// </summary>
    public static class SpriteGenerator
    {
        private static Sprite _playerSprite;
        private static Sprite _bulletSprite;
        private static Sprite _enemySprite;
        private static Sprite _whiteSquareSprite;
        private static Sprite _gridSprite;

        private static Sprite _centralFloorSprite;
        private static Sprite _forestFloorSprite;
        private static Sprite _desertFloorSprite;
        private static Sprite _baseFloorSprite;
        private static Sprite _roadFloorSprite;
        private static Sprite _waterFloorSprite;

        private static Sprite _treeSprite;
        private static Sprite _bushSprite;
        private static Sprite _rockSprite;
        private static Sprite _crateObstacleSprite;
        private static Sprite _wallBlockSprite;
        private static Sprite _houseFloorSprite;
        private static Sprite _houseRoofSprite;
        private static Sprite _brickWallSprite;

        private static Sprite _supplyWeaponSprite;
        private static Sprite _supplyHealthSprite;
        private static Sprite _supplyTacticalSprite;
        private static Sprite _itemAuraSprite;

        private static Sprite _retroGoldButtonSprite;
        private static Sprite _retroGoldSquareSprite;
        private static Sprite _retroPanelSprite;

        private static Sprite _explosiveBarrelSprite;
        private static Sprite _crashedPlaneSprite;
        private static Sprite _wreckedCarSprite;
        private static Sprite _brokenMotorcycleSprite;
        private static Sprite _hudPanelFrameSprite;
        private static Sprite _glossyBarSprite;

        private static readonly Dictionary<WeaponType, Sprite> _weaponPlayerSprites =
            new Dictionary<WeaponType, Sprite>();

        public static void ClearCache()
        {
            _playerSprite = null;
            _bulletSprite = null;
            _enemySprite = null;
            _whiteSquareSprite = null;
            _gridSprite = null;
            _centralFloorSprite = null;
            _forestFloorSprite = null;
            _desertFloorSprite = null;
            _baseFloorSprite = null;
            _roadFloorSprite = null;
            _waterFloorSprite = null;
            _treeSprite = null;
            _bushSprite = null;
            _rockSprite = null;
            _crateObstacleSprite = null;
            _wallBlockSprite = null;
            _houseFloorSprite = null;
            _houseRoofSprite = null;
            _brickWallSprite = null;
            _supplyWeaponSprite = null;
            _supplyHealthSprite = null;
            _supplyTacticalSprite = null;
            _itemAuraSprite = null;
            _retroGoldButtonSprite = null;
            _retroGoldSquareSprite = null;
            _retroPanelSprite = null;
            _explosiveBarrelSprite = null;
            _crashedPlaneSprite = null;
            _wreckedCarSprite = null;
            _brokenMotorcycleSprite = null;
            _hudPanelFrameSprite = null;
            _glossyBarSprite = null;
            _weaponPlayerSprites.Clear();
        }

        public static Sprite GetPlayerSprite()
        {
            if (_playerSprite == null)
                _playerSprite = CreateSpriteFromTexture(GeneratePlayerTexture(WeaponType.Pistol_USP), 80f);
            return _playerSprite;
        }

        public static Sprite GetPlayerSpriteForWeapon(WeaponType weaponType)
        {
            if (_weaponPlayerSprites.TryGetValue(weaponType, out Sprite cached) && cached != null)
            {
                return cached;
            }

            Sprite created = CreateSpriteFromTexture(GeneratePlayerTexture(weaponType), 80f);
            _weaponPlayerSprites[weaponType] = created;
            return created;
        }

        public static Sprite GetBulletSprite()
        {
            if (_bulletSprite == null)
                _bulletSprite = CreateSpriteFromTexture(GenerateBulletTexture(), 64f);
            return _bulletSprite;
        }

        public static Sprite GetEnemySprite()
        {
            if (_enemySprite == null)
                _enemySprite = CreateSpriteFromTexture(GenerateEnemyTexture(), 80f);
            return _enemySprite;
        }

        public static Sprite GetWhiteSquareSprite()
        {
            if (_whiteSquareSprite == null)
                _whiteSquareSprite = CreateSpriteFromTexture(GenerateWhiteSquareTexture(), 32f);
            return _whiteSquareSprite;
        }

        public static Sprite GetGridFloorSprite()
        {
            if (_gridSprite == null)
                _gridSprite = CreateSpriteFromTexture(GenerateGridTexture(), 64f);
            return _gridSprite;
        }

        public static Sprite GetCentralFloorSprite()
        {
            if (_centralFloorSprite == null)
                _centralFloorSprite = CreateSpriteFromTexture(GenerateGrassGridBackgroundTexture(), 64f);
            return _centralFloorSprite;
        }

        public static Sprite GetForestFloorSprite()
        {
            if (_forestFloorSprite == null)
                _forestFloorSprite = CreateSpriteFromTexture(GenerateNaturalGroundTexture(
                    new Color(0.28f, 0.46f, 0.22f, 1f),
                    new Color(0.34f, 0.53f, 0.26f, 1f),
                    new Color(0.22f, 0.39f, 0.18f, 1f),
                    false), 64f);
            return _forestFloorSprite;
        }

        public static Sprite GetDesertFloorSprite()
        {
            if (_desertFloorSprite == null)
                _desertFloorSprite = CreateSpriteFromTexture(GenerateNaturalGroundTexture(
                    new Color(0.72f, 0.60f, 0.36f, 1f),
                    new Color(0.78f, 0.66f, 0.41f, 1f),
                    new Color(0.64f, 0.52f, 0.30f, 1f),
                    false), 64f);
            return _desertFloorSprite;
        }

        public static Sprite GetBaseFloorSprite()
        {
            if (_baseFloorSprite == null)
                _baseFloorSprite = CreateSpriteFromTexture(GenerateNaturalGroundTexture(
                    new Color(0.45f, 0.47f, 0.44f, 1f),
                    new Color(0.51f, 0.53f, 0.50f, 1f),
                    new Color(0.38f, 0.40f, 0.37f, 1f),
                    true), 64f);
            return _baseFloorSprite;
        }

        public static Sprite GetRoadFloorSprite()
        {
            if (_roadFloorSprite == null)
                _roadFloorSprite = CreateSpriteFromTexture(GenerateRoadTexture(), 64f);
            return _roadFloorSprite;
        }

        public static Sprite GetWaterFloorSprite()
        {
            if (_waterFloorSprite == null)
                _waterFloorSprite = CreateSpriteFromTexture(GenerateNaturalGroundTexture(
                    new Color(0.16f, 0.42f, 0.58f, 1f),
                    new Color(0.22f, 0.50f, 0.66f, 1f),
                    new Color(0.12f, 0.34f, 0.48f, 1f),
                    false), 64f);
            return _waterFloorSprite;
        }

        public static Sprite GetTreeSprite()
        {
            if (_treeSprite == null)
                _treeSprite = CreateSpriteFromTexture(GenerateTreeTexture(), 64f);
            return _treeSprite;
        }

        public static Sprite GetBushSprite()
        {
            if (_bushSprite == null)
                _bushSprite = CreateSpriteFromTexture(GenerateBushTexture(), 64f);
            return _bushSprite;
        }

        public static Sprite GetRockSprite()
        {
            if (_rockSprite == null)
                _rockSprite = CreateSpriteFromTexture(GenerateRockTexture(), 64f);
            return _rockSprite;
        }

        public static Sprite GetCrateObstacleSprite()
        {
            if (_crateObstacleSprite == null)
                _crateObstacleSprite = CreateSpriteFromTexture(GenerateCrateObstacleTexture(), 64f);
            return _crateObstacleSprite;
        }

        public static Sprite GetPillarSprite()
        {
            if (_wallBlockSprite == null)
                _wallBlockSprite = CreateSpriteFromTexture(GenerateWallBlockTexture(), 64f);
            return _wallBlockSprite;
        }

        public static Sprite GetHouseFloorSprite()
        {
            if (_houseFloorSprite == null)
                _houseFloorSprite = CreateSpriteFromTexture(GenerateHouseFloorTexture(), 64f);
            return _houseFloorSprite;
        }

        public static Sprite GetHouseRoofSprite()
        {
            if (_houseRoofSprite == null)
                _houseRoofSprite = CreateSpriteFromTexture(GenerateHouseRoofTexture(), 64f);
            return _houseRoofSprite;
        }

        public static Sprite GetBrickWallSprite()
        {
            if (_brickWallSprite == null)
                _brickWallSprite = CreateSpriteFromTexture(GenerateBrickWallTexture(), 64f);
            return _brickWallSprite;
        }

        public static Sprite GetSupplyWeaponSprite()
        {
            if (_supplyWeaponSprite == null)
                _supplyWeaponSprite = CreateSpriteFromTexture(GenerateWeaponCrateItemTexture(), 64f);
            return _supplyWeaponSprite;
        }

        public static Sprite GetSupplyHealthSprite()
        {
            if (_supplyHealthSprite == null)
                _supplyHealthSprite = CreateSpriteFromTexture(GenerateHealthPotionTexture(), 64f);
            return _supplyHealthSprite;
        }

        public static Sprite GetSupplyTacticalSprite()
        {
            if (_supplyTacticalSprite == null)
                _supplyTacticalSprite = CreateSpriteFromTexture(GenerateTacticalItemTexture(), 64f);
            return _supplyTacticalSprite;
        }

        public static Sprite GetItemAuraSprite()
        {
            if (_itemAuraSprite == null)
                _itemAuraSprite = CreateSpriteFromTexture(GenerateItemAuraTexture(), 64f);
            return _itemAuraSprite;
        }

        /// <summary>
        /// Vẽ Chiến Binh Top-Down chân thực (96x96) cầm khẩu súng tương ứng với WeaponType:
        /// Mỗi loại súng (Lục USP, Tiểu Liên MP5, Shotgun M1014, Trường AK-47, Bắn Tỉa AWP, Súng Máy 6 Nòng M134, Pháo Plasma)
        /// có hình dáng, chiều dài nòng, màu sắc và chi tiết riêng trên tay nhân vật!
        /// </summary>
        public static Texture2D GeneratePlayerTexture(WeaponType weaponType = WeaponType.Rifle_AK47)
        {
            int size = 96;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;
            tex.wrapMode = TextureWrapMode.Clamp;

            Color clear = new Color(0, 0, 0, 0);
            Color shadow = new Color(0f, 0f, 0f, 0.32f);
            Color outline = new Color(0.08f, 0.10f, 0.12f, 1f);

            Color coatLight = new Color(0.82f, 0.85f, 0.88f, 1f);
            Color coatDark = new Color(0.55f, 0.60f, 0.66f, 1f);
            Color vestColor = new Color(0.24f, 0.28f, 0.32f, 1f);
            Color skinColor = new Color(0.92f, 0.74f, 0.58f, 1f);
            Color gloveColor = new Color(0.20f, 0.22f, 0.25f, 1f);
            Color helmetMain = new Color(0.52f, 0.18f, 0.16f, 1f);
            Color helmetHighlight = new Color(0.68f, 0.26f, 0.22f, 1f);
            Color gunMetal = new Color(0.18f, 0.20f, 0.23f, 1f);
            Color gunSteel = new Color(0.45f, 0.49f, 0.54f, 1f);
            Color gunWood = new Color(0.54f, 0.31f, 0.14f, 1f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    tex.SetPixel(x, y, clear);
                }
            }

            // 1. Bóng đổ dưới chân nhân vật
            DrawEllipse(tex, 40f, 45f, 22f, 26f, shadow);

            // 2. Ba lô chiến thuật sau lưng (Phía -X)
            DrawRoundedBox(tex, 16, 36, 28, 60, 4f, outline);
            DrawRoundedBox(tex, 18, 38, 27, 58, 3f, new Color(0.28f, 0.32f, 0.26f, 1f));

            // 3. Thân người & Hai Bờ Vai
            DrawEllipse(tex, 38f, 48f, 16f, 25f, outline);
            DrawEllipse(tex, 38f, 48f, 14f, 23f, coatDark);
            DrawEllipse(tex, 39f, 48f, 11.5f, 20f, coatLight);
            DrawEllipse(tex, 40f, 48f, 9f, 13f, vestColor);

            // 4. Hai cánh tay & Khẩu Súng tùy theo loại vũ khí đang cầm
            switch (weaponType)
            {
                case WeaponType.Pistol_USP:
                    DrawCapsule(tex, new Vector2(40f, 64f), new Vector2(56f, 49f), 5.6f, outline);
                    DrawCapsule(tex, new Vector2(40f, 64f), new Vector2(56f, 49f), 4.1f, coatLight);
                    DrawCapsule(tex, new Vector2(39f, 32f), new Vector2(56f, 44f), 5.6f, outline);
                    DrawCapsule(tex, new Vector2(39f, 32f), new Vector2(56f, 44f), 4.1f, coatLight);
                    DrawCircle(tex, 57f, 46f, 4.2f, gloveColor);
                    DrawRoundedBox(tex, 55, 43, 73, 49, 1f, outline);
                    DrawRoundedBox(tex, 56, 44, 71, 48, 1f, new Color(0.75f, 0.80f, 0.86f, 1f));
                    DrawRoundedBox(tex, 70, 45, 76, 47, 1f, gunMetal);
                    break;

                case WeaponType.SMG_MP5:
                    DrawCapsule(tex, new Vector2(40f, 65f), new Vector2(58f, 51f), 5.7f, outline);
                    DrawCapsule(tex, new Vector2(40f, 65f), new Vector2(58f, 51f), 4.2f, coatLight);
                    DrawCapsule(tex, new Vector2(39f, 31f), new Vector2(50f, 42f), 5.7f, outline);
                    DrawCapsule(tex, new Vector2(39f, 31f), new Vector2(50f, 42f), 4.2f, coatLight);
                    DrawCircle(tex, 59f, 50f, 4.0f, gloveColor);
                    DrawCircle(tex, 51f, 42f, 4.0f, gloveColor);
                    DrawRoundedBox(tex, 44, 41, 74, 49, 1f, gunMetal);
                    DrawRoundedBox(tex, 48, 43, 68, 47, 1f, new Color(0.28f, 0.34f, 0.42f, 1f));
                    DrawRoundedBox(tex, 58, 35, 63, 42, 1f, gunSteel);
                    DrawRoundedBox(tex, 73, 43, 84, 47, 1f, gunSteel);
                    break;

                case WeaponType.Shotgun_M1014:
                    DrawCapsule(tex, new Vector2(40f, 65f), new Vector2(61f, 51f), 5.8f, outline);
                    DrawCapsule(tex, new Vector2(40f, 65f), new Vector2(61f, 51f), 4.3f, coatLight);
                    DrawCapsule(tex, new Vector2(39f, 31f), new Vector2(50f, 41f), 5.8f, outline);
                    DrawCapsule(tex, new Vector2(39f, 31f), new Vector2(50f, 41f), 4.3f, coatLight);
                    DrawCircle(tex, 62f, 50f, 4.1f, gloveColor);
                    DrawCircle(tex, 51f, 42f, 4.0f, gloveColor);
                    DrawRoundedBox(tex, 38, 41, 52, 49, 1.5f, gunWood);
                    DrawRoundedBox(tex, 51, 40, 72, 50, 1.5f, gunMetal);
                    DrawRoundedBox(tex, 58, 41, 74, 49, 1.5f, gunWood);
                    DrawRoundedBox(tex, 72, 41, 88, 49, 1.5f, gunSteel);
                    break;

                case WeaponType.Sniper_AWP:
                    DrawCapsule(tex, new Vector2(40f, 65f), new Vector2(60f, 52f), 5.8f, outline);
                    DrawCapsule(tex, new Vector2(40f, 65f), new Vector2(60f, 52f), 4.3f, coatLight);
                    DrawCapsule(tex, new Vector2(39f, 31f), new Vector2(50f, 41f), 5.8f, outline);
                    DrawCapsule(tex, new Vector2(39f, 31f), new Vector2(50f, 41f), 4.3f, coatLight);
                    DrawCircle(tex, 61f, 51f, 4.0f, gloveColor);
                    DrawCircle(tex, 51f, 42f, 4.0f, gloveColor);
                    Color awpGreen = new Color(0.22f, 0.48f, 0.24f, 1f);
                    DrawRoundedBox(tex, 36, 41, 74, 49, 1.5f, awpGreen);
                    DrawRoundedBox(tex, 50, 42, 68, 48, 1.5f, gunMetal);
                    DrawCircle(tex, 68f, 45f, 3.5f, new Color(0.2f, 0.85f, 1f, 1f));
                    DrawRoundedBox(tex, 74, 43, 94, 47, 1f, gunMetal);
                    DrawRoundedBox(tex, 90, 41, 95, 49, 1f, gunSteel);
                    break;

                case WeaponType.Minigun_M134:
                    DrawCapsule(tex, new Vector2(40f, 66f), new Vector2(60f, 54f), 6.0f, outline);
                    DrawCapsule(tex, new Vector2(40f, 66f), new Vector2(60f, 54f), 4.5f, coatLight);
                    DrawCapsule(tex, new Vector2(39f, 30f), new Vector2(56f, 39f), 6.0f, outline);
                    DrawCapsule(tex, new Vector2(39f, 30f), new Vector2(56f, 39f), 4.5f, coatLight);
                    DrawCircle(tex, 62f, 53f, 4.2f, gloveColor);
                    DrawCircle(tex, 57f, 40f, 4.2f, gloveColor);
                    DrawRoundedBox(tex, 44, 37, 68, 53, 2f, gunMetal);
                    DrawRoundedBox(tex, 48, 39, 64, 51, 1.5f, new Color(0.85f, 0.68f, 0.18f, 1f));
                    DrawRoundedBox(tex, 67, 39, 91, 51, 1.5f, gunSteel);
                    DrawRoundedBox(tex, 68, 41, 91, 43, 1f, gunMetal);
                    DrawRoundedBox(tex, 68, 47, 91, 49, 1f, gunMetal);
                    DrawRoundedBox(tex, 84, 38, 89, 52, 1f, new Color(0.9f, 0.75f, 0.2f, 1f));
                    break;

                case WeaponType.Plasma_Cannon:
                    DrawCapsule(tex, new Vector2(40f, 65f), new Vector2(60f, 53f), 5.9f, outline);
                    DrawCapsule(tex, new Vector2(40f, 65f), new Vector2(60f, 53f), 4.4f, coatLight);
                    DrawCapsule(tex, new Vector2(39f, 31f), new Vector2(54f, 40f), 5.9f, outline);
                    DrawCapsule(tex, new Vector2(39f, 31f), new Vector2(54f, 40f), 4.4f, coatLight);
                    DrawCircle(tex, 61f, 52f, 4.1f, gloveColor);
                    DrawCircle(tex, 55f, 41f, 4.1f, gloveColor);
                    DrawRoundedBox(tex, 42, 38, 88, 52, 2f, new Color(0.18f, 0.22f, 0.34f, 1f));
                    DrawRoundedBox(tex, 46, 40, 84, 50, 1.5f, new Color(0.75f, 0.28f, 0.98f, 1f));
                    DrawRoundedBox(tex, 52, 43, 91, 47, 1f, new Color(0.25f, 0.95f, 1f, 1f));
                    DrawCircle(tex, 90f, 45f, 4.2f, new Color(0.45f, 0.98f, 1f, 1f));
                    break;

                case WeaponType.Rifle_AK47:
                default:
                    DrawCapsule(tex, new Vector2(40f, 65f), new Vector2(58f, 52f), 5.8f, outline);
                    DrawCapsule(tex, new Vector2(40f, 65f), new Vector2(58f, 52f), 4.3f, coatLight);
                    DrawCircle(tex, 60f, 51f, 4.0f, gloveColor);
                    DrawCapsule(tex, new Vector2(39f, 31f), new Vector2(50f, 41f), 5.8f, outline);
                    DrawCapsule(tex, new Vector2(39f, 31f), new Vector2(50f, 41f), 4.3f, coatLight);
                    DrawCircle(tex, 51f, 42f, 4.0f, gloveColor);
                    DrawRoundedBox(tex, 38, 42, 48, 48, 1.5f, gunWood);
                    DrawRoundedBox(tex, 47, 41, 68, 49, 1.5f, gunMetal);
                    DrawRoundedBox(tex, 50, 43, 64, 47, 1.0f, gunSteel);
                    DrawRoundedBox(tex, 66, 42, 78, 48, 1.5f, gunWood);
                    DrawRoundedBox(tex, 77, 43, 91, 47, 1.0f, gunMetal);
                    DrawRoundedBox(tex, 87, 42, 92, 48, 1.0f, gunSteel);
                    break;
            }

            // 5. Đầu & Mũ Chiến Thuật
            DrawCircle(tex, 41f, 48f, 12.5f, outline);
            DrawCircle(tex, 44f, 48f, 9.8f, skinColor);
            DrawCircle(tex, 39.5f, 48f, 10.5f, helmetMain);
            DrawCircle(tex, 38.0f, 50f, 6.5f, helmetHighlight);

            tex.Apply();
            return tex;
        }

        public static Texture2D GenerateEnemyTexture()
        {
            int size = 96;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;
            tex.wrapMode = TextureWrapMode.Clamp;

            Color clear = new Color(0, 0, 0, 0);
            Color shadow = new Color(0f, 0f, 0f, 0.34f);
            Color outline = new Color(0.09f, 0.11f, 0.09f, 1f);

            Color bodyDark = new Color(0.48f, 0.52f, 0.48f, 1f);
            Color bodyMid = new Color(0.72f, 0.76f, 0.72f, 1f);
            Color bodyBright = new Color(0.92f, 0.95f, 0.92f, 1f);
            Color clawSkin = new Color(0.82f, 0.78f, 0.70f, 1f);
            Color headDark = new Color(0.58f, 0.62f, 0.56f, 1f);
            Color eyeGlow = new Color(1.0f, 0.22f, 0.15f, 1f);
            Color bloodAccent = new Color(0.75f, 0.16f, 0.16f, 1f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    tex.SetPixel(x, y, clear);
                }
            }

            DrawEllipse(tex, 42f, 46f, 23f, 26f, shadow);

            DrawCapsule(tex, new Vector2(40f, 66f), new Vector2(68f, 61f), 6.2f, outline);
            DrawCapsule(tex, new Vector2(40f, 66f), new Vector2(68f, 61f), 4.6f, bodyMid);
            DrawCircle(tex, 72f, 60f, 4.5f, clawSkin);
            DrawCircle(tex, 75f, 58f, 2.0f, bloodAccent);

            DrawCapsule(tex, new Vector2(40f, 30f), new Vector2(68f, 35f), 6.2f, outline);
            DrawCapsule(tex, new Vector2(40f, 30f), new Vector2(68f, 35f), 4.6f, bodyMid);
            DrawCircle(tex, 72f, 36f, 4.5f, clawSkin);
            DrawCircle(tex, 75f, 38f, 2.0f, bloodAccent);

            DrawEllipse(tex, 38f, 48f, 16f, 26f, outline);
            DrawEllipse(tex, 38f, 48f, 14f, 24f, bodyDark);
            DrawEllipse(tex, 40f, 48f, 11f, 20f, bodyMid);
            DrawEllipse(tex, 36f, 48f, 7f, 14f, bodyBright);

            DrawCircle(tex, 42f, 58f, 3.2f, bloodAccent);
            DrawCircle(tex, 37f, 37f, 2.8f, bloodAccent);

            DrawCircle(tex, 46f, 48f, 12.0f, outline);
            DrawCircle(tex, 46f, 48f, 10.2f, headDark);
            DrawCircle(tex, 45f, 49f, 7.5f, bodyBright);

            DrawCircle(tex, 52.5f, 52f, 2.3f, eyeGlow);
            DrawCircle(tex, 52.5f, 44f, 2.3f, eyeGlow);

            tex.Apply();
            return tex;
        }

        private static void DrawCircle(Texture2D tex, float cx, float cy, float r, Color col)
        {
            DrawEllipse(tex, cx, cy, r, r, col);
        }

        private static void DrawEllipse(Texture2D tex, float cx, float cy, float rx, float ry, Color col)
        {
            int minX = Mathf.Max(0, Mathf.FloorToInt(cx - rx - 1));
            int maxX = Mathf.Min(tex.width - 1, Mathf.CeilToInt(cx + rx + 1));
            int minY = Mathf.Max(0, Mathf.FloorToInt(cy - ry - 1));
            int maxY = Mathf.Min(tex.height - 1, Mathf.CeilToInt(cy + ry + 1));

            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    float dx = (x - cx) / rx;
                    float dy = (y - cy) / ry;
                    float d = dx * dx + dy * dy;
                    if (d <= 1f)
                    {
                        BlendPixel(tex, x, y, col);
                    }
                }
            }
        }

        private static void DrawCapsule(Texture2D tex, Vector2 a, Vector2 b, float radius, Color col)
        {
            int minX = Mathf.Max(0, Mathf.FloorToInt(Mathf.Min(a.x, b.x) - radius - 1));
            int maxX = Mathf.Min(tex.width - 1, Mathf.CeilToInt(Mathf.Max(a.x, b.x) + radius + 1));
            int minY = Mathf.Max(0, Mathf.FloorToInt(Mathf.Min(a.y, b.y) - radius - 1));
            int maxY = Mathf.Min(tex.height - 1, Mathf.CeilToInt(Mathf.Max(a.y, b.y) + radius + 1));

            Vector2 ab = b - a;
            float abLenSq = Mathf.Max(0.0001f, ab.sqrMagnitude);

            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    Vector2 p = new Vector2(x, y);
                    float t = Mathf.Clamp01(Vector2.Dot(p - a, ab) / abLenSq);
                    Vector2 closest = a + t * ab;
                    if ((p - closest).sqrMagnitude <= radius * radius)
                    {
                        BlendPixel(tex, x, y, col);
                    }
                }
            }
        }

        private static void DrawRoundedBox(Texture2D tex, int x0, int y0, int x1, int y1, float feather, Color col)
        {
            for (int y = Mathf.Max(0, y0); y <= Mathf.Min(tex.height - 1, y1); y++)
            {
                for (int x = Mathf.Max(0, x0); x <= Mathf.Min(tex.width - 1, x1); x++)
                {
                    BlendPixel(tex, x, y, col);
                }
            }
        }

        private static void BlendPixel(Texture2D tex, int x, int y, Color src)
        {
            if (src.a >= 0.99f)
            {
                tex.SetPixel(x, y, src);
                return;
            }
            Color dst = tex.GetPixel(x, y);
            float outA = src.a + dst.a * (1f - src.a);
            if (outA <= 0.001f) return;
            Color res = (src * src.a + dst * dst.a * (1f - src.a)) / outA;
            res.a = outA;
            tex.SetPixel(x, y, res);
        }

        public static Texture2D GenerateBulletTexture()
        {
            int size = 32;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;
            tex.wrapMode = TextureWrapMode.Clamp;

            Color clear = new Color(0, 0, 0, 0);
            Color core = new Color(1f, 0.96f, 0.35f, 1f);
            Color glow = new Color(1f, 0.55f, 0.1f, 1f);

            Vector2 center = new Vector2(16f, 16f);
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = (x - center.x) / 12f;
                    float dy = (y - center.y) / 5f;
                    float d = Mathf.Sqrt(dx * dx + dy * dy);
                    if (d <= 1f)
                    {
                        tex.SetPixel(x, y, d < 0.6f ? core : glow);
                    }
                    else
                    {
                        tex.SetPixel(x, y, clear);
                    }
                }
            }

            tex.Apply();
            return tex;
        }

        public static Texture2D GenerateWhiteSquareTexture()
        {
            int size = 32;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            Color[] cols = new Color[size * size];
            for (int i = 0; i < cols.Length; i++) cols[i] = Color.white;
            tex.SetPixels(cols);
            tex.Apply();
            return tex;
        }

        public static Texture2D GenerateGridTexture()
        {
            return GenerateGrassGridBackgroundTexture();
        }

        /// <summary>
        /// Tạo nền Cỏ Xanh Kẻ Ô Vuông (Green Grass Grid Background) giống hệt ảnh mẫu ZombsRoyale / Surviv.io.
        /// </summary>
        public static Texture2D GenerateGrassGridBackgroundTexture()
        {
            int size = 64;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;
            tex.wrapMode = TextureWrapMode.Repeat;

            Color grassBase = new Color(0.357f, 0.565f, 0.243f, 1f);      // #5B903E xanh cỏ chủ đạo
            Color grassLight = new Color(0.373f, 0.584f, 0.255f, 1f);     // #5F9541 sắc cỏ sáng nhẹ
            Color subGridLine = new Color(0.325f, 0.525f, 0.216f, 1f);    // #538637 đường kẻ ô nhỏ (mỗi 16px)
            Color majorGridLine = new Color(0.286f, 0.475f, 0.184f, 1f);  // #49792F đường kẻ ô chính (mỗi 64px)

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    int hash = ((x * 374761393) + (y * 668265263)) ^ 0x5bf03635;
                    float noise = (Mathf.Abs(hash % 100) / 100f);

                    Color c = Color.Lerp(grassBase, grassLight, noise * 0.35f);

                    bool isMajorLine = (x == 0 || y == 0 || x == size - 1 || y == size - 1);
                    bool isSubLine = (x % 16 == 0 || y % 16 == 0);

                    if (isMajorLine)
                    {
                        c = majorGridLine;
                    }
                    else if (isSubLine)
                    {
                        c = subGridLine;
                    }

                    tex.SetPixel(x, y, c);
                }
            }

            tex.Apply();
            return tex;
        }

        public static Texture2D GenerateZoneTileTexture(Color primary, Color highlight, Color border, bool pavedGrid)
        {
            return GenerateNaturalGroundTexture(primary, highlight, border, pavedGrid);
        }

        public static Texture2D GenerateNaturalGroundTexture(Color primary, Color highlight, Color shadowTone, bool subtleTileSeams)
        {
            int size = 64;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;
            tex.wrapMode = TextureWrapMode.Repeat;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    int hash = ((x * 374761393) + (y * 668265263)) ^ 0x5bf03635;
                    float noise = (Mathf.Abs(hash % 1000) / 1000f) - 0.5f;
                    float wave = Mathf.Sin((x + y) * 0.19f) * 0.03f + Mathf.Cos((x - y) * 0.14f) * 0.025f;

                    Color c = Color.Lerp(primary, highlight, Mathf.Clamp01(0.5f + noise * 0.45f + wave));
                    if (noise < -0.32f)
                    {
                        c = Color.Lerp(c, shadowTone, 0.45f);
                    }

                    if (subtleTileSeams && (x == 0 || y == 0))
                    {
                        c = Color.Lerp(c, shadowTone, 0.35f);
                    }

                    c.a = 1f;
                    tex.SetPixel(x, y, c);
                }
            }

            tex.Apply();
            return tex;
        }

        public static Texture2D GenerateRoadTexture()
        {
            int size = 64;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;
            tex.wrapMode = TextureWrapMode.Repeat;

            Color road = new Color(0.54f, 0.50f, 0.38f, 1f);
            Color curb = new Color(0.44f, 0.40f, 0.30f, 1f);
            Color pebble = new Color(0.60f, 0.56f, 0.43f, 1f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    int hash = ((x * 19349663) ^ (y * 83492791)) & 0x7fffffff;
                    Color c = (hash % 7 == 0) ? pebble : road;
                    if (x < 3 || x >= size - 3 || y < 3 || y >= size - 3)
                    {
                        c = curb;
                    }
                    tex.SetPixel(x, y, c);
                }
            }

            tex.Apply();
            return tex;
        }

        public static Texture2D GenerateTreeTexture()
        {
            int size = 72;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;
            tex.wrapMode = TextureWrapMode.Clamp;

            Color clear = new Color(0f, 0f, 0f, 0f);
            Color darkOutline = new Color(0.11f, 0.16f, 0.07f, 1f);
            Color outerFoliage = new Color(0.24f, 0.37f, 0.14f, 1f);
            Color innerFoliage = new Color(0.32f, 0.48f, 0.19f, 1f);
            Color centerHighlight = new Color(0.39f, 0.56f, 0.24f, 1f);

            Vector2 center = new Vector2(36f, 36f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    Vector2 p = new Vector2(x, y);
                    float angle = Mathf.Atan2(y - center.y, x - center.x);
                    // Viền lượn sóng 8 múi tròn giống hệt cây trong ảnh mẫu ZombsRoyale
                    float scallop = Mathf.Cos(angle * 8f) * 2.4f;
                    float dist = Vector2.Distance(p, center) - scallop;

                    Color c = clear;
                    if (dist <= 30.5f)
                    {
                        if (dist >= 27.5f) c = darkOutline;
                        else if (dist >= 17.5f) c = outerFoliage;
                        else if (dist >= 15.8f) c = Color.Lerp(outerFoliage, darkOutline, 0.45f);
                        else if (dist >= 7.5f) c = innerFoliage;
                        else c = centerHighlight;
                    }

                    tex.SetPixel(x, y, c);
                }
            }

            tex.Apply();
            return tex;
        }

        public static Texture2D GenerateBushTexture()
        {
            int size = 64;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;
            tex.wrapMode = TextureWrapMode.Clamp;

            Color clear = new Color(0f, 0f, 0f, 0f);
            Color darkOutline = new Color(0.16f, 0.20f, 0.09f, 1f);
            Color oliveOuter = new Color(0.49f, 0.55f, 0.26f, 1f);
            Color oliveInner = new Color(0.60f, 0.66f, 0.34f, 1f);

            Vector2 center = new Vector2(32f, 32f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    Vector2 p = new Vector2(x, y);
                    float angle = Mathf.Atan2(y - center.y, x - center.x);
                    float scallop = Mathf.Cos(angle * 7f) * 2.1f;
                    float dist = Vector2.Distance(p, center) - scallop;

                    Color c = clear;
                    if (dist <= 26.5f)
                    {
                        if (dist >= 23.8f) c = darkOutline;
                        else if (dist >= 14.5f) c = oliveOuter;
                        else c = oliveInner;
                    }

                    tex.SetPixel(x, y, c);
                }
            }

            tex.Apply();
            return tex;
        }

        public static Texture2D GenerateRockTexture()
        {
            int size = 64;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;
            tex.wrapMode = TextureWrapMode.Clamp;

            Color clear = new Color(0, 0, 0, 0);
            Color outline = new Color(0.24f, 0.24f, 0.22f, 1f);
            Color rockBase = new Color(0.50f, 0.49f, 0.45f, 1f);
            Color rockLight = new Color(0.64f, 0.63f, 0.58f, 1f);

            Vector2 center = new Vector2(32f, 32f);
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = (x - center.x) / 24f;
                    float dy = (y - center.y) / 20f;
                    float d = Mathf.Sqrt(dx * dx + dy * dy);
                    if (d <= 1.0f)
                    {
                        Color c = d > 0.86f ? outline : (x < 30 && y > 32 ? rockLight : rockBase);
                        tex.SetPixel(x, y, c);
                    }
                    else
                    {
                        tex.SetPixel(x, y, clear);
                    }
                }
            }

            tex.Apply();
            return tex;
        }

        public static Texture2D GenerateCrateObstacleTexture()
        {
            int size = 64;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;
            tex.wrapMode = TextureWrapMode.Clamp;

            Color darkEdge = new Color(0.28f, 0.18f, 0.09f, 1f);
            Color frameWood = new Color(0.64f, 0.46f, 0.26f, 1f);
            Color innerWood = new Color(0.48f, 0.33f, 0.18f, 1f);
            Color plankShadow = new Color(0.36f, 0.24f, 0.12f, 1f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    bool isOuterBorder = x < 2 || x >= size - 2 || y < 2 || y >= size - 2;
                    bool isFrame = x < 8 || x >= size - 8 || y < 8 || y >= size - 8;
                    bool isDiagonalX = Mathf.Abs(x - y) <= 4 || Mathf.Abs(x - (size - 1 - y)) <= 4;
                    bool isVerticalPlank = (x % 8 == 0);

                    Color c = innerWood;
                    if (isOuterBorder) c = darkEdge;
                    else if (isFrame || isDiagonalX) c = frameWood;
                    else if (isVerticalPlank) c = plankShadow;

                    tex.SetPixel(x, y, c);
                }
            }

            tex.Apply();
            return tex;
        }

        public static Texture2D GeneratePillarTexture()
        {
            return GenerateWallBlockTexture();
        }

        public static Texture2D GenerateWallBlockTexture()
        {
            int size = 64;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;
            tex.wrapMode = TextureWrapMode.Clamp;

            Color outerBorder = new Color(0.18f, 0.24f, 0.08f, 1f);
            Color mossBase = new Color(0.34f, 0.42f, 0.14f, 1f);
            Color mossHighlight = new Color(0.42f, 0.50f, 0.18f, 1f);
            Color bevelDark = new Color(0.25f, 0.32f, 0.10f, 1f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    int hash = ((x * 73856093) ^ (y * 19349663)) & 0x7fffffff;
                    float n = (hash % 100) / 100f;

                    Color c = Color.Lerp(mossBase, mossHighlight, n * 0.6f);
                    if (x < 3 || x >= size - 3 || y < 3 || y >= size - 3)
                    {
                        c = outerBorder;
                    }
                    else if (x < 8 || y < 8)
                    {
                        c = bevelDark;
                    }
                    else if (x >= size - 8 || y >= size - 8)
                    {
                        c = mossHighlight;
                    }

                    tex.SetPixel(x, y, c);
                }
            }

            tex.Apply();
            return tex;
        }

        /// <summary>
        /// Sàn gỗ bên trong các Ngôi Nhà (House Interior Wooden Plank Floor).
        /// </summary>
        public static Texture2D GenerateHouseFloorTexture()
        {
            int size = 64;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;
            tex.wrapMode = TextureWrapMode.Repeat;

            Color woodMain = new Color(0.58f, 0.40f, 0.22f, 1f);
            Color woodLight = new Color(0.64f, 0.45f, 0.26f, 1f);
            Color seam = new Color(0.36f, 0.23f, 0.11f, 1f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    int plankRow = y / 16;
                    bool isHorizontalSeam = (y % 16 == 0);
                    bool isVerticalSeam = ((x + plankRow * 24) % 32 == 0);

                    Color c = (plankRow % 2 == 0) ? woodMain : woodLight;
                    if (isHorizontalSeam || isVerticalSeam)
                    {
                        c = seam;
                    }
                    tex.SetPixel(x, y, c);
                }
            }

            tex.Apply();
            return tex;
        }

        /// <summary>
        /// Mái nhà 4 mặt vát (Hipped Roof) viền đen sắc nét giống các ngôi nhà trong ảnh mẫu ZombsRoyale.
        /// </summary>
        public static Texture2D GenerateHouseRoofTexture()
        {
            int size = 96;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;
            tex.wrapMode = TextureWrapMode.Clamp;

            Color border = new Color(0.10f, 0.13f, 0.18f, 1f);
            Color roofTop = new Color(0.28f, 0.50f, 0.82f, 1f);
            Color roofBottom = new Color(0.20f, 0.38f, 0.66f, 1f);
            Color roofSide = new Color(0.24f, 0.44f, 0.74f, 1f);
            Color roofCenter = new Color(0.25f, 0.46f, 0.77f, 1f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    bool isOuter = (x < 3 || x >= size - 3 || y < 3 || y >= size - 3);
                    if (isOuter)
                    {
                        tex.SetPixel(x, y, border);
                        continue;
                    }

                    int dx = Mathf.Min(x, size - 1 - x);
                    int dy = Mathf.Min(y, size - 1 - y);
                    bool inCenterRect = (x >= 24 && x <= size - 25 && y >= 24 && y <= size - 25);
                    bool onCenterEdge = (x == 24 || x == size - 25 || y == 24 || y == size - 25) &&
                                        (x >= 24 && x <= size - 25 && y >= 24 && y <= size - 25);
                    bool onDiagBevel = !inCenterRect && Mathf.Abs(dx - dy) <= 1 && dx <= 24 && dy <= 24;

                    Color c;
                    if (onCenterEdge || onDiagBevel)
                    {
                        c = border;
                    }
                    else if (inCenterRect)
                    {
                        c = roofCenter;
                    }
                    else if (dy < dx)
                    {
                        c = (y > size / 2) ? roofTop : roofBottom;
                    }
                    else
                    {
                        c = roofSide;
                    }

                    tex.SetPixel(x, y, c);
                }
            }

            tex.Apply();
            return tex;
        }

        /// <summary>
        /// Tường gạch nâu đỏ bao quanh Ngôi Nhà (House Brick Wall).
        /// </summary>
        public static Texture2D GenerateBrickWallTexture()
        {
            int size = 64;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;
            tex.wrapMode = TextureWrapMode.Repeat;

            Color brick = new Color(0.62f, 0.27f, 0.18f, 1f);
            Color brickAlt = new Color(0.54f, 0.23f, 0.15f, 1f);
            Color mortar = new Color(0.32f, 0.16f, 0.11f, 1f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    int row = y / 16;
                    bool isMortar = (y % 16 < 2) || ((x + (row % 2) * 16) % 32 < 2) || x < 2 || x >= size - 2 || y < 2 || y >= size - 2;
                    Color c = isMortar ? mortar : ((row % 2 == 0) ? brick : brickAlt);
                    tex.SetPixel(x, y, c);
                }
            }

            tex.Apply();
            return tex;
        }

        /// <summary>
        /// Vẽ Bình Máu (Health Potion Flask) thuỷ tinh chứa dung dịch máu đỏ tươi, nút bần gỗ, cổ bình vàng và chữ thập trắng.
        /// </summary>
        public static Texture2D GenerateHealthPotionTexture()
        {
            int size = 72;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;
            tex.wrapMode = TextureWrapMode.Clamp;

            Color clear = new Color(0f, 0f, 0f, 0f);
            for (int i = 0; i < size * size; i++) tex.SetPixel(i % size, i / size, clear);

            Vector2 flaskCenter = new Vector2(36f, 27f);
            float outerRadius = 20.5f;
            float innerRadius = 17.5f;

            Color outlineDark = new Color(0.18f, 0.04f, 0.07f, 1f);
            Color glassRim = new Color(0.82f, 0.95f, 1.0f, 0.95f);
            Color glassEmpty = new Color(0.70f, 0.88f, 0.96f, 0.42f);
            Color liquidDeep = new Color(0.78f, 0.06f, 0.16f, 1f);
            Color liquidBright = new Color(1.0f, 0.20f, 0.32f, 1f);
            Color liquidSurface = new Color(1.0f, 0.55f, 0.62f, 1f);
            Color goldCollar = new Color(0.98f, 0.80f, 0.16f, 1f);
            Color corkColor = new Color(0.76f, 0.48f, 0.24f, 1f);
            Color corkShade = new Color(0.55f, 0.32f, 0.14f, 1f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    Vector2 p = new Vector2(x, y);

                    // 1. Bóng đổ mềm dưới đáy bình máu
                    float shadowDx = (x - 36f) / 18f;
                    float shadowDy = (y - 8f) / 5.5f;
                    if (shadowDx * shadowDx + shadowDy * shadowDy <= 1f)
                    {
                        float sa = (1f - (shadowDx * shadowDx + shadowDy * shadowDy)) * 0.40f;
                        tex.SetPixel(x, y, new Color(0f, 0f, 0f, sa));
                    }

                    // 2. Nút bần gỗ trên đỉnh (y: 53..63, x: 30..42)
                    if (y >= 53 && y <= 63 && x >= 30 && x <= 42)
                    {
                        bool corkBorder = (x == 30 || x == 42 || y == 63 || y == 53);
                        Color cc = corkBorder ? outlineDark : (x < 36 ? corkColor : corkShade);
                        tex.SetPixel(x, y, cc);
                        continue;
                    }

                    // 3. Cổ bình thuỷ tinh & Vòng kim loại vàng (y: 43..53, x: 28..44)
                    if (y >= 43 && y <= 53 && x >= 28 && x <= 44)
                    {
                        if (y >= 47 && y <= 50)
                        {
                            bool ringBorder = (x == 28 || x == 44);
                            tex.SetPixel(x, y, ringBorder ? outlineDark : goldCollar);
                            continue;
                        }
                        if (x >= 29 && x <= 43)
                        {
                            bool neckBorder = (x <= 30 || x >= 42);
                            bool neckRim = (x == 31 || x == 41);
                            tex.SetPixel(x, y, neckBorder ? outlineDark : (neckRim ? glassRim : glassEmpty));
                            continue;
                        }
                    }

                    // 4. Bầu bình thuỷ tinh tròn chứa máu đỏ
                    float dist = Vector2.Distance(p, flaskCenter);
                    if (dist <= outerRadius)
                    {
                        if (dist >= outerRadius - 2.0f)
                        {
                            tex.SetPixel(x, y, outlineDark);
                        }
                        else if (dist >= innerRadius)
                        {
                            tex.SetPixel(x, y, glassRim);
                        }
                        else
                        {
                            float wave = Mathf.Sin(x * 0.35f) * 1.2f;
                            float liquidTopY = 34.5f + wave;

                            Color insideColor;
                            if (y <= liquidTopY)
                            {
                                float t = Mathf.Clamp01((y - 10f) / 25f);
                                insideColor = Color.Lerp(liquidDeep, liquidBright, t);
                                if (Mathf.Abs(y - liquidTopY) <= 1.4f)
                                {
                                    insideColor = liquidSurface;
                                }
                            }
                            else
                            {
                                insideColor = glassEmpty;
                            }

                            // Biểu tượng Chữ Thập Trắng viền vàng nổi bật giữa thân bình máu
                            int dxCross = Mathf.Abs(x - 36);
                            int dyCross = Mathf.Abs(y - 25);
                            bool inCrossWhite = (dxCross <= 3 && dyCross <= 8) || (dyCross <= 3 && dxCross <= 8);
                            bool inCrossGoldBorder = !inCrossWhite && ((dxCross <= 4 && dyCross <= 9) || (dyCross <= 4 && dxCross <= 9));

                            if (inCrossWhite)
                            {
                                insideColor = Color.white;
                            }
                            else if (inCrossGoldBorder)
                            {
                                insideColor = goldCollar;
                            }

                            // Vệt phản quang thuỷ tinh cong bên trái bình
                            float glossDist = Vector2.Distance(p, new Vector2(25.5f, 32f));
                            if (glossDist <= 4.2f && x <= 28 && y >= 26)
                            {
                                insideColor = Color.Lerp(insideColor, Color.white, 0.78f);
                            }

                            tex.SetPixel(x, y, insideColor);
                        }
                    }
                }
            }

            tex.Apply();
            return tex;
        }

        /// <summary>
        /// Vẽ Hòm Vũ Khí Quân Sự (Weapon & Ammo Drop) với viền thép, sọc vàng-đen cảnh báo và biểu tượng Súng + Đạn Vàng.
        /// </summary>
        public static Texture2D GenerateWeaponCrateItemTexture()
        {
            int size = 72;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;
            tex.wrapMode = TextureWrapMode.Clamp;

            Color clear = new Color(0f, 0f, 0f, 0f);
            for (int i = 0; i < size * size; i++) tex.SetPixel(i % size, i / size, clear);

            Color outline = new Color(0.10f, 0.12f, 0.15f, 1f);
            Color steelFrame = new Color(0.26f, 0.30f, 0.36f, 1f);
            Color steelHighlight = new Color(0.46f, 0.52f, 0.60f, 1f);
            Color innerDark = new Color(0.15f, 0.18f, 0.22f, 1f);
            Color goldCorner = new Color(0.96f, 0.72f, 0.14f, 1f);
            Color bulletBrass = new Color(1.0f, 0.85f, 0.25f, 1f);
            Color bulletTip = new Color(0.95f, 0.45f, 0.15f, 1f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    // Bóng đổ dưới đáy hòm
                    if (x >= 10 && x <= 62 && y >= 9 && y <= 14)
                    {
                        tex.SetPixel(x, y, new Color(0f, 0f, 0f, 0.35f));
                    }

                    // Quai xách phía trên hòm (y: 55..61, x: 27..45)
                    if (y >= 55 && y <= 61 && x >= 27 && x <= 45)
                    {
                        bool isHandleBar = (y >= 58 || x <= 30 || x >= 42);
                        if (isHandleBar)
                        {
                            bool hBorder = (y == 61 || x == 27 || x == 45);
                            tex.SetPixel(x, y, hBorder ? outline : steelFrame);
                        }
                        continue;
                    }

                    // Thân hòm vũ khí (x: 10..62, y: 14..55)
                    if (x >= 10 && x <= 62 && y >= 14 && y <= 55)
                    {
                        int dx = Mathf.Min(x - 10, 62 - x);
                        int dy = Mathf.Min(y - 14, 55 - y);
                        if (dx + dy < 3) continue;

                        if (dx <= 1 || dy <= 1 || dx + dy == 3)
                        {
                            tex.SetPixel(x, y, outline);
                            continue;
                        }

                        // Đệm góc bằng đồng vàng ở 4 góc hòm
                        bool isCornerBumper = (dx <= 7 && dy <= 7);
                        if (isCornerBumper)
                        {
                            tex.SetPixel(x, y, goldCorner);
                            continue;
                        }

                        // Khung viền thép
                        if (dx <= 4 || dy <= 4)
                        {
                            tex.SetPixel(x, y, y > 48 ? steelHighlight : steelFrame);
                            continue;
                        }

                        // Dải sọc cảnh báo vàng - đen ở mép dưới hòm (y: 19..23)
                        if (y >= 19 && y <= 23)
                        {
                            bool yellowStripe = ((x + y) / 4) % 2 == 0;
                            tex.SetPixel(x, y, yellowStripe ? goldCorner : outline);
                            continue;
                        }

                        Color c = innerDark;

                        // Vẽ 3 Viên Đạn Đồng (Brass Bullets) xếp thẳng đứng nổi bật ở giữa hòm
                        for (int b = -1; b <= 1; b++)
                        {
                            int bx = 36 + b * 10;
                            int dxB = Mathf.Abs(x - bx);
                            if (dxB <= 3 && y >= 27 && y <= 41)
                            {
                                c = (x <= bx) ? bulletBrass : goldCorner;
                                if (y == 29) c = outline; // Rãnh vỏ đạn
                            }
                            else if (dxB <= 2 && y > 41 && y <= 46)
                            {
                                c = bulletTip;
                            }
                            else if (dxB <= 1 && y > 46 && y <= 49)
                            {
                                c = Color.white;
                            }
                        }

                        tex.SetPixel(x, y, c);
                    }
                }
            }

            tex.Apply();
            return tex;
        }

        /// <summary>
        /// Vẽ Bình Năng Lượng Chiến Thuật (Tactical Energy / Speed Booster) với lõi Plasma xanh ngọc và tia sét vàng.
        /// </summary>
        public static Texture2D GenerateTacticalItemTexture()
        {
            int size = 72;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;
            tex.wrapMode = TextureWrapMode.Clamp;

            Color clear = new Color(0f, 0f, 0f, 0f);
            for (int i = 0; i < size * size; i++) tex.SetPixel(i % size, i / size, clear);

            Color outline = new Color(0.06f, 0.14f, 0.24f, 1f);
            Color capSteel = new Color(0.32f, 0.44f, 0.58f, 1f);
            Color capLight = new Color(0.64f, 0.78f, 0.92f, 1f);
            Color plasmaDeep = new Color(0.05f, 0.58f, 0.95f, 1f);
            Color plasmaBright = new Color(0.35f, 0.95f, 1.0f, 1f);
            Color lightningGold = new Color(1.0f, 0.92f, 0.22f, 1f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    // Bóng đổ dưới chân bình năng lượng
                    float sdx = (x - 36f) / 16f;
                    float sdy = (y - 9f) / 5f;
                    if (sdx * sdx + sdy * sdy <= 1f)
                    {
                        tex.SetPixel(x, y, new Color(0f, 0f, 0f, 0.35f));
                    }

                    if (x >= 18 && x <= 54 && y >= 12 && y <= 60)
                    {
                        int dx = Mathf.Min(x - 18, 54 - x);
                        int dy = Mathf.Min(y - 12, 60 - y);
                        if (dx + dy < 4) continue;

                        if (dx <= 1 || dy <= 1 || dx + dy == 4)
                        {
                            tex.SetPixel(x, y, outline);
                            continue;
                        }

                        // Nắp kim loại trên & dưới của lõi năng lượng
                        if (y <= 19 || y >= 53)
                        {
                            tex.SetPixel(x, y, x < 36 ? capLight : capSteel);
                            continue;
                        }

                        // Buồng năng lượng Plasma xanh sáng
                        float radial = 1f - Mathf.Abs(x - 36f) / 18f;
                        Color c = Color.Lerp(plasmaDeep, plasmaBright, radial);

                        // Vẽ tia sét vàng (Lightning Bolt) ở tâm
                        float ly = y - 36f;
                        float targetX = (ly >= 0f) ? (33f + ly * 0.35f) : (39f + ly * 0.35f);
                        if (Mathf.Abs(ly) <= 12f && Mathf.Abs(x - targetX) <= 3.2f)
                        {
                            c = lightningGold;
                        }
                        else if (Mathf.Abs(ly) <= 2.2f && x >= 31 && x <= 41)
                        {
                            c = lightningGold;
                        }

                        // Vệt kính sáng dọc thân bình
                        if (x >= 22 && x <= 24 && y >= 22 && y <= 50)
                        {
                            c = Color.Lerp(c, Color.white, 0.65f);
                        }

                        tex.SetPixel(x, y, c);
                    }
                }
            }

            tex.Apply();
            return tex;
        }

        /// <summary>
        /// Vòng hào quang tròn mềm dưới chân vật phẩm rơi (thay thế hoàn toàn ô vuông cũ).
        /// </summary>
        public static Texture2D GenerateItemAuraTexture()
        {
            int size = 64;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;
            tex.wrapMode = TextureWrapMode.Clamp;

            Vector2 center = new Vector2(31.5f, 31.5f);
            float maxR = 30f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float d = Vector2.Distance(new Vector2(x, y), center) / maxR;
                    if (d >= 1f)
                    {
                        tex.SetPixel(x, y, new Color(1f, 1f, 1f, 0f));
                    }
                    else
                    {
                        float softGlow = Mathf.Pow(1f - d, 2.2f) * 0.55f;
                        float ring = Mathf.Exp(-Mathf.Pow((d - 0.76f) / 0.08f, 2f)) * 0.65f;
                        float alpha = Mathf.Clamp01(softGlow + ring);
                        tex.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
                    }
                }
            }

            tex.Apply();
            return tex;
        }

        public static Texture2D GenerateSupplyBoxTexture(Color mainColor, Color borderColor, Color emblemColor)
        {
            return GenerateWeaponCrateItemTexture();
        }

        public static Sprite GetRetroGoldButtonSprite()
        {
            if (_retroGoldButtonSprite == null)
                _retroGoldButtonSprite = CreateSpriteFromTexture(GenerateRetroGoldButtonTexture(), 64f);
            return _retroGoldButtonSprite;
        }

        public static Sprite GetRetroGoldSquareSprite()
        {
            if (_retroGoldSquareSprite == null)
                _retroGoldSquareSprite = CreateSpriteFromTexture(GenerateRetroGoldSquareTexture(), 64f);
            return _retroGoldSquareSprite;
        }

        public static Sprite GetRetroPanelSprite()
        {
            if (_retroPanelSprite == null)
                _retroPanelSprite = CreateSpriteFromTexture(GenerateRetroPanelTexture(), 64f);
            return _retroPanelSprite;
        }

        /// <summary>
        /// Tạo Sprite nút bấm dài màu vàng Pixel-Art (giống hệt nút START / NEW GAME / SETTINGS / EXIT trong ảnh mẫu).
        /// </summary>
        public static Texture2D GenerateRetroGoldButtonTexture()
        {
            int gw = 92;
            int gh = 28;
            int scale = 2;
            Texture2D tex = new Texture2D(gw * scale, gh * scale, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Point;
            tex.wrapMode = TextureWrapMode.Clamp;

            Color clear = new Color(0f, 0f, 0f, 0f);
            Color outerWhite = new Color(1.0f, 0.99f, 0.94f, 1f);
            Color darkBorder = new Color(0.24f, 0.16f, 0.04f, 1f);
            Color topHighlight = new Color(1.0f, 0.96f, 0.64f, 1f);
            Color pureHighlight = new Color(1.0f, 1.0f, 0.95f, 1f);
            Color goldMain = new Color(0.97f, 0.82f, 0.08f, 1f);
            Color goldSide = new Color(0.87f, 0.69f, 0.06f, 1f);
            Color bottomShadow = new Color(0.75f, 0.53f, 0.04f, 1f);

            for (int gy = 0; gy < gh; gy++)
            {
                for (int gx = 0; gx < gw; gx++)
                {
                    int dx = Mathf.Min(gx, gw - 1 - gx);
                    int dy = Mathf.Min(gy, gh - 1 - dy_helper(gy, gh));
                    int cornerDist = dx + dy;

                    Color c;
                    if (cornerDist < 3)
                    {
                        c = clear;
                    }
                    else if (dx == 0 || dy == 0 || cornerDist == 3)
                    {
                        c = outerWhite;
                    }
                    else if (dx == 1 || dy == 1 || cornerDist == 4)
                    {
                        c = darkBorder;
                    }
                    else if (gy >= gh - 4)
                    {
                        c = (gx >= 6 && gx <= gw - 7 && gy == gh - 3) ? pureHighlight : topHighlight;
                    }
                    else if (gy <= 4)
                    {
                        c = bottomShadow;
                    }
                    else if (dx <= 3)
                    {
                        c = goldSide;
                    }
                    else
                    {
                        c = goldMain;
                    }

                    for (int sy = 0; sy < scale; sy++)
                    {
                        for (int sx = 0; sx < scale; sx++)
                        {
                            tex.SetPixel(gx * scale + sx, gy * scale + sy, c);
                        }
                    }
                }
            }

            tex.Apply();
            return tex;
        }

        private static int dy_helper(int gy, int gh)
        {
            return gy;
        }

        /// <summary>
        /// Tạo Sprite nút vuông Pixel-Art màu vàng (giống các nút icon vuông bên phải trong ảnh mẫu).
        /// </summary>
        public static Texture2D GenerateRetroGoldSquareTexture()
        {
            int gs = 30;
            int scale = 2;
            Texture2D tex = new Texture2D(gs * scale, gs * scale, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Point;
            tex.wrapMode = TextureWrapMode.Clamp;

            Color clear = new Color(0f, 0f, 0f, 0f);
            Color outerWhite = new Color(1.0f, 0.99f, 0.94f, 1f);
            Color darkBorder = new Color(0.24f, 0.16f, 0.04f, 1f);
            Color topHighlight = new Color(1.0f, 0.96f, 0.64f, 1f);
            Color pureHighlight = new Color(1.0f, 1.0f, 0.95f, 1f);
            Color goldMain = new Color(0.97f, 0.82f, 0.08f, 1f);
            Color goldSide = new Color(0.87f, 0.69f, 0.06f, 1f);
            Color bottomShadow = new Color(0.75f, 0.53f, 0.04f, 1f);

            for (int gy = 0; gy < gs; gy++)
            {
                for (int gx = 0; gx < gs; gx++)
                {
                    int dx = Mathf.Min(gx, gs - 1 - gx);
                    int dy = Mathf.Min(gy, gs - 1 - gy);
                    int cornerDist = dx + dy;

                    Color c;
                    if (cornerDist < 3)
                    {
                        c = clear;
                    }
                    else if (dx == 0 || dy == 0 || cornerDist == 3)
                    {
                        c = outerWhite;
                    }
                    else if (dx == 1 || dy == 1 || cornerDist == 4)
                    {
                        c = darkBorder;
                    }
                    else if (gy >= gs - 4)
                    {
                        c = (gx >= 5 && gx <= gs - 6 && gy == gs - 3) ? pureHighlight : topHighlight;
                    }
                    else if (gy <= 4)
                    {
                        c = bottomShadow;
                    }
                    else if (dx <= 3)
                    {
                        c = goldSide;
                    }
                    else
                    {
                        c = goldMain;
                    }

                    for (int sy = 0; sy < scale; sy++)
                    {
                        for (int sx = 0; sx < scale; sx++)
                        {
                            tex.SetPixel(gx * scale + sx, gy * scale + sy, c);
                        }
                    }
                }
            }

            tex.Apply();
            return tex;
        }

        /// <summary>
        /// Tạo khung bảng Cài Đặt Pixel-Art viền vàng - nền tím than đậm giống ảnh mẫu.
        /// </summary>
        public static Texture2D GenerateRetroPanelTexture()
        {
            int gw = 96;
            int gh = 72;
            int scale = 2;
            Texture2D tex = new Texture2D(gw * scale, gh * scale, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Point;
            tex.wrapMode = TextureWrapMode.Clamp;

            Color clear = new Color(0f, 0f, 0f, 0f);
            Color outerWhite = new Color(1.0f, 0.99f, 0.94f, 1f);
            Color goldTrim = new Color(0.95f, 0.78f, 0.12f, 1f);
            Color darkTrim = new Color(0.24f, 0.16f, 0.05f, 1f);
            Color panelBg = new Color(0.21f, 0.19f, 0.25f, 0.98f);

            for (int gy = 0; gy < gh; gy++)
            {
                for (int gx = 0; gx < gw; gx++)
                {
                    int dx = Mathf.Min(gx, gw - 1 - gx);
                    int dy = Mathf.Min(gy, gh - 1 - gy);
                    int cornerDist = dx + dy;

                    Color c;
                    if (cornerDist < 3)
                    {
                        c = clear;
                    }
                    else if (dx == 0 || dy == 0 || cornerDist == 3)
                    {
                        c = outerWhite;
                    }
                    else if (dx <= 2 || dy <= 2 || cornerDist <= 5)
                    {
                        c = goldTrim;
                    }
                    else if (dx == 3 || dy == 3 || cornerDist == 6)
                    {
                        c = darkTrim;
                    }
                    else
                    {
                        c = panelBg;
                    }

                    for (int sy = 0; sy < scale; sy++)
                    {
                        for (int sx = 0; sx < scale; sx++)
                        {
                            tex.SetPixel(gx * scale + sx, gy * scale + sy, c);
                        }
                    }
                }
            }

            tex.Apply();
            return tex;
        }

        public static Sprite GetExplosiveBarrelSprite()
        {
            if (_explosiveBarrelSprite == null)
                _explosiveBarrelSprite = CreateSpriteFromTexture(GenerateExplosiveBarrelTexture(), 64f);
            return _explosiveBarrelSprite;
        }

        public static Sprite GetCrashedPlaneSprite()
        {
            if (_crashedPlaneSprite == null)
                _crashedPlaneSprite = CreateSpriteFromTexture(GenerateCrashedPlaneTexture(), 32f);
            return _crashedPlaneSprite;
        }

        public static Sprite GetWreckedCarSprite()
        {
            if (_wreckedCarSprite == null)
                _wreckedCarSprite = CreateSpriteFromTexture(GenerateWreckedCarTexture(), 42f);
            return _wreckedCarSprite;
        }

        public static Sprite GetBrokenMotorcycleSprite()
        {
            if (_brokenMotorcycleSprite == null)
                _brokenMotorcycleSprite = CreateSpriteFromTexture(GenerateBrokenMotorcycleTexture(), 48f);
            return _brokenMotorcycleSprite;
        }

        public static Sprite GetHudPanelFrameSprite()
        {
            if (_hudPanelFrameSprite == null)
                _hudPanelFrameSprite = CreateSpriteFromTexture(GenerateHudPanelFrameTexture(), 64f);
            return _hudPanelFrameSprite;
        }

        public static Sprite GetGlossyBarSprite()
        {
            if (_glossyBarSprite == null)
                _glossyBarSprite = CreateSpriteFromTexture(GenerateGlossyBarTexture(), 32f);
            return _glossyBarSprite;
        }

        /// <summary>
        /// Vẽ Bình Xăng Nổ (Explosive Fuel Barrel) màu đỏ tươi, đai thép đen, viền cảnh báo vàng và ký hiệu Lửa Nổ.
        /// </summary>
        public static Texture2D GenerateExplosiveBarrelTexture()
        {
            int size = 64;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;
            tex.wrapMode = TextureWrapMode.Clamp;

            Color clear = new Color(0f, 0f, 0f, 0f);
            Color outline = new Color(0.14f, 0.05f, 0.05f, 1f);
            Color steelRim = new Color(0.25f, 0.28f, 0.32f, 1f);
            Color redBody = new Color(0.88f, 0.16f, 0.16f, 1f);
            Color redHighlight = new Color(1.0f, 0.30f, 0.25f, 1f);
            Color hazardYellow = new Color(1.0f, 0.86f, 0.14f, 1f);
            Color flameOrange = new Color(1.0f, 0.52f, 0.08f, 1f);

            Vector2 center = new Vector2(32f, 32f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), center);
                    if (dist > 26.5f)
                    {
                        tex.SetPixel(x, y, clear);
                        continue;
                    }

                    if (dist >= 23.5f)
                    {
                        tex.SetPixel(x, y, outline);
                        continue;
                    }

                    if (dist >= 20.5f)
                    {
                        tex.SetPixel(x, y, steelRim);
                        continue;
                    }

                    if (dist >= 17.5f)
                    {
                        bool stripe = ((x + y) / 4) % 2 == 0;
                        tex.SetPixel(x, y, stripe ? hazardYellow : outline);
                        continue;
                    }

                    Color c = (x < 30 && y > 30) ? redHighlight : redBody;

                    // Nắp van bình xăng nhỏ góc trên bên phải
                    if (Vector2.Distance(new Vector2(x, y), new Vector2(40f, 40f)) <= 3.2f)
                    {
                        c = steelRim;
                    }

                    // Biểu tượng Ngọn Lửa / Tam Giác Cảnh Báo Nổ ở tâm bình xăng
                    float dy = y - 24f;
                    if (dy >= 0f && dy <= 15f)
                    {
                        float halfW = (15f - dy) * 0.58f;
                        if (Mathf.Abs(x - 32f) <= halfW)
                        {
                            c = (dy < 6f && Mathf.Abs(x - 32f) <= halfW * 0.55f) ? flameOrange : hazardYellow;
                        }
                    }

                    tex.SetPixel(x, y, c);
                }
            }

            tex.Apply();
            return tex;
        }

        /// <summary>
        /// Vẽ Máy Bay Hỏng (Crashed / Wrecked Airplane) nhìn từ trên xuống giống chiếc máy bay xanh trong ảnh mẫu,
        /// kèm vết nứt cánh, động cơ cháy đen và khói lửa.
        /// </summary>
        public static Texture2D GenerateCrashedPlaneTexture()
        {
            int size = 128;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;
            tex.wrapMode = TextureWrapMode.Clamp;

            Color clear = new Color(0f, 0f, 0f, 0f);
            for (int i = 0; i < size * size; i++) tex.SetPixel(i % size, i / size, clear);

            Color outline = new Color(0.10f, 0.12f, 0.18f, 1f);
            Color fuselageBlue = new Color(0.38f, 0.52f, 0.90f, 1f);
            Color fuselageLight = new Color(0.48f, 0.62f, 0.96f, 1f);
            Color wingBlue = new Color(0.32f, 0.45f, 0.80f, 1f);
            Color cockpitGlass = new Color(0.16f, 0.22f, 0.34f, 1f);
            Color scorchDark = new Color(0.14f, 0.13f, 0.12f, 1f);
            Color emberOrange = new Color(1.0f, 0.46f, 0.10f, 1f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = x - 64f;
                    float dy = y - 64f;

                    // 1. Hai cánh chính sải rộng (Main Wings)
                    float wingCenterY = 68f - Mathf.Abs(dx) * 0.24f;
                    float wingHalfThick = Mathf.Lerp(13f, 6.5f, Mathf.Abs(dx) / 56f);
                    bool inMainWing = Mathf.Abs(dx) <= 55f && Mathf.Abs(y - wingCenterY) <= wingHalfThick;

                    // Vết gãy nham nhở ở đầu cánh phải (máy bay bị hỏng)
                    if (dx > 40f && ((x + y * 2) % 9 < 3))
                    {
                        inMainWing = false;
                    }

                    // 2. Hai động cơ phản lực dưới cánh (Jet Engines)
                    bool inLeftEngine = Mathf.Abs(dx + 24f) <= 6.5f && y >= wingCenterY - 4f && y <= wingCenterY + 14f;
                    bool inRightEngine = Mathf.Abs(dx - 24f) <= 6.5f && y >= wingCenterY - 4f && y <= wingCenterY + 14f;

                    // 3. Cánh đuôi ngang phía sau (Tail Stabilizer)
                    float tailCenterY = 20f - Mathf.Abs(dx) * 0.20f;
                    bool inTailWing = Mathf.Abs(dx) <= 23f && Mathf.Abs(y - tailCenterY) <= 5.5f;

                    // 4. Thân máy bay (Fuselage) dọc từ đuôi y=10 tới mũi y=116
                    float bodyHalfWidth = 10.5f;
                    if (y > 92)
                    {
                        float tNose = (y - 92f) / 24f;
                        bodyHalfWidth = 10.5f * Mathf.Sqrt(Mathf.Clamp01(1f - tNose * tNose));
                    }
                    else if (y < 28)
                    {
                        float tTail = (28f - y) / 18f;
                        bodyHalfWidth = Mathf.Lerp(10.5f, 3.0f, Mathf.Clamp01(tTail));
                    }
                    bool inFuselage = (y >= 10 && y <= 116 && Mathf.Abs(dx) <= bodyHalfWidth);

                    if (!inMainWing && !inLeftEngine && !inRightEngine && !inTailWing && !inFuselage)
                    {
                        continue;
                    }

                    Color c = wingBlue;
                    if (inLeftEngine || inRightEngine)
                    {
                        c = new Color(0.26f, 0.36f, 0.66f, 1f);
                    }
                    if (inMainWing)
                    {
                        bool onWingEdge = Mathf.Abs(dx) >= 52f || Mathf.Abs(y - wingCenterY) >= wingHalfThick - 2.2f;
                        c = onWingEdge ? outline : wingBlue;
                    }
                    if (inTailWing)
                    {
                        bool onTailEdge = Mathf.Abs(dx) >= 20.5f || Mathf.Abs(y - tailCenterY) >= 3.5f;
                        c = onTailEdge ? outline : wingBlue;
                    }
                    if (inFuselage)
                    {
                        bool onBodyEdge = Mathf.Abs(dx) >= bodyHalfWidth - 2.2f || y <= 12 || y >= 114;
                        c = onBodyEdge ? outline : (dx < 0f ? fuselageLight : fuselageBlue);

                        // Kính buồng lái (Cockpit Windshield) ở mũi máy bay
                        if (y >= 96 && y <= 105 && Mathf.Abs(dx) <= 6.5f)
                        {
                            c = cockpitGlass;
                        }

                        // Sống đuôi đứng (Vertical Fin)
                        if (y >= 12 && y <= 28 && Mathf.Abs(dx) <= 2.2f)
                        {
                            c = outline;
                        }
                    }

                    // Vết cháy nổ & lửa âm ỉ trên động cơ trái và thân máy bay hỏng
                    float burnDist1 = Vector2.Distance(new Vector2(x, y), new Vector2(40f, 66f));
                    float burnDist2 = Vector2.Distance(new Vector2(x, y), new Vector2(74f, 56f));
                    if (burnDist1 <= 10.5f)
                    {
                        c = burnDist1 <= 4.2f ? emberOrange : Color.Lerp(c, scorchDark, 0.82f);
                    }
                    else if (burnDist2 <= 8.5f)
                    {
                        c = burnDist2 <= 3.0f ? emberOrange : Color.Lerp(c, scorchDark, 0.78f);
                    }

                    tex.SetPixel(x, y, c);
                }
            }

            tex.Apply();
            return tex;
        }

        /// <summary>
        /// Vẽ Ô Tô Hỏng (Wrecked Car) nhìn từ trên xuống với 4 bánh xe cao su, kính chắn gió nứt vỡ và nắp capo bốc khói đen.
        /// </summary>
        public static Texture2D GenerateWreckedCarTexture()
        {
            int w = 96;
            int h = 64;
            Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;
            tex.wrapMode = TextureWrapMode.Clamp;

            Color clear = new Color(0f, 0f, 0f, 0f);
            for (int i = 0; i < w * h; i++) tex.SetPixel(i % w, i / w, clear);

            Color outline = new Color(0.10f, 0.11f, 0.14f, 1f);
            Color tireBlack = new Color(0.14f, 0.14f, 0.16f, 1f);
            Color bodyMain = new Color(0.84f, 0.28f, 0.24f, 1f);
            Color bodyRoof = new Color(0.92f, 0.36f, 0.30f, 1f);
            Color glassDark = new Color(0.18f, 0.25f, 0.34f, 1f);
            Color glassCrack = new Color(0.75f, 0.88f, 0.96f, 1f);
            Color bumperSteel = new Color(0.55f, 0.58f, 0.62f, 1f);
            Color scorchHood = new Color(0.16f, 0.14f, 0.13f, 1f);

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    // 4 Bánh xe nhô nhẹ ở 2 bên sườn
                    bool inWheel = ((x >= 18 && x <= 32) || (x >= 64 && x <= 78)) &&
                                   ((y >= 7 && y <= 13) || (y >= 50 && y <= 56));
                    if (inWheel)
                    {
                        tex.SetPixel(x, y, tireBlack);
                        continue;
                    }

                    // Thân xe ô tô (x: 10..86, y: 11..52)
                    if (x >= 10 && x <= 86 && y >= 11 && y <= 52)
                    {
                        int dx = Mathf.Min(x - 10, 86 - x);
                        int dy = Mathf.Min(y - 11, 52 - y);
                        if (dx + dy < 5) continue;

                        if (dx <= 2 || dy <= 2 || dx + dy <= 6)
                        {
                            tex.SetPixel(x, y, outline);
                            continue;
                        }

                        // Cản trước & cản sau bằng thép
                        if (x <= 14 || x >= 82)
                        {
                            tex.SetPixel(x, y, bumperSteel);
                            continue;
                        }

                        Color c = bodyMain;

                        // Kính chắn gió trước (x: 56..66) và kính sau (x: 24..31)
                        if ((x >= 56 && x <= 66 && y >= 16 && y <= 47) ||
                            (x >= 24 && x <= 31 && y >= 17 && y <= 46))
                        {
                            c = glassDark;
                            // Vết rạn nứt trắng trên kính chắn gió trước
                            if (x >= 58 && x <= 64 && (Mathf.Abs((x - 60) - (y - 34)) <= 1 || Mathf.Abs((x - 61) + (y - 34)) <= 1))
                            {
                                c = glassCrack;
                            }
                        }
                        else if (x > 31 && x < 56 && y >= 16 && y <= 47)
                        {
                            // Nóc cabin ô tô
                            c = (x == 32 || x == 55 || y == 16 || y == 47) ? outline : bodyRoof;
                        }

                        // Vết cháy đen và móp méo ở nắp capo phía trước (x: 68..81)
                        if (x >= 68)
                        {
                            float burnD = Vector2.Distance(new Vector2(x, y), new Vector2(75f, 34f));
                            if (burnD <= 9.5f)
                            {
                                c = Color.Lerp(c, scorchHood, 0.85f);
                            }
                        }

                        tex.SetPixel(x, y, c);
                    }
                }
            }

            tex.Apply();
            return tex;
        }

        /// <summary>
        /// Vẽ Xe Máy Hỏng (Broken Motorcycle) nằm nghiêng trên mặt đất kèm vết dầu loang, 2 bánh xe, bình xăng và ghi-đông.
        /// </summary>
        public static Texture2D GenerateBrokenMotorcycleTexture()
        {
            int size = 64;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;
            tex.wrapMode = TextureWrapMode.Clamp;

            Color clear = new Color(0f, 0f, 0f, 0f);
            for (int i = 0; i < size * size; i++) tex.SetPixel(i % size, i / size, clear);

            Color oilSpill = new Color(0.08f, 0.09f, 0.11f, 0.55f);
            Color tireDark = new Color(0.12f, 0.13f, 0.15f, 1f);
            Color steelChrome = new Color(0.72f, 0.76f, 0.82f, 1f);
            Color seatLeather = new Color(0.20f, 0.18f, 0.18f, 1f);
            Color tankOrange = new Color(0.95f, 0.42f, 0.12f, 1f);
            Color outline = new Color(0.08f, 0.08f, 0.10f, 1f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    Vector2 p = new Vector2(x, y);

                    // Vệt dầu nhớt chảy dưới xe máy hỏng
                    if (Vector2.Distance(p, new Vector2(30f, 26f)) <= 13.5f)
                    {
                        tex.SetPixel(x, y, oilSpill);
                    }

                    // Bánh sau (x: 11..23, y: 26..38) & Bánh trước (x: 42..54, y: 26..38)
                    float dRearWheel = Vector2.Distance(p, new Vector2(16f, 32f));
                    float dFrontWheel = Vector2.Distance(p, new Vector2(48f, 32f));
                    if (dRearWheel <= 7.5f || dFrontWheel <= 7.5f)
                    {
                        bool rim = (dRearWheel <= 3.5f || dFrontWheel <= 3.5f);
                        tex.SetPixel(x, y, rim ? steelChrome : tireDark);
                    }

                    // Khung sườn, yên xe & bình xăng ở giữa (x: 18..45, y: 26..38)
                    if (x >= 18 && x <= 45 && Mathf.Abs(y - 32) <= 5)
                    {
                        if (Mathf.Abs(y - 32) == 5)
                        {
                            tex.SetPixel(x, y, outline);
                        }
                        else if (x <= 31)
                        {
                            tex.SetPixel(x, y, seatLeather); // Yên da đen
                        }
                        else if (x <= 41)
                        {
                            tex.SetPixel(x, y, tankOrange);  // Bình xăng cam đỏ
                        }
                        else
                        {
                            tex.SetPixel(x, y, steelChrome); // Cổ phuộc trước
                        }
                    }

                    // Tay lái ghi-đông ngang (x: 41..45, y: 19..45)
                    if (x >= 41 && x <= 44 && y >= 19 && y <= 45)
                    {
                        bool grip = (y <= 23 || y >= 41);
                        tex.SetPixel(x, y, grip ? tireDark : steelChrome);
                    }
                }
            }

            tex.Apply();
            return tex;
        }

        /// <summary>
        /// Khung HUD Kim Loại Viền Vàng Chiến Thuật (Dùng cho bảng Thanh Máu & EXP góc trên bên trái).
        /// </summary>
        public static Texture2D GenerateHudPanelFrameTexture()
        {
            int w = 128;
            int h = 64;
            Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Point;
            tex.wrapMode = TextureWrapMode.Clamp;

            Color clear = new Color(0f, 0f, 0f, 0f);
            Color outerDark = new Color(0.05f, 0.07f, 0.10f, 0.98f);
            Color goldRim = new Color(0.95f, 0.78f, 0.18f, 0.98f);
            Color innerBevel = new Color(0.20f, 0.25f, 0.34f, 0.96f);
            Color bgDark = new Color(0.09f, 0.12f, 0.17f, 0.94f);
            Color bgStripe = new Color(0.11f, 0.15f, 0.21f, 0.94f);

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    int dx = Mathf.Min(x, w - 1 - x);
                    int dy = Mathf.Min(y, h - 1 - y);
                    int corner = dx + dy;

                    if (corner < 4)
                    {
                        tex.SetPixel(x, y, clear);
                    }
                    else if (dx == 0 || dy == 0 || corner == 4)
                    {
                        tex.SetPixel(x, y, outerDark);
                    }
                    else if (dx <= 2 || dy <= 2 || corner <= 6)
                    {
                        tex.SetPixel(x, y, goldRim);
                    }
                    else if (dx == 3 || dy == 3 || corner == 7)
                    {
                        tex.SetPixel(x, y, innerBevel);
                    }
                    else
                    {
                        bool stripe = ((x + y) / 8) % 2 == 0;
                        tex.SetPixel(x, y, stripe ? bgDark : bgStripe);
                    }
                }
            }

            tex.Apply();
            return tex;
        }

        /// <summary>
        /// Tạo mặt cắt Thanh Máu / Thanh EXP bóng 3D (Glossy Bar Fill) giúp thanh máu nổi khối sắc nét.
        /// </summary>
        public static Texture2D GenerateGlossyBarTexture()
        {
            int w = 32;
            int h = 32;
            Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;
            tex.wrapMode = TextureWrapMode.Clamp;

            for (int y = 0; y < h; y++)
            {
                float ny = (float)y / (h - 1);
                float brightness;
                if (ny > 0.78f)
                    brightness = 1.0f; // Dải phản quang sáng trên đỉnh thanh máu
                else if (ny > 0.45f)
                    brightness = 0.92f;
                else
                    brightness = Mathf.Lerp(0.68f, 0.86f, ny / 0.45f); // Bóng đổ đậm ở đáy thanh máu

                Color c = new Color(brightness, brightness, brightness, 1f);
                for (int x = 0; x < w; x++)
                {
                    tex.SetPixel(x, y, c);
                }
            }

            tex.Apply();
            return tex;
        }

        private static Sprite CreateSpriteFromTexture(Texture2D tex, float pixelsPerUnit)
        {
            return Sprite.Create(
                tex,
                new Rect(0, 0, tex.width, tex.height),
                new Vector2(0.5f, 0.5f),
                pixelsPerUnit,
                0,
                SpriteMeshType.FullRect);
        }

        public static void SaveTextureAsPng(Texture2D tex, string filePath)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            byte[] pngData = tex.EncodeToPNG();
            File.WriteAllBytes(filePath, pngData);
        }
    }
}
