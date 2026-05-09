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
        "猫は飼っても散歩をしなくていいから楽できる！",
        "猫は甘えてくるときと自分の時間でほっとかれる時のギャップがいいんですよ",
        "毛並みがふわふわでかわいい",
        "猫はしつけが楽",
        "犬は散歩する時間があるから面倒",
        "犬は大きくなったら危ない",
        "猫は足にすりすりしてくるのがかわいい",
        "猫のにゃーはかわいいけど、犬のワンは怖い",
        "犬は鳴き声大きくて怖いよね",
        "クマの方が怖いよね",
        "犬は噛んでくることあるから怖い",
        "大型犬って見た目だけで怖い",
        "正直追いかけられたり、吠えられたりしたことあるからちょっとトラウマあるし、しつけがなってない犬は平気で初対面の人に対してそういうこと",
        "犬は動きが予測できなくて怖い",
        "猫の鋭い目が好き",
        "イルカは哺乳類だと思う"
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