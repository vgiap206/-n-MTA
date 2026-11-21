using UnityEngine;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace TopDownShooter
{
    /// <summary>
    /// Hệ thống Mini Map thời gian thực ở góc trên bên phải màn hình:
    /// - Hiển thị địa hình các khu vực (Khu Trung Tâm, Khu Rừng Rậm, Căn Cứ Quân Sự, Hoang Mạc Đá Cổ, Đại lộ).
    /// - Hiển thị vật cản (Cây rừng, Đá, Thùng chắn).
    /// - Hiển thị các Hòm Tiếp Tế & Nâng Cấp Vũ Khí (chấm chữ thập sáng màu Vàng / Xanh).
    /// - Hiển thị Kẻ Địch (chấm Đỏ) và Nhân Vật Chính + hướng ngắm bắn (tâm Xanh Dương / Vàng).
    /// - Nhấn phím M để chuyển đổi giữa chế độ Radar gần và Toàn cảnh bản đồ.
    /// </summary>
    public class MinimapController : MonoBehaviour
    {
        public static MinimapController Instance { get; private set; }

        [Header("UI Tham Chiếu của Mini Map")]
        public RawImage mapRawImage;
        public Text zoneTitleText;
        public Text legendText;

        [Header("Cấu hình Radar")]
        [Tooltip("Độ phân giải Texture của Mini Map")]
        public int mapResolution = 104;

        [Tooltip("Bán kính tầm nhìn Radar xung quanh người chơi (đơn vị mét)")]
        public float radarRange = 36f;

        [Tooltip("Chế độ xem toàn bộ bản đồ (nhấn phím M để bật/tắt)")]
        public bool fullMapMode = false;

        [Tooltip("Chu kỳ làm mới Mini Map (giây)")]
        public float refreshInterval = 0.06f;

        private Texture2D _mapTexture;
        private Color32[] _pixels;
        private float _nextUpdateTimer;

        private void Awake()
        {
            Instance = this;
            InitializeTexture();
        }

        private void InitializeTexture()
        {
            if (_mapTexture != null) return;

            mapResolution = Mathf.Clamp(mapResolution, 64, 160);
            _mapTexture = new Texture2D(mapResolution, mapResolution, TextureFormat.RGBA32, false);
            _mapTexture.filterMode = FilterMode.Point;
            _mapTexture.wrapMode = TextureWrapMode.Clamp;
            _pixels = new Color32[mapResolution * mapResolution];

            if (mapRawImage != null)
            {
                mapRawImage.texture = _mapTexture;
            }
        }

        private void Update()
        {
            CheckZoomToggleInput();

            if (Time.time >= _nextUpdateTimer)
            {
                _nextUpdateTimer = Time.time + Mathf.Max(0.04f, refreshInterval);
                RenderMinimap();
            }
        }

        private void CheckZoomToggleInput()
        {
            bool togglePressed = false;
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null && Keyboard.current.mKey.wasPressedThisFrame)
            {
                togglePressed = true;
            }
#elif ENABLE_LEGACY_INPUT_MANAGER
            if (Input.GetKeyDown(KeyCode.M))
            {
                togglePressed = true;
            }
#endif
            if (togglePressed)
            {
                fullMapMode = !fullMapMode;
                RenderMinimap();
            }
        }

        public void RenderMinimap()
        {
            if (_mapTexture == null) InitializeTexture();
            if (mapRawImage != null && mapRawImage.texture != _mapTexture)
            {
                mapRawImage.texture = _mapTexture;
            }

            PlayerController player = PlayerController.Instance;
            Vector2 playerPos = player != null ? (Vector2)player.transform.position : Vector2.zero;

            WorldMapManager world = WorldMapManager.Instance;
            float worldHalfSize = world != null ? world.halfMapSize : 64f;

            // Cập nhật tên khu vực hiện tại
            if (zoneTitleText != null)
            {
                string zoneName = world != null ? world.GetZoneDisplayName(playerPos) : "KHU TRUNG TÂM";
                zoneTitleText.text = zoneName;
            }

            Vector2 viewCenter = fullMapMode ? Vector2.zero : playerPos;
            float activeRange = fullMapMode ? worldHalfSize + 2f : radarRange;

            int res = mapResolution;
            float invRes = 1f / res;

            // 1. Vẽ nền các Khu Vực (Khu Trung Tâm, Rừng Rậm, Căn Cứ, Hoang Mạc, Đường Lớn)
            for (int py = 0; py < res; py++)
            {
                float ny = (py + 0.5f) * invRes * 2f - 1f;
                float wy = viewCenter.y + ny * activeRange;

                for (int px = 0; px < res; px++)
                {
                    float nx = (px + 0.5f) * invRes * 2f - 1f;
                    float wx = viewCenter.x + nx * activeRange;

                    Color32 col;
                    if (Mathf.Abs(wx) > worldHalfSize || Mathf.Abs(wy) > worldHalfSize)
                    {
                        col = new Color32(14, 18, 24, 255); // Ngoài biên giới bản đồ
                    }
                    else if (world != null)
                    {
                        col = world.GetMinimapFloorColor(new Vector2(wx, wy));
                    }
                    else
                    {
                        col = new Color32(32, 38, 48, 255);
                    }

                    // Vẽ lưới radar mờ
                    if (px == res / 2 || py == res / 2)
                    {
                        col = BlendColor(col, new Color32(90, 120, 150, 255), 0.14f);
                    }

                    // Viền khung Mini Map
                    if (px <= 1 || px >= res - 2 || py <= 1 || py >= res - 2)
                    {
                        col = new Color32(75, 165, 225, 255);
                    }

                    _pixels[py * res + px] = col;
                }
            }

            // 2. Vẽ các vật cản môi trường (Cây rừng, Đá, Trụ, Thùng)
            if (world != null && world.Obstacles != null)
            {
                int obsCount = world.Obstacles.Count;
                for (int i = 0; i < obsCount; i++)
                {
                    EnvironmentObstacle obs = world.Obstacles[i];
                    if (obs == null) continue;

                    Vector2 obsPos = obs.transform.position;
                    if (WorldToPixel(obsPos, viewCenter, activeRange, res, out int ox, out int oy))
                    {
                        int radius = obs.kind == ObstacleKind.Tree ? 1 : 0;
                        DrawDot(ox, oy, radius, obs.minimapColor);
                    }
                }
            }

            // 3. Vẽ các Hòm Tiếp Tế & Nâng Cấp Vũ Khí (Chữ thập sáng nổi bật)
            int supplyCount = SupplyDrop.ActiveSupplies.Count;
            bool pulseBright = (Mathf.FloorToInt(Time.time * 5f) % 2 == 0);
            for (int i = 0; i < supplyCount; i++)
            {
                SupplyDrop supply = SupplyDrop.ActiveSupplies[i];
                if (supply == null) continue;

                Vector2 sPos = supply.transform.position;
                if (WorldToPixel(sPos, viewCenter, activeRange, res, out int sx, out int sy))
                {
                    Color32 sCol = pulseBright
                        ? (Color32)supply.GetMinimapColor()
                        : new Color32(255, 255, 255, 255);
                    DrawCrossIcon(sx, sy, 2, sCol);
                }
                else if (!fullMapMode)
                {
                    // Nếu hòm tiếp tế ở ngoài tầm radar, ghim mũi tên nhỏ ở viền Mini Map để chỉ đường!
                    Vector2 dir = (sPos - viewCenter).normalized;
                    int edgeX = Mathf.Clamp(Mathf.RoundToInt((dir.x * 0.92f * 0.5f + 0.5f) * (res - 1)), 3, res - 4);
                    int edgeY = Mathf.Clamp(Mathf.RoundToInt((dir.y * 0.92f * 0.5f + 0.5f) * (res - 1)), 3, res - 4);
                    DrawDot(edgeX, edgeY, 1, supply.GetMinimapColor());
                }
            }

            // 4. Vẽ Kẻ Địch (Chấm Đỏ)
            Color32 enemyDotColor = new Color32(255, 58, 58, 255);
            int enemyCount = Enemy.ActiveEnemies.Count;
            for (int i = 0; i < enemyCount; i++)
            {
                Enemy e = Enemy.ActiveEnemies[i];
                if (e == null || e.IsDead) continue;

                if (WorldToPixel(e.transform.position, viewCenter, activeRange, res, out int ex, out int ey))
                {
                    int r = e.maxHealth >= 150f ? 2 : 1; // Quái to/Thủ lĩnh hiện chấm lớn hơn
                    DrawDot(ex, ey, r, enemyDotColor);
                }
            }

            // 5. Vẽ Nhân Vật Chính & Hướng Ngắm Bắn
            if (player != null && WorldToPixel(playerPos, viewCenter, activeRange, res, out int plx, out int ply))
            {
                Vector2 aimDir = player.transform.right;
                Color32 aimColor = new Color32(255, 235, 65, 255);
                for (int step = 2; step <= 6; step++)
                {
                    int ax = plx + Mathf.RoundToInt(aimDir.x * step);
                    int ay = ply + Mathf.RoundToInt(aimDir.y * step);
                    SetPixelSafe(ax, ay, aimColor);
                }

                DrawDot(plx, ply, 2, new Color32(45, 225, 255, 255));
                SetPixelSafe(plx, ply, new Color32(255, 255, 255, 255));
            }

            _mapTexture.SetPixels32(_pixels);
            _mapTexture.Apply(false);
        }

        private static bool WorldToPixel(Vector2 worldPos, Vector2 center, float range, int res, out int px, out int py)
        {
            float nx = (worldPos.x - center.x) / range;
            float ny = (worldPos.y - center.y) / range;

            if (nx < -1f || nx > 1f || ny < -1f || ny > 1f)
            {
                px = 0;
                py = 0;
                return false;
            }

            px = Mathf.Clamp(Mathf.RoundToInt((nx * 0.5f + 0.5f) * (res - 1)), 2, res - 3);
            py = Mathf.Clamp(Mathf.RoundToInt((ny * 0.5f + 0.5f) * (res - 1)), 2, res - 3);
            return true;
        }

        private void DrawDot(int cx, int cy, int radius, Color32 color)
        {
            for (int dy = -radius; dy <= radius; dy++)
            {
                for (int dx = -radius; dx <= radius; dx++)
                {
                    if (dx * dx + dy * dy <= radius * radius + 1)
                    {
                        SetPixelSafe(cx + dx, cy + dy, color);
                    }
                }
            }
        }

        private void DrawCrossIcon(int cx, int cy, int armLength, Color32 color)
        {
            for (int i = -armLength; i <= armLength; i++)
            {
                SetPixelSafe(cx + i, cy, color);
                SetPixelSafe(cx, cy + i, color);
            }
        }

        private void SetPixelSafe(int x, int y, Color32 color)
        {
            if (x >= 2 && x < mapResolution - 2 && y >= 2 && y < mapResolution - 2)
            {
                _pixels[y * mapResolution + x] = color;
            }
        }

        private static Color32 BlendColor(Color32 a, Color32 b, float t)
        {
            return new Color32(
                (byte)Mathf.Lerp(a.r, b.r, t),
                (byte)Mathf.Lerp(a.g, b.g, t),
                (byte)Mathf.Lerp(a.b, b.b, t),
                255);
        }
    }
}

