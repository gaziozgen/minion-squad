using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FateGames
{
    public class Collector : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            Collectible collectible = other.GetComponent<Collectible>();
            if (collectible) collectible.OnCollected.Invoke();
        }
    }

}