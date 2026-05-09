using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DataModel
{
    // お題
    public string topic;

    // どっちの陣営か
    public bool faction;

    // コメント配列
    public List<string> comments;

    // 前回の相手の出力結果
    public string previousOpponentOutput;

    // ニュートラルの一言コメント
    public string neutralComment;

}

[Serializable]
public class DebateResponse
{
    // 各陣営の出力
    public FactionResult playerResult;
    public FactionResult opponentResult;

    // ニュートラル側
    public NeutralResult neutral;
}

[Serializable]
public class FactionResult
{
    // 出力結果
    public string output;
}

[Serializable]
public class NeutralResult
{
    // スコア
    public int score;

    // 一言コメント
    public string comment;
}
