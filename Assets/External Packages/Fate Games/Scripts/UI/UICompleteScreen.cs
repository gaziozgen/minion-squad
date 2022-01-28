using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace FateGames
{
    public class UICompleteScreen : UIElement
    {
        [SerializeField] private GameObject winScreen, loseScreen;
        [SerializeField] private Text levelText, coinText, gainText;
        [SerializeField] private RectTransform spreadCoinFrom, spreadCoinTo;
        private float totalCoin = 0;

        public void SetScreen(bool success, int level)
        {
            coinText.text = PlayerProgression.COIN.ToString();
            winScreen.SetActive(success);
            loseScreen.SetActive(!success);
            if (!success)
            {
                levelText.text = "LEVEL " + PlayerProgression.CurrentLevel;
            }
            else
            {
                /*if (MainLevelManager.Instance.Coin > 0)
                    SpreadCoin();*/
            }
        }

        // Called by ContinueButton onClick
        public void Continue()
        {
            SceneManager.LoadCurrentLevel();
        }
        public void SpreadCoin()
        {
            Queue<(Transform, float)> queue = new Queue<(Transform, float)>();
            totalCoin = PlayerProgression.COIN;
            for (int i = 0; i < 20; i++)
            {
                Transform coin = ObjectPooler.Instance.SpawnFromPool("Coin Image", spreadCoinFrom.position, Quaternion.identity).transform;
                coin.parent = transform;
                //queue.Enqueue((coin, MainLevelManager.Instance.Coin / 20f));
                coin.LeanMove(coin.transform.position + new Vector3(Random.Range(-Screen.width / 15f, Screen.width / 15f), Random.Range(-Screen.width / 15f, Screen.width / 15f), 0), 0.4f)
                    .setEaseInOutBack();
            }

            LeanTween.delayedCall(0.41f, () => { StartCoroutine(SpreadCoinCoroutine(queue)); });

        }

        public IEnumerator SpreadCoinCoroutine(Queue<(Transform, float)> queue)
        {
            if (queue.Count > 0)
            {
                (Transform coin, float gain) = queue.Dequeue();
                coin.LeanMove(spreadCoinTo.position, 1.5f)
                    .setEaseInOutBack()
                    .setOnComplete(() =>
                    {
                        totalCoin += gain;
                        coinText.text = Mathf.RoundToInt(totalCoin).ToString();
                    });
                yield return new WaitForSeconds(0.05f);
                StartCoroutine(SpreadCoinCoroutine(queue));
            }
            else yield return null;
        }
    }
}