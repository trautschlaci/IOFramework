using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Mirror;
using TMPro;
using UnityEngine;

public class GrowingPlayer : NetworkBehaviour
{
    public float ViewRangeMultiplier = 3.0f;
    public TextMeshPro NameText;


    private SpriteRenderer _sprite;
    private PlayerScore _playerScore;


    private Player _player;
    private ReverseProximityChecker _reverseProximityChecker;


    [Server]
    private void ChangeSize(int oldScore, int newScore)
    {
        var lastSize = transform.localScale.x;
        var newSize = CalculateSize(newScore);
        transform.localScale = new Vector3(newSize, newSize, 1.0f);

        _player.ViewRange = ViewRangeMultiplier * CalculateCameraScale(newSize);
        if (_reverseProximityChecker != null)
        {
            _reverseProximityChecker.VisRange *= (newSize/lastSize);
        }
    }

    [Server]
    private float CalculateSize(int score)
    {
        return Mathf.Sqrt(1.0f + score / (2.0f * Mathf.PI));
    }


    public float GetCameraScale()
    {
        return CalculateCameraScale(transform.localScale.x);
    }

    private float CalculateCameraScale(float size)
    {
        return 3 + size;
    }


    private void Start()
    {
        _playerScore = GetComponent<PlayerScore>();
        _sprite = GetComponent<SpriteRenderer>();


        if (!isServer)
            return;

        _player = GetComponent<Player>();
        _reverseProximityChecker = GetComponent<ReverseProximityChecker>();
        _playerScore.OnScoreChangedServer += ChangeSize;
        ChangeSize(0, _playerScore.Score);
    }


    [ClientCallback]
    private void Update()
    {
        _sprite.sortingOrder = _playerScore.Score;
        NameText.sortingOrder = _playerScore.Score;


        if (Camera.main == null || Camera.main.transform.parent != transform)
            return;

        Camera.main.orthographicSize = CalculateCameraScale(transform.localScale.x);
    }
}
