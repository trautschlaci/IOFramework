using System;
using System.Collections;
using System.Text;
using Mirror;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class AgarPlayer : CloneablePlayerObject
{
    public PlayerScore playerScore;

    [SerializeField] 
    private float _speed = 300.0f;

    [SerializeField]
    private float _jumpSpeed = 30.0f;

    private float _size = 1;

    private bool MoveBlocked => _blockTime >= 0.0f;

    private float _blockTime;

    public override void OnStartClient()
    {
        base.OnStartClient();
        playerScore.OnScoreChangedClient += ChangeSize;
        ChangeSize(0, playerScore.Score);
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        CalculateCameraScale();
    }

    void Start()
    {
        GetComponent<CircleCollider2D>().enabled = isServer;
    }

    [ClientCallback]
    void FixedUpdate()
    {
        if (!hasAuthority || MoveBlocked) return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 target = new Vector2(mousePos.x-transform.position.x, mousePos.y-transform.position.y);
        if (Math.Abs(target.x) > 1.0f)
            target.x /= Math.Abs(target.x);
        if (Math.Abs(target.y) > 1.0f)
            target.y /= Math.Abs(target.y);

        var rigidbody2d = GetComponent<Rigidbody2D>();
        rigidbody2d.velocity = target * _speed * Time.fixedDeltaTime / (float)Math.Sqrt(transform.localScale.x);
    }

    [ClientCallback]
    void Update()
    {

        if (MoveBlocked)
        {
            _blockTime -= Time.deltaTime;
        }

        if (!hasAuthority) return;

        if (Input.GetKeyUp(KeyCode.Space) && playerScore.Score > 10)
        {
            CmdSplit(GetComponent<Rigidbody2D>().velocity, (float)NetworkTime.rtt);
        }
    }

    [ServerCallback]
    void OnTriggerStay2D(Collider2D other)
    {
        Edible edible = other.GetComponent<Edible>();
        if (edible == null)
            return;

        if (edible.CanBeEatenBy(gameObject))
        {
            playerScore.Score += edible.EarnedScore();
            edible.SetEdible(false);
            edible.Destroy();
        }
    }

    private float CalculateSize(int score)
    {
        return (float) Math.Sqrt(1.0f + score / (2*(float)Math.PI));
    }

    [Client]
    public void CalculateCameraScale()
    {
        Camera.main.orthographicSize = 3 + _size;
    }

    [Command]
    private void CmdSplit(Vector3 velocity, float latency)
    {
        Vector3 target = transform.position + 2*velocity*Math.Max(latency, Time.fixedDeltaTime);
        playerScore.Score = (int)(playerScore.Score / 2.0f);
        GameObject half = SpawnClone(target);
        half.GetComponent<PlayerScore>().Score = playerScore.Score;
        NetworkServer.Spawn(half, connectionToClient);

        half.GetComponent<AgarPlayer>().TargetGiveStartVelocity(velocity.normalized);
    }

    [TargetRpc]
    private void TargetGiveStartVelocity(Vector3 velocity)
    {
        _blockTime = 0.3f;
        GetComponent<Rigidbody2D>().velocity = velocity * GetComponent<Collider2D>().bounds.extents.x * _jumpSpeed;
    }

    [Client]
    private void ChangeSize(int oldScore, int newScore)
    {
        _size = CalculateSize(newScore);
        transform.localScale = new Vector3(_size, _size, 1.0f);
        if (isLocalPlayer)
            CalculateCameraScale();
    }

    public override int CompareTo(Player other)
    {
        var otherScore = other.GetComponent<PlayerScore>();
        if (otherScore == null)
            return base.CompareTo(other);

        return playerScore.Score.CompareTo(otherScore.Score);
    }
}

