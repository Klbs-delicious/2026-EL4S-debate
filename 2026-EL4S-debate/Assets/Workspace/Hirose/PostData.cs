/*
 * 最終更新日時：2026/05/08
 * 作成者：廣瀬宗貴
 *
 * 概要：
 * AIレスバトルで使用するテスト用コメントデータを管理するクラス。
 */

public static class PostData
{
    public static string[] GetPythonPosts()
    {
        return new string[]
        {
            "Pythonはライブラリが多い",
            "初心者でも書きやすい",
            "試作が早くできる",
            "C++より処理速度は遅いけど、開発効率は高い"
        };
    }

    public static string[] GetCppPosts()
    {
        return new string[]
        {
            "Pythonは実行速度が遅い",
            "C++は型が明確で、大規模開発でも安全に書きやすい",
            "コンパイル時にエラーを見つけやすい",
            "処理速度が必要なゲームやシステム開発ではC++が強い"
        };
    }
}