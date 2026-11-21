using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif

namespace TopDownShooter
{
    /// <summary>
    /// Tự động khởi tạo và liên kết đầy đủ các thành phần trong Scene:
    /// - Bản đồ thế giới đa vùng & Ngôi Nhà (WorldMapManager)
    /// - Nhân vật & Kho 7 Súng (Player)
    /// - Camera di chuyển theo nhân vật (CameraFollow)
    /// - Bộ sinh kẻ địch (EnemySpawner)
    /// - Bộ sinh Tiếp Tế & Nâng Cấp Vũ Khí (SupplySpawner)
    /// - Quản lý âm thanh (AudioManager)
    /// - Giao diện người dùng (Canvas UI: Chữ Level, Thanh EXP, Thanh Máu, Mini Map & Menu Trước Khi Vào Game)
    /// </summary>
    public static class GameBootstrapper
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void OnAfterSceneLoad()
        {
            EnsureSceneSetup();
        }

        public static void EnsureSceneSetup(
            Sprite playerSpriteAsset = null,
            Sprite whiteSquareSpriteAsset = null,
            Sprite gridSpriteAsset = null,
            Bullet bulletPrefabAsset = null,
            SupplyDrop supplyPrefabAsset = null,
            AudioClip shootClip = null,
            AudioClip enemyHitClip = null,
            AudioClip playerHitClip = null,
            AudioClip playerDeathClip = null,
            AudioClip enemyDeathClip = null,
            AudioClip bgmClip = null,
            AudioClip pickupClip = null,
            Sprite retroGoldBtnSpriteAsset = null,
            Sprite retroGoldSquareSpriteAsset = null,
            Sprite retroPanelSpriteAsset = null)
        {
            // 1. Nền phụ ArenaFloor (nằm dưới cùng sortingOrder = -30)
            ArenaFloorFollow floor = Object.FindAnyObjectByType<ArenaFloorFollow>();
            if (floor == null)
            {
                GameObject floorObj = new GameObject("ArenaFloor");
                floorObj.AddComponent<SpriteRenderer>();
                floor = floorObj.AddComponent<ArenaFloorFollow>();
            }
            SpriteRenderer floorSr = floor.GetComponent<SpriteRenderer>();
            if (floorSr != null)
            {
                floor.transform.localScale = new Vector3(2f, 2f, 1f);
                floorSr.sprite = gridSpriteAsset != null ? gridSpriteAsset : SpriteGenerator.GetCentralFloorSprite();
                floorSr.drawMode = SpriteDrawMode.Tiled;
                floorSr.size = new Vector2(40f, 40f);
                floorSr.sortingOrder = -30;
            }

            // 2. Thế giới Bản Đồ Đa Khu Vực & Ngôi Nhà
            WorldMapManager worldMap = Object.FindAnyObjectByType<WorldMapManager>();
            if (worldMap == null)
            {
                GameObject worldObj = new GameObject("WorldMapManager");
                worldMap = worldObj.AddComponent<WorldMapManager>();
            }
            worldMap.BuildWorldIfNeeded();

            // 3. Nhân vật (Player)
            PlayerController player = Object.FindAnyObjectByType<PlayerController>();
            if (player == null)
            {
                GameObject playerObj = new GameObject("Player");
                playerObj.tag = "Player";
                playerObj.transform.position = Vector3.zero;

                SpriteRenderer sr = playerObj.AddComponent<SpriteRenderer>();
                sr.sprite = playerSpriteAsset != null ? playerSpriteAsset : SpriteGenerator.GetPlayerSprite();
                sr.sortingOrder = 5;

                Rigidbody2D rb = playerObj.AddComponent<Rigidbody2D>();
                rb.gravityScale = 0f;
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

                CircleCollider2D col = playerObj.AddComponent<CircleCollider2D>();
                col.radius = 0.36f;

                player = playerObj.AddComponent<PlayerController>();
            }
            else if (playerSpriteAsset != null)
            {
                SpriteRenderer playerSr = player.GetComponent<SpriteRenderer>();
                if (playerSr != null) playerSr.sprite = playerSpriteAsset;
            }

            if (bulletPrefabAsset != null && player.bulletPrefab == null)
            {
                player.bulletPrefab = bulletPrefabAsset;
            }

            // 4. Camera di chuyển theo nhân vật
            Camera mainCam = Camera.main;
            if (mainCam == null)
            {
                mainCam = Object.FindAnyObjectByType<Camera>();
            }

            if (mainCam != null)
            {
                mainCam.orthographic = true;
                mainCam.orthographicSize = 6.5f;
                mainCam.backgroundColor = new Color(0.357f, 0.565f, 0.243f, 1f);

                CameraFollow camFollow = mainCam.GetComponent<CameraFollow>();
                if (camFollow == null)
                {
                    camFollow = mainCam.gameObject.AddComponent<CameraFollow>();
                }
                camFollow.target = player.transform;
            }

            // 5. Hệ thống sinh kẻ địch (EnemySpawner)
            EnemySpawner spawner = Object.FindAnyObjectByType<EnemySpawner>();
            if (spawner == null)
            {
                GameObject spawnerObj = new GameObject("EnemySpawner");
                spawner = spawnerObj.AddComponent<EnemySpawner>();
                spawner.EnsureDefaultEnemyTypes();
            }

            // 6. Hệ thống sinh Tiếp Tế & Nâng Cấp Vũ Khí (SupplySpawner)
            SupplySpawner supplySpawner = Object.FindAnyObjectByType<SupplySpawner>();
            if (supplySpawner == null)
            {
                GameObject supplyObj = new GameObject("SupplySpawner");
                supplySpawner = supplyObj.AddComponent<SupplySpawner>();
            }
            if (supplyPrefabAsset != null && supplySpawner.supplyPrefab == null)
            {
                supplySpawner.supplyPrefab = supplyPrefabAsset;
            }

            // 7. Hệ thống âm thanh (AudioManager)
            AudioManager audioMgr = Object.FindAnyObjectByType<AudioManager>();
            if (audioMgr == null)
            {
                GameObject audioObj = new GameObject("AudioManager");
                audioMgr = audioObj.AddComponent<AudioManager>();
            }

            if (shootClip != null) audioMgr.shootSound = shootClip;
            if (enemyHitClip != null) audioMgr.enemyHitSound = enemyHitClip;
            if (playerHitClip != null) audioMgr.playerHitSound = playerHitClip;
            if (playerDeathClip != null) audioMgr.playerDeathSound = playerDeathClip;
            if (enemyDeathClip != null) audioMgr.enemyDeathSound = enemyDeathClip;
            if (bgmClip != null) audioMgr.backgroundMusic = bgmClip;
            if (pickupClip != null) audioMgr.pickupSound = pickupClip;

            // 8. Giao diện UI (Chữ Level, Thanh EXP, Mini Map & Menu Trước Khi Vào Game)
            UIManager uiMgr = Object.FindAnyObjectByType<UIManager>();
            MinimapController minimap = Object.FindAnyObjectByType<MinimapController>();
            MainMenuManager menuMgr = Object.FindAnyObjectByType<MainMenuManager>();

            if (uiMgr == null ||
                uiMgr.levelText == null ||
                uiMgr.expSlider == null ||
                minimap == null ||
                minimap.mapRawImage == null ||
                menuMgr == null ||
                menuMgr.mainMenuOverlay == null ||
                menuMgr.settingsModal == null)
            {
                Sprite uiBarSprite = whiteSquareSpriteAsset != null
                    ? whiteSquareSpriteAsset
                    : SpriteGenerator.GetWhiteSquareSprite();
                Sprite goldBtnSprite = retroGoldBtnSpriteAsset != null
                    ? retroGoldBtnSpriteAsset
                    : SpriteGenerator.GetRetroGoldButtonSprite();
                Sprite goldSqSprite = retroGoldSquareSpriteAsset != null
                    ? retroGoldSquareSpriteAsset
                    : SpriteGenerator.GetRetroGoldSquareSprite();
                Sprite panelSprite = retroPanelSpriteAsset != null
                    ? retroPanelSpriteAsset
                    : SpriteGenerator.GetRetroPanelSprite();

                BuildHUD(uiBarSprite, goldBtnSprite, goldSqSprite, panelSprite);
            }

            // 9. EventSystem cho UI
            if (Object.FindAnyObjectByType<EventSystem>() == null)
            {
                GameObject esObj = new GameObject("EventSystem");
                esObj.AddComponent<EventSystem>();
#if ENABLE_INPUT_SYSTEM
                esObj.AddComponent<InputSystemUIInputModule>();
#else
                esObj.AddComponent<StandaloneInputModule>();
#endif
            }
        }

        public static UIManager BuildHUD(
            Sprite barSprite,
            Sprite goldBtnSprite = null,
            Sprite goldSquareSprite = null,
            Sprite retroPanelSprite = null)
        {
            Font defaultFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (goldBtnSprite == null) goldBtnSprite = SpriteGenerator.GetRetroGoldButtonSprite();
            if (goldSquareSprite == null) goldSquareSprite = SpriteGenerator.GetRetroGoldSquareSprite();
            if (retroPanelSprite == null) retroPanelSprite = SpriteGenerator.GetRetroPanelSprite();

            Canvas canvas = Object.FindAnyObjectByType<Canvas>();
            GameObject canvasObj;
            if (canvas == null)
            {
                canvasObj = new GameObject("Canvas");
                canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = 100;

                CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920f, 1080f);
                scaler.matchWidthOrHeight = 0.5f;

                canvasObj.AddComponent<GraphicRaycaster>();
            }
            else
            {
                canvasObj = canvas.gameObject;
            }

            UIManager uiMgr = canvasObj.GetComponent<UIManager>();
            if (uiMgr == null)
            {
                uiMgr = canvasObj.AddComponent<UIManager>();
            }

            DestroyChildIfExists(canvasObj.transform, "HUDPanel");
            DestroyChildIfExists(canvasObj.transform, "LevelUpBanner");
            DestroyChildIfExists(canvasObj.transform, "MinimapPanel");
            DestroyChildIfExists(canvasObj.transform, "HudPauseMenuButton");
            DestroyChildIfExists(canvasObj.transform, "MainMenuOverlay");

            // --- A. HUD PANEL (Góc trên bên trái - Thiết kế khung viền Vàng Kim Loại & Thanh Máu 3D chỉnh chu) ---
            Sprite hudFrameSprite = SpriteGenerator.GetHudPanelFrameSprite();
            Sprite glossyBarSprite = SpriteGenerator.GetGlossyBarSprite();

            GameObject hudPanel = new GameObject("HUDPanel");
            hudPanel.transform.SetParent(canvasObj.transform, false);
            RectTransform panelRect = hudPanel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0f, 1f);
            panelRect.anchorMax = new Vector2(0f, 1f);
            panelRect.pivot = new Vector2(0f, 1f);
            panelRect.anchoredPosition = new Vector2(22f, -18f);
            panelRect.sizeDelta = new Vector2(850f, 186f);

            Image panelBg = hudPanel.AddComponent<Image>();
            panelBg.sprite = hudFrameSprite != null ? hudFrameSprite : barSprite;
            panelBg.color = Color.white;
            panelBg.raycastTarget = false;

            // 1. Huy hiệu LEVEL & ĐỢT QUÁI (Khung trái nổi bật)
            GameObject badgeBgObj = new GameObject("LevelBadgeFrame");
            badgeBgObj.transform.SetParent(hudPanel.transform, false);
            RectTransform badgeBgRect = badgeBgObj.AddComponent<RectTransform>();
            badgeBgRect.anchorMin = new Vector2(0f, 1f);
            badgeBgRect.anchorMax = new Vector2(0f, 1f);
            badgeBgRect.pivot = new Vector2(0f, 1f);
            badgeBgRect.anchoredPosition = new Vector2(18f, -18f);
            badgeBgRect.sizeDelta = new Vector2(182f, 102f);

            Image badgeBgImg = badgeBgObj.AddComponent<Image>();
            badgeBgImg.sprite = goldSquareSprite != null ? goldSquareSprite : barSprite;
            badgeBgImg.color = new Color(0.16f, 0.20f, 0.28f, 0.96f);
            badgeBgImg.raycastTarget = false;

            GameObject levelObj = new GameObject("LevelText");
            levelObj.transform.SetParent(badgeBgObj.transform, false);
            RectTransform levelRect = levelObj.AddComponent<RectTransform>();
            levelRect.anchorMin = Vector2.zero;
            levelRect.anchorMax = Vector2.one;
            levelRect.offsetMin = new Vector2(6f, 4f);
            levelRect.offsetMax = new Vector2(-6f, -4f);

            Text levelTxt = levelObj.AddComponent<Text>();
            levelTxt.font = defaultFont;
            levelTxt.fontSize = 27;
            levelTxt.fontStyle = FontStyle.Bold;
            levelTxt.alignment = TextAnchor.MiddleCenter;
            levelTxt.color = new Color(1f, 0.90f, 0.22f, 1f);
            levelTxt.text = "LEVEL 1\n<size=18><color=#FFAA33>ĐỢT QUÁI 1</color></size>";
            levelTxt.raycastTarget = false;

            Outline levelOutline = levelObj.AddComponent<Outline>();
            levelOutline.effectColor = new Color(0f, 0f, 0f, 0.9f);
            levelOutline.effectDistance = new Vector2(1.5f, -1.5f);

            // 2. THANH MÁU CHỈNH CHU (HealthBar đặt ở hàng trên cùng, to rõ kèm huy hiệu Trái Tim ❤)
            Slider hpSlider = CreateProgressBar(
                hudPanel.transform,
                "HealthBar",
                barSprite,
                glossyBarSprite,
                goldSquareSprite,
                defaultFont,
                "❤",
                new Color(1f, 0.25f, 0.32f, 1f),
                new Vector2(214f, -18f),
                new Vector2(614f, 48f),
                new Color(0.18f, 0.07f, 0.09f, 0.96f),
                new Color(0.18f, 0.92f, 0.38f, 1f),
                "MÁU (HP): 100 / 100  (100%)",
                out Image hpFill,
                out Text hpDetailTxt);

            // 3. THANH KINH NGHIỆM (ExpBar đặt ngay dưới Thanh Máu kèm huy hiệu Ngôi Sao ★)
            Slider expSlider = CreateProgressBar(
                hudPanel.transform,
                "ExpBar",
                barSprite,
                glossyBarSprite,
                goldSquareSprite,
                defaultFont,
                "★",
                new Color(1f, 0.88f, 0.22f, 1f),
                new Vector2(214f, -74f),
                new Vector2(614f, 44f),
                new Color(0.08f, 0.14f, 0.24f, 0.96f),
                new Color(0.18f, 0.74f, 1.0f, 1f),
                "EXP: 0 / 100  (0%)",
                out Image expFill,
                out Text expDetailTxt);

            // 4. Thanh thông tin Súng & Chỉ số ở đáy bảng HUD
            GameObject statsStrip = new GameObject("StatsStripBg");
            statsStrip.transform.SetParent(hudPanel.transform, false);
            RectTransform stripRect = statsStrip.AddComponent<RectTransform>();
            stripRect.anchorMin = new Vector2(0f, 0f);
            stripRect.anchorMax = new Vector2(1f, 0f);
            stripRect.pivot = new Vector2(0.5f, 0f);
            stripRect.anchoredPosition = new Vector2(0f, 14f);
            stripRect.sizeDelta = new Vector2(-36f, 40f);

            Image stripImg = statsStrip.AddComponent<Image>();
            stripImg.sprite = barSprite;
            stripImg.color = new Color(0.05f, 0.07f, 0.11f, 0.90f);
            stripImg.raycastTarget = false;

            GameObject statsObj = new GameObject("StatsSummaryText");
            statsObj.transform.SetParent(statsStrip.transform, false);
            RectTransform statsRect = statsObj.AddComponent<RectTransform>();
            statsRect.anchorMin = Vector2.zero;
            statsRect.anchorMax = Vector2.one;
            statsRect.offsetMin = new Vector2(12f, 2f);
            statsRect.offsetMax = new Vector2(-12f, -2f);

            Text statsTxt = statsObj.AddComponent<Text>();
            statsTxt.font = defaultFont;
            statsTxt.fontSize = 18;
            statsTxt.fontStyle = FontStyle.Bold;
            statsTxt.alignment = TextAnchor.MiddleCenter;
            statsTxt.color = new Color(0.92f, 0.96f, 1f, 1f);
            statsTxt.text = "Súng Lục USP [Cấp 1]   |   Công: 25   |   Tốc Bắn: 3.2/s";
            statsTxt.raycastTarget = false;

            Outline statsOutline = statsObj.AddComponent<Outline>();
            statsOutline.effectColor = new Color(0f, 0f, 0f, 0.85f);
            statsOutline.effectDistance = new Vector2(1f, -1f);

            // --- B. THÔNG BÁO LÊN CẤP & NHẶT TIẾP TẾ (LevelUpBanner) ---
            GameObject bannerObj = new GameObject("LevelUpBanner");
            bannerObj.transform.SetParent(canvasObj.transform, false);
            RectTransform bannerRect = bannerObj.AddComponent<RectTransform>();
            bannerRect.anchorMin = new Vector2(0.5f, 0.5f);
            bannerRect.anchorMax = new Vector2(0.5f, 0.5f);
            bannerRect.pivot = new Vector2(0.5f, 0.5f);
            bannerRect.anchoredPosition = new Vector2(0f, 195f);
            bannerRect.sizeDelta = new Vector2(860f, 120f);

            Text bannerTxt = bannerObj.AddComponent<Text>();
            bannerTxt.font = defaultFont;
            bannerTxt.fontSize = 30;
            bannerTxt.fontStyle = FontStyle.Bold;
            bannerTxt.alignment = TextAnchor.MiddleCenter;
            bannerTxt.color = new Color(1f, 0.92f, 0.2f, 1f);
            bannerTxt.text = "LEVEL UP!";
            bannerTxt.raycastTarget = false;
            bannerObj.SetActive(false);

            // --- C. MINI MAP PANEL (Góc trên bên phải màn hình) ---
            BuildMinimapUI(canvasObj.transform, barSprite, defaultFont);

            // --- D. NÚT TẠM DỪNG / CÀI ĐẶT NHANH TRONG TRẬN & MÀN HÌNH MENU TRƯỚC KHI VÀO GAME ---
            BuildMainMenuUI(canvasObj, barSprite, goldBtnSprite, goldSquareSprite, retroPanelSprite, defaultFont);

            // Gán tham chiếu cho UIManager
            uiMgr.levelText = levelTxt;
            uiMgr.expSlider = expSlider;
            uiMgr.expFillImage = expFill;
            uiMgr.expDetailText = expDetailTxt;
            uiMgr.healthSlider = hpSlider;
            uiMgr.healthFillImage = hpFill;
            uiMgr.healthDetailText = hpDetailTxt;
            uiMgr.statsSummaryText = statsTxt;
            uiMgr.levelUpNoticeText = bannerTxt;

            return uiMgr;
        }

        private static void BuildMainMenuUI(
            GameObject canvasObj,
            Sprite barSprite,
            Sprite goldBtnSprite,
            Sprite goldSquareSprite,
            Sprite retroPanelSprite,
            Font font)
        {
            MainMenuManager menuMgr = canvasObj.GetComponent<MainMenuManager>();
            if (menuMgr == null)
            {
                menuMgr = canvasObj.AddComponent<MainMenuManager>();
            }

            Color darkBrownText = new Color(0.32f, 0.20f, 0.03f, 1f);
            Color goldAccent = new Color(0.98f, 0.85f, 0.14f, 1f);

            // 1. Nút vuông vàng [||] mở Menu/Cài Đặt ngay trên HUD trong lúc chơi (cạnh Mini Map)
            Button hudPauseBtn = CreateRetroGoldButton(
                canvasObj.transform,
                "HudPauseMenuButton",
                goldSquareSprite,
                font,
                "||",
                26,
                darkBrownText,
                new Vector2(1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(-312f, -20f),
                new Vector2(68f, 68f),
                out _);

            // 2. Lớp phủ toàn màn hình trước khi vào game (Nền tím than #35303E giống ảnh mẫu)
            GameObject overlayObj = new GameObject("MainMenuOverlay");
            overlayObj.transform.SetParent(canvasObj.transform, false);
            RectTransform overlayRect = overlayObj.AddComponent<RectTransform>();
            overlayRect.anchorMin = Vector2.zero;
            overlayRect.anchorMax = Vector2.one;
            overlayRect.offsetMin = Vector2.zero;
            overlayRect.offsetMax = Vector2.zero;

            Image overlayBg = overlayObj.AddComponent<Image>();
            overlayBg.sprite = barSprite;
            overlayBg.color = new Color(0.21f, 0.19f, 0.24f, 0.96f);
            overlayBg.raycastTarget = true;

            // --- TRANG CHÍNH: CỘT NÚT DÀI BÊN TRÁI & CỤM NÚT VUÔNG ICON BÊN PHẢI (GIỐNG ẢNH MẪU) ---
            GameObject mainPage = new GameObject("MainButtonsPage");
            mainPage.transform.SetParent(overlayObj.transform, false);
            RectTransform mainPageRect = mainPage.AddComponent<RectTransform>();
            mainPageRect.anchorMin = new Vector2(0.5f, 0.5f);
            mainPageRect.anchorMax = new Vector2(0.5f, 0.5f);
            mainPageRect.pivot = new Vector2(0.5f, 0.5f);
            mainPageRect.anchoredPosition = Vector2.zero;
            mainPageRect.sizeDelta = new Vector2(1240f, 820f);

            // Tiêu đề Game phía trên
            CreateLabel(
                mainPage.transform,
                "GameTitleText",
                font,
                "GAME BẮN SÚNG 2D SINH TỒN",
                50,
                goldAccent,
                new Vector2(0f, 330f),
                new Vector2(1180f, 70f),
                TextAnchor.MiddleCenter);

            CreateLabel(
                mainPage.transform,
                "GameSubtitleText",
                font,
                "CHỌN BẮT ĐẦU TRÒ CHƠI HOẶC CÀI ĐẶT ÂM THANH & ĐỒ HỌA TRƯỚC KHI VÀO TRẬN",
                21,
                new Color(0.92f, 0.90f, 0.85f, 1f),
                new Vector2(0f, 272f),
                new Vector2(1180f, 40f),
                TextAnchor.MiddleCenter);

            // CỘT 4 NÚT VÀNG DÀI XẾP DỌC (Đã xóa cụm nút vuông bên phải theo yêu cầu)
            float btnColX = 0f;
            Vector2 wideBtnSize = new Vector2(480f, 110f);

            Button startBtn = CreateRetroGoldButton(
                mainPage.transform,
                "Btn_StartGame",
                goldBtnSprite,
                font,
                "BẮT ĐẦU TRÒ CHƠI",
                32,
                darkBrownText,
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(btnColX, 150f),
                wideBtnSize,
                out _);

            Button newGameBtn = CreateRetroGoldButton(
                mainPage.transform,
                "Btn_NewGame",
                goldBtnSprite,
                font,
                "CHƠI MỚI (NEW GAME)",
                30,
                darkBrownText,
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(btnColX, 22f),
                wideBtnSize,
                out _);

            Button settingsBtn = CreateRetroGoldButton(
                mainPage.transform,
                "Btn_Settings",
                goldBtnSprite,
                font,
                "CÀI ĐẶT (SETTINGS)",
                30,
                darkBrownText,
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(btnColX, -106f),
                wideBtnSize,
                out _);

            Button exitBtn = CreateRetroGoldButton(
                mainPage.transform,
                "Btn_ExitGame",
                goldBtnSprite,
                font,
                "THOÁT GAME (EXIT)",
                32,
                darkBrownText,
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(btnColX, -234f),
                wideBtnSize,
                out _);

            CreateLabel(
                mainPage.transform,
                "MenuFooterHint",
                font,
                "Mẹo: Trong lúc chơi nhấn phím [ESC] hoặc nút [||] góc trên để mở lại Menu & Cài Đặt",
                18,
                new Color(0.78f, 0.78f, 0.84f, 1f),
                new Vector2(0f, -340f),
                new Vector2(1100f, 36f),
                TextAnchor.MiddleCenter);

            // --- BẢNG CÀI ĐẶT (SETTINGS MODAL: TĂNG/GIẢM ÂM THANH & GIẢM/TĂNG ĐỒ HỌA) ---
            GameObject settingsModal = new GameObject("SettingsModal");
            settingsModal.transform.SetParent(overlayObj.transform, false);
            RectTransform settingsRect = settingsModal.AddComponent<RectTransform>();
            settingsRect.anchorMin = new Vector2(0.5f, 0.5f);
            settingsRect.anchorMax = new Vector2(0.5f, 0.5f);
            settingsRect.pivot = new Vector2(0.5f, 0.5f);
            settingsRect.anchoredPosition = Vector2.zero;
            settingsRect.sizeDelta = new Vector2(1060f, 760f);

            Image settingsBg = settingsModal.AddComponent<Image>();
            settingsBg.sprite = retroPanelSprite;
            settingsBg.color = Color.white;

            CreateLabel(
                settingsModal.transform,
                "SettingsHeader",
                font,
                "CÀI ĐẶT TRÒ CHƠI (SETTINGS)",
                40,
                goldAccent,
                new Vector2(0f, 295f),
                new Vector2(960f, 56f),
                TextAnchor.MiddleCenter);

            // 1. Điều chỉnh Âm Lượng Nhạc Nền
            Text musicValTxt = CreateLabel(
                settingsModal.transform,
                "MusicVolumeLabel",
                font,
                "ÂM LƯỢNG NHẠC NỀN: 55%",
                25,
                Color.white,
                new Vector2(0f, 210f),
                new Vector2(880f, 38f),
                TextAnchor.MiddleCenter);

            Button musicMinusBtn = CreateRetroGoldButton(
                settingsModal.transform, "Btn_MusicMinus", goldSquareSprite, font, "◄-", 28, darkBrownText,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                new Vector2(-340f, 145f), new Vector2(82f, 74f), out _);

            Slider musicSlider = CreateInteractiveSlider(
                settingsModal.transform,
                "MusicVolumeSlider",
                barSprite,
                new Vector2(-25f, 145f),
                new Vector2(500f, 38f),
                new Color(0.12f, 0.11f, 0.16f, 1f),
                goldAccent,
                0.55f);

            Button musicPlusBtn = CreateRetroGoldButton(
                settingsModal.transform, "Btn_MusicPlus", goldSquareSprite, font, "+►", 28, darkBrownText,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                new Vector2(290f, 145f), new Vector2(82f, 74f), out _);

            Button settingsMusicMuteBtn = CreateRetroGoldButton(
                settingsModal.transform, "Btn_SettingsMusicMute", goldSquareSprite, font, "♪", 30, darkBrownText,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                new Vector2(390f, 145f), new Vector2(82f, 74f), out Text musicMuteIconTxt);

            // 2. Điều chỉnh Âm Lượng Tiếng Súng / Hiệu Ứng (SFX)
            Text sfxValTxt = CreateLabel(
                settingsModal.transform,
                "SfxVolumeLabel",
                font,
                "ÂM THANH TIẾNG SÚNG (SFX): 85%",
                25,
                Color.white,
                new Vector2(0f, 62f),
                new Vector2(880f, 38f),
                TextAnchor.MiddleCenter);

            Button sfxMinusBtn = CreateRetroGoldButton(
                settingsModal.transform, "Btn_SfxMinus", goldSquareSprite, font, "◄-", 28, darkBrownText,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                new Vector2(-340f, -3f), new Vector2(82f, 74f), out _);

            Slider sfxSlider = CreateInteractiveSlider(
                settingsModal.transform,
                "SfxVolumeSlider",
                barSprite,
                new Vector2(-25f, -3f),
                new Vector2(500f, 38f),
                new Color(0.12f, 0.11f, 0.16f, 1f),
                new Color(1f, 0.68f, 0.16f, 1f),
                0.85f);

            Button sfxPlusBtn = CreateRetroGoldButton(
                settingsModal.transform, "Btn_SfxPlus", goldSquareSprite, font, "+►", 28, darkBrownText,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                new Vector2(290f, -3f), new Vector2(82f, 74f), out _);

            Button settingsSfxMuteBtn = CreateRetroGoldButton(
                settingsModal.transform, "Btn_SettingsSfxMute", goldSquareSprite, font, "🔊", 28, darkBrownText,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                new Vector2(390f, -3f), new Vector2(82f, 74f), out Text sfxMuteIconTxt);

            // 3. Điều chỉnh Chất Lượng Đồ Họa (Giảm Đồ Họa / Tăng Đồ Họa)
            Text graphicsTierTxt = CreateLabel(
                settingsModal.transform,
                "GraphicsTierLabel",
                font,
                "CHẤT LƯỢNG ĐỒ HỌA: CAO (ĐẦY ĐỦ CHI TIẾT)",
                25,
                goldAccent,
                new Vector2(0f, -90f),
                new Vector2(920f, 38f),
                TextAnchor.MiddleCenter);

            Button graphicsPrevBtn = CreateRetroGoldButton(
                settingsModal.transform, "Btn_GraphicsPrev", goldSquareSprite, font, "◄", 32, darkBrownText,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                new Vector2(-410f, -162f), new Vector2(82f, 78f), out _);

            Button graphicsLowBtn = CreateRetroGoldButton(
                settingsModal.transform, "Btn_GraphicsLow", goldBtnSprite, font, "GIẢM ĐỒ HỌA (THẤP)", 21, darkBrownText,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                new Vector2(-235f, -162f), new Vector2(235f, 78f), out _);

            Button graphicsMedBtn = CreateRetroGoldButton(
                settingsModal.transform, "Btn_GraphicsMed", goldBtnSprite, font, "TRUNG BÌNH", 22, darkBrownText,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                new Vector2(0f, -162f), new Vector2(215f, 78f), out _);

            Button graphicsHighBtn = CreateRetroGoldButton(
                settingsModal.transform, "Btn_GraphicsHigh", goldBtnSprite, font, "ĐỒ HỌA CAO", 22, darkBrownText,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                new Vector2(235f, -162f), new Vector2(235f, 78f), out _);

            Button graphicsNextBtn = CreateRetroGoldButton(
                settingsModal.transform, "Btn_GraphicsNext", goldSquareSprite, font, "►", 32, darkBrownText,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                new Vector2(410f, -162f), new Vector2(82f, 78f), out _);

            // 4. Hàng nút Đóng Cài Đặt & Vào Trận Ngay
            Button settingsConfirmBtn = CreateRetroGoldButton(
                settingsModal.transform, "Btn_SettingsBack", goldBtnSprite, font, "✓ LƯU & QUAY LẠI", 27, darkBrownText,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                new Vector2(-210f, -285f), new Vector2(370f, 88f), out _);

            Button settingsPlayNowBtn = CreateRetroGoldButton(
                settingsModal.transform, "Btn_SettingsPlayNow", goldBtnSprite, font, "▶ VÀO CHƠI NGAY", 27, darkBrownText,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                new Vector2(210f, -285f), new Vector2(370f, 88f), out _);

            settingsModal.SetActive(false);

            // --- BẢNG HƯỚNG DẪN & KHO SÚNG (HELP MODAL) ---
            GameObject helpModal = new GameObject("HelpModal");
            helpModal.transform.SetParent(overlayObj.transform, false);
            RectTransform helpRect = helpModal.AddComponent<RectTransform>();
            helpRect.anchorMin = new Vector2(0.5f, 0.5f);
            helpRect.anchorMax = new Vector2(0.5f, 0.5f);
            helpRect.pivot = new Vector2(0.5f, 0.5f);
            helpRect.anchoredPosition = Vector2.zero;
            helpRect.sizeDelta = new Vector2(980f, 660f);

            Image helpBg = helpModal.AddComponent<Image>();
            helpBg.sprite = retroPanelSprite;
            helpBg.color = Color.white;

            CreateLabel(
                helpModal.transform,
                "HelpTitle",
                font,
                "HƯỚNG DẪN ĐIỀU KHIỂN & NÂNG CẤP SÚNG",
                36,
                goldAccent,
                new Vector2(0f, 240f),
                new Vector2(900f, 54f),
                TextAnchor.MiddleCenter);

            CreateLabel(
                helpModal.transform,
                "HelpBody",
                font,
                "• Di chuyển 4 hướng: Phím [W][A][S][D] hoặc 4 phím Mũi Tên.\n" +
                "• Ngắm bắn: Xoay chuột về phía kẻ địch (Nhân vật tự động bắn về phía trước).\n" +
                "• Khám phá các Ngôi Nhà: Bước qua cửa vào bên trong các ngôi nhà sàn gỗ để nhặt Súng Mới.\n" +
                "• 7 Khẩu Súng Nâng Cấp: Súng Lục USP -> Tiểu Liên MP5 -> Shotgun M1014 (5 tia) ->\n" +
                "  Súng Trường AK-47 -> Bắn Tỉa AWP (Xuyên 4) -> Súng Máy 6 Nòng M134 -> Pháo Điện Từ Plasma.\n" +
                "• Phím [M]: Phóng to / Thu nhỏ Mini Map   |   Phím [ESC]: Mở Menu & Cài Đặt.",
                23,
                Color.white,
                new Vector2(0f, 20f),
                new Vector2(880f, 320f),
                TextAnchor.MiddleLeft);

            Button helpBackBtn = CreateRetroGoldButton(
                helpModal.transform, "Btn_HelpBack", goldBtnSprite, font, "✓ ĐÃ HIỂU (QUAY LẠI)", 27, darkBrownText,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                new Vector2(0f, -230f), new Vector2(400f, 88f), out _);

            helpModal.SetActive(false);

            // Gán toàn bộ tham chiếu vào MainMenuManager
            menuMgr.mainMenuOverlay = overlayObj;
            menuMgr.mainButtonsPage = mainPage;
            menuMgr.settingsModal = settingsModal;
            menuMgr.helpModal = helpModal;

            menuMgr.startButton = startBtn;
            menuMgr.newGameButton = newGameBtn;
            menuMgr.settingsButton = settingsBtn;
            menuMgr.exitButton = exitBtn;
            menuMgr.hudPauseButton = hudPauseBtn;

            menuMgr.musicSlider = musicSlider;
            menuMgr.musicMinusBtn = musicMinusBtn;
            menuMgr.musicPlusBtn = musicPlusBtn;
            menuMgr.settingsMusicMuteBtn = settingsMusicMuteBtn;
            menuMgr.musicValueText = musicValTxt;
            menuMgr.musicMuteBtnLabel = musicMuteIconTxt;

            menuMgr.sfxSlider = sfxSlider;
            menuMgr.sfxMinusBtn = sfxMinusBtn;
            menuMgr.sfxPlusBtn = sfxPlusBtn;
            menuMgr.settingsSfxMuteBtn = settingsSfxMuteBtn;
            menuMgr.sfxValueText = sfxValTxt;
            menuMgr.sfxMuteBtnLabel = sfxMuteIconTxt;

            menuMgr.graphicsPrevBtn = graphicsPrevBtn;
            menuMgr.graphicsNextBtn = graphicsNextBtn;
            menuMgr.graphicsLowBtn = graphicsLowBtn;
            menuMgr.graphicsMedBtn = graphicsMedBtn;
            menuMgr.graphicsHighBtn = graphicsHighBtn;
            menuMgr.graphicsTierText = graphicsTierTxt;

            menuMgr.settingsConfirmBtn = settingsConfirmBtn;
            menuMgr.settingsPlayNowBtn = settingsPlayNowBtn;
            menuMgr.helpBackBtn = helpBackBtn;

            menuMgr.BindUIInteractions();
        }

        private static Button CreateRetroGoldButton(
            Transform parent,
            string name,
            Sprite buttonSprite,
            Font font,
            string text,
            int fontSize,
            Color textColor,
            Vector2 anchorMin,
            Vector2 anchorMax,
            Vector2 pivot,
            Vector2 anchoredPos,
            Vector2 size,
            out Text labelOut)
        {
            GameObject btnObj = new GameObject(name);
            btnObj.transform.SetParent(parent, false);
            RectTransform rect = btnObj.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = pivot;
            rect.anchoredPosition = anchoredPos;
            rect.sizeDelta = size;

            Image img = btnObj.AddComponent<Image>();
            img.sprite = buttonSprite;
            img.color = Color.white;
            img.raycastTarget = true;

            Button btn = btnObj.AddComponent<Button>();
            btn.targetGraphic = img;
            ColorBlock cb = btn.colors;
            cb.normalColor = Color.white;
            cb.highlightedColor = new Color(1f, 0.96f, 0.78f, 1f);
            cb.pressedColor = new Color(0.84f, 0.68f, 0.16f, 1f);
            cb.selectedColor = new Color(1f, 0.96f, 0.78f, 1f);
            btn.colors = cb;

            GameObject txtObj = new GameObject("Label");
            txtObj.transform.SetParent(btnObj.transform, false);
            RectTransform txtRect = txtObj.AddComponent<RectTransform>();
            txtRect.anchorMin = Vector2.zero;
            txtRect.anchorMax = Vector2.one;
            txtRect.offsetMin = new Vector2(8f, 4f);
            txtRect.offsetMax = new Vector2(-8f, -4f);

            labelOut = txtObj.AddComponent<Text>();
            labelOut.font = font;
            labelOut.fontSize = fontSize;
            labelOut.fontStyle = FontStyle.Bold;
            labelOut.alignment = TextAnchor.MiddleCenter;
            labelOut.color = textColor;
            labelOut.text = text;
            labelOut.raycastTarget = false;

            return btn;
        }

        private static Text CreateLabel(
            Transform parent,
            string name,
            Font font,
            string content,
            int fontSize,
            Color color,
            Vector2 anchoredPos,
            Vector2 size,
            TextAnchor alignment)
        {
            GameObject obj = new GameObject(name);
            obj.transform.SetParent(parent, false);
            RectTransform rect = obj.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = anchoredPos;
            rect.sizeDelta = size;

            Text txt = obj.AddComponent<Text>();
            txt.font = font;
            txt.fontSize = fontSize;
            txt.fontStyle = FontStyle.Bold;
            txt.alignment = alignment;
            txt.color = color;
            txt.text = content;
            txt.raycastTarget = false;
            return txt;
        }

        private static Slider CreateInteractiveSlider(
            Transform parent,
            string name,
            Sprite barSprite,
            Vector2 anchoredPos,
            Vector2 size,
            Color bgColor,
            Color fillColor,
            float defaultValue)
        {
            GameObject sliderObj = new GameObject(name);
            sliderObj.transform.SetParent(parent, false);
            RectTransform sliderRect = sliderObj.AddComponent<RectTransform>();
            sliderRect.anchorMin = new Vector2(0.5f, 0.5f);
            sliderRect.anchorMax = new Vector2(0.5f, 0.5f);
            sliderRect.pivot = new Vector2(0.5f, 0.5f);
            sliderRect.anchoredPosition = anchoredPos;
            sliderRect.sizeDelta = size;

            Slider slider = sliderObj.AddComponent<Slider>();
            slider.interactable = true;

            GameObject bgObj = new GameObject("Background");
            bgObj.transform.SetParent(sliderObj.transform, false);
            RectTransform bgRect = bgObj.AddComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;

            Image bgImg = bgObj.AddComponent<Image>();
            bgImg.sprite = barSprite;
            bgImg.color = bgColor;
            bgImg.raycastTarget = true;

            GameObject fillArea = new GameObject("Fill Area");
            fillArea.transform.SetParent(sliderObj.transform, false);
            RectTransform fillAreaRect = fillArea.AddComponent<RectTransform>();
            fillAreaRect.anchorMin = Vector2.zero;
            fillAreaRect.anchorMax = Vector2.one;
            fillAreaRect.offsetMin = new Vector2(4f, 4f);
            fillAreaRect.offsetMax = new Vector2(-4f, -4f);

            GameObject fillObj = new GameObject("Fill");
            fillObj.transform.SetParent(fillArea.transform, false);
            RectTransform fillRect = fillObj.AddComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.offsetMin = Vector2.zero;
            fillRect.offsetMax = Vector2.zero;

            Image fillImg = fillObj.AddComponent<Image>();
            fillImg.sprite = barSprite;
            fillImg.color = fillColor;
            fillImg.raycastTarget = false;

            GameObject handleArea = new GameObject("Handle Slide Area");
            handleArea.transform.SetParent(sliderObj.transform, false);
            RectTransform handleAreaRect = handleArea.AddComponent<RectTransform>();
            handleAreaRect.anchorMin = Vector2.zero;
            handleAreaRect.anchorMax = Vector2.one;
            handleAreaRect.offsetMin = new Vector2(10f, 0f);
            handleAreaRect.offsetMax = new Vector2(-10f, 0f);

            GameObject handleObj = new GameObject("Handle");
            handleObj.transform.SetParent(handleArea.transform, false);
            RectTransform handleRect = handleObj.AddComponent<RectTransform>();
            handleRect.sizeDelta = new Vector2(24f, size.y + 10f);

            Image handleImg = handleObj.AddComponent<Image>();
            handleImg.sprite = barSprite;
            handleImg.color = new Color(1f, 0.98f, 0.85f, 1f);
            handleImg.raycastTarget = true;

            slider.fillRect = fillRect;
            slider.handleRect = handleRect;
            slider.targetGraphic = handleImg;
            slider.direction = Slider.Direction.LeftToRight;
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.value = defaultValue;

            return slider;
        }

        private static void BuildMinimapUI(Transform canvasTransform, Sprite barSprite, Font font)
        {
            GameObject minimapPanel = new GameObject("MinimapPanel");
            minimapPanel.transform.SetParent(canvasTransform, false);
            RectTransform mmRect = minimapPanel.AddComponent<RectTransform>();
            mmRect.anchorMin = new Vector2(1f, 1f);
            mmRect.anchorMax = new Vector2(1f, 1f);
            mmRect.pivot = new Vector2(1f, 1f);
            mmRect.anchoredPosition = new Vector2(-24f, -20f);
            mmRect.sizeDelta = new Vector2(276f, 326f);

            Image mmBg = minimapPanel.AddComponent<Image>();
            mmBg.sprite = barSprite;
            mmBg.color = new Color(0.05f, 0.08f, 0.13f, 0.88f);
            mmBg.raycastTarget = false;

            GameObject zoneTitleObj = new GameObject("ZoneTitleText");
            zoneTitleObj.transform.SetParent(minimapPanel.transform, false);
            RectTransform ztRect = zoneTitleObj.AddComponent<RectTransform>();
            ztRect.anchorMin = new Vector2(0f, 1f);
            ztRect.anchorMax = new Vector2(1f, 1f);
            ztRect.pivot = new Vector2(0.5f, 1f);
            ztRect.anchoredPosition = new Vector2(0f, -8f);
            ztRect.sizeDelta = new Vector2(-12f, 32f);

            Text ztText = zoneTitleObj.AddComponent<Text>();
            ztText.font = font;
            ztText.fontSize = 20;
            ztText.fontStyle = FontStyle.Bold;
            ztText.alignment = TextAnchor.MiddleCenter;
            ztText.color = new Color(0.35f, 0.92f, 1f, 1f);
            ztText.text = "KHU TRUNG TÂM";
            ztText.raycastTarget = false;

            GameObject rawObj = new GameObject("MinimapRadar");
            rawObj.transform.SetParent(minimapPanel.transform, false);
            RectTransform rawRect = rawObj.AddComponent<RectTransform>();
            rawRect.anchorMin = new Vector2(0.5f, 0.5f);
            rawRect.anchorMax = new Vector2(0.5f, 0.5f);
            rawRect.pivot = new Vector2(0.5f, 0.5f);
            rawRect.anchoredPosition = new Vector2(0f, -2f);
            rawRect.sizeDelta = new Vector2(236f, 236f);

            RawImage rawImage = rawObj.AddComponent<RawImage>();
            rawImage.raycastTarget = false;

            GameObject northObj = new GameObject("CompassNorth");
            northObj.transform.SetParent(rawObj.transform, false);
            RectTransform nRect = northObj.AddComponent<RectTransform>();
            nRect.anchorMin = new Vector2(0.5f, 1f);
            nRect.anchorMax = new Vector2(0.5f, 1f);
            nRect.pivot = new Vector2(0.5f, 1f);
            nRect.anchoredPosition = new Vector2(0f, -4f);
            nRect.sizeDelta = new Vector2(36f, 22f);

            Text nText = northObj.AddComponent<Text>();
            nText.font = font;
            nText.fontSize = 15;
            nText.fontStyle = FontStyle.Bold;
            nText.alignment = TextAnchor.UpperCenter;
            nText.color = new Color(1f, 0.9f, 0.3f, 0.9f);
            nText.text = "B";
            nText.raycastTarget = false;

            GameObject legendObj = new GameObject("MinimapLegend");
            legendObj.transform.SetParent(minimapPanel.transform, false);
            RectTransform legRect = legendObj.AddComponent<RectTransform>();
            legRect.anchorMin = new Vector2(0f, 0f);
            legRect.anchorMax = new Vector2(1f, 0f);
            legRect.pivot = new Vector2(0.5f, 0f);
            legRect.anchoredPosition = new Vector2(0f, 6f);
            legRect.sizeDelta = new Vector2(-10f, 34f);

            Text legText = legendObj.AddComponent<Text>();
            legText.font = font;
            legText.fontSize = 13;
            legText.alignment = TextAnchor.MiddleCenter;
            legText.color = new Color(0.85f, 0.90f, 0.95f, 1f);
            legText.text = "Vàng/Xanh: Tiếp Tế | Đỏ: Địch | [M]: Bản Đồ";
            legText.raycastTarget = false;

            MinimapController controller = minimapPanel.GetComponent<MinimapController>();
            if (controller == null)
            {
                controller = minimapPanel.AddComponent<MinimapController>();
            }
            controller.mapRawImage = rawImage;
            controller.zoneTitleText = ztText;
            controller.legendText = legText;
            controller.RenderMinimap();
        }

        private static void DestroyChildIfExists(Transform parent, string childName)
        {
            Transform existing = parent.Find(childName);
            if (existing != null)
            {
                if (Application.isPlaying)
                    Object.Destroy(existing.gameObject);
                else
                    Object.DestroyImmediate(existing.gameObject);
            }
        }

        private static Slider CreateProgressBar(
            Transform parent,
            string name,
            Sprite barSprite,
            Sprite glossyFillSprite,
            Sprite iconBadgeSprite,
            Font font,
            string iconSymbol,
            Color iconColor,
            Vector2 anchoredPos,
            Vector2 size,
            Color bgColor,
            Color fillColor,
            string defaultLabel,
            out Image fillImg,
            out Text labelTxt)
        {
            GameObject containerObj = new GameObject(name + "_Container");
            containerObj.transform.SetParent(parent, false);
            RectTransform containerRect = containerObj.AddComponent<RectTransform>();
            containerRect.anchorMin = new Vector2(0f, 1f);
            containerRect.anchorMax = new Vector2(0f, 1f);
            containerRect.pivot = new Vector2(0f, 1f);
            containerRect.anchoredPosition = anchoredPos;
            containerRect.sizeDelta = size;

            float iconWidth = size.y + 4f;

            // Huy hiệu Icon bên trái Thanh Máu / EXP (❤ hoặc ★)
            GameObject iconBox = new GameObject("IconBadge");
            iconBox.transform.SetParent(containerObj.transform, false);
            RectTransform iconRect = iconBox.AddComponent<RectTransform>();
            iconRect.anchorMin = new Vector2(0f, 0f);
            iconRect.anchorMax = new Vector2(0f, 1f);
            iconRect.pivot = new Vector2(0f, 0.5f);
            iconRect.anchoredPosition = Vector2.zero;
            iconRect.sizeDelta = new Vector2(iconWidth, 0f);

            Image iconBg = iconBox.AddComponent<Image>();
            iconBg.sprite = iconBadgeSprite != null ? iconBadgeSprite : barSprite;
            iconBg.color = Color.white;
            iconBg.raycastTarget = false;

            GameObject iconTxtObj = new GameObject("IconSymbol");
            iconTxtObj.transform.SetParent(iconBox.transform, false);
            RectTransform iconTxtRect = iconTxtObj.AddComponent<RectTransform>();
            iconTxtRect.anchorMin = Vector2.zero;
            iconTxtRect.anchorMax = Vector2.one;
            iconTxtRect.offsetMin = Vector2.zero;
            iconTxtRect.offsetMax = Vector2.zero;

            Text iconTxt = iconTxtObj.AddComponent<Text>();
            iconTxt.font = font;
            iconTxt.fontSize = 24;
            iconTxt.fontStyle = FontStyle.Bold;
            iconTxt.alignment = TextAnchor.MiddleCenter;
            iconTxt.color = iconColor;
            iconTxt.text = iconSymbol;
            iconTxt.raycastTarget = false;

            Outline iconOutline = iconTxtObj.AddComponent<Outline>();
            iconOutline.effectColor = new Color(0.15f, 0.08f, 0.02f, 0.95f);
            iconOutline.effectDistance = new Vector2(1.2f, -1.2f);

            // Phần thanh trượt chính bên phải Icon
            GameObject sliderObj = new GameObject(name);
            sliderObj.transform.SetParent(containerObj.transform, false);
            RectTransform sliderRect = sliderObj.AddComponent<RectTransform>();
            sliderRect.anchorMin = new Vector2(0f, 0f);
            sliderRect.anchorMax = new Vector2(1f, 1f);
            sliderRect.offsetMin = new Vector2(iconWidth + 6f, 2f);
            sliderRect.offsetMax = new Vector2(0f, -2f);

            Slider slider = sliderObj.AddComponent<Slider>();
            slider.interactable = false;
            slider.transition = Selectable.Transition.None;

            // Viền vàng kim loại bao quanh thanh máu
            GameObject outerBorderObj = new GameObject("OuterMetallicFrame");
            outerBorderObj.transform.SetParent(sliderObj.transform, false);
            RectTransform outerBorderRect = outerBorderObj.AddComponent<RectTransform>();
            outerBorderRect.anchorMin = Vector2.zero;
            outerBorderRect.anchorMax = Vector2.one;
            outerBorderRect.offsetMin = Vector2.zero;
            outerBorderRect.offsetMax = Vector2.zero;

            Image outerBorderImg = outerBorderObj.AddComponent<Image>();
            outerBorderImg.sprite = barSprite;
            outerBorderImg.color = new Color(0.86f, 0.72f, 0.24f, 0.95f);
            outerBorderImg.raycastTarget = false;

            // Nền tối bên trong thanh
            GameObject bgObj = new GameObject("Background");
            bgObj.transform.SetParent(sliderObj.transform, false);
            RectTransform bgRect = bgObj.AddComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = new Vector2(3f, 3f);
            bgRect.offsetMax = new Vector2(-3f, -3f);

            Image bgImg = bgObj.AddComponent<Image>();
            bgImg.sprite = barSprite;
            bgImg.color = bgColor;
            bgImg.raycastTarget = false;

            GameObject fillArea = new GameObject("Fill Area");
            fillArea.transform.SetParent(sliderObj.transform, false);
            RectTransform fillAreaRect = fillArea.AddComponent<RectTransform>();
            fillAreaRect.anchorMin = Vector2.zero;
            fillAreaRect.anchorMax = Vector2.one;
            fillAreaRect.offsetMin = new Vector2(5f, 5f);
            fillAreaRect.offsetMax = new Vector2(-5f, -5f);

            GameObject fillObj = new GameObject("Fill");
            fillObj.transform.SetParent(fillArea.transform, false);
            RectTransform fillRect = fillObj.AddComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.offsetMin = Vector2.zero;
            fillRect.offsetMax = Vector2.zero;

            fillImg = fillObj.AddComponent<Image>();
            fillImg.sprite = glossyFillSprite != null ? glossyFillSprite : barSprite;
            fillImg.color = fillColor;
            fillImg.type = Image.Type.Simple;
            fillImg.raycastTarget = false;

            // Vạch phản quang sáng bóng phía trên thanh máu/EXP
            GameObject shineObj = new GameObject("TopGlossHighlight");
            shineObj.transform.SetParent(fillObj.transform, false);
            RectTransform shineRect = shineObj.AddComponent<RectTransform>();
            shineRect.anchorMin = new Vector2(0f, 0.65f);
            shineRect.anchorMax = new Vector2(1f, 0.94f);
            shineRect.offsetMin = new Vector2(2f, 0f);
            shineRect.offsetMax = new Vector2(-2f, 0f);

            Image shineImg = shineObj.AddComponent<Image>();
            shineImg.sprite = barSprite;
            shineImg.color = new Color(1f, 1f, 1f, 0.26f);
            shineImg.raycastTarget = false;

            slider.fillRect = fillRect;
            slider.targetGraphic = fillImg;
            slider.direction = Slider.Direction.LeftToRight;
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.value = 1f;

            GameObject txtObj = new GameObject("Label");
            txtObj.transform.SetParent(sliderObj.transform, false);
            RectTransform txtRect = txtObj.AddComponent<RectTransform>();
            txtRect.anchorMin = Vector2.zero;
            txtRect.anchorMax = Vector2.one;
            txtRect.offsetMin = Vector2.zero;
            txtRect.offsetMax = Vector2.zero;

            labelTxt = txtObj.AddComponent<Text>();
            labelTxt.font = font;
            labelTxt.fontSize = 19;
            labelTxt.fontStyle = FontStyle.Bold;
            labelTxt.alignment = TextAnchor.MiddleCenter;
            labelTxt.color = Color.white;
            labelTxt.text = defaultLabel;
            labelTxt.raycastTarget = false;

            Outline txtOutline = txtObj.AddComponent<Outline>();
            txtOutline.effectColor = new Color(0f, 0f, 0f, 0.92f);
            txtOutline.effectDistance = new Vector2(1.4f, -1.4f);

            return slider;
        }
    }
}
