using Mirror;
using UnityEngine;

public abstract class RewardBase : NetworkBehaviour
{
    public bool IsAvailable = true;
    public int ConstantScore = 1;

    public void SetAvailable(bool value)
    {
        IsAvailable = value;
    }

    public virtual bool CanBeGivenToOther(GameObject other)
    {
        return IsAvailable;
    }

    public virtual int EarnedScore()
    {
        return ConstantScore;
    }

    public virtual void ClaimReward(GameObject other)
    {
        other.GetComponent<PlayerScore>().Score += EarnedScore();
        SetAvailable(false);
        Destroy();
    }

    public abstract void Destroy();

}
