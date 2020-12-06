using Mirror;
using UnityEngine;
using UnityEngine.UI;

namespace Io_Framework
{
    // Base class for entries of leaderboard.
    public abstract class LeaderboardEntryBase : MonoBehaviour
    {

        #region Client

        [Tooltip("The text component that shows the displayed player's position on the leaderboard.")]
        public Text PositionText;

        [Tooltip("The text component that shows the displayed player's name.")]
        public Text NameText;

        [Tooltip("The text component that shows the displayed player's score.")]
        public Text ScoreText;


        [Client]
        public void SetValues(int position, string playerName, int score, bool isOwnScore)
        {
            PositionText.text = (position+1) + ".";
            NameText.text = playerName;
            ScoreText.text = score.ToString();
            SwitchHighlight(isOwnScore);
        }

        protected abstract void SwitchHighlight(bool value);

        #endregion

    }
}
