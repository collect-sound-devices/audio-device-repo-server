using DeviceControllerLib.Models.RestApi;
using DeviceControllerLib.Services;
using Microsoft.AspNetCore.Mvc;

namespace DeviceControllerLib.Controllers
{
    // For detailed API documentation, including endpoint descriptions, request/response formats, 
    // and examples, please refer to the `rest-api-documentation.md` file located in the project root.
    [ApiController]
    [Route("api/[controller]")]
    public class AudioDevicesController(IAudioDeviceStorage storage, ICryptService cryptService) : ControllerBase
    {
        [HttpGet]
        public IEnumerable<EntireDeviceMessage> GetAll() => storage.GetAll();

        [HttpPost]
        public IActionResult Add([FromBody] EntireDeviceMessage entireDeviceMessage)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            entireDeviceMessage = entireDeviceMessage with { HostName = cryptService.ComputeChecksum(entireDeviceMessage.HostName) };

            storage.Add(entireDeviceMessage);
            return CreatedAtAction(
                nameof(GetByKey),
                new { pnpId = entireDeviceMessage.PnpId, hostName = entireDeviceMessage.HostName },
                entireDeviceMessage
            );
        }

        [HttpGet("{pnpId}/{hostName}")]
        public IActionResult GetByKey(string pnpId, string hostName)
        {
            foreach (var realHostNameOrCheckSummed in new[] { true, false })
            {
                var device = storage.GetAll().FirstOrDefault(
                    d => d.PnpId == pnpId && d.HostName == hostName
                );
                if (device != null)
                {
                    return Ok(device);
                }

                if (!realHostNameOrCheckSummed)
                {
                    break;
                }

                hostName = cryptService.ComputeChecksum(hostName);
            }
            return NotFound();
        }

        [HttpDelete("{pnpId}/{hostName}")]
        public IActionResult Remove(string pnpId, string hostName)
        {
            foreach (var realHostNameOrCheckSummed in new[] { true, false })
            {
                if (storage.GetAll().FirstOrDefault(
                        d => d.PnpId == pnpId && d.HostName == hostName
                    ) != null)
                {
                    storage.Remove(pnpId, hostName);
                    return NoContent();
                }

                if (!realHostNameOrCheckSummed)
                {
                    break;
                }

                hostName = cryptService.ComputeChecksum(hostName);
            }

            return NotFound();
        }

        [HttpPut("{pnpId}/{hostName}")]
        public IActionResult UpdateVolume(string pnpId, string hostName, [FromBody] VolumeChangeMessage volumeChangeMessage)
        {
            hostName = cryptService.ComputeChecksum(hostName);

            storage.UpdateVolume(pnpId, hostName, volumeChangeMessage);
            return NoContent();
        }

        [HttpGet("search")]
        public IEnumerable<EntireDeviceMessage> Search([FromQuery] string query)
        {
            var hashedHost = cryptService.ComputeChecksum(query);

            return storage.Search(query)
                .Union(storage.Search(hashedHost));
        }
    }
}
