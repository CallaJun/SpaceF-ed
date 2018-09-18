using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
public class BlackOut : MonoBehaviour
{

    public static BlackOut singleton;
    public Material EffectMaterial;
	Renderer renderer;
	public float intensity = 0;
    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    /// 
    private void Start()
    {
        singleton = this;
    }
    void OnEnable()
    {
        // GetComponent<Camera>().depthTextureMode |= DepthTextureMode.DepthNormals;
    }
    void OnRenderImage(RenderTexture src, RenderTexture dst)
    {

        if (EffectMaterial != null)
            Graphics.Blit(src, dst, EffectMaterial);
    }
	void Update() {
		EffectMaterial.SetFloat("_Magnitude", intensity);
	}

	public IEnumerator ChangeIntensity(float newIntensity, float time) {
		float nowTime = 0, startVal = intensity;
		while (nowTime <= time) {
			nowTime += Time.deltaTime;
			float t = Mathf.Clamp01(nowTime / time);
			float val = Mathf.Lerp(startVal, newIntensity, t);
			intensity = val;
			EffectMaterial.SetFloat("_Magnitude", intensity);
			yield return null;
		}
	}
}

