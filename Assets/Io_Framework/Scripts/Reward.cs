using UnityEngine;

public class Reward : MonoBehaviour
{
    public bool IsAvailable = true;
    public int ConstantScore = 0;

    public virtual void SetAvailable(bool value)
    {
        IsAvailable = value;
    }

    public virtual bool CanBeEatenBy(GameObject other)
    {
        return IsAvailable;
    }

    public virtual int EarnedScore()
    {
        return ConstantScore;
    }

    public virtual void Destroy()
    {
    }
}
