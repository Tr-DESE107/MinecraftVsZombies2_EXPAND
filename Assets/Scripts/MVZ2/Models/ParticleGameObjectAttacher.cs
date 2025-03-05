using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleGameObjectAttacher : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        m_ParticleSystem = GetComponent<ParticleSystem>();
        m_Particles = new ParticleSystem.Particle[m_ParticleSystem.main.maxParticles];
    }

    // Update is called once per frame
    void LateUpdate()
    {
        int count = m_ParticleSystem.GetParticles(m_Particles);

        while (m_Instances.Count < count)
            m_Instances.Add(Instantiate(m_Prefab, transform));

        bool worldSpace = (m_ParticleSystem.main.simulationSpace == ParticleSystemSimulationSpace.World);
        for (int i = 0; i < m_Instances.Count; i++)
        {
            var instance = m_Instances[i];
            if (i < count)
            {
                var particle = m_Particles[i];
                if (worldSpace)
                    instance.transform.position = particle.position;
                else
                    instance.transform.localPosition = particle.position;
                instance.SetActive(true);
                if (scaleMethod == ScaleMethod.Lifetime)
                {
                    instance.transform.localScale = Vector3.one * (particle.remainingLifetime / particle.startLifetime * scale);
                }
                else
                {
                    var current = particle.GetCurrentSize3D(m_ParticleSystem);
                    var start = particle.startSize3D;
                    var x = current.x / start.x * scale;
                    var y = current.y / start.y * scale;
                    var z = current.z / start.z * scale;
                    instance.transform.localScale = new Vector3(x, y, z);
                }
            }
            else
            {
                instance.SetActive(false);
            }
        }
    }
    [SerializeField]
    public ScaleMethod scaleMethod;
    [SerializeField]
    public float scale = 1;
    [SerializeField]
    public Color color = Color.white;

    private ParticleSystem m_ParticleSystem;
    private ParticleSystem.Particle[] m_Particles;
}

public enum ScaleMethod
{
    Lifetime,
    Size
}