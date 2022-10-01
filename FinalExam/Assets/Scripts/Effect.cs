using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour {

    public float spawnEffectTime = 2;
    public float pause = 1;
    public AnimationCurve fadeIn;
    private int shaderProperty;
    private float timer = 0;
    ParticleSystem particleSystem;
    Renderer renderer;

	void Start ()
    {
        shaderProperty = Shader.PropertyToID("_cutoff");
        renderer = GetComponent<Renderer>();
        particleSystem = GetComponentInChildren <ParticleSystem>();
        var main = particleSystem.main;
        main.duration = spawnEffectTime;
        particleSystem.Stop();
    }
	
	void Update ()
    {
        if (!particleSystem.isStopped)
        {
            ActiveEffect();
        }

    }

    void ActiveEffect()
    {
        if (timer < spawnEffectTime + pause)
        {
            timer += Time.deltaTime;
        }
        else
        {
            particleSystem.Stop();
            timer = 0;
        }
        renderer.material.SetFloat(shaderProperty, fadeIn.Evaluate(Mathf.InverseLerp(0, spawnEffectTime, timer)));
    }
}
