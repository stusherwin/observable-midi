namespace SetupManager.Core
{
    public interface IDeviceInfoProvider<out T>
        where T : DeviceInfo
    {
        T[] GetDeviceInfos();
    }
}
