using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>�������� Ŭ���̾�Ʈ�� �����Ѵ�.</summary>
public enum ServerPackets
{
    welcome = 1,
    spawnPlayer,
    playerPosition,
    playerRotation,
    playerDisconnected,
    playerHealth,
    playerRespawned,
    createItemSpawner,
    itemSpawnd,
    itemPickedUp,
    spawnProjectile,
    projectilePosition,
    projectileExploded,
}

/// <summary>Ŭ���̾�Ʈ���� ������ �����Ѵ�.</summary>
public enum ClientPackets
{
    welcomeReceived = 1,
    playerMovement,
    playerShoot,
    playerThrowItem,
}

public class Packet : IDisposable
{
    private List<byte> buffer;
    private byte[] readableBuffer;
    private int readPos;

    /// <summary>�� �� ��Ŷ�� �����(ID ����).</summary>
    public Packet()
    {
        buffer = new List<byte>(); // ���� �ʱ�ȭ
        readPos = 0; // �д� ��ġ 0���� ����
    }

    /// <summary>������ ID�� �� ��Ŷ�� ����, �۽ſ� ���ȴ�.</summary>
    /// <param name="_id">��Ŷ ID</param>
    public Packet(int _id)
    {
        buffer = new List<byte>(); // ���� �ʱ�ȭ
        readPos = 0; // �д� ��ġ 0���� ����

        Write(_id); // ���ۿ� ��Ŷ Id �ۼ�
    }

    /// <summary>�����͸� ���� �� �ִ� ��Ŷ�� ����, �۽ſ� ���ȴ�.</summary>
    /// <param name="_data">��Ŷ�� �߰��� Byte</param>
    public Packet(byte[] _data)
    {
        buffer = new List<byte>(); // ���� �ʱ�ȭ
        readPos = 0; // �д� ��ġ 0���� ����

        SetBytes(_data);
    }

    #region Functions
    /// <summary>��Ŷ�� ���� ����, ���� �� �ֵ��� �غ��Ѵ�.</summary>
    /// <param name="_data">��Ŷ�� �߰��� Byte</param>
    public void SetBytes(byte[] _data)
    {
        Write(_data);
        readableBuffer = buffer.ToArray();
    }

    /// <summary>���� ���� �κп� ��Ŷ ������ ���̸� �����Ѵ�.</summary>
    public void WriteLength()
    {
        buffer.InsertRange(0, BitConverter.GetBytes(buffer.Count)); // ���� ó���� ��Ŷ�� ����Ʈ ���̸� ����.
    }

    /// <summary>������ ���� �κп� �־��� int�� �����Ѵ�.</summary>
    /// <param name="_value">������ Int</param>
    public void InsertInt(int _value)
    {
        buffer.InsertRange(0, BitConverter.GetBytes(_value)); // ���� ó���� ����Ʈ�� ��ȯ�� int ����
    }

    /// <summary>��Ŷ�� ������ �迭 �������� �����´�.</summary>
    public byte[] ToArray()
    {
        readableBuffer = buffer.ToArray();
        return readableBuffer;
    }

    /// <summary>��Ŷ�� ���̸� �����´�.</summary>
    public int Length()
    {
        return buffer.Count; // ������ ���̸� ����
    }

    /// <summary>��Ŷ�� ���Ե� ���� ���� �������� ���̸� �����´�.</summary>
    public int UnreadLength()
    {
        return Length() - readPos; // ���� ���� ��ȯ(���� ����)
    }

    /// <summary>������ �� �ֵ��� ��Ŷ �ν��Ͻ��� �缳���Ѵ�.</summary>
    /// <param name="_shouldReset">��Ŷ�� �缳������ ����</param>
    public void Reset(bool _shouldReset = true)
    {
        if (_shouldReset)
        {
            buffer.Clear(); // ���� ����
            readableBuffer = null;
            readPos = 0; // ���� ��ġ �ʱ�ȭ
        }
        else
        {
            readPos -= 4; // ���������� ���� int�� "���� ����"
        }
    }
    #endregion

    #region Write Data
    /// <summary>��Ŷ�� Byte�� �����͸� �߰��Ѵ�.</summary>
    /// <param name="_value">�߰� �� Byte</param>
    public void Write(byte _value)
    {
        buffer.Add(_value);
    }
    /// <summary>��Ŷ�� Byte Array�� �����͸� �߰��Ѵ�.</summary>
    /// <param name="_value">�߰� �� Byte Array</param>
    public void Write(byte[] _value)
    {
        buffer.AddRange(_value);
    }
    /// <summary>��Ŷ�� Short�� �����͸� �߰��Ѵ�.</summary>
    /// <param name="_value">�߰� �� Short.</param>
    public void Write(short _value)
    {
        buffer.AddRange(BitConverter.GetBytes(_value));
    }
    /// <summary>��Ŷ�� Int�� �����͸� �߰��Ѵ�.</summary>
    /// <param name="_value">�߰� �� Int.</param>
    public void Write(int _value)
    {
        buffer.AddRange(BitConverter.GetBytes(_value));
    }
    /// <summary>��Ŷ�� Long�� �����͸� �߰��Ѵ�.</summary>
    /// <param name="_value">�߰� �� Long.</param>
    public void Write(long _value)
    {
        buffer.AddRange(BitConverter.GetBytes(_value));
    }
    /// <summary>��Ŷ�� Float �����͸� �߰��Ѵ�.</summary>
    /// <param name="_value">�߰� �� Float.</param>
    public void Write(float _value)
    {
        buffer.AddRange(BitConverter.GetBytes(_value));
    }
    /// <summary>��Ŷ�� Bool �����͸� �߰��Ѵ�.</summary>
    /// <param name="_value">�߰� �� Bool.</param>
    public void Write(bool _value)
    {
        buffer.AddRange(BitConverter.GetBytes(_value));
    }
    /// <summary>��Ŷ�� String �����͸� �߰��Ѵ�.</summary>
    /// <param name="_value">�߰� �� String.</param>
    public void Write(string _value)
    {
        Write(_value.Length); // ��Ŷ�� String ���� �߰�
        buffer.AddRange(Encoding.ASCII.GetBytes(_value)); // ���ڿ� ��ü �߰�
    }
    /// <summary>
    /// ��Ŷ�� Vector3 �����͸� �߰��Ѵ�.
    /// </summary>
    /// <param name="_value"> �߰� �� Vector3</param>
    public void Write(Vector3 _value)
    {
        //��Ŷ�� X, Y, Z���� ���� ����
        Write(_value.x);
        Write(_value.y);
        Write(_value.z);
    }
    /// <summary>
    /// ��Ŷ�� Quaternion �����͸� �߰��Ѵ�.
    /// </summary>
    /// <param name="_value">�߰� �� Quaternion</param>
    public void Write(Quaternion _value)
    {
        //��Ŷ�� X, Y, Z, W���� ���� ����
        Write(_value.x);
        Write(_value.y);
        Write(_value.z);
        Write(_value.w);
    }
    #endregion

    #region Read Data
    /// <summary>��Ŷ���� Byte�� �д´�.</summary>
    /// <param name="_moveReadPos">������ �б� ��ġ�� �̵����� ����</param>
    public byte ReadByte(bool _moveReadPos = true)
    {
        if (buffer.Count > readPos)
        {
            // ���� ���� ����Ʈ�� �ִ� ���
            byte _value = readableBuffer[readPos]; // readPos�� ��ġ���� ����Ʈ ��������
            if (_moveReadPos)
            {
                // _moveReadPos�� true�� ���
                readPos += 1; // readPos�� 1 ����
            }
            return _value; // Byte�� ��ȯ
        }
        else
        {
            throw new Exception("Could not read value of type 'byte'!");
        }
    }

    /// <summary>��Ŷ���� ByteArray�� �д´�.</summary>
    /// <param name="_length">����Ʈ �迭�� ����</param>
    /// <param name="_moveReadPos">������ �б� ��ġ�� �̵����� ����</param>
    public byte[] ReadBytes(int _length, bool _moveReadPos = true)
    {
        if (buffer.Count > readPos)
        {
            // ���� ���� ����Ʈ�� �ִ� ���
            byte[] _value = buffer.GetRange(readPos, _length).ToArray(); // _length ������ readPos ��ġ���� ����Ʈ ��������
            if (_moveReadPos)
            {
                // _moveReadPos�� true�� ���
                readPos += _length; // readPos�� _length��ŭ ����
            }
            return _value; // ByteArray�� ��ȯ
        }
        else
        {
            throw new Exception("Could not read value of type 'byte[]'!");
        }
    }

    /// <summary>��Ŷ���� short�� �д´�.</summary>
    /// <param name="_moveReadPos">������ �б� ��ġ�� �̵����� ����</param>
    public short ReadShort(bool _moveReadPos = true)
    {
        if (buffer.Count > readPos)
        {
            // ���� ���� ����Ʈ�� �ִ� ���
            short _value = BitConverter.ToInt16(readableBuffer, readPos); // ����Ʈ�� short�� ��ȯ
            if (_moveReadPos)
            {
                // _moveReadPos�� true�̰� ���� ���� ����Ʈ�� �ִ� ���
                readPos += 2; // readPos�� 2 ����
            }
            return _value; // Short�� ��ȯ
        }
        else
        {
            throw new Exception("Could not read value of type 'short'!");
        }
    }

    /// <summary>��Ŷ���� int�� �д´�.</summary>
    /// <param name="_moveReadPos">������ �б� ��ġ�� �̵����� ����</param>
    public int ReadInt(bool _moveReadPos = true)
    {
        if (buffer.Count > readPos)
        {
            // ���� ���� ����Ʈ�� �ִ� ���
            int _value = BitConverter.ToInt32(readableBuffer, readPos); // Byte�� Int�� ��ȯ
            if (_moveReadPos)
            {
                // _moveReadPos�� true�� ���
                readPos += 4; // readPos�� 4 ����
            }
            return _value; // Int�� ��ȯ
        }
        else
        {
            throw new Exception("Could not read value of type 'int'!");
        }
    }

    /// <summary>��Ŷ���� long�� �д´�.</summary>
    /// <param name="_moveReadPos">������ �б� ��ġ�� �̵����� ����</param>
    public long ReadLong(bool _moveReadPos = true)
    {
        if (buffer.Count > readPos)
        {
            // ���� ���� ����Ʈ�� �ִ� ���
            long _value = BitConverter.ToInt64(readableBuffer, readPos); // ����Ʈ�� long���� ��ȯ
            if (_moveReadPos)
            {
                // _moveReadPos�� true�� ���
                readPos += 8; // readPos�� 8 ����
            }
            return _value; // Long ��ȯ
        }
        else
        {
            throw new Exception("Could not read value of type 'long'!");
        }
    }

    /// <summary>��Ŷ���� Float�� �н��ϴ�.</summary>
    /// <param name="_moveReadPos">������ �б� ��ġ�� �̵����� ����</param>
    public float ReadFloat(bool _moveReadPos = true)
    {
        if (buffer.Count > readPos)
        {
            // ���� ���� ����Ʈ�� �ִ� ���
            float _value = BitConverter.ToSingle(readableBuffer, readPos); // ����Ʈ�� Float�� ��ȯ
            if (_moveReadPos)
            {
                // _moveReadPos�� true�� ���
                readPos += 4; // readPos�� 4 ����
            }
            return _value; // Float ��ȯ
        }
        else
        {
            throw new Exception("Could not read value of type 'float'!");
        }
    }

    /// <summary>��Ŷ���� Bool�� �д´�.</summary>
    /// <param name="_moveReadPos">������ �б� ��ġ�� �̵����� ����</param>
    public bool ReadBool(bool _moveReadPos = true)
    {
        if (buffer.Count > readPos)
        {
            // ���� ���� ����Ʈ�� �ִ� ���
            bool _value = BitConverter.ToBoolean(readableBuffer, readPos); // ����Ʈ�� Bool�� ��ȯ
            if (_moveReadPos)
            {
                // _moveReadPos�� true�� ���
                readPos += 1; // readPos�� 1 ����
            }
            return _value; // Bool ��ȯ
        }
        else
        {
            throw new Exception("Could not read value of type 'bool'!");
        }
    }

    /// <summary>��Ŷ���� String�� �д´�.</summary>
    /// <param name="_moveReadPos">������ �б� ��ġ�� �̵����� ����</param>
    public string ReadString(bool _moveReadPos = true)
    {
        try
        {
            int _length = ReadInt(); // ���ڿ��� ���� ���ϱ�
            string _value = Encoding.ASCII.GetString(readableBuffer, readPos, _length); // Byte�� String�� ��ȯ
            if (_moveReadPos && _value.Length > 0)
            {
                // _moveReadPos�� ���̸� ���ڿ��� ��� ���� �ʽ��ϴ�.
                readPos += _length; // ���ڿ� ���̸�ŭ readPos ����
            }
            return _value; // ���ڿ��� ��ȯ
        }
        catch
        {
            throw new Exception("Could not read value of type 'string'!");
        }
    }
    /// <summary>
    /// ��Ŷ���� Vector3�� �д´�.
    /// </summary>
    /// <param name="_moveReadPos">������ �б� ��ġ�� �̵����� ����</param>
    /// <returns></returns>
    public Vector3 ReadVector3(bool _moveReadPos = true)
    {
        return new Vector3(ReadFloat(_moveReadPos), ReadFloat(_moveReadPos), ReadFloat(_moveReadPos));
    }
    /// <summary>
    /// ��Ŷ���� Quaternion�� �д´�.
    /// </summary>
    /// <param name="_moveReadPos">������ �б� ��ġ�� �̵����� ����</param>
    /// <returns></returns>
    public Quaternion ReadQuaternion(bool _moveReadPos = true)
    {
        return new Quaternion(ReadFloat(_moveReadPos), ReadFloat(_moveReadPos), ReadFloat(_moveReadPos), ReadFloat(_moveReadPos));
    }
    #endregion

    private bool disposed = false;

    protected virtual void Dispose(bool _disposing)
    {
        if (!disposed)
        {
            if (_disposing)
            {
                buffer = null;
                readableBuffer = null;
                readPos = 0;
            }

            disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
