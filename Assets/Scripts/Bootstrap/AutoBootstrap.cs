using Unity.NetCode;

public class AutoBootstrap : ClientServerBootstrap
{
    public override bool Initialize(string defaultWorldName)
    {
        AutoConnectPort = 7777;
        CreateDefaultClientServerWorlds();
        return true;
    }
}