using System;
using System.Collections;
using System.IO;
using System.Text;

namespace Eve.Scheduler.Controller
{
    public class YamlProvider : ILogProvider
    {
        const string indentString = "\t";
        const string timeFormat = "yyyy/MM/dd HH:mm:ss";

        public YamlProvider()
        {
        }

        public string Extention => "yaml";
        public void Dispose(Stream stream)
        {
            stream.Flush();
            stream.Close();
        }
        public void Init(Stream stream)
        {
        }


        private static void WriteIndent(StringBuilder builder, int indent)
        {
            for (int i = 0; i < indent; i++) builder.Append(indentString);
        }

        private static string WriteLiteralString(string input)
        {
            return input.Replace("\r", "\\r").Replace("\n", "\\n").Replace("\"", "\\\"");
        }

        private static void WriteObject(StringBuilder builder, object obj, string name, int indent)
        {
            WriteIndent(builder, indent);
            if (!string.IsNullOrEmpty(name))
            {
                builder.Append($"{name}: ");
            }
            else
            {
                builder.Append("value: ");
            }
            var type = obj?.GetType();
            if (obj == null)
                builder.AppendLine("null");
            else if (type.IsPrimitive)
                builder.AppendLine(obj.ToString());
            else if (obj is string)
                builder.Append("\"").Append(WriteLiteralString((string)obj)).AppendLine("\"");
            else if (obj is DateTime)
                builder.AppendLine(((DateTime)obj).ToString(timeFormat));
            else if (obj is byte[])
                builder.AppendLine(WriteLiteralString(Convert.ToBase64String((byte[])obj)));
            else if (obj is IEnumerable)
            {
                builder.AppendLine();
                WriteArray(builder, (IEnumerable)obj, indent);
            }
            else
            {
                builder.AppendLine();
                foreach (var p in type.GetProperties())
                {
                    WriteObject(builder, p.GetValue(obj), p.Name, indent + 1);
                }
            }
        }

        private static void WriteArray(StringBuilder builder, IEnumerable obj, int indent)
        {
            foreach (var item in obj)
            {
                WriteIndent(builder, indent);
                builder.AppendLine($"-{indentString}type: {item?.GetType()?.Name ?? "null"}");
                WriteObject(builder, item, null, indent + 1);
            }
        }

        public byte[] GetByte(object obj)
        {
            return GetBytes(obj);
        }

        public static byte[] GetBytes(object obj)
        {
            StringBuilder builder = new StringBuilder($@"
- log:
    CreateOn: {DateTime.UtcNow.ToString(timeFormat)}
");
            WriteObject(builder, obj, "Data", 1);
            return Encoding.UTF8.GetBytes(builder.ToString());
        }
    }
}
