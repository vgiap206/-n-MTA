using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TopDownShooter
{
    /// <summary>
    /// Chướng ngại vật Bình Xăng Nổ (Explosive Fuel Barrel):
    /// - Khi bị đạn bắn trúng đủ sát thương sẽ phát nổ dữ dội, tạo sóng xung kích lửa,
    ///   gây sát thương diện rộng (AoE) tiêu diệt hàng loạt kẻ địch xung quanh và kích nổ dây chuyền các bình xăng gần đó.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(CircleCollider2D))]
    public class ExplosiveBarrel : MonoBehaviour
    {
        public static readonly List<ExplosiveBarrel> ActiveBarrels = new List<ExplosiveBarrel>();

        [Header("Chỉ số Bình Xăng Nổ")]
        public float maxHealth = 50f;
        public float currentHealth = 50f;
        public float explosionRadius = 5.2f;
        public float enemyExplosionDamage = 240f;
        public float playerSplashDamage = 18f;

        private SpriteRenderer _sr;
        private bool _exploded;
        private Vector3 _basePosition;
        private Coroutine _hitFlashRoutine;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStaticList()
        {
            ActiveBarrels.Clear();
        }

        private void Awake()
        {
            _sr = GetComponent<SpriteRenderer>();
            currentHealth = maxHealth;
            _basePosition = transform.position;
        }

        private void OnEnable()
        {
            if (!ActiveBarrels.Contains(this))
            {
                ActiveBarrels.Add(this);
            }
        }

        private void OnDisable()
        {
            ActiveBarrels.Remove(this);
        }

        public void TakeDamage(float damage)
        {
            if (_exploded || damage <= 0f) return;

            currentHealth -= damage;
            if (currentHealth <= 0f)
            {
                Explode();
            }
            else
            {
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayEnemyHit();
                }

                if (_hitFlashRoutine != null) StopCoroutine(_hitFlashRoutine);
                _hitFlashRoutine = StartCoroutine(HitFlashAndShake());
            }
        }

        private IEnumerator HitFlashAndShake()
        {
            if (_sr != null)
            {
                _sr.color = new Color(1f, 0.55f, 0.2f, 1f);
            }

            float elapsed = 0f;
            while (elapsed < 0.09f)
            {
                elapsed += Time.deltaTime;
                Vector2 jitter = Random.insideUnitCircle * 0.08f;
                transform.position = _basePosition + new Vector3(jitter.x, jitter.y, 0f);
                yield return null;
            }

            transform.position = _basePosition;
            if (!_exploded && _sr != null)
            {
                _sr.color = Color.white;
            }
        }

        public void Explode()
        {
            if (_exploded) return;
            _exploded = true;

            Vector3 pos = transform.position;

            // 1. Gây sát thương nổ diện rộng lên toàn bộ Kẻ Địch trong bán kính nổ
            List<Enemy> enemiesSnapshot = new List<Enemy>(Enemy.ActiveEnemies);
            for (int i = 0; i < enemiesSnapshot.Count; i++)
            {
                Enemy e = enemiesSnapshot[i];
                if (e == null || e.IsDead) continue;

                float dist = Vector2.Distance(pos, e.transform.position);
                if (dist <= explosionRadius)
                {
                    float falloff = Mathf.Clamp(1.15f - (dist / explosionRadius) * 0.55f, 0.5f, 1.15f);
                    e.TakeDamage(enemyExplosionDamage * falloff);
                }
            }

            // 2. Sát thương lan nhẹ nếu Người Chơi đứng sát bình xăng
            if (PlayerController.Instance != null && !PlayerController.Instance.IsDead)
            {
                float playerDist = Vector2.Distance(pos, PlayerController.Instance.transform.position);
                if (playerDist <= explosionRadius * 0.55f)
                {
                    PlayerController.Instance.TakeDamage(playerSplashDamage);
                }
            }

            // 3. Kích nổ dây chuyền các Bình Xăng Nổ khác ở gần
            List<ExplosiveBarrel> barrelsSnapshot = new List<ExplosiveBarrel>(ActiveBarrels);
            for (int i = 0; i < barrelsSnapshot.Count; i++)
            {
                ExplosiveBarrel other = barrelsSnapshot[i];
                if (other == null || other == this || other._exploded) continue;

                float d = Vector2.Distance(pos, other.transform.position);
                if (d <= explosionRadius * 0.85f)
                {
                    other.TakeDamage(other.maxHealth + 10f);
                }
            }

            // 4. Âm thanh nổ & Hiệu ứng hình ảnh vụ nổ + Vết cháy đen trên mặt đất
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayEnemyDeath();
                AudioManager.Instance.PlayShoot();
            }

            SpawnExplosionVisualEffect(pos, explosionRadius);

            if (WorldMapManager.Instance != null)
            {
                EnvironmentObstacle obs = GetComponent<EnvironmentObstacle>();
                if (obs != null) WorldMapManager.Instance.Obstacles.Remove(obs);
            }

            Destroy(gameObject);
        }

        private static void SpawnExplosionVisualEffect(Vector3 position, float radius)
        {
            // Để lại vệt đất cháy xém dưới mặt đất (tự mờ đi sau 25 giây)
            GameObject scorchObj = new GameObject("BarrelScorchCrater");
            scorchObj.transform.position = position;
            scorchObj.transform.localScale = Vector3.one * (radius * 0.7f);
            SpriteRenderer scorchSr = scorchObj.AddComponent<SpriteRenderer>();
            scorchSr.sprite = SpriteGenerator.GetItemAuraSprite();
            scorchSr.color = new Color(0.08f, 0.07f, 0.06f, 0.75f);
            scorchSr.sortingOrder = -1;
            Destroy(scorchObj, 25f);

            // Tạo hiệu ứng cầu lửa nở rộng
            GameObject blastObj = new GameObject("BarrelExplosionBlast");
            blastObj.transform.position = position;
            BarrelExplosionVfx vfx = blastObj.AddComponent<BarrelExplosionVfx>();
            vfx.Initialize(radius);
        }
    }

    /// <summary>
    /// Hiệu ứng vụ nổ cầu lửa & sóng xung kích khi Bình Xăng Nổ phát nổ.
    /// </summary>
    public class BarrelExplosionVfx : MonoBehaviour
    {
        private float _targetRadius = 5f;
        private SpriteRenderer _outerRing;
        private SpriteRenderer _innerCore;

        public void Initialize(float radius)
        {
            _targetRadius = radius;

            GameObject outerObj = new GameObject("BlastOuterFire");
            outerObj.transform.SetParent(transform, false);
            _outerRing = outerObj.AddComponent<SpriteRenderer>();
            _outerRing.sprite = SpriteGenerator.GetItemAuraSprite();
            _outerRing.color = new Color(1f, 0.42f, 0.08f, 0.92f);
            _outerRing.sortingOrder = 8;

            GameObject innerObj = new GameObject("BlastInnerCore");
            innerObj.transform.SetParent(transform, false);
            _innerCore = innerObj.AddComponent<SpriteRenderer>();
            _innerCore.sprite = SpriteGenerator.GetItemAuraSprite();
            _innerCore.color = new Color(1f, 0.96f, 0.55f, 0.98f);
            _innerCore.sortingOrder = 9;

            StartCoroutine(AnimateBlast());
        }

        private IEnumerator AnimateBlast()
        {
            float duration = 0.38f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float easeOut = 1f - Mathf.Pow(1f - t, 3f);

                float outerScale = Mathf.Lerp(0.6f, _targetRadius * 2.1f, easeOut);
                float innerScale = Mathf.Lerp(0.4f, _targetRadius * 1.35f, easeOut);

                if (_outerRing != null)
                {
                    _outerRing.transform.localScale = new Vector3(outerScale, outerScale, 1f);
                    _outerRing.color = new Color(1f, Mathf.Lerp(0.55f, 0.18f, t), 0.05f, (1f - t) * 0.9f);
                }

                if (_innerCore != null)
                {
                    _innerCore.transform.localScale = new Vector3(innerScale, innerScale, 1f);
                    _innerCore.color = new Color(1f, 0.95f, 0.55f, Mathf.Pow(1f - t, 1.6f));
                }

                yield return null;
            }

            Destroy(gameObject);
        }
    }
}

