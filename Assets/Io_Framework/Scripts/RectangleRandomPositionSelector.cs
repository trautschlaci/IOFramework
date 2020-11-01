using UnityEngine;

public class RectangleRandomPositionSelector : RandomPositionSelector
{
    public RectTransform SpawnArea;

    public override Vector3 RandomPosition()
    {
        var x = Random.Range(SpawnArea.rect.xMin * SpawnArea.localScale.x, SpawnArea.rect.xMax * SpawnArea.localScale.x);
        var y = Random.Range(SpawnArea.rect.yMin * SpawnArea.localScale.y, SpawnArea.rect.yMax * SpawnArea.localScale.y);

        var target = new Vector3(x, y, 0);

        return target;
    }
}
