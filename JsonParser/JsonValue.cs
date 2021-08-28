using System.Collections.Generic;
using System.Text;

namespace JsonParser
{
    public abstract class JsonValue
    {
    }
    public class StringValue : JsonValue
    {
        public StringValue(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public override string ToString()
        {
            return $"\"{Value}\"";
        }
    }
    public class NullValue : JsonValue
    {

        public override string ToString()
        {
            return $"null";
        }
    }
    public class TrueValue : JsonValue
    {

        public override string ToString()
        {
            return $"true";
        }
    }
    public class FalseValue : JsonValue
    {

        public override string ToString()
        {
            return $"false";
        }
    }
    public class NumberValue : JsonValue
    {
        public NumberValue(double value)
        {
            Value = value;
        }

        public double Value { get; }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
    public class ArrayValue : JsonValue
    {
        public List<JsonValue> Value { get; } = new List<JsonValue>();

        public override string ToString()
        {
            if (Value.Count==0)
            {
                return "[]";
            }
            var temp = new StringBuilder("[");
            foreach (var item in Value)
            {
                temp.Append(item.ToString());
                temp.Append(",");
            }
            temp.Remove(temp.Length - 1, 1);
            temp.Append("]");
            return temp.ToString();
        }
    }
    public class ObjectValue : JsonValue
    {
        public Dictionary<string, JsonValue> Value { get; } = new Dictionary<string, JsonValue>();

        public override string ToString()
        {
            if (Value.Count == 0)
            {
                return "{}";
            }
            var temp = new StringBuilder("{");
            foreach (var item in Value)
            {
                temp.Append($"\"{item.Key}\"");
                temp.Append(":");
                temp.Append(item.Value.ToString());
                temp.Append(",");
            }
            temp.Remove(temp.Length - 1, 1);
            temp.Append("}");
            return temp.ToString();
        }
    }
}
