using UnityEngine;

namespace TopDownShooter
{
    /// <summary>
    /// Quản lý toàn bộ âm thanh trong game & Cài Đặt Âm Lượng (Nhạc nền + Tiếng súng/Hiệu ứng):
    /// - Tiếng súng (shootSound)
    /// - Tiếng kẻ địch bị tấn công (enemyHitSound)
    /// - Tiếng người chơi bị tấn công (playerHitSound)
    /// - Tiếng người chơi bị chết (playerDeathSound)
    /// - Tiếng kẻ địch bị chết (enemyDeathSound)
    /// - Tiếng nhạc nền (backgroundMusic)
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        private const string PrefMasterVol = "TDS_MasterVol";
        private const string PrefMusicVol = "TDS_MusicVol";
        private const string PrefSfxVol = "TDS_SfxVol";
        private const string PrefMusicMute = "TDS_MusicMute";
        private const string PrefSfxMute = "TDS_SfxMute";

        [Header("Nguồn phát âm thanh (Audio Sources)")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;

        [Header("Danh sách âm thanh (Audio Clips)")]
        [Tooltip("Tiếng súng khi nhân vật bắn")]
        public AudioClip shootSound;

        [Tooltip("Tiếng kẻ địch bị tấn công")]
        public AudioClip enemyHitSound;

        [Tooltip("Tiếng người chơi bị tấn công")]
        public AudioClip playerHitSound;

        [Tooltip("Tiếng người chơi bị chết")]
        public AudioClip playerDeathSound;

        [Tooltip("Tiếng kẻ địch bị chết")]
        public AudioClip enemyDeathSound;

        [Tooltip("Tiếng nhạc nền")]
        public AudioClip backgroundMusic;

        [Tooltip("Tiếng nhặt tiếp tế / nâng cấp vũ khí")]
        public AudioClip pickupSound;

        [Header("Âm lượng (Volume)")]
        [Range(0f, 1f)] public float masterVolume = 1.0f;
        [Range(0f, 1f)] public float musicVolume = 0.55f;
        [Range(0f, 1f)] public float sfxVolume = 0.85f;
        public bool isMusicMuted = false;
        public bool isSfxMuted = false;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            LoadSavedAudioSettings();
            SetupAudioSources();
            EnsureClipsGenerated();
            PlayBackgroundMusic();
        }

        private void LoadSavedAudioSettings()
        {
            masterVolume = Mathf.Clamp01(PlayerPrefs.GetFloat(PrefMasterVol, 1.0f));
            musicVolume = Mathf.Clamp01(PlayerPrefs.GetFloat(PrefMusicVol, 0.55f));
            sfxVolume = Mathf.Clamp01(PlayerPrefs.GetFloat(PrefSfxVol, 0.85f));
            isMusicMuted = PlayerPrefs.GetInt(PrefMusicMute, 0) == 1;
            isSfxMuted = PlayerPrefs.GetInt(PrefSfxMute, 0) == 1;
            AudioListener.volume = masterVolume;
        }

        private void SetupAudioSources()
        {
            if (musicSource == null)
            {
                GameObject musicObj = new GameObject("MusicSource");
                musicObj.transform.SetParent(transform);
                musicSource = musicObj.AddComponent<AudioSource>();
            }

            musicSource.loop = true;
            musicSource.playOnAwake = true;
            musicSource.spatialBlend = 0f;

            if (sfxSource == null)
            {
                GameObject sfxObj = new GameObject("SFXSource");
                sfxObj.transform.SetParent(transform);
                sfxSource = sfxObj.AddComponent<AudioSource>();
            }

            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
            sfxSource.spatialBlend = 0f;

            ApplyVolumeToSources();
        }

        public void ApplyVolumeToSources()
        {
            AudioListener.volume = Mathf.Clamp01(masterVolume);
            if (musicSource != null)
            {
                musicSource.volume = isMusicMuted ? 0f : Mathf.Clamp01(musicVolume * masterVolume);
                musicSource.mute = isMusicMuted;
            }
            if (sfxSource != null)
            {
                sfxSource.volume = isSfxMuted ? 0f : Mathf.Clamp01(sfxVolume * masterVolume);
                sfxSource.mute = isSfxMuted;
            }
        }

        public void SetMasterVolume(float value)
        {
            masterVolume = Mathf.Clamp01(value);
            PlayerPrefs.SetFloat(PrefMasterVol, masterVolume);
            ApplyVolumeToSources();
        }

        public void SetMusicVolume(float value)
        {
            musicVolume = Mathf.Clamp01(value);
            if (musicVolume > 0.01f) isMusicMuted = false;
            PlayerPrefs.SetFloat(PrefMusicVol, musicVolume);
            PlayerPrefs.SetInt(PrefMusicMute, isMusicMuted ? 1 : 0);
            ApplyVolumeToSources();
        }

        public void SetSfxVolume(float value)
        {
            sfxVolume = Mathf.Clamp01(value);
            if (sfxVolume > 0.01f) isSfxMuted = false;
            PlayerPrefs.SetFloat(PrefSfxVol, sfxVolume);
            PlayerPrefs.SetInt(PrefSfxMute, isSfxMuted ? 1 : 0);
            ApplyVolumeToSources();
        }

        public void ToggleMusicMute()
        {
            isMusicMuted = !isMusicMuted;
            PlayerPrefs.SetInt(PrefMusicMute, isMusicMuted ? 1 : 0);
            ApplyVolumeToSources();
        }

        public void ToggleSfxMute()
        {
            isSfxMuted = !isSfxMuted;
            PlayerPrefs.SetInt(PrefSfxMute, isSfxMuted ? 1 : 0);
            ApplyVolumeToSources();
        }

        public void EnsureClipsGenerated()
        {
            if (shootSound == null) shootSound = SoundGenerator.CreateShootClip();
            if (enemyHitSound == null) enemyHitSound = SoundGenerator.CreateEnemyHitClip();
            if (playerHitSound == null) playerHitSound = SoundGenerator.CreatePlayerHitClip();
            if (playerDeathSound == null) playerDeathSound = SoundGenerator.CreatePlayerDeathClip();
            if (enemyDeathSound == null) enemyDeathSound = SoundGenerator.CreateEnemyDeathClip();
            if (backgroundMusic == null) backgroundMusic = SoundGenerator.CreateBackgroundMusicClip();
            if (pickupSound == null) pickupSound = SoundGenerator.CreatePickupClip();
        }

        public void PlayBackgroundMusic()
        {
            if (musicSource == null || backgroundMusic == null) return;
            ApplyVolumeToSources();
            if (musicSource.clip != backgroundMusic || !musicSource.isPlaying)
            {
                musicSource.clip = backgroundMusic;
                musicSource.Play();
            }
        }

        public void PlayShoot()
        {
            PlaySFX(shootSound, 0.55f);
        }

        public void PlayEnemyHit()
        {
            PlaySFX(enemyHitSound, 0.65f);
        }

        public void PlayPlayerHit()
        {
            PlaySFX(playerHitSound, 0.9f);
        }

        public void PlayEnemyDeath()
        {
            PlaySFX(enemyDeathSound, 0.85f);
        }

        public void PlayPlayerDeath()
        {
            PlaySFX(playerDeathSound, 1.0f);
        }

        public void PlayPickup()
        {
            PlaySFX(pickupSound, 0.9f);
        }

        private void PlaySFX(AudioClip clip, float volumeScale = 1f)
        {
            if (clip == null || sfxSource == null || isSfxMuted) return;
            sfxSource.PlayOneShot(clip, sfxVolume * masterVolume * volumeScale);
        }
    }
}
