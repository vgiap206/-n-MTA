using System;
using System.IO;
using UnityEngine;

namespace TopDownShooter
{
    /// <summary>
    /// Tạo hiệu ứng âm thanh (AudioClip) và xuất file WAV tự động cho toàn bộ game:
    /// - Tiếng súng (Shoot)
    /// - Tiếng kẻ địch bị tấn công (Enemy Hit)
    /// - Tiếng người chơi bị tấn công (Player Hit)
    /// - Tiếng kẻ địch bị chết (Enemy Death)
    /// - Tiếng người chơi bị chết (Player Death)
    /// - Tiếng nhạc nền (Background Music)
    /// </summary>
    public static class SoundGenerator
    {
        private const int SampleRate = 44100;

        public static AudioClip CreateShootClip()
        {
            float[] samples = GenerateShootSamples();
            return CreateClip("Gunshot", samples);
        }

        public static AudioClip CreateEnemyHitClip()
        {
            float[] samples = GenerateEnemyHitSamples();
            return CreateClip("EnemyHit", samples);
        }

        public static AudioClip CreatePlayerHitClip()
        {
            float[] samples = GeneratePlayerHitSamples();
            return CreateClip("PlayerHit", samples);
        }

        public static AudioClip CreateEnemyDeathClip()
        {
            float[] samples = GenerateEnemyDeathSamples();
            return CreateClip("EnemyDeath", samples);
        }

        public static AudioClip CreatePlayerDeathClip()
        {
            float[] samples = GeneratePlayerDeathSamples();
            return CreateClip("PlayerDeath", samples);
        }

        public static AudioClip CreateBackgroundMusicClip()
        {
            float[] samples = GenerateBackgroundMusicSamples();
            return CreateClip("BackgroundMusic", samples);
        }

        public static AudioClip CreatePickupClip()
        {
            float[] samples = GeneratePickupSamples();
            return CreateClip("PickupSupply", samples);
        }

        public static float[] GeneratePickupSamples()
        {
            float duration = 0.28f;
            int count = Mathf.CeilToInt(SampleRate * duration);
            float[] data = new float[count];
            float[] notes = { 523.25f, 659.25f, 783.99f, 1046.5f };
            float phase = 0f;

            for (int i = 0; i < count; i++)
            {
                float t = (float)i / count;
                int idx = Mathf.Min(notes.Length - 1, Mathf.FloorToInt(t * notes.Length));
                float freq = notes[idx];
                phase += 2f * Mathf.PI * freq / SampleRate;
                float env = Mathf.Pow(1f - t * 0.4f, 1.2f);
                data[i] = Mathf.Clamp((Mathf.Sin(phase) * 0.6f + Mathf.Sin(phase * 2f) * 0.25f) * env * 0.7f, -1f, 1f);
            }

            ApplyFade(data);
            return data;
        }

        public static float[] GenerateShootSamples()
        {
            float duration = 0.13f;
            int count = Mathf.CeilToInt(SampleRate * duration);
            float[] data = new float[count];
            System.Random rng = new System.Random(101);
            float phase = 0f;

            for (int i = 0; i < count; i++)
            {
                float t = (float)i / count;
                float env = Mathf.Pow(1f - t, 2.2f);
                float freq = Mathf.Lerp(780f, 130f, Mathf.Pow(t, 0.55f));
                phase += 2f * Mathf.PI * freq / SampleRate;

                float tone = Mathf.Sin(phase) * 0.65f;
                float noise = ((float)rng.NextDouble() * 2f - 1f) * 0.55f * Mathf.Pow(1f - t, 3.0f);
                data[i] = Mathf.Clamp((tone + noise) * env * 0.75f, -1f, 1f);
            }

            ApplyFade(data);
            return data;
        }

        public static float[] GenerateEnemyHitSamples()
        {
            float duration = 0.09f;
            int count = Mathf.CeilToInt(SampleRate * duration);
            float[] data = new float[count];
            System.Random rng = new System.Random(202);
            float phase = 0f;

            for (int i = 0; i < count; i++)
            {
                float t = (float)i / count;
                float env = Mathf.Pow(1f - t, 2.5f);
                float freq = Mathf.Lerp(360f, 110f, t);
                phase += 2f * Mathf.PI * freq / SampleRate;

                float square = Mathf.Sign(Mathf.Sin(phase)) * 0.45f;
                float noise = ((float)rng.NextDouble() * 2f - 1f) * 0.35f;
                data[i] = Mathf.Clamp((square + noise) * env * 0.65f, -1f, 1f);
            }

            ApplyFade(data);
            return data;
        }

        public static float[] GeneratePlayerHitSamples()
        {
            float duration = 0.22f;
            int count = Mathf.CeilToInt(SampleRate * duration);
            float[] data = new float[count];
            System.Random rng = new System.Random(303);
            float phase = 0f;

            for (int i = 0; i < count; i++)
            {
                float t = (float)i / count;
                float env = Mathf.Pow(1f - t, 1.5f);
                float freq = Mathf.Lerp(210f, 75f, t);
                phase += 2f * Mathf.PI * freq / SampleRate;

                float saw = (2f * ((phase / (2f * Mathf.PI)) - Mathf.Floor(0.5f + phase / (2f * Mathf.PI)))) * 0.55f;
                float noise = ((float)rng.NextDouble() * 2f - 1f) * 0.4f * (1f - t);
                data[i] = Mathf.Clamp((saw + noise) * env * 0.8f, -1f, 1f);
            }

            ApplyFade(data);
            return data;
        }

        public static float[] GenerateEnemyDeathSamples()
        {
            float duration = 0.24f;
            int count = Mathf.CeilToInt(SampleRate * duration);
            float[] data = new float[count];
            System.Random rng = new System.Random(404);
            float phase1 = 0f;
            float phase2 = 0f;

            for (int i = 0; i < count; i++)
            {
                float t = (float)i / count;
                float env = Mathf.Pow(1f - t, 1.8f);
                float freq1 = Mathf.Lerp(480f, 95f, t);
                float freq2 = Mathf.Lerp(680f, 140f, t);
                phase1 += 2f * Mathf.PI * freq1 / SampleRate;
                phase2 += 2f * Mathf.PI * freq2 / SampleRate;

                float wave = (Mathf.Sin(phase1) * 0.5f + Mathf.Sin(phase2) * 0.3f);
                float noise = ((float)rng.NextDouble() * 2f - 1f) * 0.25f * (1f - t);
                data[i] = Mathf.Clamp((wave + noise) * env * 0.75f, -1f, 1f);
            }

            ApplyFade(data);
            return data;
        }

        public static float[] GeneratePlayerDeathSamples()
        {
            float duration = 0.7f;
            int count = Mathf.CeilToInt(SampleRate * duration);
            float[] data = new float[count];
            float[] notes = { 392f, 349.23f, 311.13f, 261.63f, 196f };
            float phase = 0f;

            for (int i = 0; i < count; i++)
            {
                float t = (float)i / count;
                int noteIdx = Mathf.Min(notes.Length - 1, Mathf.FloorToInt(t * notes.Length));
                float localT = (t * notes.Length) - noteIdx;
                float freq = notes[noteIdx] * (1f - localT * 0.06f);
                phase += 2f * Mathf.PI * freq / SampleRate;

                float env = (1f - localT * 0.4f) * (1f - t * 0.3f);
                float tone = Mathf.Sin(phase) * 0.55f + Mathf.Sign(Mathf.Sin(phase * 0.5f)) * 0.25f;
                data[i] = Mathf.Clamp(tone * env * 0.8f, -1f, 1f);
            }

            ApplyFade(data);
            return data;
        }

        public static float[] GenerateBackgroundMusicSamples()
        {
            // 8-second seamless synth loop (120 BPM, 16 beats = 32 eighth notes)
            float duration = 8.0f;
            int count = Mathf.CeilToInt(SampleRate * duration);
            float[] data = new float[count];

            float[] bassNotes =
            {
                110f, 110f, 110f, 110f, 130.81f, 130.81f, 130.81f, 130.81f,
                146.83f, 146.83f, 146.83f, 146.83f, 130.81f, 130.81f, 164.81f, 164.81f,
                110f, 110f, 110f, 110f, 130.81f, 130.81f, 130.81f, 130.81f,
                146.83f, 146.83f, 164.81f, 164.81f, 110f, 110f, 110f, 110f
            };

            float[] leadNotes =
            {
                440f, 0f, 523.25f, 440f, 392f, 0f, 440f, 523.25f,
                587.33f, 0f, 523.25f, 440f, 392f, 0f, 329.63f, 0f,
                440f, 0f, 523.25f, 659.25f, 587.33f, 0f, 523.25f, 440f,
                392f, 440f, 523.25f, 392f, 440f, 0f, 440f, 0f
            };

            int totalSteps = bassNotes.Length;
            float stepDuration = duration / totalSteps;
            float bassPhase = 0f;
            float leadPhase = 0f;

            for (int i = 0; i < count; i++)
            {
                float time = (float)i / SampleRate;
                int step = Mathf.Min(totalSteps - 1, Mathf.FloorToInt(time / stepDuration));
                float stepTime = (time - step * stepDuration) / stepDuration;

                // Bassline
                float bassFreq = bassNotes[step];
                bassPhase += 2f * Mathf.PI * bassFreq / SampleRate;
                float bassEnv = Mathf.Pow(1f - stepTime, 1.3f);
                float bassSample = (Mathf.Sin(bassPhase) * 0.7f + Mathf.Sin(bassPhase * 2f) * 0.3f) * bassEnv * 0.25f;

                // Lead melody
                float leadFreq = leadNotes[step];
                float leadSample = 0f;
                if (leadFreq > 0f)
                {
                    leadPhase += 2f * Mathf.PI * leadFreq / SampleRate;
                    float leadEnv = Mathf.Pow(1f - stepTime, 1.6f);
                    leadSample = (Mathf.Sin(leadPhase) * 0.6f + Mathf.Sin(leadPhase * 0.5f) * 0.4f) * leadEnv * 0.18f;
                }

                data[i] = Mathf.Clamp(bassSample + leadSample, -1f, 1f);
            }

            ApplyFade(data);
            return data;
        }

        private static AudioClip CreateClip(string name, float[] samples)
        {
            AudioClip clip = AudioClip.Create(name, samples.Length, 1, SampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private static void ApplyFade(float[] samples)
        {
            int fadeSamples = Mathf.Min(220, samples.Length / 10); // ~5 ms fade in/out
            for (int i = 0; i < fadeSamples; i++)
            {
                float factor = (float)i / fadeSamples;
                samples[i] *= factor;
                samples[samples.Length - 1 - i] *= factor;
            }
        }

        public static void SaveWavFile(string filePath, float[] samples)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            using (BinaryWriter bw = new BinaryWriter(fs))
            {
                int byteRate = SampleRate * 2;
                int dataSize = samples.Length * 2;

                // RIFF header
                bw.Write(new char[] { 'R', 'I', 'F', 'F' });
                bw.Write(36 + dataSize);
                bw.Write(new char[] { 'W', 'A', 'V', 'E' });

                // fmt subchunk
                bw.Write(new char[] { 'f', 'm', 't', ' ' });
                bw.Write(16); // PCM chunk size
                bw.Write((short)1); // AudioFormat PCM = 1
                bw.Write((short)1); // NumChannels = 1 (Mono)
                bw.Write(SampleRate);
                bw.Write(byteRate);
                bw.Write((short)2); // BlockAlign
                bw.Write((short)16); // BitsPerSample

                // data subchunk
                bw.Write(new char[] { 'd', 'a', 't', 'a' });
                bw.Write(dataSize);

                for (int i = 0; i < samples.Length; i++)
                {
                    short pcm = (short)Mathf.Clamp(samples[i] * 32767f, short.MinValue, short.MaxValue);
                    bw.Write(pcm);
                }
            }
        }
    }
}

