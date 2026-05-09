/*
 * 最終更新日時：2026/05/09
 * 作成者：廣瀬宗貴
 *
 * 概要：
 * Flaskサーバーに対して、AIレスバトル1ラウンド分の情報を送信し、
 * Flask経由でGemini APIの結果を取得するクラス。
 *
 * 注意：
 * Unity側にはGemini APIキーを絶対に置かない。
 */

using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class APICommunicator : MonoBehaviour
{
    [Header("Flask Server URL")]
    private string flaskUrl = "http://10.64.61.8:5000/api/gemini";


    [Serializable]
    public class BattleRequestData
    {
        public string battle_id;
        public int round;
        public string topic;
        public SideRequestData side_a;
        public SideRequestData side_b;
        public string previous_neutral_comment;
    }

    [Serializable]
    public class SideRequestData
    {
        public string side;
        public string[] comments;
        public string previous_opponent_output;
    }

    [Serializable]
    public class BattleCombinedResult
    {
        public SideResult side_a_result;
        public SideResult side_b_result;
        public NeutralResult neutral_result;
    }

    [Serializable]
    public class SideResult
    {
        public string side;
        public string output_result;
    }

    [Serializable]
    public class NeutralResult
    {
        public int score;
        public string selected_side;
        public string comment;
    }

    [Serializable]
    private class GeminiRequest
    {
        public Content[] contents;
    }

    [Serializable]
    private class Content
    {
        public Part[] parts;
    }

    [Serializable]
    private class Part
    {
        public string text;
    }

    [Serializable]
    private class GeminiResponse
    {
        public Candidate[] candidates;
    }

    [Serializable]
    private class Candidate
    {
        public Content content;
    }

    public IEnumerator SendBattleRequest(
        BattleRequestData battleData,
        Action<BattleCombinedResult> onSuccess,
        Action<string> onError = null
    )
    {
        string prompt = CreatePrompt(battleData);

        GeminiRequest requestData = new GeminiRequest
        {
            contents = new Content[]
            {
                new Content
                {
                    parts = new Part[]
                    {
                        new Part { text = prompt }
                    }
                }
            }
        };

        string json = JsonUtility.ToJson(requestData);

        UnityWebRequest request = new UnityWebRequest(flaskUrl, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Flask APIエラー: " + request.responseCode);
            Debug.LogError(request.downloadHandler.text);
            onError?.Invoke(request.error + " / " + request.downloadHandler.text);
            yield break;
        }

        GeminiResponse geminiResponse =
            JsonUtility.FromJson<GeminiResponse>(request.downloadHandler.text);

        if (geminiResponse == null ||
            geminiResponse.candidates == null ||
            geminiResponse.candidates.Length == 0 ||
            geminiResponse.candidates[0].content == null ||
            geminiResponse.candidates[0].content.parts == null ||
            geminiResponse.candidates[0].content.parts.Length == 0)
        {
            onError?.Invoke("Geminiのレスポンスが空、または形式が想定外です。");
            yield break;
        }

        string aiText = geminiResponse.candidates[0].content.parts[0].text;
        aiText = CleanJsonText(aiText);

        Debug.Log("AI JSON: " + aiText);

        BattleCombinedResult result =
            JsonUtility.FromJson<BattleCombinedResult>(aiText);

        if (result == null ||
            result.side_a_result == null ||
            result.side_b_result == null ||
            result.neutral_result == null)
        {
            onError?.Invoke("AIのJSON変換に失敗しました。");
            yield break;
        }

        onSuccess?.Invoke(result);
    }

    private string CreatePrompt(BattleRequestData data)
    {
        return
$@"あなたはAIレスバトルアプリの進行AIです。

以下の情報をもとに、2つの陣営の代表発言と、ニュートラルAIの判定を作成してください。

【お題】
{data.topic}

【ラウンド】
{data.round}

【{data.side_a.side} のコメント】
{CreateCommentText(data.side_a.comments)}

【{data.side_a.side} が参考にする前回の相手出力】
{GetSafeText(data.side_a.previous_opponent_output)}

【{data.side_b.side} のコメント】
{CreateCommentText(data.side_b.comments)}

【{data.side_b.side} が参考にする前回の相手出力】
{GetSafeText(data.side_b.previous_opponent_output)}

【前回のニュートラルAIの一言コメント】
{GetSafeText(data.previous_neutral_comment)}

【代表発言の条件】
・各陣営の発言は全角1文字として、50文字～70文字
・少し熱量のある、人間らしい口調にする
・相手陣営への人格否定はしない
・ユーザーの元コメントの表現を尊重し、可能な限りそのままの形で使う
・一般的な考えは排除し、あくまでコメント内容を補強する形で付け加える

【ニュートラルAIの条件】
・両陣営のどちらの意見に納得感があるかを判断する
・評価は論理性、説得力、反論力、エンタメ性の４項目
・それぞれの評価項目は10段階で、合計評価の高さを決める
・scoreは、-100～100で、負が大きいほうがside_a寄り、正が大きいほうがside_b寄りとする
・selected_sideには選んだ陣営名を書く
・commentは一言で理由を書く

【出力形式】
必ず以下のJSON形式だけで出力してください。
JSON以外の説明文は絶対に出力しないでください。

{{
  ""side_a_result"": {{
    ""side"": ""{data.side_a.side}"",
    ""output_result"": ""{data.side_a.side}の代表発言""
  }},
  ""side_b_result"": {{
    ""side"": ""{data.side_b.side}"",
    ""output_result"": ""{data.side_b.side}の代表発言""
  }},
  ""neutral_result"": {{
    ""score"": 80,
    ""selected_side"": ""選んだ陣営名"",
    ""comment"": ""一言コメント""
  }}
}}";
    }

    private string CreateCommentText(string[] comments)
    {
        if (comments == null || comments.Length == 0)
        {
            return "コメントはありません。";
        }

        string text = "";

        for (int i = 0; i < comments.Length; i++)
        {
            text += "- " + comments[i] + "\n";
        }

        return text;
    }

    private string GetSafeText(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return "なし";
        }

        return text;
    }

    private string CleanJsonText(string text)
    {
        text = text.Trim();

        if (text.StartsWith("```json"))
        {
            text = text.Substring(7);
        }

        if (text.StartsWith("```"))
        {
            text = text.Substring(3);
        }

        if (text.EndsWith("```"))
        {
            text = text.Substring(0, text.Length - 3);
        }

        int start = text.IndexOf("{");
        int end = text.LastIndexOf("}");

        if (start >= 0 && end >= 0 && end > start)
        {
            text = text.Substring(start, end - start + 1);
        }

        return text.Trim();
    }
}