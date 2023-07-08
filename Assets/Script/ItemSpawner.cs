using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public static Dictionary<int, ItemSpawner> spawners = new Dictionary<int, ItemSpawner>();
    private static int NextSpawnerId = 1;

    public int SpawnerId;
    public bool HasItem = false;

    private void Start()
    {
        HasItem = false;
        SpawnerId = NextSpawnerId;
        NextSpawnerId++;
        spawners.Add(SpawnerId, this);

        StartCoroutine(SpawnItem());
    }

    private void OnTriggerEnter2D(Collider2D Other)
    {
        if(Other.CompareTag("Player"))
        {
            Player _Player = Other.GetComponent<Player>();
            if(_Player.AttemptPickUpItem())
            {
                ItemPickUp(_Player.Id);
            }
        }
    }

    private IEnumerator SpawnItem()
    {
        yield return new WaitForSeconds(10.0f);

        HasItem = true;
        ServerSend.ItemSpawnd(SpawnerId);
    }

    private void ItemPickUp(int _ByPlayer)
    {
        HasItem = false;
        ServerSend.itemPickedUp(SpawnerId, _ByPlayer);

        StartCoroutine(SpawnItem());
    }
}
