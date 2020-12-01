using Mirror;
using UnityEngine;
using UnityEngine.UI;

namespace Io_Framework.Examples.JumpIO
{
    public class JumpIOLeaderBoardEntry : LeaderBoardEntryBase
    {
        public Color32 HighlightColor;

        [Client]
        protected override void SwitchHighlight(bool value)
        {
            if (value)
            {
                PositionText.color = HighlightColor;
                NameText.color = HighlightColor;
                ScoreText.color = HighlightColor;
            }
            else
            {
                PositionText.color = Color.black;
                NameText.color = Color.black;
                ScoreText.color = Color.black;
            }
        }

    }
}
