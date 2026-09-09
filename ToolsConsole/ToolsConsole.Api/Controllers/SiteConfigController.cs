using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToolsConsole.Application.Common;
using ToolsConsole.Application.Dtos;
using ToolsConsole.Application.Interfaces;

namespace ToolsConsole.Api.Controllers;

/// <summary>站点展示配置（只暴露允许前台读取的白名单配置）。</summary>
[ApiController]
[Route("[controller]/[action]")]
public sealed class SiteConfigController : ControllerBase
{
    public const string TechStackKey = "SiteConfig:TechStack";
    public const string ProjectsKey = "SiteConfig:Projects";

    private readonly ISystemConfigService _systemConfigService;

    public SiteConfigController(ISystemConfigService systemConfigService)
    {
        _systemConfigService = systemConfigService;
    }

    /// <summary>GET /SiteConfig/Get</summary>
    [HttpGet]
    [AllowAnonymous]
    [ActionName("Get")]
    public async Task<IActionResult> GetAsync(CancellationToken cancellationToken)
    {
        var techStack = await _systemConfigService.GetByKeyAsync(TechStackKey, cancellationToken);
        var projects = await _systemConfigService.GetByKeyAsync(ProjectsKey, cancellationToken);

        var dto = new SiteConfigDtos.SiteConfigDto
        {
            TechStack = ParseJsonArray(techStack?.ConfigValue),
            Projects = ParseJsonArray(projects?.ConfigValue),
        };

        return Ok(ApiResult<SiteConfigDtos.SiteConfigDto>.Ok(dto));
    }

    private static JsonNode? ParseJsonArray(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        try
        {
            return JsonNode.Parse(value);
        }
        catch
        {
            return null;
        }
    }
}
