using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerScore : NetworkBehaviour
{
    public Player playerObject;

    [SyncVar(hook = nameof(SetScoreClient))]
    public int score;
    public int Score
    {
        get => score;
        set
        {
            OnScoreChangedServer?.Invoke(score, value);
            score = value;
        }
    }

    public event Action<int, int> OnScoreChangedClient;
    public event Action<int, int> OnScoreChangedServer;

    private LeaderBoard _leaderBoard;

    [Client]
    private void SetScoreClient(int oldScore, int newScore)
    {
        OnScoreChangedClient?.Invoke(oldScore, newScore);
    }

    public override void OnStartServer()
    {
        _leaderBoard = FindObjectOfType<LeaderBoard>();
        OnScoreChangedServer += ScoreChangedServer;
        OnScoreChangedServer?.Invoke(0, score);
        playerObject.OnPlayerDestroyedServer += PlayerDestroyed;
    }

    [Server]
    private void ScoreChangedServer(int oldScore, int newScore)
    {
        _leaderBoard.ChangeScore(playerObject.PlayerId, playerObject.PlayerName, newScore-oldScore);
    }

    [Server]
    private void PlayerDestroyed()
    {
        _leaderBoard.RemoveScore(playerObject.PlayerId, score);
    }

}
