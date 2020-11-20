using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpIOPlayer : Player
{
    private Animator animator;
    private int IsDeadParamID;

    void Start()
    {
        animator = GetComponent<Animator>();
        IsDeadParamID = Animator.StringToHash("IsDead");
    }

    public override void Destroy()
    {
        animator.SetBool(IsDeadParamID, true);
    }

    public void ExecuteDestroy()
    {
        base.Destroy();
    }
}
