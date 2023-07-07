using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
   public static NetworkManager Instance;

    public GameObject PlayerPrefab;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }

        else if (Instance != this)
        {
            Debug.Log("Instance Already Exists, Destroying Object");
            Destroy(this);
        }
    }

    private void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 100;

        Server.Start(50, 7777);
    }

    private void OnApplicationQuit()
    {
        Server.Stop();
    }

    public Player InstantiatePlayer()
    {
        return Instantiate(PlayerPrefab, Vector3.zero, Quaternion.identity).GetComponent<Player>();
    }
}
