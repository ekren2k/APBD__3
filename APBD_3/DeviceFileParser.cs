using System.Text;

namespace APBD_3;

public class DeviceFileParser
{
    private const int MinimumRequiredElements = 4;

    private const int IndexPosition = 0;
    private const int DeviceNamePosition = 1;
    private const int EnabledStatusPosition = 2;
    private string _inputDeviceFile;

    private DeviceRepository _deviceRepository;
    

    public DeviceFileParser(string filePath, DeviceRepository deviceRepository)
    {
        _inputDeviceFile = filePath;
        _deviceRepository = deviceRepository;

        if (!File.Exists(_inputDeviceFile))
        {
            throw new FileNotFoundException("The input device file could not be found.");
        }

        var lines = File.ReadAllLines(_inputDeviceFile);
        ParseDevices(lines);
    }
    
    private void ParseDevices(string[] lines)
    {
        for (int i = 0; i < lines.Length; i++)
        {
            try
            {
                Device parsedDevice;
                    
                if (lines[i].StartsWith("P-"))
                {
                    parsedDevice = ParsePC(lines[i], i);
                }
                else if (lines[i].StartsWith("SW-"))
                {
                    parsedDevice = ParseSmartwatch(lines[i], i);
                }
                else if (lines[i].StartsWith("ED-"))
                {
                    parsedDevice = ParseEmbedded(lines[i], i);
                }
                else
                {
                    throw new ArgumentException($"Line {i} is corrupted.");
                }
                    
                _deviceRepository.AddDevice(parsedDevice);
            }
            catch (ArgumentException argEx)
            {
                Console.WriteLine(argEx.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Something went wrong during parsing this line: {lines[i]}. The exception message: {ex.Message}");
            }
        }
    }
    
    public PersonalComputer ParsePC(string line, int lineNumber)
    {
        const int SystemPosition = 3;
        
        var infoSplits = line.Split(',');

        if (infoSplits.Length < MinimumRequiredElements)
        {
            throw new ArgumentException($"Corrupted line {lineNumber}", line);
        }
        
        if (bool.TryParse(infoSplits[EnabledStatusPosition], out bool _) is false)
        {
            throw new ArgumentException($"Corrupted line {lineNumber}: can't parse enabled status for computer.", line);
        }
        
        return new PersonalComputer(infoSplits[IndexPosition], infoSplits[DeviceNamePosition], 
            bool.Parse(infoSplits[EnabledStatusPosition]), infoSplits[SystemPosition]);
    }

    public Smartwatch ParseSmartwatch(string line, int lineNumber)
    {
        const int BatteryPosition = 3;
        
        var infoSplits = line.Split(',');

        if (infoSplits.Length < MinimumRequiredElements)
        {
            throw new ArgumentException($"Corrupted line {lineNumber}", line);
        }
        
        if (bool.TryParse(infoSplits[EnabledStatusPosition], out bool _) is false)
        {
            throw new ArgumentException($"Corrupted line {lineNumber}: can't parse enabled status for smartwatch.", line);
        }

        if (int.TryParse(infoSplits[BatteryPosition].Replace("%", ""), out int _) is false)
        {
            throw new ArgumentException($"Corrupted line {lineNumber}: can't parse battery level for smartwatch.", line);
        }

        return new Smartwatch(infoSplits[IndexPosition], infoSplits[DeviceNamePosition], 
            bool.Parse(infoSplits[EnabledStatusPosition]), int.Parse(infoSplits[BatteryPosition].Replace("%", "")));
    }

    public Embedded ParseEmbedded(string line, int lineNumber)
    {
        const int IpAddressPosition = 3;
        const int NetworkNamePosition = 4;
        
        var infoSplits = line.Split(',');

        if (infoSplits.Length < MinimumRequiredElements + 1)
        {
            throw new ArgumentException($"Corrupted line {lineNumber}", line);
        }
        
        if (bool.TryParse(infoSplits[EnabledStatusPosition], out bool _) is false)
        {
            throw new ArgumentException($"Corrupted line {lineNumber}: can't parse enabled status for embedded device.", line);
        }

        return new Embedded(infoSplits[IndexPosition], infoSplits[DeviceNamePosition], 
            bool.Parse(infoSplits[EnabledStatusPosition]), infoSplits[IpAddressPosition], 
            infoSplits[NetworkNamePosition]);
    }
    
    public void SaveDevices(string outputPath)
    {
        StringBuilder devicesSb = new();

        foreach (var storedDevice in _deviceRepository.GetDevices())
        {
            if (storedDevice is Smartwatch smartwatchCopy)
            {
                devicesSb.AppendLine($"{smartwatchCopy.Id},{smartwatchCopy.Name}," +
                                     $"{smartwatchCopy.IsEnabled},{smartwatchCopy.BatteryLevel}%");
            }
            else if (storedDevice is PersonalComputer pcCopy)
            {
                devicesSb.AppendLine($"{pcCopy.Id},{pcCopy.Name}," +
                                     $"{pcCopy.IsEnabled},{pcCopy.OperatingSystem}");
            }
            else
            {
                var embeddedCopy = storedDevice as Embedded;
                devicesSb.AppendLine($"{embeddedCopy.Id},{embeddedCopy.Name}," +
                                     $"{embeddedCopy.IsEnabled},{embeddedCopy.IpAddress}," +
                                     $"{embeddedCopy.NetworkName}");
            }
        }
        
        File.WriteAllLines(outputPath, devicesSb.ToString().Split('\n'));
    }
}