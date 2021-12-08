using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ProtoPacket
{
    private List<byte> bytes = new List<byte>();
    public int Length => bytes.Count;
    public bool IsDefault => bytes.Count == 0;
    public void AddBytes(byte[] bytes) => this.bytes.AddRange(bytes);
    public byte[] GetBytes() => bytes.ToArray();
}

