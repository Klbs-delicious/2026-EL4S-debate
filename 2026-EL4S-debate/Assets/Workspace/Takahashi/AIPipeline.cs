using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// AI通信の流れを管理するクラス
/// </summary>
public class AIPipeline : MonoBehaviour
{
    // データの進行状況を定義
    public enum AIState { Idle, Processing, Connecting, Verdict }

    [SerializeField]
    private AIState currentState = AIState.Idle;

    [Header("参照")]
    [SerializeField, Tooltip("ゲーム全体の流れを管理するクラス")]
    GameSystem gameSystem;

    [SerializeField, Tooltip("AIレスバトルのAPI送信テストを行うクラス")]
    BattleAPI battleAPI;

    [Header("Settings")]
    [SerializeField, Tooltip("何秒ごとに判定するか（AIにチャット内容を送るのか）")]
    private float judgeInterval = 10f;
    private float timer;

    [SerializeField]
    bool isDebugMode = true;
    [SerializeField] private IAIssuemanager aiManager;

    private bool aiOutput = false;
    public APICommunicator.BattleCombinedResult result;

    void Update()
    {
        // 送信待ち時間の更新
        if (currentState == AIState.Idle)
        {
            timer += Time.deltaTime;
            if (timer >= judgeInterval)
            {
                // AIに送る処理を開始
                StartProcessing();
            }

            // デバッグモードの場合は、常にAIに送る処理を開始する
            if (isDebugMode)
            {
                // AIに送る処理を開始
                Debug.Log("Debug Mode: AIに送る処理を開始");

                StartProcessing();
            }
        }

        else if (currentState == AIState.Connecting)
        {
            // AIからの結果を待つ状態
            if (battleAPI.output)
            {
                battleAPI.output = false;
                OnReceiveResult();
            }
        }
    }

    /// <summary>
    /// チャットメッセージを受け取ってバッファに追加する
    /// </summary>
    /// <param name="team"></param>
    /// <param name="message"></param>
    public void AddChatMessage(string _team, string _message)
    {
        if (_team == "A")
        {
            // TODO: Aチームのチャットを受け取る処理
            battleAPI.CommentSet_A(_message);
        }
        else if (_team == "B")
        {
            // TODO: Bチームのチャットを受け取る処理
            battleAPI.CommentSet_B(_message);
        }
    }

    /// <summary>
    /// AIに送るための処理を開始する
    /// </summary>
    void StartProcessing()
    {
        currentState = AIState.Processing;
        Debug.Log("--- 1. Processing: スナップショット取得 ---");

        // 通信開始
        StartConnecting();
    }

    /// <summary>
    /// 通信開始処理
    /// </summary>
    void StartConnecting()
    {
        currentState = AIState.Connecting;
        Debug.Log("--- 2. Connecting: IT担当のAIへ送信中 ---");

        // ここで通信の処理を呼ぶ
        battleAPI.RunBattleTestButton();
    }

    /// <summary>
    /// AIからの結果を受け取る処理
    /// </summary>
    void OnReceiveResult()
    {
        currentState = AIState.Verdict;
        Debug.Log("--- 3. Verdict: 結果受信・演出中 ---");

        // TODO: ここで受け取った結果をもとに演出の処理を呼ぶ
        result = battleAPI.save_result;

        aiManager.ReflecttionAnswer(result);

        //// スコアの更新
        //int score = result.neutral_result.score;
        //gameSystem.UpdateScore(score);

        // 演出時間を考慮して3秒後にIdleに戻る
        Invoke("BackToIdle", 3.0f);
    }

    /// <summary>
    /// Idle状態に戻る処理
    /// 主に次の判定サイクルの準備（タイマーリセットなど）を行う
    /// </summary>
    void BackToIdle()
    {
        Debug.Log("--- 4. Idle: 次の判定サイクル ---");
        timer = 0;
        currentState = AIState.Idle;

        // デバッグモードの場合は、常にAIに送る処理を開始する
        isDebugMode = false;
    }
}