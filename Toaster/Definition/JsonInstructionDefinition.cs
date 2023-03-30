using Newtonsoft.Json;

namespace Toaster.Definition;

public class JsonInstructionDefinition
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("instruction-type")]
    public string TypePath { get; set; }

    [JsonProperty("time")]
    public int Time { get; set; } = 1;

    [JsonProperty("parameters")]
    public DefinitionParameterFlag[] ParameterFlags { get; set; }
}