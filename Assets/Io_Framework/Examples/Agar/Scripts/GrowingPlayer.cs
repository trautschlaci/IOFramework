using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class GrowingPlayer : NetworkBehaviour
{
    public float ViewRangeMultiplier = 3.0f;

    private PlayerScore _playerScore;
    private Player _player;
    private ReverseProximityChecker _reverseProximityChecker;
    private float _size;


    [Server]
    public override void OnStartServer()
    {
        _player = GetComponent<Player>();
        _playerScore = GetComponent<PlayerScore>();
        _reverseProximityChecker = GetComponent<ReverseProximityChecker>();
        _playerScore.OnScoreChangedServer += ChangeSize;
        _size = transform.localScale.x;
        ChangeSize(0, _playerScore.Score);
    }

    [Server]
    private void ChangeSize(int oldScore, int newScore)
    {
        var lastSize = _size;
        _size = CalculateSize(newScore);
        transform.localScale = new Vector3(_size, _size, 1.0f);
        if (connectionToClient.identity == netIdentity)
            TargetSetCameraScale(_size);

        _player.ViewRange = ViewRangeMultiplier * GetCameraScale(_size);
        if (_reverseProximityChecker != null)
        {
            _reverseProximityChecker.VisRange *= (_size/lastSize);
        }
    }

    [Server]
    private float CalculateSize(int score)
    {
        return Mathf.Sqrt(1.0f + score / (2.0f * Mathf.PI));
    }

    private float GetCameraScale(float size)
    {
        return 4 + size;
    }

    [TargetRpc]
    private void TargetSetCameraScale(float size)
    {
        if (Camera.main == null || Camera.main.transform.parent != transform)
            return;

        Camera.main.orthographicSize = GetCameraScale(size);
    }
}
