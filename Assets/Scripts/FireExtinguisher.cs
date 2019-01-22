using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireExtinguisher : MonoBehaviour {
    public bool IsHoldingExtinguisher = false;
    public GameObject ExtinguisherParticles;
    private ParticleSystem[] _particles;
    public BoxCollider ParticlesCollider;

    private AudioSource _audioSource;
    private bool _isAudioPlayed = false;

    void Start ()
    {
        _audioSource = GetComponent<AudioSource>();
        _particles = ExtinguisherParticles.GetComponentsInChildren<ParticleSystem>();

        foreach (ParticleSystem particleSystem in _particles)
        {
            particleSystem.enableEmission = false;
        }
    }
	
	void Update () {
	    if (IsHoldingExtinguisher && (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger) ||
                    Input.GetKey(KeyCode.K)))
        {
            if (_isAudioPlayed == false)
            {
                _audioSource.volume = ObjectivesSelector.SoundsVolume;
                _audioSource.Play();

                _isAudioPlayed = true;
            }

            foreach (ParticleSystem particleSystem in _particles)
            {
                particleSystem.enableEmission = true;
                ParticlesCollider.enabled = true;
            }
        } else
        {
            foreach (ParticleSystem particleSystem in _particles)
            {
                particleSystem.enableEmission = false;
                ParticlesCollider.enabled = false;
            }

            _audioSource.Stop();
            _isAudioPlayed = false;
        }
	}

    public IEnumerator DestroyFire(GameObject fire)
    {
        yield return new WaitForSeconds(1.5f);
        Destroy(fire);
    }
}
