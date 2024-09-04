using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    /*public ComputeShader waveFunctions;
    public RenderTexture waveTexture;
    float time = 0;*/

    WavesCascade[] cascades = new WavesCascade[3];
    [Range(0, 2)]
    public int cascadeID;

    /*[SerializeField]
    Material waterMaterial;
    [SerializeField]
    [Range(0, 1)]
    float amp, freq, speed, spectrumPeak, lowPassFilter, distribution;
    [SerializeField]
    [Range(0.0f, 360.0f)]
    float angle;
    [SerializeField]
    kernel kernelID;
    enum kernel {SinWave, Gradient, Third, Fourth };*/

    int textureResolution = 256;
    float mean = 0.0f, stdDev = 1.0f;

    [Header("Do no touch settings")]
    [SerializeField]
    WavesSettings wavesSettings;
    [SerializeField]
    float lengthScale0 = 250;
    [SerializeField]
    float lengthScale1 = 17;
    [SerializeField]
    float lengthScale2 = 5;
    [SerializeField]
    Material material;

    [Header("Computes")]
    [SerializeField]
    ComputeShader fftShader;
    [SerializeField]
    ComputeShader initialSpectrumShader;
    [SerializeField]
    ComputeShader timeDependentSpectrumShader;
    [SerializeField]
    ComputeShader texturesMergerShader;

    Texture2D gaussianNoise;
    FastFourierTransform fft;



    //////////////////////////
    [Header("Testing")]
    public RenderTexture initialSpectrum;
    public RenderTexture precomputedData;
    public RenderTexture buffer;
    public RenderTexture DxDz;
    public RenderTexture DyDxz;
    public RenderTexture DyxDyz;
    public RenderTexture DxxDzz;
    public RenderTexture displacement;
    public RenderTexture derivatives;
    public RenderTexture turbulence;
    void OnEnable()
    {
        Application.targetFrameRate = 30;

        fft = new FastFourierTransform(256, fftShader);
        /*waveFunctions = Resources.Load<ComputeShader>("WaveFunction");
        waveTexture = new RenderTexture(1024, 1024, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
        waveTexture.enableRandomWrite = true;
        waveTexture.Create();*/

        gaussianNoise = GaussianNoise.GenerateGaussianNoise(textureResolution, mean, stdDev);

        cascades[0] = new WavesCascade(256, initialSpectrumShader, timeDependentSpectrumShader, texturesMergerShader, fft, gaussianNoise);
        cascades[1] = new WavesCascade(256, initialSpectrumShader, timeDependentSpectrumShader, texturesMergerShader, fft, gaussianNoise);
        cascades[2] = new WavesCascade(256, initialSpectrumShader, timeDependentSpectrumShader, texturesMergerShader, fft, gaussianNoise);

        InitializeCascades();
    }
    void Update()
    {

        cascades[0].CalculateWavesAtTime(Time.time);
        cascades[1].CalculateWavesAtTime(Time.time);
        cascades[2].CalculateWavesAtTime(Time.time);

        initialSpectrum = cascades[cascadeID].InitialSpectrum;
        precomputedData = cascades[cascadeID].PrecomputedData;
        buffer = cascades[cascadeID].buffer;
        DxDz = cascades[cascadeID].DxDz;
        DyDxz = cascades[cascadeID].DyDxz;
        DyxDyz = cascades[cascadeID].DyxDyz;
        DxxDzz = cascades[cascadeID].DxxDzz;
        displacement = cascades[cascadeID].displacement;
        derivatives = cascades[cascadeID].derivatives;
        turbulence = cascades[cascadeID].turbulence;

        material.SetTexture("_Displacement_c0", cascades[0].Displacement);
        material.SetTexture("_Derivatives_c0", cascades[0].Derivatives);
        material.SetTexture("_Turbulence_c0", cascades[0].Turbulence);

        material.SetTexture("_Displacement_c1", cascades[1].Displacement);
        material.SetTexture("_Derivatives_c1", cascades[1].Derivatives);
        material.SetTexture("_Turbulence_c1", cascades[1].Turbulence);

        material.SetTexture("_Displacement_c2", cascades[2].Displacement);
        material.SetTexture("_Derivatives_c2", cascades[2].Derivatives);
        material.SetTexture("_Turbulence_c2", cascades[2].Turbulence);
    }
        
    public void InitializeCascades()
    {
        float boundary1 = 2 * Mathf.PI / lengthScale1 * 6f;
        float boundary2 = 2 * Mathf.PI / lengthScale2 * 6f;
        cascades[0].CalculateInitials(wavesSettings, lengthScale0, 0.0001f, boundary1);
        cascades[1].CalculateInitials(wavesSettings, lengthScale1, boundary1, boundary2);
        cascades[2].CalculateInitials(wavesSettings, lengthScale2, boundary2, 9999);

        Shader.SetGlobalFloat("LengthScale0", lengthScale0);
        Shader.SetGlobalFloat("LengthScale1", lengthScale1);
        Shader.SetGlobalFloat("LengthScale2", lengthScale2);
    }

    /*void OnDisable()
    {
        waveTexture.Release();
        waveTexture = null;
    }*/

    void OnDestroy()
    {
        for (int i = 0; i <= 2; i++)
        {
            cascades[i].Dispose();
        }
    }

}
