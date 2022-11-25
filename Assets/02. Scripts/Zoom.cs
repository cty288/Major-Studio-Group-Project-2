using System;
using System.Collections;
using System.Collections.Generic;
using Mikrocosmos;
using UnityEngine;

public class Zoom :MonoBehaviour
{
    public Material material;

    //猫眼位置
    public Transform spyhole;

    public float zoomIntensity = 0.4f;

    public float size = 0.15f;

    public float edgeIntensity = 0.05f;

    private void Awake() {
        material = ImageEffectController.Singleton.GetScriptableRendererFeatureMaterial(0);
        gameObject.SetActive(false);
    }

    private void Update() {
        if (material)
        {
            material.SetVector("_Pos", spyhole.transform.position);
            material.SetFloat("_ZoomIntensity", zoomIntensity);
            material.SetFloat("_EdgeIntensity", edgeIntensity);
            material.SetFloat("_Size", size);
            
        }
    }
}
