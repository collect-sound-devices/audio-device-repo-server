using System.Text;
using System.IO.Hashing;
using DeviceControllerLib.Services;

namespace DeviceRepoAspNetCore.Services;

public class ChecksumService(ILogger<ChecksumService> logger) : IChecksumService
{
    public string ComputeChecksum(string str)
    {
        logger.LogDebug("Computing CRC32 checksum for string of length {Length}", str.Length);
        var data = Encoding.UTF8.GetBytes(str);
        // Create a CRC32 instance
        var crc = new Crc32();
        crc.Append(data);

        // Get the CRC32 hash as a byte array
        var hashBytes = crc.GetCurrentHash();
        // Reverse the byte order (major and minor bytes)
        Array.Reverse(hashBytes);

        var result = Convert.ToHexString(hashBytes).PadLeft(8, '0').ToLower();
        logger.LogDebug("Computed CRC32 checksum: {Checksum}", result);
        return result;
    }
}
