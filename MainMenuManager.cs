using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace TopDownShooter
{
    /// <summary>
    /// Quản lý Màn Hình Menu Trước Khi Vào Game & Bảng Cài Đặt theo phong cách Nút Vàng Pixel-Art (giống ảnh mẫu):
    /// - Nút BẮT ĐẦU TRÒ CHƠI (START)
    /// - Nút CHƠI MỚI (NEW GAME)
    /// - Nút CÀI ĐẶT (SETTINGS): Tăng/Giảm Âm Thanh (Nhạc nền & Hiệu ứng súng) và Tăng/Giảm Đồ Họa (Thấp / Trung Bình / Cao)
    /// - Nút THOÁT GAME (EXIT)
    /// - Cụm nút vuông Icon nhanh bên phải giống ảnh mẫu (Nhạc nền, Loa, Cài đặt, Tăng/Giảm, Hướng dẫn, Thoát).
    /// </summary>
    public class MainMenuManager : MonoBehaviour
    {
        public static MainMenuManager Instance { get; private set; }

        /// <summary>
        /// Khi người chơi chết và tự động khởi động lại màn chơi, đặt cờ này để vào thẳng trận đấu.
        /// </summary>
        public static bool SkipMenuOnNextReload = false;

        private const string PrefGraphicsTier = "TDS_GraphicsTier";

        [Header("Các Panel Giao Diện Menu")]
        public GameObject mainMenuOverlay;
        public GameObject mainButtonsPage;
        public GameObject settingsModal;
        public GameObject helpModal;

        [Header("Nút Chính (Cột Trái Giống Ảnh Mẫu)")]
        public Button startButton;
        public Button newGameButton;
        public Button settingsButton;
        public Button exitButton;
        public Button hudPauseButton;

        [Header("Nút Vuông Icon Nhanh (Cụm Phải Giống Ảnh Mẫu)")]
        public Button iconCheckStartBtn;
        public Button iconCloseExitBtn;
        public Button iconWrenchSettingsBtn;
        public Button iconHelpBtn;
        public Button iconMusicToggleBtn;
        public Button iconSfxToggleBtn;
        public Button iconVolDownBtn;
        public Button iconVolUpBtn;
        public Button iconRestartBtn;
        public Button iconGraphicsCycleBtn;
        public Button iconPlayTriangleBtn;

        [Header("Nút & Thanh Trượt Trong Bảng Cài Đặt (Settings Modal)")]
        public Slider musicSlider;
        public Button musicMinusBtn;
        public Button musicPlusBtn;
        public Button settingsMusicMuteBtn;
        public Text musicValueText;
        public Text musicMuteBtnLabel;

        public Slider sfxSlider;
        public Button sfxMinusBtn;
        public Button sfxPlusBtn;
        public Button settingsSfxMuteBtn;
        public Text sfxValueText;
        public Text sfxMuteBtnLabel;

        public Button graphicsPrevBtn;
        public Button graphicsNextBtn;
        public Button graphicsLowBtn;
        public Button graphicsMedBtn;
        public Button graphicsHighBtn;
        public Text graphicsTierText;

        public Button settingsConfirmBtn;
        public Button settingsPlayNowBtn;
        public Button helpBackBtn;
        public Text quickStatusText;

        private int _graphicsTier = 2; // 0 = Thấp (Giảm Đồ Họa), 1 = Trung Bình, 2 = Cao
        private bool _isMenuOpen = true;
        private bool _suppressSliderCallback;
        private bool _listenersBound;

        public bool IsMenuOpen => _isMenuOpen;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStaticFlags()
        {
            SkipMenuOnNextReload = false;
        }

        private void Awake()
        {
            Instance = this;
            _graphicsTier = Mathf.Clamp(PlayerPrefs.GetInt(PrefGraphicsTier, 2), 0, 2);
            BindUIInteractions();
        }

        private void Start()
        {
            BindUIInteractions();
            ApplyGraphicsQuality(_graphicsTier, false);
            SyncSettingsUIFromManagers();

            if (SkipMenuOnNextReload)
            {
                SkipMenuOnNextReload = false;
                StartOrResumeGame(false);
            }
            else
            {
                OpenMainMenu();
            }
        }

        /// <summary>
        /// Liên kết sự kiện click cho toàn bộ các nút để hoạt động cả khi load trực tiếp từ Scene đã lưu.
        /// </summary>
        public void BindUIInteractions()
        {
            if (_listenersBound) return;
            if (startButton == null && mainMenuOverlay == null) return;
            _listenersBound = true;

            BindBtn(startButton, () => StartOrResumeGame(true));
            BindBtn(newGameButton, StartNewGame);
            BindBtn(settingsButton, OpenSettingsPanel);
            BindBtn(exitButton, ExitGame);
            BindBtn(hudPauseButton, TogglePauseMenuFromHud);

            BindBtn(iconCheckStartBtn, () => StartOrResumeGame(true));
            BindBtn(iconCloseExitBtn, ExitGame);
            BindBtn(iconWrenchSettingsBtn, OpenSettingsPanel);
            BindBtn(iconHelpBtn, OpenHelpPanel);
            BindBtn(iconMusicToggleBtn, ToggleMusicMute);
            BindBtn(iconSfxToggleBtn, ToggleSfxMute);
            BindBtn(iconVolDownBtn, () => AdjustAllVolumeQuick(-0.1f));
            BindBtn(iconVolUpBtn, () => AdjustAllVolumeQuick(0.1f));
            BindBtn(iconRestartBtn, StartNewGame);
            BindBtn(iconGraphicsCycleBtn, CycleGraphicsQuality);
            BindBtn(iconPlayTriangleBtn, () => StartOrResumeGame(true));

            BindBtn(musicMinusBtn, () => AdjustMusicVolume(-0.1f));
            BindBtn(musicPlusBtn, () => AdjustMusicVolume(0.1f));
            BindBtn(settingsMusicMuteBtn, ToggleMusicMute);

            BindBtn(sfxMinusBtn, () => AdjustSfxVolume(-0.1f));
            BindBtn(sfxPlusBtn, () => AdjustSfxVolume(0.1f));
            BindBtn(settingsSfxMuteBtn, ToggleSfxMute);

            BindBtn(graphicsPrevBtn, () => StepGraphicsQuality(-1));
            BindBtn(graphicsNextBtn, () => StepGraphicsQuality(1));
            BindBtn(graphicsLowBtn, () => SetGraphicsQualityTier(0));
            BindBtn(graphicsMedBtn, () => SetGraphicsQualityTier(1));
            BindBtn(graphicsHighBtn, () => SetGraphicsQualityTier(2));

            BindBtn(settingsConfirmBtn, CloseSettingsPanel);
            BindBtn(settingsPlayNowBtn, () => StartOrResumeGame(true));
            BindBtn(helpBackBtn, CloseHelpPanel);

            if (musicSlider != null)
            {
                musicSlider.onValueChanged.RemoveAllListeners();
                musicSlider.onValueChanged.AddListener(OnMusicSliderChanged);
            }

            if (sfxSlider != null)
            {
                sfxSlider.onValueChanged.RemoveAllListeners();
                sfxSlider.onValueChanged.AddListener(OnSfxSliderChanged);
            }
        }

        private static void BindBtn(Button btn, UnityEngine.Events.UnityAction action)
        {
            if (btn == null) return;
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(action);
        }

        private void Update()
        {
            bool escPressed = false;
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                escPressed = true;
            }
#elif ENABLE_LEGACY_INPUT_MANAGER
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                escPressed = true;
            }
#endif
            if (escPressed)
            {
                if (_isMenuOpen)
                {
                    if (settingsModal != null && settingsModal.activeSelf)
                    {
                        CloseSettingsPanel();
                    }
                    else if (helpModal != null && helpModal.activeSelf)
                    {
                        CloseHelpPanel();
                    }
                    else
                    {
                        StartOrResumeGame(true);
                    }
                }
                else
                {
                    OpenMainMenu();
                }
            }
        }

        private void OnDestroy()
        {
            if (Time.timeScale == 0f)
            {
                Time.timeScale = 1f;
            }
        }

        public void TogglePauseMenuFromHud()
        {
            PlayUiClick();
            if (_isMenuOpen)
                StartOrResumeGame(false);
            else
                OpenMainMenu();
        }

        public void OpenMainMenu()
        {
            _isMenuOpen = true;
            Time.timeScale = 0f;

            if (mainMenuOverlay != null) mainMenuOverlay.SetActive(true);
            if (mainButtonsPage != null) mainButtonsPage.SetActive(true);
            if (settingsModal != null) settingsModal.SetActive(false);
            if (helpModal != null) helpModal.SetActive(false);

            SyncSettingsUIFromManagers();
        }

        /// <summary>
        /// Nút BẮT ĐẦU TRÒ CHƠI (START): Đóng menu và vào chơi.
        /// </summary>
        public void StartOrResumeGame(bool playSound = true)
        {
            if (playSound) PlayUiClick();
            _isMenuOpen = false;
            Time.timeScale = 1f;

            if (mainMenuOverlay != null) mainMenuOverlay.SetActive(false);
            if (settingsModal != null) settingsModal.SetActive(false);
            if (helpModal != null) helpModal.SetActive(false);
        }

        /// <summary>
        /// Nút CHƠI MỚI (NEW GAME): Bắt đầu lại màn chơi từ Đợt 1 & Cấp 1.
        /// </summary>
        public void StartNewGame()
        {
            PlayUiClick();
            Time.timeScale = 1f;
            SkipMenuOnNextReload = true;
            Scene active = SceneManager.GetActiveScene();
            SceneManager.LoadScene(active.name);
        }

        /// <summary>
        /// Nút CÀI ĐẶT (SETTINGS): Mở bảng chỉnh Âm Thanh & Đồ Họa.
        /// </summary>
        public void OpenSettingsPanel()
        {
            PlayUiClick();
            if (mainMenuOverlay != null) mainMenuOverlay.SetActive(true);
            if (mainButtonsPage != null) mainButtonsPage.SetActive(false);
            if (helpModal != null) helpModal.SetActive(false);
            if (settingsModal != null) settingsModal.SetActive(true);

            SyncSettingsUIFromManagers();
        }

        public void CloseSettingsPanel()
        {
            PlayUiClick();
            if (settingsModal != null) settingsModal.SetActive(false);
            if (mainButtonsPage != null) mainButtonsPage.SetActive(true);
            PlayerPrefs.Save();
        }

        public void OpenHelpPanel()
        {
            PlayUiClick();
            if (mainMenuOverlay != null) mainMenuOverlay.SetActive(true);
            if (mainButtonsPage != null) mainButtonsPage.SetActive(false);
            if (settingsModal != null) settingsModal.SetActive(false);
            if (helpModal != null) helpModal.SetActive(true);
        }

        public void CloseHelpPanel()
        {
            PlayUiClick();
            if (helpModal != null) helpModal.SetActive(false);
            if (mainButtonsPage != null) mainButtonsPage.SetActive(true);
        }

        /// <summary>
        /// Nút THOÁT GAME (EXIT): Thoát ứng dụng hoặc dừng chế độ Play trong Unity Editor.
        /// </summary>
        public void ExitGame()
        {
            PlayUiClick();
            Time.timeScale = 1f;
            PlayerPrefs.Save();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        // --- ĐIỀU CHỈNH ÂM THANH (TĂNG / GIẢM ÂM LƯỢNG) ---

        public void AdjustMusicVolume(float delta)
        {
            if (AudioManager.Instance == null) return;
            float next = Mathf.Clamp01(AudioManager.Instance.musicVolume + delta);
            AudioManager.Instance.SetMusicVolume(next);
            SyncSettingsUIFromManagers();
            PlayUiClick();
        }

        public void OnMusicSliderChanged(float value)
        {
            if (_suppressSliderCallback || AudioManager.Instance == null) return;
            AudioManager.Instance.SetMusicVolume(value);
            SyncSettingsUIFromManagers();
        }

        public void ToggleMusicMute()
        {
            if (AudioManager.Instance == null) return;
            AudioManager.Instance.ToggleMusicMute();
            SyncSettingsUIFromManagers();
            PlayUiClick();
        }

        public void AdjustSfxVolume(float delta)
        {
            if (AudioManager.Instance == null) return;
            float next = Mathf.Clamp01(AudioManager.Instance.sfxVolume + delta);
            AudioManager.Instance.SetSfxVolume(next);
            SyncSettingsUIFromManagers();
            PlayUiClick();
        }

        public void OnSfxSliderChanged(float value)
        {
            if (_suppressSliderCallback || AudioManager.Instance == null) return;
            AudioManager.Instance.SetSfxVolume(value);
            SyncSettingsUIFromManagers();
        }

        public void ToggleSfxMute()
        {
            if (AudioManager.Instance == null) return;
            AudioManager.Instance.ToggleSfxMute();
            SyncSettingsUIFromManagers();
            PlayUiClick();
        }

        public void AdjustAllVolumeQuick(float delta)
        {
            if (AudioManager.Instance == null) return;
            AudioManager.Instance.SetMusicVolume(Mathf.Clamp01(AudioManager.Instance.musicVolume + delta));
            AudioManager.Instance.SetSfxVolume(Mathf.Clamp01(AudioManager.Instance.sfxVolume + delta));
            SyncSettingsUIFromManagers();
            PlayUiClick();
        }

        // --- ĐIỀU CHỈNH ĐỒ HỌA (GIẢM / TĂNG ĐỒ HỌA) ---

        public void StepGraphicsQuality(int delta)
        {
            int nextTier = Mathf.Clamp(_graphicsTier + delta, 0, 2);
            ApplyGraphicsQuality(nextTier, true);
        }

        public void CycleGraphicsQuality()
        {
            int nextTier = (_graphicsTier + 1) % 3;
            ApplyGraphicsQuality(nextTier, true);
        }

        public void SetGraphicsQualityTier(int tier)
        {
            ApplyGraphicsQuality(Mathf.Clamp(tier, 0, 2), true);
        }

        private void ApplyGraphicsQuality(int tier, bool playSound)
        {
            _graphicsTier = Mathf.Clamp(tier, 0, 2);
            PlayerPrefs.SetInt(PrefGraphicsTier, _graphicsTier);

            int totalUnityQualityLevels = QualitySettings.names != null ? QualitySettings.names.Length : 0;
            if (totalUnityQualityLevels > 0)
            {
                int mappedIndex = 0;
                if (_graphicsTier == 1) mappedIndex = totalUnityQualityLevels / 2;
                else if (_graphicsTier == 2) mappedIndex = totalUnityQualityLevels - 1;

                QualitySettings.SetQualityLevel(mappedIndex, false);
            }

            if (WorldMapManager.Instance != null)
            {
                WorldMapManager.Instance.ApplyGraphicsDetailLevel(_graphicsTier);
            }

            if (MinimapController.Instance != null)
            {
                MinimapController.Instance.refreshInterval = _graphicsTier == 0 ? 0.30f : (_graphicsTier == 1 ? 0.16f : 0.10f);
            }

            SyncSettingsUIFromManagers();
            if (playSound) PlayUiClick();
        }

        public string GetGraphicsTierDisplayName()
        {
            switch (_graphicsTier)
            {
                case 0:
                    return "THẤP (GIẢM ĐỒ HỌA - TỐI ƯU FPS)";
                case 1:
                    return "TRUNG BÌNH (CÂN BẰNG)";
                case 2:
                default:
                    return "CAO (ĐẦY ĐỦ CHI TIẾT)";
            }
        }

        public void SyncSettingsUIFromManagers()
        {
            _suppressSliderCallback = true;

            float musicVol = AudioManager.Instance != null ? AudioManager.Instance.musicVolume : 0.55f;
            float sfxVol = AudioManager.Instance != null ? AudioManager.Instance.sfxVolume : 0.85f;
            bool musicMuted = AudioManager.Instance != null && AudioManager.Instance.isMusicMuted;
            bool sfxMuted = AudioManager.Instance != null && AudioManager.Instance.isSfxMuted;

            if (musicSlider != null) musicSlider.value = musicVol;
            if (sfxSlider != null) sfxSlider.value = sfxVol;

            if (musicValueText != null)
            {
                musicValueText.text = musicMuted
                    ? "ÂM LƯỢNG NHẠC NỀN: ĐÃ TẮT (0%)"
                    : $"ÂM LƯỢNG NHẠC NỀN: {Mathf.RoundToInt(musicVol * 100f)}%";
            }

            if (sfxValueText != null)
            {
                sfxValueText.text = sfxMuted
                    ? "ÂM THANH TIẾNG SÚNG (SFX): ĐÃ TẮT (0%)"
                    : $"ÂM THANH TIẾNG SÚNG (SFX): {Mathf.RoundToInt(sfxVol * 100f)}%";
            }

            if (musicMuteBtnLabel != null)
            {
                musicMuteBtnLabel.text = musicMuted ? "♪✕" : "♪";
            }

            if (sfxMuteBtnLabel != null)
            {
                sfxMuteBtnLabel.text = sfxMuted ? "🔊✕" : "🔊";
            }

            if (graphicsTierText != null)
            {
                graphicsTierText.text = $"CHẤT LƯỢNG ĐỒ HỌA: {GetGraphicsTierDisplayName()}";
            }

            if (quickStatusText != null)
            {
                string mStr = musicMuted ? "Tắt" : $"{Mathf.RoundToInt(musicVol * 100f)}%";
                string sStr = sfxMuted ? "Tắt" : $"{Mathf.RoundToInt(sfxVol * 100f)}%";
                string gStr = _graphicsTier == 0 ? "THẤP (Siêu Mượt)" : (_graphicsTier == 1 ? "TRUNG BÌNH" : "CAO");
                quickStatusText.text = $"Nhạc Nền: {mStr}   |   Tiếng Súng: {sStr}   |   Đồ Họa: {gStr}";
            }

            _suppressSliderCallback = false;
        }

        private void PlayUiClick()
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayPickup();
            }
        }
    }
}
