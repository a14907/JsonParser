using System;
using System.Collections.Generic;
using System.Text;

namespace JsonParser
{
    public static class Parser
    {
        const char _objectBegin = '{';
        const char _objectEnd = '}';
        const char _arrayBegin = '[';
        const char _arrayEnd = ']';
        const char _stringSplit = '\"';
        const char _kvSplit = ':';
        const char _split = ',';
        const char _number0 = '0';
        const char _number1 = '1';
        const char _number2 = '2';
        const char _number3 = '3';
        const char _number4 = '4';
        const char _number5 = '5';
        const char _number6 = '6';
        const char _number7 = '7';
        const char _number8 = '8';
        const char _number9 = '9';
        const char _numberNegtive = '-';
        const char _numberSplit = '.';
        const char _nullStart = 'n';
        const char _trueStart = 't';
        const char _falseStart = 'f';

        public static JsonValue Process(string raw)
        {
            if (string.IsNullOrEmpty(raw))
            {
                throw new ArgumentNullException(nameof(raw));
            }
            var data = raw.AsSpan().Trim();
            return ProcessImpl(data).Value;
        }

        private static JsonParseResult ProcessImpl(ReadOnlySpan<char> data)
        {
            switch (data[0])
            {
                case _objectBegin:
                    return ProcessObject(data);
                case _arrayBegin:
                    return ProcessArray(data);
                case _stringSplit:
                    return ProcessString(data);
                case _nullStart:
                    return ProcessNull(data);
                case _trueStart:
                    return ProcessTrue(data);
                case _falseStart:
                    return ProcessFalse(data);
                case _number0:
                case _number1:
                case _number2:
                case _number3:
                case _number4:
                case _number5:
                case _number6:
                case _number7:
                case _number8:
                case _number9:
                case _numberNegtive:
                case _numberSplit:
                    return ProcessDouble(data);
                default:
                    throw new InvalidOperationException("unsupport json");
            }
        }

        private static JsonParseResult ProcessArray(ReadOnlySpan<char> data)
        {
            EnsureStartWith(data, _arrayBegin);
            var result = new ArrayValue();
            data = data.Slice(1);
            while (true)
            {
                if (data[0] == _arrayEnd)
                {
                    data = data.Slice(1);
                    break;
                }
                var item = ProcessImpl(data);
                result.Value.Add(item.Value);
                data = item.Data.TrimStart();
                if (data[0] == _split)
                {
                    data = data.Slice(1);
                    continue;
                }
                else if (data[0] == _arrayEnd)
                {
                    data = data.Slice(1);
                    break;
                }
                else
                {
                    throw new Exception("array handle err");
                }
            }
            return new JsonParseResult(data, result);
        }

        private static void EnsureStartWith(ReadOnlySpan<char> data, char c)
        {
            if (data[0] == c)
            {
                return;
            }
            throw new Exception($"{data.ToString()} not start with {c}");
        }

        private static JsonParseResult ProcessObject(ReadOnlySpan<char> data)
        {
            EnsureStartWith(data, _objectBegin);
            var result = new ObjectValue();
            data = data.Slice(1);
            while (true)
            {
                if (data[0] == _objectEnd)
                {
                    data = data.Slice(1);
                    break;
                }
                //key
                var keyRes = ProcessString(data);
                data = keyRes.Data.TrimStart();
                EnsureStartWith(data, _kvSplit);

                data = data.Slice(1);
                //value
                var valueRes = ProcessImpl(data);
                result.Value.Add((keyRes.Value as StringValue).Value, valueRes.Value);
                data = valueRes.Data.TrimStart();
                if (data[0] == _split)
                {
                    data = data.Slice(1);
                    continue;
                }
                else if (data[0] == _objectEnd)
                {
                    data = data.Slice(1);
                    break;
                }
                else
                {
                    throw new Exception("object handle err");
                }
            }
            return new JsonParseResult(data, result);
        }
        private static JsonParseResult ProcessString(ReadOnlySpan<char> data)
        {
            data = data.TrimStart();
            EnsureStartWith(data, _stringSplit);
            data = data.Slice(1);
            var sb = new StringBuilder();
            int i = 0;
            char prechar = ' ';
            for (; i < data.Length; i++)
            {
                var c = data[i];
                if (c == _stringSplit && prechar != '\\')
                {
                    i++;
                    break;
                }
                prechar = c;
                sb.Append(c);
            }
            data = data.Slice(i);
            return new JsonParseResult(data, new StringValue(sb.ToString()));
        }
        private static JsonParseResult ProcessDouble(ReadOnlySpan<char> data)
        {
            //only support normal number now! maybe extend future
            data = data.TrimStart();

            var sb = new StringBuilder();
            int i = 0;
            for (; i < data.Length; i++)
            {
                var c = data[i];
                if (c == _number0 ||
                    c == _number1 ||
                    c == _number2 ||
                    c == _number3 ||
                    c == _number4 ||
                    c == _number5 ||
                    c == _number6 ||
                    c == _number7 ||
                    c == _number8 ||
                    c == _number9 ||
                    c == _numberNegtive ||
                    c == _numberSplit)
                {
                    sb.Append(c);
                    continue;
                }
                break;
            }
            data = data.Slice(i);
            if (double.TryParse(sb.ToString(), out double r) == false)
            {
                throw new Exception("number handle err");
            }
            return new JsonParseResult(data, new NumberValue(r));
        }

        private static JsonParseResult ProcessNull(ReadOnlySpan<char> data)
        {
            data = data.TrimStart();

            var nullstr = new string(data.Slice(0, 4));
            if (nullstr != "null")
            {
                throw new Exception("null handle err");
            }
            data = data.Slice(4);
            return new JsonParseResult(data, new NullValue());
        }

        private static JsonParseResult ProcessFalse(ReadOnlySpan<char> data)
        {
            data = data.TrimStart();

            var nullstr = new string(data.Slice(0, 5));
            if (nullstr != "false")
            {
                throw new Exception("false handle err");
            }
            data = data.Slice(5);
            return new JsonParseResult(data, new FalseValue());
        }

        private static JsonParseResult ProcessTrue(ReadOnlySpan<char> data)
        {
            data = data.TrimStart();

            var nullstr = new string(data.Slice(0, 4));
            if (nullstr != "true")
            {
                throw new Exception("true handle err");
            }
            data = data.Slice(4);
            return new JsonParseResult(data, new TrueValue());
        }
    }
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
