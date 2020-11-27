using Mirror;
using UnityEngine;
using UnityEngine.UI;

namespace Io_Framework
{
    public abstract class LeaderBoardEntryBase : MonoBehaviour
    {
        public Text PositionText;
        public Text NameText;
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

    }
}
