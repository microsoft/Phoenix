using System.Web.Script.Serialization;

namespace JsonPrettyPrinterPlus.JsonSerialization
{
    public static class JsonExtensions
    {
        public static string ToJSON(this object graph)
        {
            return graph.ToJSON(false);
        }

        public static string ToJSON(this object graph, bool prettyPrint)
        {
            var unprettyJson = new JavaScriptSerializer().Serialize(graph);

            if (!prettyPrint)
                return unprettyJson;

            return unprettyJson.PrettyPrintJson();
        }

        public static T DeserializeFromJson<T>(this string json)
        {
            return new JavaScriptSerializer().Deserialize<T>(json);
        }
    }
}