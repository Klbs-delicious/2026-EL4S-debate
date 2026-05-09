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
            "開発効率が高い",
            "かんたん",
            "分かりやすい",
            "処理速度は遅いけど、使う用途を考えればいいだけ",
            "配列の長さとかを決めなくていいのが楽"
        };
    }

    public static string[] GetCppPosts()
    {
        return new string[]
        {
            "Pythonおもんない",
            "Python使ってるやつ頭悪そう",
            "くだらん",
            "殺すぞ",
            "勝ったな風呂入ってくる",
            "C++使える奴の方が年収高そう"
        };
    }
}