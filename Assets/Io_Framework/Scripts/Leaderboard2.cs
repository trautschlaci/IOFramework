using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace Io_Framework
{
    // Component that manages score of all players. It is also responsible for sending data of the top players to the clients.
    public class Leaderboard2: NetworkBehaviour
    {

        #region Public fields Client

        [Tooltip("The GameObject on the leaderboard UI, which contains the entries.")]
        public GameObject EntryContainer;

        [Tooltip("Example of an entry, which gets cloned and changed for displaying each top player's score.")]
        public GameObject EntryTemplate;

        #endregion



        #region Public fields Client and Server

        [Tooltip("How many players should be displayed on the top list.")]
        public int NumberOfEntries = 10;

        #endregion



        #region Public fields Server

        [Tooltip("How often should the scores of players be refreshed, in seconds.")]
        public float RefreshDelay = 0.2f;

        [Tooltip("Set to false to stop refreshing the scores.")]
        public bool IsRefreshStopped;

        #endregion



        #region Client

        public ScoreEntry OwnScore { get; private set; } = new ScoreEntry(-1, null, -1);

        private readonly List<GameObject> _leaderBoardEntries = new List<GameObject>();



        public override void OnStartClient()
        {
            _topScores.Callback += OnTopScoresUpdated;

            for (var i = 0; i < NumberOfEntries + 1; i++)
            {
                _leaderBoardEntries.Add(Instantiate(EntryTemplate, EntryContainer.transform));
                _leaderBoardEntries[i].SetActive(false);
            }

            for (var i = 0; i < _topScores.Count; i++)
            {
                _leaderBoardEntries[i].SetActive(true);
                _leaderBoardEntries[i].GetComponent<LeaderboardEntryBase2>().SetValues(i, _topScores[i].PlayerName, _topScores[i].Score, false);
            }
        }


        [Client]
        private void OnTopScoresUpdated(SyncListScoreEntry.Operation op, int index, ScoreEntry oldScore, ScoreEntry newScore)
        {
            switch (op)
            {
                case SyncListScoreEntry.Operation.OP_ADD:
                    // index is where it got added in the list
                    // item is the new item
                    _leaderBoardEntries[index].SetActive(true);
                    _leaderBoardEntries[index].GetComponent<LeaderboardEntryBase2>().SetValues(index, newScore.PlayerName, newScore.Score, newScore.PlayerId == OwnScore.PlayerId);
                    if (newScore.PlayerId == OwnScore.PlayerId)
                    {
                        _leaderBoardEntries[_leaderBoardEntries.Count - 1].SetActive(false);
                    }
                    break;

                case SyncListScoreEntry.Operation.OP_CLEAR:
                    // list got cleared
                    foreach (var entry in _leaderBoardEntries)
                    {
                        entry.SetActive(false);
                    }
                    break;

                case SyncListScoreEntry.Operation.OP_INSERT:
                    // index is where it got added in the list
                    // item is the new item
                    _leaderBoardEntries[index].SetActive(true);
                    _leaderBoardEntries[index].GetComponent<LeaderboardEntryBase2>().SetValues(index, newScore.PlayerName, newScore.Score, newScore.PlayerId == OwnScore.PlayerId);
                    if (newScore.PlayerId == OwnScore.PlayerId)
                    {
                        _leaderBoardEntries[_leaderBoardEntries.Count - 1].SetActive(false);
                    }
                    break;

                case SyncListScoreEntry.Operation.OP_REMOVEAT:
                    // index is where it got removed in the list
                    // item is the item that was removed
                    _leaderBoardEntries[index].SetActive(false);
                    break;

                case SyncListScoreEntry.Operation.OP_SET:
                    // index is the index of the item that was updated
                    // item is the previous item
                    _leaderBoardEntries[index].GetComponent<LeaderboardEntryBase2>().SetValues(index, newScore.PlayerName, newScore.Score, newScore.PlayerId == OwnScore.PlayerId);
                    if (newScore.PlayerId == OwnScore.PlayerId)
                    {
                        _leaderBoardEntries[_leaderBoardEntries.Count - 1].SetActive(false);
                    }
                    break;
            }
        }

        #endregion



        #region Client and Server

        private readonly SyncListScoreEntry _topScores = new SyncListScoreEntry();


        private void OnDisable()
        {
            if (isServer)
                return;

            // Cleanup the leaderboard.
            for (var i = _leaderBoardEntries.Count - 1; i >= 0; i--)
            {
                var temp = _leaderBoardEntries[i];
                _leaderBoardEntries.RemoveAt(i);
                Destroy(temp);
            }
        }

        private void OnValidate()
        {
            if (EntryTemplate.GetComponent<LeaderboardEntryBase2>() == null)
            {
                Debug.LogError("Leaderboard2: EntryTemplate must have script derived from LeaderboardEntryBase2");
            }
        }


        [TargetRpc]
        private void TargetUpdateOwnScore(NetworkConnection conn, ScoreEntry score, int position)
        {
            OwnScore = score;
            for (var i = 0; i < _topScores.Count; i++)
            {
                if (_topScores[i].PlayerId == OwnScore.PlayerId)
                {
                    _leaderBoardEntries[_leaderBoardEntries.Count - 1].SetActive(false);
                    _leaderBoardEntries[i].GetComponent<LeaderboardEntryBase2>().SetValues(position, OwnScore.PlayerName, OwnScore.Score, true);
                    return;
                }
            }

            // If own player is not amongst the top players, then the last entry should display it.

            _leaderBoardEntries[_leaderBoardEntries.Count - 1].SetActive(true);
            _leaderBoardEntries[_leaderBoardEntries.Count - 1].GetComponent<LeaderboardEntryBase2>().SetValues(position, OwnScore.PlayerName, OwnScore.Score, true);
        }

        #endregion



        #region Server

        public static Leaderboard2 ServerSingleton { get; private set; }


        private readonly List<ScoreEntry> _scoreOfPlayers = new List<ScoreEntry>();


        [Server]
        public void ChangeScore(int playerId, string playerName, int changeValue)
        {
            var scoreEntryIndex = GetScoreEntryIndexById(playerId);
            if (scoreEntryIndex == -1)
            {
                _scoreOfPlayers.Add(new ScoreEntry(playerId, playerName, changeValue));
            }
            else
            {
                var tempEntry = _scoreOfPlayers[scoreEntryIndex];
                tempEntry.Score += changeValue;
                _scoreOfPlayers[scoreEntryIndex] = tempEntry;
            }
        }

        [Server]
        public void RemoveScore(int playerId, int value)
        {
            var scoreEntryIndex = GetScoreEntryIndexById(playerId);

            if (scoreEntryIndex == -1) return;

            var tempEntry = _scoreOfPlayers[scoreEntryIndex];
            tempEntry.Score -= value;
            _scoreOfPlayers[scoreEntryIndex] = tempEntry;
        }

        [Server]
        public void RemovePlayer(int playerId)
        {
            _scoreOfPlayers.RemoveAt(GetScoreEntryIndexById(playerId));
        }


        public override void OnStartServer()
        {
            InitializeSingleton();
            StartCoroutine(Refresh());
        }

        public override void OnStopServer()
        {
            IsRefreshStopped = true;
        }


        [Server]
        private void InitializeSingleton()
        {
            if (ServerSingleton != null)
            {
                if (ServerSingleton == this) return;

                Debug.Log("Multiple LeaderBoards detected in the scene. The duplicate will be destroyed.");
                Destroy(gameObject);

                return;
            }

            ServerSingleton = this;
        }

        [Server]
        private IEnumerator Refresh()
        {
            while (!IsRefreshStopped)
            {
                RemoveDisconnectedPlayers();

                _scoreOfPlayers.Sort();

                RefreshTopList();

                SendScoresToOwners();

                yield return new WaitForSeconds(RefreshDelay);
            }
        }

        [Server]
        private void RemoveDisconnectedPlayers()
        {
            for (var i = _scoreOfPlayers.Count - 1; i >= 0; i--)
            {
                var connectionId = _scoreOfPlayers[i].PlayerId;
                if (!NetworkServer.connections.ContainsKey(connectionId))
                {
                    _scoreOfPlayers.RemoveAt(i);
                }
            }
        }

        [Server]
        private void RefreshTopList()
        {
            for (var i = 0; i < NumberOfEntries && i < _scoreOfPlayers.Count; i++)
            {
                if (_topScores.Count < i + 1)
                {
                    _topScores.Add(_scoreOfPlayers[i]);
                }
                else
                {
                    _topScores[i] = _scoreOfPlayers[i];
                }
            }

            for (var i = _scoreOfPlayers.Count; i < _topScores.Count; i++)
            {
                _topScores.RemoveAt(_scoreOfPlayers.Count);
            }
        }

        [Server]
        private void SendScoresToOwners()
        {
            for (var i = 0; i < _scoreOfPlayers.Count; i++)
            {
                var connectionId = _scoreOfPlayers[i].PlayerId;
                if (NetworkServer.connections.ContainsKey(connectionId))
                {
                    TargetUpdateOwnScore(NetworkServer.connections[connectionId], _scoreOfPlayers[i], i);
                }
            }
        }

        [Server]
        private int GetScoreEntryIndexById(int playerId)
        {
            for (var i = 0; i < _scoreOfPlayers.Count; i++)
            {
                if (_scoreOfPlayers[i].PlayerId == playerId)
                {
                    return i;
                }
            }

            return -1;
        }

        #endregion

    }

    [Serializable]
    public class SyncListScoreEntry : SyncList<ScoreEntry> { }

    [Serializable]
    public struct ScoreEntry: IComparable<ScoreEntry>
    {
        public int PlayerId;
        public int Score;
        public string PlayerName;

        public ScoreEntry(int id, string name, int score)
        {
            PlayerId = id;
            PlayerName = name;
            Score = score;
        }

        public int CompareTo(ScoreEntry other)
        {
            return other.Score.CompareTo(Score);
        }
    }

}
