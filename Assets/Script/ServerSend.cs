using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    #endregion
}
