using UnityEngine;

public class Edible : MonoBehaviour
{
    public bool IsEdible = true;
    public int ConstantScore = 0;

    public virtual void SetEdible(bool value)
    {
        IsEdible = value;
    }

    public virtual bool CanBeEatenBy(GameObject other)
    {
        return IsEdible;
    }

    public virtual int EarnedScore()
    {
        return ConstantScore;
    }

    public virtual void Destroy()
    {
    }
}
