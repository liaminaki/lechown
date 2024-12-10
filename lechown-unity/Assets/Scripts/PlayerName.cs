using Unity.Collections;
using Unity.Netcode;
using System;

public struct PlayerName : INetworkSerializable, IEquatable<PlayerName>
{
    public FixedString64Bytes Name;

    public PlayerName(FixedString64Bytes name)
    {
        Name = name;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref Name);
    }

    // Implement the IEquatable<PlayerName> interface to compare PlayerName instances
    public bool Equals(PlayerName other)
    {
        return Name.Equals(other.Name);
    }

    // Override the GetHashCode method to ensure consistency with Equals
    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }
}
