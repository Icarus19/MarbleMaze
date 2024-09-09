using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAR : MonoBehaviour
{
    public Material material;
    public int size;
    Color color;
    public ComputeShader computeShader;
    public RenderTexture renTex;

    // Start is called before the first frame update
    void Start()
    {
        Texture2D tex = GaussianNoise.GenerateGaussianNoise(size, 0.0f, 1.0f);
        renTex = new RenderTexture(size, size, 0, RenderTextureFormat.ARGBFloat);
        renTex.enableRandomWrite = true;
        renTex.Create();

        computeShader.SetTexture(0, "_Gaussian", tex);
        computeShader.SetTexture(0, "Result", renTex);
        computeShader.Dispatch(0, size / 8, size / 8, 1);

        material.SetTexture("_WaveTexture", renTex);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnDisable()
    {
        renTex.Release();
        renTex = null;
    }
}
