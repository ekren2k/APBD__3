namespace APBD_3;

public class DeviceManagerFactory
{
    public static DeviceManager CreateDeviceManager()
    {
        DeviceRepository repository = new DeviceRepository();
        DeviceFileParser deviceFileParser = new("input.txt", repository);
        Console.WriteLine("Devices presented after file read.");
        return new DeviceManager(repository);
    }
}