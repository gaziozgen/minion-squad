using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace FateGames
{
    public class UIManager : MonoBehaviour
    {
        private static UIManager instance;
        public Canvas UICanvas { get; set; }
        public static UIManager Instance { get => instance; }
        [Header("Prefabs")]
        [SerializeField] private GameObject uiPrefab = null;
        [SerializeField] private GameObject uiLoadingScreenPrefab = null;
        [SerializeField] private GameObject uiCompleteScreenPrefab = null;
        [SerializeField] private GameObject uiLevelTextPrefab = null;
        [SerializeField] private GameObject uiStartTextPrefab = null;
        private void Awake()
        {
            if (!instance)
                instance = this;
            else
            {
                Destroy(gameObject);
                return;
            }
            CreateUI();
        }

        private void OnLevelWasLoaded(int level)
        {
            CreateUI();
        }

        private void CreateUI()
        {
            UICanvas = Instantiate(uiPrefab).GetComponentInChildren<Canvas>();
        }

        public void CreateUILevelText()
        {
            GameObject go = Instantiate(uiLevelTextPrefab, UICanvas.transform);
            TextMeshProUGUI levelText = go.GetComponentInChildren<TextMeshProUGUI>();
            levelText.text = "Level " + PlayerProgression.CurrentLevel;
        }

        public void CreateUIStartText() => Instantiate(uiStartTextPrefab, UICanvas.transform);
        public void CreateUICompleteScreen(bool success)
        {
            GameObject go = Instantiate(uiCompleteScreenPrefab, UICanvas.transform);
            UICompleteScreen uiCompleteScreen = go.GetComponent<UICompleteScreen>();
            uiCompleteScreen.SetScreen(success, PlayerProgression.CurrentLevel);
        }
        public void CreateLoadingScreen() => Instantiate(uiLoadingScreenPrefab, UICanvas.transform);
    }
}
