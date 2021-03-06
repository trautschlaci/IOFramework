﻿using Mirror;
using UnityEngine;
using UnityEngine.UI;

namespace Io_Framework.Examples.Agar
{
    public class LeaderboardEntryAgar : LeaderboardEntryBase
    {

        #region Client

        public Image BackgroundImage;


        private Color _normalColor;


        private void Awake()
        {
            _normalColor = BackgroundImage.color;
        }


        [Client]
        protected override void SwitchHighlight(bool value)
        {
            if (value)
            {
                BackgroundImage.color = new Color(0.5f, 0.8f, 0.5f);
            }
            else
            {
                BackgroundImage.color = _normalColor;
            }
        }

        #endregion

    }
}
