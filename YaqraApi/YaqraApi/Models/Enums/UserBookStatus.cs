using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace YaqraApi.Models.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum UserBookStatus
    {
        TO_READ,
        CURRENTLY_READING,
        ALREADY_READ
    }
}
