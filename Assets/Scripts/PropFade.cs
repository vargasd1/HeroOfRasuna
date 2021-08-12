using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropFade : MonoBehaviour
{
    private List<Material> mats = new List<Material>();
    private float alpha = 1;
    private float timer = 2;

    // Start is called before the first frame update
    void Start()
    {
        Physics.IgnoreLayerCollision(10, 12);
        foreach (Transform child in transform)
        {
            foreach (Material mat in child.GetComponent<Renderer>().materials)
            {
                mats.Add(mat);
            }
        }


    }

    // Update is called once per frame
    void Update()
    {
        if (timer >= 0)
        {
            timer -= Time.fixedUnscaledDeltaTime;
        }
        else
        {
            SetMaterialTransparent();
            FadeOutMaterial();
            alpha -= Time.unscaledDeltaTime * 0.2f;
            if (alpha <= 0f)
            {
                Destroy(gameObject);
            }
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
        }
    }
    private void FadeOutMaterial()
    {
        foreach (Material m in mats)
        {
            byte alpha2 = (byte)(alpha * 255);
            m.color = new Color32(81, 81, 81, alpha2);
        }
    }
}
