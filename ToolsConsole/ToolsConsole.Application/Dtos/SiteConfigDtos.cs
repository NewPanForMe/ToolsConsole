using System.Text.Json.Nodes;

namespace ToolsConsole.Application.Dtos;

/// <summary>站点展示配置 DTO。</summary>
public static class SiteConfigDtos
{
    public sealed class SiteConfigDto
    {
        public JsonNode? TechStack { get; set; }

        public JsonNode? Projects { get; set; }
    }
}
