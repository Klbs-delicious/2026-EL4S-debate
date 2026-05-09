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
            "開発効率が高い"
        };
    }

    public static string[] GetCppPosts()
    {
        return new string[]
        {
            "Pythonおもんない",
            "Python使ってるやつ頭悪そう",
            "4ね"
        };
    }
}