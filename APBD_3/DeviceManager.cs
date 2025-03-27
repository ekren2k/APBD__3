
namespace APBD_3;

public class DeviceManager
{
    private DeviceRepository _deviceRepository;

    public DeviceManager(DeviceRepository deviceRepository)
    {
        _deviceRepository = deviceRepository;
    }
    
    public void TurnOnDevice(string id)
    {
        foreach (var storedDevice in _deviceRepository.GetDevices())
        {
            if (storedDevice.Id.Equals(id))
            {
                storedDevice.TurnOn();
                return;
            }
        }
        
        throw new ArgumentException($"Device with ID {id} is not stored.", nameof(id));
    }

    public void TurnOffDevice(string id)
    {
        foreach (var storedDevice in _deviceRepository.GetDevices())
        {
            if (storedDevice.Id.Equals(id))
            {
                storedDevice.TurnOff();
                return;
            }
        }
        
        throw new ArgumentException($"Device with ID {id} is not stored.", nameof(id));
    }

    public void ShowAllDevices()
    {
        _deviceRepository.ShowAllDevices();
    }

    public void AddDevice(Device device)
    {
        _deviceRepository.AddDevice(device);
    }
    
    public void EditDevice(Device device)
    {
        _deviceRepository.EditDevice(device);
    }

    public void RemoveDeviceById(string id)
    {
        _deviceRepository.RemoveDeviceById(id);
    }
    



    
}