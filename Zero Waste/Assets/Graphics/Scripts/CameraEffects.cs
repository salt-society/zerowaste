using UnityEngine;

[ExecuteInEditMode]
public class CameraEffects : MonoBehaviour
{
    public Material EffectMaterial;

    [Space]
    public bool repeating;
    [Range(0, 10)]
    public int iterations;
    [Range(0, 4)]
    public int resolution;

    private RenderTexture src;
    private RenderTexture dst;

    void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        this.src = src;
        this.dst = dst;

        if (!repeating)
        {
            DirectImageEffect();
        }
        else
        {
            RepeatingEffect();
        }

    }

    void DirectImageEffect()
    {
        Graphics.Blit(src, dst, EffectMaterial);
    }

    void RepeatingEffect()
    {
        int width = src.width >> resolution;
        int height = src.height >> resolution;

        RenderTexture rt = RenderTexture.GetTemporary(width, height);
        Graphics.Blit(src, rt);

        for (int i = 0; i < iterations; i++)
        {
            RenderTexture rt2 = RenderTexture.GetTemporary(width, height);
            Graphics.Blit(rt, rt2, EffectMaterial);
            RenderTexture.ReleaseTemporary(rt);
            rt = rt2;
        }

        Graphics.Blit(rt, dst);
        RenderTexture.ReleaseTemporary(rt);
    }
}
