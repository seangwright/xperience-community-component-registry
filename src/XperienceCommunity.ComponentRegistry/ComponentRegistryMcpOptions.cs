namespace XperienceCommunity.ComponentRegistry;

/// <summary>
/// Configuration options for the component registry MCP endpoint.
/// </summary>
public class ComponentRegistryMcpOptions
{
    /// <summary>
    /// Configuration section key.
    /// </summary>
    public const string SectionName = "XperienceCommunity:ComponentRegistry:Mcp";

    /// <summary>
    /// Enables MCP endpoint and tool registration.
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// HTTP endpoint path for MCP transport.
    /// </summary>
    public string EndpointPath { get; set; } = "/mcp/component-registry";
}
