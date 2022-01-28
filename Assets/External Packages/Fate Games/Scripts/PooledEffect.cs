using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FateGames
{
    public class PooledEffect : MonoBehaviour, IPooledObject
    {
        [SerializeField] private ParticleSystem effect = null;
        [SerializeField] private AudioSource sound = null;

        public ParticleSystem Effect { get => effect; }

        public void OnObjectSpawn()
        {
            effect.Play();
            if (sound)
                sound.Play();
        }
    }
}