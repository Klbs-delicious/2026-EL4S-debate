/*
 * 最終更新日時：2026/05/08
 * 作成者：廣瀬宗貴
 *
 * 概要：
 * Gemini APIに対して、AIレスバトル1ラウンド分の情報を送信し、
 * 両陣営の代表発言とニュートラルAIの判定結果を一度に取得するクラス。
 */

using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class APICommunicator : MonoBehaviour
{
    [Header("Gemini API Key")]
    [SerializeField] private string apiKey = "";

    [Header("Preferred Model")]
    [SerializeField] private string preferredModel = "gemini-1.5-flash";

    private const string BaseUrl = "https://generativelanguage.googleapis.com/v1beta";

    private string usableModel = "";

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

    [Serializable]
    private class ModelListResponse
    {
        public GeminiModel[] models;
    }

    [Serializable]
    private class GeminiModel
    {
        public string name;
        public string[] supportedGenerationMethods;
    }

    public IEnumerator SendBattleRequest(
        BattleRequestData battleData,
        Action<BattleCombinedResult> onSuccess,
        Action<string> onError = null
    )
    {
        if (string.IsNullOrEmpty(apiKey))
        {
            onError?.Invoke("Gemini APIキーが設定されていません。");
            yield break;
        }

        if (string.IsNullOrEmpty(usableModel))
        {
            yield return StartCoroutine(SelectUsableModel(
                () => { },
                error => onError?.Invoke(error)
            ));

            if (string.IsNullOrEmpty(usableModel))
            {
                yield break;
            }
        }

        string url = $"{BaseUrl}/models/{usableModel}:generateContent?key={apiKey}";
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

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("生成APIエラー: " + request.responseCode);
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

    private IEnumerator SelectUsableModel(Action onSuccess, Action<string> onError)
    {
        string url = $"{BaseUrl}/models?key={apiKey}";

        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            onError?.Invoke(request.error + " / " + request.downloadHandler.text);
            yield break;
        }

        ModelListResponse modelList =
            JsonUtility.FromJson<ModelListResponse>(request.downloadHandler.text);

        if (modelList == null || modelList.models == null)
        {
            onError?.Invoke("利用可能なGeminiモデルが取得できませんでした。");
            yield break;
        }

        string preferredFullName = "models/" + preferredModel;

        for (int i = 0; i < modelList.models.Length; i++)
        {
            if (modelList.models[i].name == preferredFullName &&
                SupportsGenerateContent(modelList.models[i]))
            {
                usableModel = preferredModel;
                onSuccess?.Invoke();
                yield break;
            }
        }

        for (int i = 0; i < modelList.models.Length; i++)
        {
            if (SupportsGenerateContent(modelList.models[i]) &&
                modelList.models[i].name.Contains("flash"))
            {
                usableModel = modelList.models[i].name.Replace("models/", "");
                onSuccess?.Invoke();
                yield break;
            }
        }

        onError?.Invoke("generateContent対応モデルが見つかりませんでした。");
    }

    private bool SupportsGenerateContent(GeminiModel model)
    {
        if (model.supportedGenerationMethods == null)
        {
            return false;
        }

        for (int i = 0; i < model.supportedGenerationMethods.Length; i++)
        {
            if (model.supportedGenerationMethods[i] == "generateContent")
            {
                return true;
            }
        }

        return false;
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
・各陣営の発言は200文字以内
・少し熱量のある、人間らしい口調にする
・相手陣営への人格否定や侮辱はしない
・ユーザーの元コメントの主張をできるだけ反映する
・相手の主張も踏まえて反論または補強を行う

【ニュートラルAIの条件】
・両陣営のどちらの意見に納得感があるか判断する
・scoreは、選んだ陣営への納得度を0〜100で表す
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