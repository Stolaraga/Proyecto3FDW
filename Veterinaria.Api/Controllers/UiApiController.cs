using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("uiapi")]
public class UiApiController : ControllerBase
{
    private readonly IHttpClientFactory _httpFactory;
    private readonly string _base;

    public UiApiController(IHttpClientFactory httpFactory, IConfiguration cfg)
    {
        _httpFactory = httpFactory;
        _base = (cfg["ApiBaseUrl"] ?? "https://localhost:7138/api").TrimEnd('/');
    }

    [HttpGet("clientes")]
    public async Task<IActionResult> Clientes()
    {
        var http = _httpFactory.CreateClient();
        var r = await http.GetAsync($"{_base}/Clientes");
        var body = await r.Content.ReadAsStringAsync();
        return Content(body, "application/json");
    }

    [HttpGet("mascotas")]
    public async Task<IActionResult> Mascotas([FromQuery] Guid clienteId)
    {
        var http = _httpFactory.CreateClient();
        var r = await http.GetAsync($"{_base}/Mascotas?clienteId={clienteId}");
        var body = await r.Content.ReadAsStringAsync();
        return Content(body, "application/json");
    }

    [HttpGet("mascotas/{id:guid}")]
    public async Task<IActionResult> Mascota(Guid id)
    {
        var http = _httpFactory.CreateClient();
        var r = await http.GetAsync($"{_base}/Mascotas/{id}");
        var body = await r.Content.ReadAsStringAsync();
        return Content(body, "application/json");
    }

    [HttpGet("catalogo-procedimientos")]
    public async Task<IActionResult> Catalogo()
    {
        var http = _httpFactory.CreateClient();
        var r = await http.GetAsync($"{_base}/catalogo-procedimientos");
        var body = await r.Content.ReadAsStringAsync();
        return Content(body, "application/json");
    }

    [HttpPost("procedimiento-mascota")]
    public async Task<IActionResult> CrearProcedimiento([FromBody] JsonElement payload, [FromQuery] string? codigo)
    {
        var http = _httpFactory.CreateClient();
        var url = $"{_base}/ProcedimientoMascotas" + (string.IsNullOrWhiteSpace(codigo) ? "" : $"?codigo={Uri.EscapeDataString(codigo)}");
        var content = new StringContent(payload.GetRawText(), Encoding.UTF8, "application/json");
        var r = await http.PostAsync(url, content);
        var body = await r.Content.ReadAsStringAsync();
        return StatusCode((int)r.StatusCode, body);
    }
}
