
/// <summary>
/// チャット入力のインターフェース
/// </summary>
public interface IChatInputHandler
{
    // 入力を確定（送信）させる
    void OnSubmit();

    // どのチームの入力かを識別するタグ（"A" or "B"）
    string TeamTag { get; set; }
}