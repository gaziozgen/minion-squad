using UnityEngine;
namespace FateGames
{
    public static class PlayerProgression
    {
        public static int CurrentLevel
        {
            get
            {
                int result = PlayerPrefs.GetInt("CurrentLevel");
                if (result == 0)
                {
                    PlayerProgression.CurrentLevel = 1;
                    result++;
                }
                return result;
            }
            set => PlayerPrefs.SetInt("CurrentLevel", value);
        }
        public static int MONEY { get => PlayerPrefs.GetInt("MONEY"); set => PlayerPrefs.SetInt("MONEY", value); }
        public static int COIN { get => PlayerPrefs.GetInt("COIN"); set => PlayerPrefs.SetInt("COIN", value); }
        public static int GEM { get => PlayerPrefs.GetInt("GEM"); set => PlayerPrefs.SetInt("GEM", value); }
        public static int KEY { get => PlayerPrefs.GetInt("KEY"); set => PlayerPrefs.SetInt("KEY", value); }
        public static int GOLD { get => PlayerPrefs.GetInt("GOLD"); set => PlayerPrefs.SetInt("GOLD", value); }
        public static int DIAMOND { get => PlayerPrefs.GetInt("DIAMOND"); set => PlayerPrefs.SetInt("DIAMOND", value); }
    }

}
