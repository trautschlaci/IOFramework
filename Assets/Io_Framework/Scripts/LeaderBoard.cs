﻿using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace Io_Framework
{
    public class LeaderBoard: NetworkBehaviour
    {
        public GameObject EntryContainer;
        public GameObject EntryTemplate;
        public int NumberOfEntries = 10;
        public float RefreshDelaySeconds = 0.5f;
        public bool IsRefreshStopped;

        // Client
        private readonly List<GameObject> _entries = new List<GameObject>();
        public ScoreEntry OwnScore { get; private set; }
        public int OwnPosition { get; private set; }

        // Client and Server
        private readonly SyncListEntry _topScores = new SyncListEntry();

        // Server
        private readonly List<ScoreEntry> _scoreOfPlayers = new List<ScoreEntry>();

        public override void OnStartClient()
        {
            OwnScore = new ScoreEntry(-1, null, -1);
            _topScores.Callback += OnTopScoresUpdated;
            for (int i = 0; i < NumberOfEntries+1; i++)
            {
                _entries.Add(Instantiate(EntryTemplate, EntryContainer.transform));
                _entries[i].SetActive(false);
            }
            for (var i = 0; i < _topScores.Count; i++)
            {
                _entries[i].SetActive(true);
                _entries[i].GetComponent<LeaderBoardEntry>().Set(i, _topScores[i].PlayerName, _topScores[i].Score, false);
            }
        }

        public override void OnStopClient()
        {
            base.OnStopClient();
            for (var i = _entries.Count - 1; i >= 0; i--)
            {
                var temp = _entries[i];
                _entries.RemoveAt(i);
                Destroy(temp);
            }
        }

        [Client]
        void OnTopScoresUpdated(SyncListEntry.Operation op, int index, ScoreEntry oldScore, ScoreEntry newScore)
        {
            switch (op)
            {
                case SyncListEntry.Operation.OP_ADD:
                    // index is where it got added in the list
                    // item is the new item
                    _entries[index].SetActive(true);
                    _entries[index].GetComponent<LeaderBoardEntry>().Set(index, newScore.PlayerName, newScore.Score, newScore.PlayerId == OwnScore.PlayerId);
                    if (newScore.PlayerId == OwnScore.PlayerId)
                    {
                        _entries[_entries.Count - 1].SetActive(false);
                    }
                    break;
                case SyncListEntry.Operation.OP_CLEAR:
                    // list got cleared
                    foreach (var entry in _entries)
                    {
                        entry.SetActive(false);
                    }
                    break;
                case SyncListEntry.Operation.OP_INSERT:
                    // index is where it got added in the list
                    // item is the new item
                    _entries[index].SetActive(true);
                    _entries[index].GetComponent<LeaderBoardEntry>().Set(index, newScore.PlayerName, newScore.Score, newScore.PlayerId == OwnScore.PlayerId);
                    if (newScore.PlayerId == OwnScore.PlayerId)
                    {
                        _entries[_entries.Count - 1].SetActive(false);
                    }
                    break;
                case SyncListEntry.Operation.OP_REMOVEAT:
                    // index is where it got removed in the list
                    // item is the item that was removed
                    _entries[index].SetActive(false);
                    break;
                case SyncListEntry.Operation.OP_SET:
                    // index is the index of the item that was updated
                    // item is the previous item
                    _entries[index].GetComponent<LeaderBoardEntry>().Set(index, newScore.PlayerName, newScore.Score, newScore.PlayerId==OwnScore.PlayerId);
                    if (newScore.PlayerId == OwnScore.PlayerId)
                    {
                        _entries[_entries.Count - 1].SetActive(false);
                    }
                    break;
            }
        }

        [TargetRpc]
        public void TargetUpdateOwnScore(NetworkConnection conn, ScoreEntry score, int position)
        {
            OwnScore = score;
            OwnPosition = position;
            for (var i = 0; i < _topScores.Count; i++)
            {
                if (_topScores[i].PlayerId == OwnScore.PlayerId)
                {
                    _entries[_entries.Count - 1].SetActive(false);
                    _entries[i].GetComponent<LeaderBoardEntry>().Set(OwnPosition, OwnScore.PlayerName, OwnScore.Score, true);
                    return;
                } 
            }
            _entries[_entries.Count - 1].SetActive(true);
            _entries[_entries.Count - 1].GetComponent<LeaderBoardEntry>().Set(OwnPosition, OwnScore.PlayerName, OwnScore.Score, true);
        }

        [ServerCallback]
        void Start()
        {
            StartCoroutine(Refresh());
        }

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

        [Server]
        private IEnumerator Refresh()
        {
            while (!IsRefreshStopped)
            {
                RemoveDisconnectedPlayers();

                _scoreOfPlayers.Sort();

                RefreshTopList();

                SendScoresToOwners();

                yield return new WaitForSeconds(RefreshDelaySeconds);
            }
        }

        [Server]
        private void RemoveDisconnectedPlayers()
        {
            for (var i = _scoreOfPlayers.Count-1; i >= 0; i--)
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
            for (int i = 0; i < NumberOfEntries && i < _scoreOfPlayers.Count; i++)
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

            for (int i = _scoreOfPlayers.Count; i < _topScores.Count; i++)
            {
                _topScores.RemoveAt(_scoreOfPlayers.Count);
            }
        }

        [Server]
        private void SendScoresToOwners()
        {
            for (int i = 0; i < _scoreOfPlayers.Count; i++)
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
            for (int i=0; i<_scoreOfPlayers.Count; i++)
            {
                if (_scoreOfPlayers[i].PlayerId == playerId)
                {
                    return i;
                }
            }

            return -1;
        }

        [Server]
        public override void OnStopServer()
        {
            IsRefreshStopped = true;
        }
    }

    [Serializable]
    public class SyncListEntry : SyncList<ScoreEntry> { }

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