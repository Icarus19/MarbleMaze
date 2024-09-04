using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GaussianNoise
{
   public static Texture2D GenerateGaussianNoise(int resolution, float mean, float stdDev)
    {
        Texture2D texture = new Texture2D(resolution, resolution);
        Color color;

        for(int x = 0; x < resolution; x++)
        {
            for(int y = 0; y < resolution; y++)
            {
                float r = Mathf.Clamp01((float)GenerateGaussian((double)mean, (double)stdDev));
                float g = Mathf.Clamp01((float)GenerateGaussian((double)mean, (double)stdDev));
                float b = Mathf.Clamp01((float)GenerateGaussian((double)mean, (double)stdDev));

                color = new Color(r, g, 0.0f);
                texture.SetPixel(x, y, color);

            }
        }

        texture.Apply();
        return texture;
    }

    static double GenerateGaussian(double mean, double stdDev)
    {
        double u1 = 1.0 - Random.value;
        double u2 = 1.0 - Random.value;
        double randStdNormal = Mathf.Sqrt((float)(-2.0 * Mathf.Log((float)u1))) * Mathf.Sin((float)(2.0 * Mathf.PI * u2));
        return mean + stdDev * randStdNormal;
    }
}
