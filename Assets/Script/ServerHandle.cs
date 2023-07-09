using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerHandle
{
    public static void WelcomeReceived(int _FromClient, Packet _Packet)
    {
        int _ClientIdCheck = _Packet.ReadInt();
        string _UserName = _Packet.ReadString();

        Debug.Log($"{Server.Clients[_FromClient].MyTcp.Socket.Client.RemoteEndPoint} " +
            $"Connected Successfully And Is Now Player {_FromClient} : {_UserName}.");

        if (_FromClient != _ClientIdCheck)
        {
            Debug.Log($"Player \"{_UserName}\" (ID: {_FromClient}) " +
                $"Has Assumed The Wrong Client ID ({_ClientIdCheck})");
        }
        Server.Clients[_FromClient].SendIntoGame(_UserName);
    }

    public static void PlayerMovement(int _FromClient, Packet _Packet)
    {
        bool[] _Inputs = new bool[_Packet.ReadInt()];
        for (int i = 0; i < _Inputs.Length; i++)
        {
            _Inputs[i] = _Packet.ReadBool();
        }
        Quaternion _Rotation = _Packet.ReadQuaternion();

        Server.Clients[_FromClient].MyPlayer.SetInput(_Inputs, _Rotation);
    }

    public static void PlayerShoot(int _FromClient, Packet _Packet)
    {
        Vector3 _ShootDirection = _Packet.ReadVector3();
        Debug.Log($"{Server.Clients[_FromClient].MyPlayer.UserName} Shoot To {_ShootDirection}(Dir)");

        Server.Clients[_FromClient].MyPlayer.Shoot(_ShootDirection);
    }

    public static void SelectCharacter(int _FromClient, Packet _Packet)
    {
        int _Type = _Packet.ReadInt();
        Server.Clients[_FromClient].MyPlayer.TypeSetting(_Type);

        foreach(Client _Player in Server.Clients.Values)
        {
            if (_Player.MyPlayer.Type <= 0)
                return;
        }

        ServerSend.DoneSelect();
    }

    public static void PlayerThrowItem(int _FromClient, Packet _Packet)
    {
        Debug.Log("ThrowStart");

        Vector3 _ThrowDirection = _Packet.ReadVector3();

        Server.Clients[_FromClient].MyPlayer.ThrowItem(_ThrowDirection);
    }

    public static void WaitOtherPlayer(int _FromClient, Packet _Packet)
    {
        int PlayerCount = _Packet.ReadInt();
        Server.AddPlayerCount(PlayerCount);
    }
}
