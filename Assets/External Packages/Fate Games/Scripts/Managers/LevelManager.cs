using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace FateGames
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private UnityEvent onStart, onSuccess, onFail;
        public void StartLevel()
        {
            onStart.Invoke();
            FindObjectOfType<PoliceCar>().StartPlay();
        }
        public void FinishLevel(bool success)
        {
            if (success) onSuccess.Invoke();
            else onFail.Invoke();
        }
    }
}