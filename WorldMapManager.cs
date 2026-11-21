using System.Collections.Generic;
using UnityEngine;

namespace TopDownShooter
{
    public enum MapZoneType
    {
        CentralHub,
        DeepForest,
        MilitaryBase,
        AncientRuins
    }

    /// <summary>
    /// Quản lý Bản Đồ Thế Giới Mở Rộng (640 x 640 đơn vị) với nền Cỏ Xanh Kẻ Ô Vuông (ZombsRoyale style)
    /// và đầy đủ hệ thống Chướng Ngại Vật:
    /// - Nhà cửa (Nhà mở cửa bước vào sàn gỗ + Nhà mái xanh/đỏ ZombsRoyale)
    /// - Cây cối, Bụi cây, Đá tảng, Thùng gỗ
    /// - Bình Xăng Nổ (ExplosiveBarrel - bắn vào phát nổ diệt quái diện rộng)
    /// - Máy Bay Hỏng (Crashed Airplane)
    /// - Ô Tô Hỏng (Wrecked Car)
    /// - Xe Máy Hỏng (Broken Motorcycle)
    /// </summary>
    public class WorldMapManager : MonoBehaviour
    {
        public static WorldMapManager Instance { get; private set; }

        [Header("Kích thước Bản Đồ Mở Rộng (World Bounds: 640 x 640)")]
        public float halfMapSize = 320f;
        public float centralHubHalfSize = 80f;
        public float roadWidth = 7.0f;

        [Header("Sprites Môi Trường & Chướng Ngại Vật")]
        public Sprite centralFloorSprite;
        public Sprite forestFloorSprite;
        public Sprite desertFloorSprite;
        public Sprite baseFloorSprite;
        public Sprite roadFloorSprite;
        public Sprite waterFloorSprite;
        public Sprite houseFloorSprite;
        public Sprite houseRoofSprite;
        public Sprite brickWallSprite;
        public Sprite treeSprite;
        public Sprite bushSprite;
        public Sprite rockSprite;
        public Sprite crateSprite;
        public Sprite pillarSprite;
        public Sprite explosiveBarrelSprite;
        public Sprite crashedPlaneSprite;
        public Sprite wreckedCarSprite;
        public Sprite brokenMotorcycleSprite;

        public readonly List<EnvironmentObstacle> Obstacles = new List<EnvironmentObstacle>();

        private void Awake()
        {
            Instance = this;
            if (halfMapSize < 250f) halfMapSize = 320f;
            SupplyDrop.ActiveSupplies.RemoveAll(s => s == null);
            Enemy.ActiveEnemies.RemoveAll(e => e == null);
            ExplosiveBarrel.ActiveBarrels.RemoveAll(b => b == null);
        }

        private void Start()
        {
            BuildWorldIfNeeded(false);
        }

        private void EnsureSpritesLoaded()
        {
            if (centralFloorSprite == null) centralFloorSprite = SpriteGenerator.GetCentralFloorSprite();
            if (houseFloorSprite == null) houseFloorSprite = SpriteGenerator.GetHouseFloorSprite();
            if (houseRoofSprite == null) houseRoofSprite = SpriteGenerator.GetHouseRoofSprite();
            if (brickWallSprite == null) brickWallSprite = SpriteGenerator.GetBrickWallSprite();
            if (treeSprite == null) treeSprite = SpriteGenerator.GetTreeSprite();
            if (bushSprite == null) bushSprite = SpriteGenerator.GetBushSprite();
            if (rockSprite == null) rockSprite = SpriteGenerator.GetRockSprite();
            if (crateSprite == null) crateSprite = SpriteGenerator.GetCrateObstacleSprite();
            if (pillarSprite == null) pillarSprite = SpriteGenerator.GetPillarSprite();
            if (explosiveBarrelSprite == null) explosiveBarrelSprite = SpriteGenerator.GetExplosiveBarrelSprite();
            if (crashedPlaneSprite == null) crashedPlaneSprite = SpriteGenerator.GetCrashedPlaneSprite();
            if (wreckedCarSprite == null) wreckedCarSprite = SpriteGenerator.GetWreckedCarSprite();
            if (brokenMotorcycleSprite == null) brokenMotorcycleSprite = SpriteGenerator.GetBrokenMotorcycleSprite();
        }

        public void BuildWorldIfNeeded(bool forceRebuild = false)
        {
            if (halfMapSize < 250f) halfMapSize = 320f;
            EnsureSpritesLoaded();
            Obstacles.Clear();

            Transform existingProps = transform.Find("WorldObstaclesRoot");
            EnvironmentObstacle[] existing = GetComponentsInChildren<EnvironmentObstacle>(true);

            if (!forceRebuild && existingProps != null && existing != null && existing.Length > 60)
            {
                Obstacles.AddRange(existing);
                return;
            }

            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                GameObject child = transform.GetChild(i).gameObject;
                if (Application.isPlaying)
                    Destroy(child);
                else
                    DestroyImmediate(child);
            }

            BuildWorldBoundaries();
            PopulateWorldObstacles();
        }

        private void BuildWorldBoundaries()
        {
            GameObject borderRoot = new GameObject("MapBoundaries");
            borderRoot.transform.SetParent(transform, false);

            float s = halfMapSize;
            float thickness = 3.0f;
            float length = s * 2f + thickness * 2f;

            CreateBoundaryCollider(borderRoot.transform, "Wall_North", new Vector2(0f, s + thickness * 0.5f), new Vector2(length, thickness));
            CreateBoundaryCollider(borderRoot.transform, "Wall_South", new Vector2(0f, -s - thickness * 0.5f), new Vector2(length, thickness));
            CreateBoundaryCollider(borderRoot.transform, "Wall_East", new Vector2(s + thickness * 0.5f, 0f), new Vector2(thickness, length));
            CreateBoundaryCollider(borderRoot.transform, "Wall_West", new Vector2(-s - thickness * 0.5f, 0f), new Vector2(thickness, length));
        }

        private void CreateBoundaryCollider(Transform parent, string name, Vector2 pos, Vector2 size)
        {
            GameObject wall = new GameObject(name);
            wall.transform.SetParent(parent, false);
            wall.transform.position = new Vector3(pos.x, pos.y, 0f);

            BoxCollider2D col = wall.AddComponent<BoxCollider2D>();
            col.size = size;
        }

        private void PopulateWorldObstacles()
        {
            GameObject root = new GameObject("WorldObstaclesRoot");
            root.transform.SetParent(transform, false);

            // --- A. KHU VỰC TRUNG TÂM GẦN NGƯỜI CHƠI (Bố cục giống ảnh mẫu ZombsRoyale để thấy ngay các chướng ngại vật) ---
            // 1. Ngôi Nhà Mái Xanh Dương lớn bên trái & Container Đỏ (giống ảnh mẫu)
            SpawnZombsRoofBuilding(root.transform, "BlueHouse_WestMain", new Vector2(-13.5f, 1.5f), new Vector2(6.5f, 11.5f), new Color(0.92f, 0.96f, 1f, 1f));
            SpawnZombsRoofBuilding(root.transform, "BlueHouse_NorthWest", new Vector2(-13.5f, 14.5f), new Vector2(7.0f, 6.5f), new Color(0.92f, 0.96f, 1f, 1f));
            SpawnZombsRoofBuilding(root.transform, "RedContainer_SouthWest", new Vector2(-7.2f, -4.5f), new Vector2(2.8f, 5.4f), new Color(1.0f, 0.48f, 0.44f, 1f));
            SpawnCrate(root.transform, new Vector2(-7.2f, 3.2f), 2.1f);

            // 2. Ngôi Nhà Có Cửa Vào (Sàn Gỗ + Tường Gạch) để người chơi bước vào ẩn nấp & nhặt súng
            SpawnEnterableHouse(root.transform, "EnterableHouse_NorthEast", new Vector2(16f, 16f), new Vector2(8f, 6.5f), 0);
            SpawnEnterableHouse(root.transform, "EnterableHouse_SouthEast", new Vector2(17f, -15f), new Vector2(8f, 6.5f), 1);

            // 3. Máy Bay Hỏng (Crashed Airplane) rơi ở phía Đông gần người chơi (giống ảnh mẫu)
            SpawnCrashedPlane(root.transform, new Vector2(10.5f, 3.5f), 28f, 1.55f);
            SpawnExplosiveBarrel(root.transform, new Vector2(6.5f, 5.5f));
            SpawnExplosiveBarrel(root.transform, new Vector2(13.8f, 0.8f));

            // 4. Ô Tô Hỏng & Xe Máy Hỏng xung quanh khu vực khởi đầu
            SpawnWreckedCar(root.transform, new Vector2(-4.5f, 9.5f), 18f, new Color(0.95f, 0.35f, 0.30f, 1f));    // Ô tô đỏ hỏng
            SpawnWreckedCar(root.transform, new Vector2(7.5f, -7.8f), -32f, new Color(0.96f, 0.80f, 0.22f, 1f));   // Taxi vàng hỏng
            SpawnBrokenMotorcycle(root.transform, new Vector2(-3.8f, -6.2f), 42f);
            SpawnBrokenMotorcycle(root.transform, new Vector2(5.2f, 8.8f), -55f);

            // 5. Các cụm Bình Xăng Nổ chiến thuật gần người chơi
            SpawnExplosiveBarrel(root.transform, new Vector2(-5.5f, 4.5f));
            SpawnExplosiveBarrel(root.transform, new Vector2(-4.4f, 5.1f));
            SpawnExplosiveBarrel(root.transform, new Vector2(4.2f, -4.5f));
            SpawnExplosiveBarrel(root.transform, new Vector2(-9.5f, -9.5f));

            // 6. Cây cối viền lượn sóng, bụi cây & đá tròn gần trung tâm (giống ảnh mẫu)
            SpawnTree(root.transform, new Vector2(-2.5f, 13.5f), 1.45f);
            SpawnTree(root.transform, new Vector2(3.5f, 13.0f), 1.35f);
            SpawnTree(root.transform, new Vector2(14.5f, 9.5f), 1.5f);
            SpawnTree(root.transform, new Vector2(11.5f, -3.5f), 1.4f);
            SpawnTree(root.transform, new Vector2(-1.5f, -11.5f), 1.45f);
            SpawnBush(root.transform, new Vector2(-7.5f, -0.5f), 1.35f);
            SpawnBush(root.transform, new Vector2(2.8f, 6.2f), 1.3f);
            SpawnBush(root.transform, new Vector2(13.2f, -7.2f), 1.4f);
            SpawnRock(root.transform, new Vector2(-6.5f, 12.2f), 1.25f);
            SpawnRock(root.transform, new Vector2(16.8f, 5.2f), 1.35f);
            SpawnRock(root.transform, new Vector2(15.5f, -4.8f), 1.2f);

            // --- B. PHÂN BỔ CHƯỚNG NGẠI VẬT TRÊN TOÀN BẢN ĐỒ RỘNG 640x640 ---
            System.Random rng = new System.Random(202611);
            Color[] carTints =
            {
                new Color(0.95f, 0.35f, 0.30f, 1f), // Đỏ
                new Color(0.98f, 0.82f, 0.22f, 1f), // Vàng Taxi
                new Color(0.35f, 0.65f, 0.96f, 1f), // Xanh Cảnh Sát
                new Color(0.52f, 0.68f, 0.38f, 1f), // Xanh Quân Đội
                new Color(0.85f, 0.52f, 0.25f, 1f)  // Cam Rỉ Sét
            };

            Color[] roofTints =
            {
                new Color(0.92f, 0.96f, 1.0f, 1f),  // Mái Xanh Dương ZombsRoyale
                new Color(1.0f, 0.52f, 0.46f, 1f),  // Mái Đỏ Cam
                new Color(0.65f, 0.88f, 0.62f, 1f), // Mái Xanh Lá
                new Color(0.96f, 0.84f, 0.52f, 1f)  // Mái Vàng Cát
            };

            // 1. 45 Ngôi Nhà (Cả nhà mở cửa bước vào và nhà mái xanh/đỏ) khắp bản đồ 640x640
            for (int i = 0; i < 45; i++)
            {
                Vector2 pos = RandomMapPos(rng, 26f, halfMapSize - 22f);
                if (IsTooCloseToObstacle(pos, 14f)) continue;

                if (i % 3 == 0)
                {
                    int doorSide = i % 4;
                    SpawnEnterableHouse(root.transform, $"EnterableHouse_{i}", pos, new Vector2(8.0f, 6.5f), doorSide);
                }
                else
                {
                    bool vertical = (i % 2 == 0);
                    Vector2 houseSize = vertical ? new Vector2(6.2f, 10.5f) : new Vector2(10.5f, 6.2f);
                    Color tint = roofTints[i % roofTints.Length];
                    SpawnZombsRoofBuilding(root.transform, $"RoofBuilding_{i}", pos, houseSize, tint);
                }
            }

            // 2. 18 Hiện trường Máy Bay Hỏng (Crashed Airplanes) trên khắp bản đồ
            for (int i = 0; i < 18; i++)
            {
                Vector2 pos = RandomMapPos(rng, 28f, halfMapSize - 24f);
                if (IsTooCloseToObstacle(pos, 12f)) continue;

                float angle = (float)(rng.NextDouble() * 360.0);
                float scale = Mathf.Lerp(1.45f, 1.85f, (float)rng.NextDouble());
                SpawnCrashedPlane(root.transform, pos, angle, scale);

                // Đặt thêm Bình Xăng Nổ & Thùng Gỗ cạnh xác máy bay
                SpawnExplosiveBarrel(root.transform, pos + new Vector2(3.8f, 1.5f));
                SpawnCrate(root.transform, pos + new Vector2(-3.6f, -2.0f), 1.5f);
            }

            // 3. 55 Ô Tô Hỏng (Wrecked Cars)
            for (int i = 0; i < 55; i++)
            {
                Vector2 pos = RandomMapPos(rng, 18f, halfMapSize - 15f);
                if (IsTooCloseToObstacle(pos, 6.5f)) continue;

                float angle = (float)(rng.NextDouble() * 360.0);
                Color carColor = carTints[i % carTints.Length];
                SpawnWreckedCar(root.transform, pos, angle, carColor);
            }

            // 4. 65 Xe Máy Hỏng (Broken Motorcycles)
            for (int i = 0; i < 65; i++)
            {
                Vector2 pos = RandomMapPos(rng, 16f, halfMapSize - 14f);
                if (IsTooCloseToObstacle(pos, 5.0f)) continue;

                float angle = (float)(rng.NextDouble() * 360.0);
                SpawnBrokenMotorcycle(root.transform, pos, angle);
            }

            // 5. 95 Cụm Bình Xăng Nổ (Explosive Fuel Barrels)
            for (int i = 0; i < 95; i++)
            {
                Vector2 pos = RandomMapPos(rng, 14f, halfMapSize - 12f);
                if (IsTooCloseToObstacle(pos, 4.5f)) continue;

                SpawnExplosiveBarrel(root.transform, pos);
                if (i % 3 == 0)
                {
                    SpawnExplosiveBarrel(root.transform, pos + new Vector2(1.15f, 0.65f));
                }
            }

            // 6. 260 Cây Cối, 160 Bụi Cây, 110 Đá Tảng & 70 Thùng Gỗ trên khắp bản đồ rộng
            for (int i = 0; i < 260; i++)
            {
                Vector2 pos = RandomMapPos(rng, 15f, halfMapSize - 10f);
                if (IsTooCloseToObstacle(pos, 4.2f)) continue;
                SpawnTree(root.transform, pos, Mathf.Lerp(1.25f, 1.75f, (float)rng.NextDouble()));
            }

            for (int i = 0; i < 160; i++)
            {
                Vector2 pos = RandomMapPos(rng, 12f, halfMapSize - 10f);
                if (IsTooCloseToObstacle(pos, 3.2f)) continue;
                SpawnBush(root.transform, pos, Mathf.Lerp(1.1f, 1.5f, (float)rng.NextDouble()));
            }

            for (int i = 0; i < 110; i++)
            {
                Vector2 pos = RandomMapPos(rng, 15f, halfMapSize - 10f);
                if (IsTooCloseToObstacle(pos, 4.0f)) continue;
                SpawnRock(root.transform, pos, Mathf.Lerp(1.15f, 1.65f, (float)rng.NextDouble()));
            }

            for (int i = 0; i < 70; i++)
            {
                Vector2 pos = RandomMapPos(rng, 16f, halfMapSize - 12f);
                if (IsTooCloseToObstacle(pos, 4.5f)) continue;
                SpawnCrate(root.transform, pos, Mathf.Lerp(1.4f, 1.9f, (float)rng.NextDouble()));
            }
        }

        private static Vector2 RandomMapPos(System.Random rng, float minRadius, float maxCoord)
        {
            for (int attempt = 0; attempt < 12; attempt++)
            {
                float x = Mathf.Lerp(-maxCoord, maxCoord, (float)rng.NextDouble());
                float y = Mathf.Lerp(-maxCoord, maxCoord, (float)rng.NextDouble());
                Vector2 p = new Vector2(x, y);
                if (p.magnitude >= minRadius)
                {
                    return p;
                }
            }
            return new Vector2(minRadius + 5f, minRadius + 5f);
        }

        private bool IsTooCloseToObstacle(Vector2 pos, float minDist)
        {
            float minSq = minDist * minDist;
            for (int i = 0; i < Obstacles.Count; i++)
            {
                EnvironmentObstacle obs = Obstacles[i];
                if (obs == null) continue;
                if (((Vector2)obs.transform.position - pos).sqrMagnitude < minSq)
                {
                    return true;
                }
            }
            return false;
        }

        private void SpawnZombsRoofBuilding(Transform parent, string name, Vector2 center, Vector2 size, Color roofTint)
        {
            GameObject house = new GameObject(name);
            house.transform.SetParent(parent, false);
            house.transform.position = new Vector3(center.x, center.y, 0f);

            SpriteRenderer sr = house.AddComponent<SpriteRenderer>();
            sr.sprite = houseRoofSprite;
            sr.drawMode = SpriteDrawMode.Sliced;
            sr.size = size;
            sr.color = roofTint;
            sr.sortingOrder = 6;

            BoxCollider2D col = house.AddComponent<BoxCollider2D>();
            col.size = size * 0.94f;

            EnvironmentObstacle obs = house.AddComponent<EnvironmentObstacle>();
            obs.kind = ObstacleKind.House;
            obs.blocksBullets = true;
            obs.minimapColor = new Color(0.28f, 0.52f, 0.88f, 1f);
            obs.minimapRadius = Mathf.Max(size.x, size.y) * 0.45f;
            Obstacles.Add(obs);
        }

        private void SpawnEnterableHouse(Transform parent, string name, Vector2 center, Vector2 size, int doorSide)
        {
            GameObject houseObj = new GameObject(name);
            houseObj.transform.SetParent(parent, false);
            houseObj.transform.position = new Vector3(center.x, center.y, 0f);

            float wallThickness = 0.85f;
            float doorWidth = 2.8f;

            // Sàn gỗ bên trong ngôi nhà
            GameObject floorObj = new GameObject("InteriorWoodFloor");
            floorObj.transform.SetParent(houseObj.transform, false);
            floorObj.transform.position = new Vector3(center.x, center.y, 0f);
            floorObj.transform.localScale = new Vector3(2f, 2f, 1f);
            SpriteRenderer floorSr = floorObj.AddComponent<SpriteRenderer>();
            floorSr.sprite = houseFloorSprite;
            floorSr.drawMode = SpriteDrawMode.Tiled;
            floorSr.size = new Vector2(size.x - wallThickness * 0.5f, size.y - wallThickness * 0.5f) * 0.5f;
            floorSr.sortingOrder = -3;

            float halfW = size.x * 0.5f;
            float halfH = size.y * 0.5f;

            if (doorSide == 1)
            {
                float segW = (size.x - doorWidth) * 0.5f;
                SpawnHouseWallSegment(houseObj.transform, center + new Vector2(-halfW + segW * 0.5f, halfH), new Vector2(segW, wallThickness));
                SpawnHouseWallSegment(houseObj.transform, center + new Vector2(halfW - segW * 0.5f, halfH), new Vector2(segW, wallThickness));
            }
            else
            {
                SpawnHouseWallSegment(houseObj.transform, center + new Vector2(0f, halfH), new Vector2(size.x + wallThickness, wallThickness));
            }

            if (doorSide == 0)
            {
                float segW = (size.x - doorWidth) * 0.5f;
                SpawnHouseWallSegment(houseObj.transform, center + new Vector2(-halfW + segW * 0.5f, -halfH), new Vector2(segW, wallThickness));
                SpawnHouseWallSegment(houseObj.transform, center + new Vector2(halfW - segW * 0.5f, -halfH), new Vector2(segW, wallThickness));
            }
            else
            {
                SpawnHouseWallSegment(houseObj.transform, center + new Vector2(0f, -halfH), new Vector2(size.x + wallThickness, wallThickness));
            }

            if (doorSide == 2)
            {
                float segH = (size.y - doorWidth) * 0.5f;
                SpawnHouseWallSegment(houseObj.transform, center + new Vector2(-halfW, -halfH + segH * 0.5f), new Vector2(wallThickness, segH));
                SpawnHouseWallSegment(houseObj.transform, center + new Vector2(-halfW, halfH - segH * 0.5f), new Vector2(wallThickness, segH));
            }
            else
            {
                SpawnHouseWallSegment(houseObj.transform, center + new Vector2(-halfW, 0f), new Vector2(wallThickness, size.y));
            }

            if (doorSide == 3)
            {
                float segH = (size.y - doorWidth) * 0.5f;
                SpawnHouseWallSegment(houseObj.transform, center + new Vector2(halfW, -halfH + segH * 0.5f), new Vector2(wallThickness, segH));
                SpawnHouseWallSegment(houseObj.transform, center + new Vector2(halfW, halfH - segH * 0.5f), new Vector2(wallThickness, segH));
            }
            else
            {
                SpawnHouseWallSegment(houseObj.transform, center + new Vector2(halfW, 0f), new Vector2(wallThickness, size.y));
            }
        }

        private void SpawnHouseWallSegment(Transform parent, Vector2 pos, Vector2 size)
        {
            GameObject wall = new GameObject("HouseBrickWall");
            wall.transform.SetParent(parent, false);
            wall.transform.position = new Vector3(pos.x, pos.y, 0f);

            SpriteRenderer sr = wall.AddComponent<SpriteRenderer>();
            sr.sprite = brickWallSprite != null ? brickWallSprite : pillarSprite;
            sr.drawMode = SpriteDrawMode.Tiled;
            sr.size = size;
            sr.sortingOrder = 4;

            BoxCollider2D col = wall.AddComponent<BoxCollider2D>();
            col.size = size;

            EnvironmentObstacle obs = wall.AddComponent<EnvironmentObstacle>();
            obs.kind = ObstacleKind.Wall;
            obs.blocksBullets = true;
            obs.minimapColor = new Color(0.75f, 0.34f, 0.22f, 1f);
            obs.minimapRadius = Mathf.Max(size.x, size.y) * 0.45f;
            Obstacles.Add(obs);
        }

        private void SpawnExplosiveBarrel(Transform parent, Vector2 pos)
        {
            GameObject obj = new GameObject("ExplosiveBarrel");
            obj.transform.SetParent(parent, false);
            obj.transform.position = new Vector3(pos.x, pos.y, 0f);
            obj.transform.localScale = Vector3.one * 1.15f;

            SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
            sr.sprite = explosiveBarrelSprite;
            sr.sortingOrder = 4;

            CircleCollider2D col = obj.AddComponent<CircleCollider2D>();
            col.radius = 0.38f;

            obj.AddComponent<ExplosiveBarrel>();

            EnvironmentObstacle obs = obj.AddComponent<EnvironmentObstacle>();
            obs.kind = ObstacleKind.ExplosiveBarrel;
            obs.blocksBullets = false; // Để viên đạn kích hoạt ExplosiveBarrel.TakeDamage()
            obs.minimapColor = new Color(1.0f, 0.22f, 0.16f, 1f);
            obs.minimapRadius = 1.1f;
            Obstacles.Add(obs);
        }

        private void SpawnCrashedPlane(Transform parent, Vector2 pos, float angleDeg, float scale)
        {
            GameObject obj = new GameObject("CrashedAirplane");
            obj.transform.SetParent(parent, false);
            obj.transform.position = new Vector3(pos.x, pos.y, 0f);
            obj.transform.rotation = Quaternion.Euler(0f, 0f, angleDeg);
            obj.transform.localScale = Vector3.one * scale;

            SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
            sr.sprite = crashedPlaneSprite;
            sr.sortingOrder = 5;

            // Collider cho thân máy bay & cánh máy bay
            BoxCollider2D bodyCol = obj.AddComponent<BoxCollider2D>();
            bodyCol.size = new Vector2(0.72f, 3.1f);

            BoxCollider2D wingCol = obj.AddComponent<BoxCollider2D>();
            wingCol.size = new Vector2(3.1f, 0.72f);

            EnvironmentObstacle obs = obj.AddComponent<EnvironmentObstacle>();
            obs.kind = ObstacleKind.CrashedPlane;
            obs.blocksBullets = true;
            obs.minimapColor = new Color(0.38f, 0.56f, 0.95f, 1f);
            obs.minimapRadius = 2.4f;
            Obstacles.Add(obs);
        }

        private void SpawnWreckedCar(Transform parent, Vector2 pos, float angleDeg, Color carTint)
        {
            GameObject obj = new GameObject("WreckedCar");
            obj.transform.SetParent(parent, false);
            obj.transform.position = new Vector3(pos.x, pos.y, 0f);
            obj.transform.rotation = Quaternion.Euler(0f, 0f, angleDeg);
            obj.transform.localScale = Vector3.one * 1.3f;

            SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
            sr.sprite = wreckedCarSprite;
            sr.color = carTint;
            sr.sortingOrder = 4;

            BoxCollider2D col = obj.AddComponent<BoxCollider2D>();
            col.size = new Vector2(1.75f, 0.98f);

            EnvironmentObstacle obs = obj.AddComponent<EnvironmentObstacle>();
            obs.kind = ObstacleKind.WreckedCar;
            obs.blocksBullets = true;
            obs.minimapColor = new Color(0.85f, 0.42f, 0.28f, 1f);
            obs.minimapRadius = 1.4f;
            Obstacles.Add(obs);
        }

        private void SpawnBrokenMotorcycle(Transform parent, Vector2 pos, float angleDeg)
        {
            GameObject obj = new GameObject("BrokenMotorcycle");
            obj.transform.SetParent(parent, false);
            obj.transform.position = new Vector3(pos.x, pos.y, 0f);
            obj.transform.rotation = Quaternion.Euler(0f, 0f, angleDeg);
            obj.transform.localScale = Vector3.one * 1.15f;

            SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
            sr.sprite = brokenMotorcycleSprite;
            sr.sortingOrder = 3;

            BoxCollider2D col = obj.AddComponent<BoxCollider2D>();
            col.size = new Vector2(0.95f, 0.48f);

            EnvironmentObstacle obs = obj.AddComponent<EnvironmentObstacle>();
            obs.kind = ObstacleKind.BrokenMotorcycle;
            obs.blocksBullets = true;
            obs.minimapColor = new Color(0.95f, 0.58f, 0.20f, 1f);
            obs.minimapRadius = 1.0f;
            Obstacles.Add(obs);
        }

        private void SpawnTree(Transform parent, Vector2 pos, float scale)
        {
            GameObject obj = new GameObject("ForestTree");
            obj.transform.SetParent(parent, false);
            obj.transform.position = new Vector3(pos.x, pos.y, 0f);
            obj.transform.localScale = Vector3.one * scale;

            SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
            sr.sprite = treeSprite;
            sr.sortingOrder = 6;

            CircleCollider2D col = obj.AddComponent<CircleCollider2D>();
            col.radius = 0.25f;

            EnvironmentObstacle obs = obj.AddComponent<EnvironmentObstacle>();
            obs.kind = ObstacleKind.Tree;
            obs.blocksBullets = true;
            obs.minimapColor = new Color(0.18f, 0.42f, 0.14f, 1f);
            obs.minimapRadius = 1.4f;
            Obstacles.Add(obs);
        }

        private void SpawnBush(Transform parent, Vector2 pos, float scale)
        {
            GameObject obj = new GameObject("ForestBush");
            obj.transform.SetParent(parent, false);
            obj.transform.position = new Vector3(pos.x, pos.y, 0f);
            obj.transform.localScale = Vector3.one * scale;

            SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
            sr.sprite = bushSprite;
            sr.sortingOrder = 2;
        }

        private void SpawnRock(Transform parent, Vector2 pos, float scale)
        {
            GameObject obj = new GameObject("RockObstacle");
            obj.transform.SetParent(parent, false);
            obj.transform.position = new Vector3(pos.x, pos.y, 0f);
            obj.transform.localScale = Vector3.one * scale;

            SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
            sr.sprite = rockSprite;
            sr.sortingOrder = 3;

            CircleCollider2D col = obj.AddComponent<CircleCollider2D>();
            col.radius = 0.34f;

            EnvironmentObstacle obs = obj.AddComponent<EnvironmentObstacle>();
            obs.kind = ObstacleKind.Rock;
            obs.blocksBullets = true;
            obs.minimapColor = new Color(0.65f, 0.65f, 0.62f, 1f);
            obs.minimapRadius = 1.2f;
            Obstacles.Add(obs);
        }

        private void SpawnCrate(Transform parent, Vector2 pos, float scale)
        {
            GameObject obj = new GameObject("SupplyCrateCover");
            obj.transform.SetParent(parent, false);
            obj.transform.position = new Vector3(pos.x, pos.y, 0f);
            obj.transform.localScale = Vector3.one * scale;

            SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
            sr.sprite = crateSprite;
            sr.sortingOrder = 3;

            BoxCollider2D col = obj.AddComponent<BoxCollider2D>();
            col.size = new Vector2(0.92f, 0.92f);

            EnvironmentObstacle obs = obj.AddComponent<EnvironmentObstacle>();
            obs.kind = ObstacleKind.Crate;
            obs.blocksBullets = true;
            obs.minimapColor = new Color(0.58f, 0.40f, 0.22f, 1f);
            obs.minimapRadius = 1.1f;
            Obstacles.Add(obs);
        }

        public void ApplyGraphicsDetailLevel(int qualityTier)
        {
            Transform root = transform.Find("WorldObstaclesRoot");
            if (root == null) return;

            for (int i = 0; i < root.childCount; i++)
            {
                Transform child = root.GetChild(i);
                if (child.name == "ForestBush")
                {
                    child.gameObject.SetActive(qualityTier >= 1 && (qualityTier >= 2 || i % 2 == 0));
                }
            }
        }

        public MapZoneType GetZoneAt(Vector2 pos)
        {
            return MapZoneType.CentralHub;
        }

        public string GetZoneDisplayName(Vector2 pos)
        {
            return $"CHIẾN TRƯỜNG ({Mathf.RoundToInt(halfMapSize * 2f)}x{Mathf.RoundToInt(halfMapSize * 2f)})";
        }

        public Color GetMinimapFloorColor(Vector2 worldPos)
        {
            return new Color(0.357f, 0.565f, 0.243f, 1f);
        }
    }
}
