using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace FateGames
{
    public class UIStartText : UIElement
    {
        private static UIStartText instance;

        public static UIStartText Instance { get => instance; }

        private void Awake()
        {
            if (!instance)
                instance = this;
            else
            {
                Destroy(gameObject);
                return;
            }
        }
    }
}