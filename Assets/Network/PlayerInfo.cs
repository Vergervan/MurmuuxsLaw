// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: PlayerInfo.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
/// <summary>Holder for reflection information generated from PlayerInfo.proto</summary>
public static partial class PlayerInfoReflection {

  #region Descriptor
  /// <summary>File descriptor for PlayerInfo.proto</summary>
  public static pbr::FileDescriptor Descriptor {
    get { return descriptor; }
  }
  private static pbr::FileDescriptor descriptor;

  static PlayerInfoReflection() {
    byte[] descriptorData = global::System.Convert.FromBase64String(
        string.Concat(
          "ChBQbGF5ZXJJbmZvLnByb3RvItkBCgpQbGF5ZXJJbmZvEiUKBHR5cGUYASAB",
          "KA4yFy5QbGF5ZXJJbmZvLk1lc3NhZ2VUeXBlEigKCWRpcmVjdGlvbhgCIAEo",
          "DjIVLlBsYXllckluZm8uRGlyZWN0aW9uEhAKCGFuaW1OYW1lGAMgASgJIjQK",
          "C01lc3NhZ2VUeXBlEgsKB0Nvbm5lY3QQABIICgRNb3ZlEAESDgoKRGlzY29u",
          "bmVjdBACIjIKCURpcmVjdGlvbhIGCgJVcBAAEggKBERvd24QARIICgRMZWZ0",
          "EAISCQoFUmlnaHQQA2IGcHJvdG8z"));
    descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
        new pbr::FileDescriptor[] { },
        new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
          new pbr::GeneratedClrTypeInfo(typeof(global::PlayerInfo), global::PlayerInfo.Parser, new[]{ "Type", "Direction", "AnimName" }, null, new[]{ typeof(global::PlayerInfo.Types.MessageType), typeof(global::PlayerInfo.Types.Direction) }, null, null)
        }));
  }
  #endregion

}
#region Messages
public sealed partial class PlayerInfo : pb::IMessage<PlayerInfo>
#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    , pb::IBufferMessage
#endif
{
  private static readonly pb::MessageParser<PlayerInfo> _parser = new pb::MessageParser<PlayerInfo>(() => new PlayerInfo());
  private pb::UnknownFieldSet _unknownFields;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public static pb::MessageParser<PlayerInfo> Parser { get { return _parser; } }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public static pbr::MessageDescriptor Descriptor {
    get { return global::PlayerInfoReflection.Descriptor.MessageTypes[0]; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  pbr::MessageDescriptor pb::IMessage.Descriptor {
    get { return Descriptor; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public PlayerInfo() {
    OnConstruction();
  }

  partial void OnConstruction();

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public PlayerInfo(PlayerInfo other) : this() {
    type_ = other.type_;
    direction_ = other.direction_;
    animName_ = other.animName_;
    _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public PlayerInfo Clone() {
    return new PlayerInfo(this);
  }

  /// <summary>Field number for the "type" field.</summary>
  public const int TypeFieldNumber = 1;
  private global::PlayerInfo.Types.MessageType type_ = global::PlayerInfo.Types.MessageType.Connect;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public global::PlayerInfo.Types.MessageType Type {
    get { return type_; }
    set {
      type_ = value;
    }
  }

  /// <summary>Field number for the "direction" field.</summary>
  public const int DirectionFieldNumber = 2;
  private global::PlayerInfo.Types.Direction direction_ = global::PlayerInfo.Types.Direction.Up;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public global::PlayerInfo.Types.Direction Direction {
    get { return direction_; }
    set {
      direction_ = value;
    }
  }

  /// <summary>Field number for the "animName" field.</summary>
  public const int AnimNameFieldNumber = 3;
  private string animName_ = "";
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public string AnimName {
    get { return animName_; }
    set {
      animName_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
    }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public override bool Equals(object other) {
    return Equals(other as PlayerInfo);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public bool Equals(PlayerInfo other) {
    if (ReferenceEquals(other, null)) {
      return false;
    }
    if (ReferenceEquals(other, this)) {
      return true;
    }
    if (Type != other.Type) return false;
    if (Direction != other.Direction) return false;
    if (AnimName != other.AnimName) return false;
    return Equals(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public override int GetHashCode() {
    int hash = 1;
    if (Type != global::PlayerInfo.Types.MessageType.Connect) hash ^= Type.GetHashCode();
    if (Direction != global::PlayerInfo.Types.Direction.Up) hash ^= Direction.GetHashCode();
    if (AnimName.Length != 0) hash ^= AnimName.GetHashCode();
    if (_unknownFields != null) {
      hash ^= _unknownFields.GetHashCode();
    }
    return hash;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public override string ToString() {
    return pb::JsonFormatter.ToDiagnosticString(this);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public void WriteTo(pb::CodedOutputStream output) {
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    output.WriteRawMessage(this);
  #else
    if (Type != global::PlayerInfo.Types.MessageType.Connect) {
      output.WriteRawTag(8);
      output.WriteEnum((int) Type);
    }
    if (Direction != global::PlayerInfo.Types.Direction.Up) {
      output.WriteRawTag(16);
      output.WriteEnum((int) Direction);
    }
    if (AnimName.Length != 0) {
      output.WriteRawTag(26);
      output.WriteString(AnimName);
    }
    if (_unknownFields != null) {
      _unknownFields.WriteTo(output);
    }
  #endif
  }

  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
    if (Type != global::PlayerInfo.Types.MessageType.Connect) {
      output.WriteRawTag(8);
      output.WriteEnum((int) Type);
    }
    if (Direction != global::PlayerInfo.Types.Direction.Up) {
      output.WriteRawTag(16);
      output.WriteEnum((int) Direction);
    }
    if (AnimName.Length != 0) {
      output.WriteRawTag(26);
      output.WriteString(AnimName);
    }
    if (_unknownFields != null) {
      _unknownFields.WriteTo(ref output);
    }
  }
  #endif

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public int CalculateSize() {
    int size = 0;
    if (Type != global::PlayerInfo.Types.MessageType.Connect) {
      size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) Type);
    }
    if (Direction != global::PlayerInfo.Types.Direction.Up) {
      size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) Direction);
    }
    if (AnimName.Length != 0) {
      size += 1 + pb::CodedOutputStream.ComputeStringSize(AnimName);
    }
    if (_unknownFields != null) {
      size += _unknownFields.CalculateSize();
    }
    return size;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public void MergeFrom(PlayerInfo other) {
    if (other == null) {
      return;
    }
    if (other.Type != global::PlayerInfo.Types.MessageType.Connect) {
      Type = other.Type;
    }
    if (other.Direction != global::PlayerInfo.Types.Direction.Up) {
      Direction = other.Direction;
    }
    if (other.AnimName.Length != 0) {
      AnimName = other.AnimName;
    }
    _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public void MergeFrom(pb::CodedInputStream input) {
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    input.ReadRawMessage(this);
  #else
    uint tag;
    while ((tag = input.ReadTag()) != 0) {
      switch(tag) {
        default:
          _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
          break;
        case 8: {
          Type = (global::PlayerInfo.Types.MessageType) input.ReadEnum();
          break;
        }
        case 16: {
          Direction = (global::PlayerInfo.Types.Direction) input.ReadEnum();
          break;
        }
        case 26: {
          AnimName = input.ReadString();
          break;
        }
      }
    }
  #endif
  }

  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input) {
    uint tag;
    while ((tag = input.ReadTag()) != 0) {
      switch(tag) {
        default:
          _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
          break;
        case 8: {
          Type = (global::PlayerInfo.Types.MessageType) input.ReadEnum();
          break;
        }
        case 16: {
          Direction = (global::PlayerInfo.Types.Direction) input.ReadEnum();
          break;
        }
        case 26: {
          AnimName = input.ReadString();
          break;
        }
      }
    }
  }
  #endif

  #region Nested types
  /// <summary>Container for nested types declared in the PlayerInfo message type.</summary>
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public static partial class Types {
    public enum MessageType {
      [pbr::OriginalName("Connect")] Connect = 0,
      [pbr::OriginalName("Move")] Move = 1,
      [pbr::OriginalName("Disconnect")] Disconnect = 2,
    }

    public enum Direction {
      [pbr::OriginalName("Up")] Up = 0,
      [pbr::OriginalName("Down")] Down = 1,
      [pbr::OriginalName("Left")] Left = 2,
      [pbr::OriginalName("Right")] Right = 3,
    }

  }
  #endregion

}

#endregion


#endregion Designer generated code
