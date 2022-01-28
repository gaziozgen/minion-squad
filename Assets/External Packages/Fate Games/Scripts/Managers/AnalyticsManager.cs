using GameAnalyticsSDK;

namespace FateGames
{
    public static class AnalyticsManager
    {
        public static void Initialize()
        {
            GameAnalytics.Initialize();
        }

        public static void ReportStartProgress()
        {
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, "level" + PlayerProgression.CurrentLevel);
        }

        public static void ReportFinishProgress(bool success)
        {
            GameAnalytics.NewProgressionEvent(success ? GAProgressionStatus.Complete : GAProgressionStatus.Fail, "level" + PlayerProgression.CurrentLevel);
        }
    }
}

