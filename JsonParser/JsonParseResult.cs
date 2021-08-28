using System;

namespace JsonParser
{
    ref struct JsonParseResult
    {
        public JsonParseResult(ReadOnlySpan<char> data, JsonValue value)
        {
            Data = data;
            Value = value;
        }

        public ReadOnlySpan<char> Data { get; set; }
        public JsonValue Value { get; set; }

    }
}
