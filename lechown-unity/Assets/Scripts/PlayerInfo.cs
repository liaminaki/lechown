using Unity.Netcode;

[System.Serializable]
public struct PlayerInfo : INetworkSerializable
{
    public ulong ClientId;
    public string PlayerName;
    public string Status;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref ClientId);
        serializer.SerializeValue(ref PlayerName);
        serializer.SerializeValue(ref Status);
    }
}
