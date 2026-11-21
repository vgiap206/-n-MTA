using System.Collections;
using UnityEngine;

namespace TopDownShooter
{
    /// <summary>
    /// Quản lý giao diện người dùng (UI):
    /// - Bảng HUD góc trái thiết kế chỉnh chu: Huy hiệu Level & Đợt Quái, Thanh Máu 3D đổi màu theo % máu, Thanh EXP,
    ///   và thanh trạng thái Vũ Khí / Chỉ Số.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("UI Chính: Chữ Level và Thanh EXP")]
        public UnityEngine.UI.Text levelText;
        public UnityEngine.UI.Slider expSlider;
        public UnityEngine.UI.Image expFillImage;
        public UnityEngine.UI.Text expDetailText;

        [Header("UI Thanh Máu Chỉnh Chu & Thông Báo")]
        public UnityEngine.UI.Slider healthSlider;
        public UnityEngine.UI.Image healthFillImage;
        public UnityEngine.UI.Text healthDetailText;
        public UnityEngine.UI.Text waveInfoText;
        public UnityEngine.UI.Text statsSummaryText;
        public UnityEngine.UI.Text levelUpNoticeText;

        private Coroutine _bannerRoutine;
        private float _uiTickTimer;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            if (PlayerController.Instance != null)
            {
                PlayerController.Instance.OnLevelUp += HandlePlayerLevelUp;
                PlayerController.Instance.OnSupplyPickedUp += HandleSupplyPickedUp;
                RefreshUI(PlayerController.Instance);
            }

            if (levelUpNoticeText != null)
            {
                levelUpNoticeText.gameObject.SetActive(false);
            }
        }

        private void Update()
        {
            _uiTickTimer += Time.deltaTime;
            if (_uiTickTimer >= 0.25f)
            {
                _uiTickTimer = 0f;
                UpdateWaveDisplay();
            }
        }

        private void OnDestroy()
        {
            if (PlayerController.Instance != null)
            {
                PlayerController.Instance.OnLevelUp -= HandlePlayerLevelUp;
                PlayerController.Instance.OnSupplyPickedUp -= HandleSupplyPickedUp;
            }
        }

        public void RefreshUI(PlayerController player)
        {
            if (player == null) return;

            int wave = EnemySpawner.Instance != null ? EnemySpawner.Instance.currentWave : 1;

            // 1. Cập nhật Huy Hiệu Level & Đợt Quái
            if (levelText != null)
            {
                levelText.text = $"LEVEL {player.level}\n<size=18><color=#FFAA33>ĐỢT QUÁI {wave}</color></size>";
            }

            // 2. Cập nhật Thanh Máu (Đổi màu xanh -> vàng cam -> đỏ báo động theo % máu còn lại)
            float hpRatio = player.maxHealth > 0f
                ? Mathf.Clamp01(player.currentHealth / player.maxHealth)
                : 0f;

            if (healthSlider != null)
            {
                healthSlider.minValue = 0f;
                healthSlider.maxValue = 1f;
                healthSlider.value = hpRatio;
            }

            if (healthFillImage != null)
            {
                if (healthSlider == null) healthFillImage.fillAmount = hpRatio;

                if (hpRatio > 0.55f)
                    healthFillImage.color = new Color(0.18f, 0.92f, 0.38f, 1f); // Xanh lục khoẻ mạnh
                else if (hpRatio > 0.28f)
                    healthFillImage.color = new Color(1.0f, 0.72f, 0.14f, 1f);  // Vàng cam cảnh báo
                else
                    healthFillImage.color = new Color(0.98f, 0.20f, 0.25f, 1f); // Đỏ nguy kịch
            }

            if (healthDetailText != null)
            {
                int curHp = Mathf.CeilToInt(player.currentHealth);
                int maxHp = Mathf.CeilToInt(player.maxHealth);
                int pct = Mathf.RoundToInt(hpRatio * 100f);
                healthDetailText.text = $"MÁU (HP): {curHp} / {maxHp}  ({pct}%)";
            }

            // 3. Cập nhật Thanh EXP
            float expRatio = player.expToNextLevel > 0
                ? Mathf.Clamp01((float)player.currentExp / player.expToNextLevel)
                : 0f;

            if (expSlider != null)
            {
                expSlider.minValue = 0f;
                expSlider.maxValue = 1f;
                expSlider.value = expRatio;
            }
            else if (expFillImage != null)
            {
                expFillImage.fillAmount = expRatio;
            }

            if (expDetailText != null)
            {
                int expPct = Mathf.RoundToInt(expRatio * 100f);
                expDetailText.text = $"EXP: {player.currentExp} / {player.expToNextLevel}  ({expPct}%)";
            }

            UpdateWaveDisplay();
        }

        private void UpdateWaveDisplay()
        {
            if (EnemySpawner.Instance == null) return;

            if (waveInfoText != null)
            {
                waveInfoText.text = EnemySpawner.Instance.GetWaveDifficultySummary();
            }

            if (PlayerController.Instance != null && statsSummaryText != null)
            {
                PlayerController player = PlayerController.Instance;
                statsSummaryText.text =
                    $"{player.GetWeaponDescription()}   |   Công: {player.attackDamage:0.#}   |   Tốc Bắn: {player.fireRate:0.0}/s   |   {EnemySpawner.Instance.GetWaveDifficultySummary()}";
            }
        }

        private void HandlePlayerLevelUp(int newLevel)
        {
            ShowBanner(
                $"LEVEL UP! (CẤP {newLevel})\n+Máu  +Tấn Công  +Tốc Độ Bắn",
                new Color(1f, 0.92f, 0.2f, 1f),
                2.2f);
        }

        private void HandleSupplyPickedUp(string message)
        {
            ShowBanner(
                $"NHẶT VẬT PHẨM THÀNH CÔNG!\n{message}",
                new Color(0.3f, 0.96f, 1f, 1f),
                2.4f);
        }

        public void ShowBanner(string text, Color color, float duration)
        {
            if (levelUpNoticeText == null) return;

            if (_bannerRoutine != null) StopCoroutine(_bannerRoutine);
            _bannerRoutine = StartCoroutine(BannerRoutine(text, color, duration));
        }

        private IEnumerator BannerRoutine(string text, Color color, float duration)
        {
            levelUpNoticeText.gameObject.SetActive(true);
            levelUpNoticeText.color = color;
            levelUpNoticeText.text = text;
            yield return new WaitForSeconds(duration);
            if (levelUpNoticeText != null)
            {
                levelUpNoticeText.gameObject.SetActive(false);
            }
        }
    }
}
