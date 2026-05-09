using UnityEngine;

/// <summary>
/// ゲーム全体の流れ（ラウンド制）を管理するクラス
/// </summary>
public class GameSystem : MonoBehaviour
{
    public enum GamePhase { Ready, InBattle, RoundEnd, Result }

    [Header("ゲームの状態")]
    [SerializeField] private GamePhase currentPhase = GamePhase.Ready;
    [SerializeField] private int currentRound = 1;
    [SerializeField] private int maxRounds = 6;

    [Header("参照")]
    [SerializeField] private AIPipeline pipeline;
    [SerializeField] private TeamBAutoChatHandler teamB;


    [Header("設定")]
    [SerializeField, Tooltip("1ラウンドあたりの時間")]
    private float roundDuration = 10f;
    private float roundTimer;

    private void Awake()
    {
        pipeline.enabled = false;
        teamB.enabled = false;
    }

    void Start()
    {
        StartGame();
    }

    void Update()
    {
        if (currentPhase == GamePhase.InBattle)
        {
            pipeline.enabled = true;
            teamB.enabled = true;

            roundTimer -= Time.deltaTime;
            if (roundTimer <= 0f)
            {
                ProcessRoundEnd();
            }
        }
    }

    public void StartGame()
    {
        currentRound = 1;
        StartNewRound();
    }

    private void StartNewRound()
    {
        Debug.Log($"第 {currentRound} ラウンド開始！");
        currentPhase = GamePhase.InBattle;
        roundTimer = roundDuration;

        // Pipeline側のタイマーもリセットして同期させる（必要なら）
        // pipeline.ResetTimer(); 
    }

    private void ProcessRoundEnd()
    {
        Debug.Log($"第 {currentRound} ラウンド終了");

        if (currentRound >= maxRounds)
        {
            EndGame();
        }
        else
        {
            // 次のラウンドへ
            currentRound++;
            StartNewRound();

            // TODO: ここで「ROUND 2」などのUI演出を挟む場合は
            // ステートを RoundEnd にしてコルーチンなどで待機すると「いい塩梅」です
        }
    }

    public void EndGame()
    {
        currentPhase = GamePhase.Result;
        pipeline.enabled = false;
        teamB.enabled = false;
        pipeline.CancelInvoke();

        Debug.Log("全ラウンド終了！最終結果発表！");
    }

    public void UpdateScore(int _score)
    {
        // AIPipelineから判定が返ってきた時に呼ばれる
        Debug.Log($"Round {currentRound} Result: {_score}");
    }
}