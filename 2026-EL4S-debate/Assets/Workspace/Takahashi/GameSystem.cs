using UnityEngine;

/// <summary>
/// ゲーム全体の流れを管理するクラス
/// </summary>
public class GameSystem : MonoBehaviour
{
    // ゲームの進行状況
    public enum GamePhase { Ready, InBattle, Result }
    
    [Header("ゲームの状態")]
    [SerializeField]
    private GamePhase currentPhase = GamePhase.Ready;

    [Header("参照")]
    [SerializeField, Tooltip("AI通信の流れを管理します。")]
    private AIPipeline pipeline;
    [SerializeField, Tooltip("チームBの自動チャット入力を管理します。")]
    private TeamBAutoChatHandler teamB; 

    [Header("設定")]
    [SerializeField,Tooltip("試合時間です。")]
    private float matchDuration = 60f;
    private float matchTimer;

    private void Awake()
    {
        // パイプラインは非アクティブにしておく
        pipeline.enabled = false;
        teamB.enabled = false; 
    }

    void Start()
    {
        // ゲーム開始の準備
        StartGame();
    }

    void Update()
    {
        if (currentPhase == GamePhase.InBattle)
        {
            // 進行を有効化する
            pipeline.enabled = true;
            teamB.enabled = true;

            matchTimer -= Time.deltaTime;
            if (matchTimer <= 0f)
            {
                // タイムアップ
                EndGame();
            }
        }
    }

    /// <summary>
    /// ゲームを開始するメソッド
    /// </summary>
    public void StartGame()
    {
        currentPhase = GamePhase.InBattle;
        matchTimer = matchDuration;

        // Pipelineを有効化
        // またはカウント開始を指示
        Debug.Log("レスバ開始！");
    }

    public void EndGame()
    {
        currentPhase = GamePhase.Result;

        //進行を止める
        pipeline.enabled = false; 
        teamB.enabled = false;

        pipeline.CancelInvoke();  // 進行中のInvoke（擬似通信）も止める

        // Pipelineの動きを止めて、最終結果を表示する
        Debug.Log("タイムアップ！終了！");
    }

    /// <summary>
    /// スコアを更新するメソッド
    /// AIPipelineのVerdict（判定）から呼ばれる想定
    /// </summary>
    /// <param name="_score"></param>
    public void UpdateScore(int _score)
    {
        // TODO: UIにスコアを反映させる
    }
}