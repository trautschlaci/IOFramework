using Mirror;
using UnityEngine;

namespace Io_Framework.Examples.Agar
{
    public class PlayerControllerAgar : NetworkBehaviour
    {

        #region Public fields Client

        public float InputSyncInterval = 0.1f;

        #endregion



        #region Public fields Server

        public float Speed = 3.0f;
        public float JumpSpeed = 10.0f;
        public float MoveBlockInterval = 0.3f;

        #endregion



        #region Client

        private float _inputSyncTime;
        private Vector2 _mousePosClient;
        private bool _jumpPressedClient;


        [ClientCallback]
        private void Update()
        {
            if (!isLocalPlayer)
                return;

            _mousePosClient = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _jumpPressedClient = _jumpPressedClient || Input.GetButtonDown("Jump");

            if (_inputSyncTime < Time.time)
            {
                var inputInfo = new InputInfo
                {
                    MousePos = _mousePosClient,
                    JumpPressed = _jumpPressedClient
                };

                CmdSendInputInfo(inputInfo);

                _jumpPressedClient = false;
                _inputSyncTime = Time.time + InputSyncInterval;
            }
        }

        #endregion



        #region Client and Server

        private Rigidbody2D _rigidBody;
        private PlayerAgar _player;
        private RectTransform _boundary;
        private Vector3[] _boundaryCorners;
        private Transform _transform;


        private struct InputInfo
        {
            public Vector2 MousePos;
            public bool JumpPressed;
        }


        private void Awake()
        {
            _rigidBody = GetComponent<Rigidbody2D>();
            _player = GetComponent<PlayerAgar>();
            _transform = transform;
        }

        private void Start()
        {
            _rigidBody.simulated = isServer;

            _boundary = GameObject.Find("Map").GetComponent<RectTransform>();
            _boundaryCorners = new Vector3[4];
            _boundary.GetWorldCorners(_boundaryCorners);
        }


        [Command]
        private void CmdSendInputInfo(InputInfo input)
        {
            var clones = PlayerObjectManager.Singleton.GetPlayerObjects(connectionToClient.connectionId);
            if (clones == null) return;

            foreach (var playerClone in clones)
            {
                playerClone.GetComponent<PlayerControllerAgar>().UpdateMovement(input);
            }
        }

        #endregion



        #region Server

        private Vector2 _moveVectorServer;
        private bool _jumpPressedServer;
        private float _moveBlockTime;


        [Server]
        public void GiveStartVelocity(Vector2 startVelocityDir)
        {
            _moveBlockTime = Time.fixedTime + MoveBlockInterval;

            _rigidBody.velocity = startVelocityDir * JumpSpeed * Time.fixedDeltaTime * 50f * Mathf.Sqrt(_transform.localScale.x);
        }


        [ServerCallback]
        private void FixedUpdate()
        {
            Move();

            if (_jumpPressedServer)
            {
                _jumpPressedServer = false;
                _player.Split(_moveVectorServer.normalized);
            }

            var newX = Mathf.Clamp(_transform.position.x, _boundaryCorners[0].x - 0.01f, _boundaryCorners[2].x + 0.01f);
            var newY = Mathf.Clamp(_transform.position.y, _boundaryCorners[0].y - 0.01f, _boundaryCorners[2].y + 0.01f);
            _transform.position = new Vector3(newX, newY, 0);
        }


        [Server]
        private void UpdateMovement(InputInfo input)
        {
            var mousePos = input.MousePos;
            _moveVectorServer = new Vector2(mousePos.x - _transform.position.x, mousePos.y - _transform.position.y);

            if (_moveVectorServer.magnitude > 1)
                _moveVectorServer = _moveVectorServer.normalized;

            if (input.JumpPressed)
                _jumpPressedServer = true;
        }

        [Server]
        private void Move()
        {
            if (_moveBlockTime > Time.fixedTime)
                return;

            _rigidBody.velocity = _moveVectorServer * Speed * Time.fixedDeltaTime * 50f / Mathf.Sqrt(_transform.localScale.x);
        }

        #endregion

    }
}
