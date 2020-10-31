using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Spawnable : NetworkBehaviour
{
    [SerializeField]
    private float _repeatRate = 1.0f;

    [Server]
    public IEnumerator SpawnRepeat()
    {
        while (true)
        {
            Spawn();
            yield return new WaitForSeconds(_repeatRate);
        }
    }

    [Server]
    private void Spawn()
    {
        float x = Random.Range(-15.0f, 15.0f);
        float y = Random.Range(-15.0f, 15.0f);

        Vector3 target = new Vector3(x, y, 0);

        GameObject spawnGameObject = Instantiate(gameObject, target, Quaternion.identity);
        NetworkServer.Spawn(spawnGameObject);
    }

}
