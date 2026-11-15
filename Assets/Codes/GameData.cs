using UnityEngine;

// 스테이지 기록 (별, 최고 기록, 클리어 여부)
// static 변수라서 씬을 바꿔도 유지되고, 게임을 완전히 껐다 켜면 초기화됩니다.
public static class GameData
{
    public const int NormalStageCount = 5;  // 일반 스테이지 1~5
    public const int HardStageIndex = 5;    // 어려운 스테이지 (6번째)
    public const int TotalStageCount = 6;

    public static int selectedStage = 0;    // StageScene에서 고른 스테이지

    public static int[] bestStars = new int[TotalStageCount];
    public static float[] bestTimes = new float[TotalStageCount];
    public static bool[] cleared = new bool[TotalStageCount];

    // 게임이 시작될 때 한 번 실행 → 기록 초기화
    // (이 프로젝트는 Enter Play Mode 옵션이 켜져 있어서, 이게 없으면
    //  에디터에서 플레이를 껐다 켜도 기록이 남아 있게 됩니다)
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetAll()
    {
        selectedStage = 0;
        bestStars = new int[TotalStageCount];
        bestTimes = new float[TotalStageCount];
        cleared = new bool[TotalStageCount];
    }

    // 일반 스테이지 5개를 모두 별 3개로 깼는지
    public static bool IsHardUnlocked()
    {
        for (int i = 0; i < NormalStageCount; i++)
            if (bestStars[i] < 3) return false;
        return true;
    }

    // 일반 스테이지에서 모은 별 개수 (최대 15)
    public static int NormalStars()
    {
        int sum = 0;
        for (int i = 0; i < NormalStageCount; i++) sum += bestStars[i];
        return sum;
    }

    public static bool IsUnlocked(int stage)
    {
        return stage < NormalStageCount || IsHardUnlocked();
    }

    // 더 좋은 기록만 저장
    public static void SaveResult(int stage, int stars, float time)
    {
        cleared[stage] = true;
        if (stars > bestStars[stage]) bestStars[stage] = stars;
        if (bestTimes[stage] <= 0f || time < bestTimes[stage]) bestTimes[stage] = time;
    }

    public static int TotalStars()
    {
        int sum = 0;
        foreach (int s in bestStars) sum += s;
        return sum;
    }

    public static string FormatTime(float t)
    {
        int min = (int)(t / 60f);
        float sec = t - min * 60;
        return string.Format("{0}:{1:00.00}", min, sec);
    }
}
