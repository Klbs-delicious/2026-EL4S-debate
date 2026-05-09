/*
 * 最終更新日時：2026/05/08
 * 作成者：廣瀬宗貴
 *
 * 概要：
 * APICommunicatorを使って、AIレスバトルのAPI送信テストを行うクラス。
 * 1回のAPI送信で、両陣営の出力結果とニュートラルAIの判定結果を取得する。
 */

using System.Collections;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BattleAPI : MonoBehaviour
{
    [SerializeField] private APICommunicator api;

    [SerializeField] private CommentSet commentSet_A = new CommentSet(); // コメントセットの参照（必要に応じて使用）
    [SerializeField] private CommentSet commentSet_B = new CommentSet();

    [SerializeField] public string battle_topic = "Python派 vs C++派";
    [SerializeField] public string battle_side_a = "Python派";
    [SerializeField] public string battle_side_b = "C++派";
    public bool output = false;
    public APICommunicator.BattleCombinedResult save_result;
    public int now_round = 1;
    private void Start()
    {
        //StartCoroutine(RunBattleTest());
    }
    public void RunBattleTestButton()
    {
        StartCoroutine(RunBattleTest());
    }

    private IEnumerator RunBattleTest()
    {
        output = false; // 出力前にフラグを下げておく
        APICommunicator.BattleRequestData battleData =
            CreateBattleData();

        yield return StartCoroutine(api.SendBattleRequest(
            battleData,
            result =>
            {
                commentSet_A.ClearComments();
                commentSet_B.ClearComments();
                OutputResult(result);
                now_round++; // 次のラウンドに進む
            },
            error =>
            {
                Debug.LogError("API失敗: " + error);
            }
        ));
    }

    private APICommunicator.BattleRequestData CreateBattleData()
    {
        return new APICommunicator.BattleRequestData
        {
            battle_id = "battle_001",
            round = now_round,
            topic = battle_topic,

            side_a = new APICommunicator.SideRequestData
            {
                side = battle_side_a,
                comments = commentSet_A.GetPosts(),
                previous_opponent_output = save_result.side_b_result.output_result // 前回の相手の出力を渡す（初回は空文字）
            },

            side_b = new APICommunicator.SideRequestData
            {
                side = battle_side_b,
                comments = commentSet_B.GetPosts(),
                previous_opponent_output = save_result.side_a_result.output_result
            },

            previous_neutral_comment = ""
        };
    }

    public void CommentSet_A(string str)
    {
        commentSet_A.AddComment(str);
    }

    public void CommentSet_B(string str)
    {
        commentSet_B.AddComment(str);
    }

    private void OutputResult(APICommunicator.BattleCombinedResult result)
    {
        Debug.Log("========== AIレスバトル生成結果 ==========");

        Debug.Log("【" + result.side_a_result.side + "】");
        Debug.Log(result.side_a_result.output_result);

        Debug.Log("【" + result.side_b_result.side + "】");
        Debug.Log(result.side_b_result.output_result);

        Debug.Log("【ニュートラルAI判定】");
        Debug.Log("スコア: " + result.neutral_result.score);
        Debug.Log("選択陣営: " + result.neutral_result.selected_side);
        Debug.Log("一言コメント: " + result.neutral_result.comment);

        save_result = result; // 結果を保存しておく（必要に応じて後で使用可能）

        output = true; // 出力完了後、フラグを下げる
    }
}