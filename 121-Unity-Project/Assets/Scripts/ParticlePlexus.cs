using System.Collections.Generic;
using UnityEngine;

// Adapted from https://www.youtube.com/watch?v=ruNPkuYT1Ck

[RequireComponent(typeof(ParticleSystem))]
public class ParticlePlexus : MonoBehaviour
{
    // Start is called before the first frame update
    public float maxDistance = 1.0f;
    new ParticleSystem particleSystem;
    ParticleSystem.Particle[] particles;

    ParticleSystem.MainModule particleSystemMainModule;

    public LineRenderer lineRendererTemplate;
    List<LineRenderer> lineRenderers = new List<LineRenderer>();

    Transform _transform;

    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
        particleSystemMainModule = particleSystem.main;
        _transform = transform;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        int maxParticles = particleSystemMainModule.maxParticles;
        
        if (particles == null || particles.Length < maxParticles) {
            particles = new ParticleSystem.Particle[maxParticles];
        }

        particleSystem.GetParticles(particles);
        int particleCount = particleSystem.particleCount;

        float maxDistanceSqr = maxDistance * maxDistance;
        int lrIndex = 0;
        int lineRendererCount = lineRenderers.Count;

        for (int i = 0; i < particleCount; i++) {
            Vector3 p1_pos = particles[i].position;

            for (int j = i + 1; j < particleCount; j++) {
                Vector3 p2_pos = particles[j].position;
                float distanceSqr = Vector3.SqrMagnitude(p1_pos - p2_pos);
                
                if (distanceSqr <= maxDistanceSqr) {
                    LineRenderer lr;
                    if(lrIndex == lineRendererCount){
                        lr = Instantiate(lineRendererTemplate, _transform, false);
                        lineRenderers.Add(lr);
                    }

                    lr = lineRenderers[lrIndex];
                    lr.enabled = true;

                    lr.SetPosition(0, p1_pos); 
                    lr.SetPosition(1, p2_pos);

                    lrIndex++;
                    lineRendererCount++; 
                }
            }
        }

        for(int i = lrIndex; i < lineRendererCount; i++) {
            lineRenderers[i].enabled = false;
        }

    }
}
