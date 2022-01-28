using UnityEngine;
using UnityEngine.Events;
namespace FateGames
{
    public class Collectible : MonoBehaviour
    {
        [SerializeField] private UnityEvent onCollected;
        public UnityEvent OnCollected { get => onCollected; }

    }
}