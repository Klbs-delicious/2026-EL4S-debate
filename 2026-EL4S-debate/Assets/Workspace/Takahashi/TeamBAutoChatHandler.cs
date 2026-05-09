using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// チームB（相手側）の自動チャット入力を管理するクラス
/// </summary>
public class TeamBAutoChatHandler : MonoBehaviour, IChatInputHandler
{
    [Header("設定")]
    public string TeamTag { get; set; } = "B";

    [SerializeField, Tooltip("何秒ごとに発言するか")]
    private float autoChatInterval = 5f;

    [SerializeField, Tooltip("ループさせる発言リスト")]
    private List<string> presetMessages = new List<string>
    {
        "それは主観ですよね？ソースあります？",
        "論点がズレてます。今話してるのは実装コストの話です。",
        "Pythonで十分とか、パフォーマンス度外視すぎませんか？",
        "もっと論理的に話してもらえますか（笑）",
        "それ、さっきも言いましたよね？"

    };

    [Header("参照")]
    [SerializeField] private AIPipeline pipeline;

    private float timer;
    private int messageIndex = 0;
    private bool isActive = false;

    void Start()
    {
        // GameSystemから呼ばれるまで待つ場合はここをfalseに
        isActive = true;
    }

    void Update()
    {
        if (!isActive) { return; }

        timer += Time.deltaTime;
        if (timer >= autoChatInterval)
        {
            timer = 0;
            OnSubmit();
        }
    }

    public void OnSubmit()
    {
        if (presetMessages.Count == 0) { return; }

        // リストからメッセージを取得
        string message = presetMessages[messageIndex];

        // Pipelineに送信
        pipeline.AddChatMessage(TeamTag, message);

        //// 画面ログへの表示（演出担当へのブリッジ）
        //Debug.Log($"<color=red>TeamB:</color> {message}");

        // 次のメッセージへ（ループ）
        messageIndex = (messageIndex + 1) % presetMessages.Count;
    }

    // ゲーム開始時に外から叩く用
    public void SetActive(bool state) => isActive = state;
}