using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerNamePresenter : MonoBehaviour
{
    public Player PlayerToPresent;
    public TextMeshPro nameText;

    private int direction = 1;

    void Start()
    {
        if(PlayerToPresent == null)
            PlayerToPresent = GetComponent<Player>();
    }

    void Update()
    {
        nameText.text = PlayerToPresent.PlayerName;
        nameText.transform.rotation = Camera.main.transform.rotation;
        var scale = nameText.transform.localScale;
        if (PlayerToPresent.transform.localScale.x * direction < 0)
        {
            direction *= -1;
            scale.x *= -1;
        }
        nameText.transform.localScale = scale;
    }
}
