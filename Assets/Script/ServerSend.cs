using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ServerSend
{
    private static void SendTcpData(int _ToClient, Packet _Packet)
    {
        _Packet.WriteLength();
        Server.Clients[_ToClient].MyTcp.SendData(_Packet);
    }

    private static void SendTcpDataToAll(Packet _Packet)
    {
        _Packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            Server.Clients[i].MyTcp.SendData(_Packet);
        }
    }

    private static void SendTcpDataToAll(int _ExceptClient, Packet _Packet)
    {
        _Packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            if (i != _ExceptClient)
            {
                Server.Clients[i].MyTcp.SendData(_Packet);
            }
        }
    }

    private static void SendUdpData(int _ToClient, Packet _Packet)
    {
        _Packet.WriteLength();
        Server.Clients[_ToClient].MyUdp.SendData(_Packet);
    }

    private static void SendUdpDataToAll(Packet _Packet)
    {
        _Packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            Server.Clients[i].MyUdp.SendData(_Packet);
        }
    }

    private static void SendUdpDataToAll(int _ExceptClient, Packet _Packet)
    {
        _Packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            if (i != _ExceptClient)
            {
                Server.Clients[i].MyUdp.SendData(_Packet);
            }
        }
    }

    #region Packets
    public static void Welcome(int _ToClient, string _Msg)
    {
        using (Packet _Packet = new Packet((int)ServerPackets.welcome))
        {
            _Packet.Write(_Msg);
            _Packet.Write(_ToClient);

            SendTcpData(_ToClient, _Packet);
        }
    }

    //캐릭터 선택창으로 넘어가라는 패킷
    public static void GoToCharacterSelect(bool IsGotoCharacter)
    {
        Debug.Log("GotoCharacterScene");

        using (Packet _Packet = new Packet((int)ServerPackets.goToCharacterSelect))
        {
            _Packet.Write(IsGotoCharacter);

            SendTcpDataToAll(_Packet);
        }
    }

    //캐릭터 선택 데이터를 회신 하는 패킷
    public static void ReceiveSelectData(int _ToClient, int _Type)
    {
        using (Packet _Packet = new Packet((int)ServerPackets.receiveSelectData))
        {
            _Packet.Write(_ToClient);
            _Packet.Write(_Type);

            SendTcpDataToAll(_Packet);
        }
    }

    public static void SpawnPlayer(int _ToClient, Player _Player)
    {
        using (Packet _Packet = new Packet((int)ServerPackets.spawnPlayer))
        {
            _Packet.Write(_Player.Id);
            _Packet.Write(_Player.UserName);
            _Packet.Write(_Player.transform.position);
            _Packet.Write(_Player.transform.transform.rotation);

            SendTcpData(_ToClient, _Packet);
        }
    }

    public static void PlayerPosition(Player _Player)
    {
        using (Packet _Packet = new Packet((int)ServerPackets.playerPosition))
        {
            _Packet.Write(_Player.Id);
            _Packet.Write(_Player.transform.position);

            SendUdpDataToAll(_Packet);
        }
    }

    public static void PlayerRotation(Player _Player)
    {
        using (Packet _Packet = new Packet((int)ServerPackets.playerRotation))
        {
            _Packet.Write(_Player.Id);
            _Packet.Write(_Player.transform.rotation);

            SendUdpDataToAll(_Player.Id, _Packet);
        }
    }

    public static void PlayerDisconnected(int _PlayerId)
    {
        using(Packet _Packet = new Packet((int)ServerPackets.playerDisconnected))
        {
            _Packet.Write(_PlayerId);

            SendTcpDataToAll(_Packet);
        }
    }

    public static void PlayerHealth(Player _Player)
    {
        using(Packet _Packet = new Packet((int)ServerPackets.playerHealth))
        {
            _Packet.Write(_Player.Id);
            _Packet.Write(_Player.Health);

            SendTcpDataToAll(_Packet);
        }
    }

    public static void PlayerRespawn(Player _Player)
    {
        using (Packet _Packet = new Packet((int)ServerPackets.playerRespawned))
        {
            _Packet.Write(_Player.Id);

            SendTcpDataToAll(_Packet);
        }
    }

    public static void CreateItemSpawner(int _ToClient, int _SpawnerId, Vector3 _SpawnerPosition, bool _HasItem)
    {
        using(Packet _Packet = new Packet ((int)ServerPackets.createItemSpawner))
        {
            _Packet.Write(_SpawnerId);
            _Packet.Write(_SpawnerPosition);
            _Packet.Write(_HasItem);

            SendTcpData(_ToClient, _Packet);
        }
    }

    public static void ItemSpawnd(int _SpawnerId)
    {
        using(Packet _Packet = new Packet((int)ServerPackets.itemSpawnd))
        {
            _Packet.Write(_SpawnerId);

            SendTcpDataToAll(_Packet);
        }
    }

    public static void itemPickedUp(int _SpawnerId, int _ByPlayer)
    {
        using (Packet _Packet = new Packet((int)ServerPackets.itemPickedUp))
        {
            _Packet.Write(_SpawnerId);
            _Packet.Write(_ByPlayer);

            SendTcpDataToAll(_Packet);
        }
    }

    public static void SpawnProjectile(Projectile _Projectile, int _ThrowByPlayer)
    {
        using (Packet _Packet = new Packet((int)ServerPackets.spawnProjectile))
        {
            _Packet.Write(_Projectile.Id);
            _Packet.Write(_Projectile.transform.position);
            _Packet.Write(_ThrowByPlayer);

            SendTcpDataToAll(_Packet);
        }
    }

    public static void ProjectilePosition(Projectile _Projectile)
    {
        using (Packet _Packet = new Packet((int)ServerPackets.projectilePosition))
        {
            _Packet.Write(_Projectile.Id);
            _Packet.Write(_Projectile.transform.position);

            SendTcpDataToAll(_Packet);
        }
    }

    public static void ProjectileExplode(Projectile _Projectile)
    {
        using (Packet _Packet = new Packet((int)ServerPackets.projectileExploded))
        {
            _Packet.Write(_Projectile.Id);
            _Packet.Write(_Projectile.transform.position);

            SendTcpDataToAll(_Packet);
        }
    }
    #endregion
}
