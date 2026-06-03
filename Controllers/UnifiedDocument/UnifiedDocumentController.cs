using Chap10.Dtos.UnifiedDTO;
using Microsoft.AspNetCore.Mvc;
using Chap10.Services.Shared;
using Chap10.Dtos;

namespace Chap10.Controllers;
[ApiController]
[Route("api/[controller]")]
public class UnifiedDocumentController : ControllerBase
{
    private readonly IUnifiedDocumentService _service;

    public UnifiedDocumentController(IUnifiedDocumentService service)
    {
        _service = service;
    }

    [HttpGet("{vin}")]
    public async Task<ActionResult<APIResponse<UnifiedDocumentDto>>> GetByVin(
        string vin)
    {
        if (string.IsNullOrWhiteSpace(vin))
        {
            return BadRequest("VIN is required.");
        }

        var documents = await _service.GetDocumentsByVinAsync(int.Parse(vin));

        return APIResponse<UnifiedDocumentDto>.Ok(documents);
    }

}