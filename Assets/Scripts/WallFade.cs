using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script is used to fade out the walls that the player is behind.
/// 
/// ATTATCHED TO: All objects that are meant to fade
/// </summary>
public class WallFade : MonoBehaviour
{
    private List<Material> mats = new List<Material>();
    public float alpha = 1;
    public bool fadeOut = false;
    public bool fadeIn = false;

    void Start()
    {
        foreach (Material mat in gameObject.GetComponent<Renderer>().materials)
        {
            mats.Add(mat);
        }
    }

    void Update()
    {
        if (fadeOut && alpha > 0)
        {
            FadeOutMaterial();
            fadeIn = false;
        }
        if (fadeIn && alpha < 1)
        {
            FadeInMaterial();
            fadeOut = false;
        }
    }

    public void SetMaterialTransparent()
    {
        foreach (Material m in mats)
        {
            m.SetFloat("_Mode", 2);
            m.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            m.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            m.SetInt("_ZWrite", 0);
            m.DisableKeyword("_ALPHATEST_ON");
            m.EnableKeyword("_ALPHABLEND_ON");
            m.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            m.renderQueue = 3000;
            fadeOut = true;
        }
    }
    private void FadeOutMaterial()
    {
        foreach (Material m in mats)
        {
            alpha = AnimMath.Slide(alpha, .25f, 0.001f);
            byte alpha2 = (byte)(alpha * 255);
            m.color = new Color32(255, 255, 255, alpha2);
        }
        if (alpha <= 0.24f)
        {
            fadeOut = false;
        }
    }

    private void FadeInMaterial()
    {
        foreach (Material m in mats)
        {
            alpha = AnimMath.Slide(alpha, 1, 0.001f);
            byte alpha2 = (byte)(alpha * 255);
            m.color = new Color32(255, 255, 255, alpha2);
        }
        if (alpha >= 0.99f)
        {
            SetMaterialOpaque();
            fadeIn = false;
        }
    }

    private void SetMaterialOpaque()
    {
        foreach (Material m in mats)
        {
            m.SetFloat("_Mode", 0);
            m.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            m.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            m.SetInt("_ZWrite", 1);
            m.DisableKeyword("_ALPHATEST_ON");
            m.DisableKeyword("_ALPHABLEND_ON");
            m.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            m.renderQueue = -1;
        }
    }
}
