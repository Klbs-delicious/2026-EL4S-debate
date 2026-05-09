using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// プレイヤーのチャット入力を処理するクラス
/// </summary>
public class PlayerChatInput : MonoBehaviour, IChatInputHandler
{
    [Header("参照")]
    [SerializeField] private TMP_InputField inputField; // チャット入力フィールド  
    [SerializeField] private AIPipeline pipeline;       // AI通信の流れを管理するクラス

    [Header("設定")]
    [SerializeField] private string teamTag = "A";

    public string TeamTag
    {
        get => teamTag;
        set => teamTag = value;
    }

    private void Start()
    {
        // 念のため初期化
        if (inputField != null)
        {
            inputField.ActivateInputField();
        }
    }

    private void Update()
    {
        // Enterキー または テンキーのEnterが押された瞬間を判定
        if (Keyboard.current.enterKey.wasPressedThisFrame || Keyboard.current.numpadEnterKey.wasPressedThisFrame)
        {
            // チャットを開く処理など
            OnSubmit();
        }
    }

    /// <summary>
    /// 入力を確定（送信）させる
    /// </summary>
    public void OnSubmit()
    {
        if (string.IsNullOrWhiteSpace(inputField.text)) { return; }

        // パイプラインにメッセージを投げる
        pipeline.AddChatMessage(TeamTag, inputField.text);

        // UI側の後処理
        inputField.text = "";
        inputField.ActivateInputField(); 
    }
}