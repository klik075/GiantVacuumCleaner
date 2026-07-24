using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpawnManager
{
    public string SpawnContainerTag = "PlayerSpawnSpace";
    private PlayerSpawnContainer _playerSpawnContainer;
    [SerializeField]
    private int minLvToMove = 3;
    public void Init()
    {
        _playerSpawnContainer = Managers.FindObjectWithTag<PlayerSpawnContainer>(SpawnContainerTag);
        _playerSpawnContainer.FindSpawnSpace();
        SetPlayerSpace();
    }
    public void SetPlayerSpace()
    {
        int rndIndex;
        Player player = Managers.Instance.playerTransform.GetComponent<Player>();
        if (player == null)
            return;

        rndIndex = Random.Range(0, _playerSpawnContainer.spawnSpace.Count);

        player.transform.position = _playerSpawnContainer.spawnSpace[rndIndex].position;
    }


}
