using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

public class ProtoPacket
{
    private List<byte> bytes = new List<byte>();
    public int Length => bytes.Count;
    public bool IsDefault => bytes.Count == 0;
    public int Position { get; private set; }
    public void AddBytes(byte[] bytes) => this.bytes.AddRange(bytes);
    public void AddBytes(byte[] bytes, int length)
    {
        byte[] newbytes = new byte[length];
        for (int i = 0; i < length; i++)
            newbytes[i] = bytes[i];
        this.bytes.AddRange(newbytes);
    }
    public byte[] GetBytes() => bytes.ToArray();

    public ProtoPacket WriteInt(int x)
    {
        AddBytes(BitConverter.GetBytes(x));
        return this;
    }
    public ProtoPacket WriteShort(short x)
    {
        AddBytes(BitConverter.GetBytes(x));
        return this;
    }
    public ProtoPacket WriteFloat(float x)
    {
        AddBytes(BitConverter.GetBytes(x));
        return this;
    }
    public ProtoPacket WriteByte(byte x)
    {
        this.bytes.Add(x);
        return this;
    }
    public ProtoPacket WriteBool(bool x)
    {
        unsafe
        {
            this.bytes.Add(*(byte*)&x);
        }
        return this;
    }

    public int ReadInt()
    {
        int curPos = Position;
        Position += sizeof(int);
        return BitConverter.ToInt32(bytes.ToArray(), curPos);
    }
    public short ReadShort()
    {
        int curPos = Position;
        Position += sizeof(short);
        return BitConverter.ToInt16(bytes.ToArray(), curPos);
    }
    public float ReadFloat()
    {
        int curPos = Position;
        Position += sizeof(float);
        return BitConverter.ToSingle(bytes.ToArray(), curPos);
    }
    public byte ReadByte()
    {
        int curPos = Position;
        Position += sizeof(byte);
        return bytes.ElementAt(curPos);
    }
    public bool ReadBool()
    {
        int curPos = Position;
        Position += sizeof(bool);
        return BitConverter.ToBoolean(bytes.ToArray(), curPos);
    }

    public void SendPacket(Socket socket)
    {
        socket.Send(BitConverter.GetBytes(bytes.Count));
        socket.Send(bytes.ToArray());
    }
}

