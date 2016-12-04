﻿using System;

namespace NetGL.Core.Mathematics {
    public class FractalNoise {
        public FractalNoise(float inH, float inLacunarity, float inOctaves)
            : this(inH, inLacunarity, inOctaves, null) {

        }

        public FractalNoise(float inH, float inLacunarity, float inOctaves, PerlinNoise noise) {
            m_Lacunarity = inLacunarity;
            m_Octaves = inOctaves;
            m_IntOctaves = (int)inOctaves;
            m_Exponent = new float[m_IntOctaves + 1];
            float frequency = 1.0F;
            for (int i = 0; i < m_IntOctaves + 1; i++) {
                m_Exponent[i] = (float)Math.Pow(m_Lacunarity, -inH);
                frequency *= m_Lacunarity;
            }

            if (noise == null)
                m_Noise = new PerlinNoise();
            else
                m_Noise = noise;
        }

        public float HybridMultifractal(float x, float y, float offset) {
            float weight, signal, remainder, result;

            result = (m_Noise.Noise(x, y) + offset) * m_Exponent[0];
            weight = result;
            x *= m_Lacunarity;
            y *= m_Lacunarity;
            int i;
            for (i = 1; i < m_IntOctaves; i++) {
                if (weight > 1.0F) weight = 1.0F;
                signal = (m_Noise.Noise(x, y) + offset) * m_Exponent[i];
                result += weight * signal;
                weight *= signal;
                x *= m_Lacunarity;
                y *= m_Lacunarity;
            }
            remainder = m_Octaves - m_IntOctaves;
            result += remainder * m_Noise.Noise(x, y) * m_Exponent[i];

            return result;
        }

        public float RidgedMultifractal(float x, float y, float offset, float gain) {
            float weight, signal, result;
            int i;

            signal = Math.Abs(m_Noise.Noise(x, y));
            signal = offset - signal;
            signal *= signal;
            result = signal;
            weight = 1.0F;

            for (i = 1; i < m_IntOctaves; i++) {
                x *= m_Lacunarity;
                y *= m_Lacunarity;

                weight = signal * gain;
                weight = MathF.Clamp01(weight);

                signal = Math.Abs(m_Noise.Noise(x, y));
                signal = offset - signal;
                signal *= signal;
                signal *= weight;
                result += signal * m_Exponent[i];
            }

            return result;
        }

        public float BrownianMotion(float x, float y) {
            float value, remainder;
            long i;

            value = 0.0F;
            for (i = 0; i < m_IntOctaves; i++) {
                value = m_Noise.Noise(x, y) * m_Exponent[i];
                x *= m_Lacunarity;
                y *= m_Lacunarity;
            }
            remainder = m_Octaves - m_IntOctaves;
            value += remainder * m_Noise.Noise(x, y) * m_Exponent[i];

            return value;
        }


        private readonly PerlinNoise m_Noise;
        private readonly float[] m_Exponent;
        private readonly int m_IntOctaves;
        private readonly float m_Octaves;
        private readonly float m_Lacunarity;
    }
}
