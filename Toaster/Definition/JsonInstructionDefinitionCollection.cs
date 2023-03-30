using Newtonsoft.Json;

namespace Toaster.Definition;

public class JsonInstructionDefinitionCollection
{
    [JsonProperty("assembly")]
    public string Assembly { get; set; }

    [JsonProperty("namespace")]
    public string NamespacePrefix { get; set; }

    [JsonProperty("type-postfix")]
    public string TypePostfix { get; set; }

    [JsonProperty("definitions")]
    public JsonInstructionDefinition[] Definitions { get; set; }
}