using System.Collections.Generic;

namespace JsonPrettyPrinterPlus.JsonPrettyPrinterInternals
{
    public class PPScopeState
    {
        public enum JsonScope
        {
            Object,
            Array
        }

        private readonly Stack<JsonScope> _jsonScopeStack = new Stack<JsonScope>();

        public bool IsTopTypeArray
        {
            get { return _jsonScopeStack.Count > 0 && _jsonScopeStack.Peek() == JsonScope.Array; }
        }

        public int ScopeDepth
        {
            get { return _jsonScopeStack.Count; }
        }

        public void PushObjectContextOntoStack()
        {
            _jsonScopeStack.Push(JsonScope.Object);
        }

        public JsonScope PopJsonType()
        {
            return _jsonScopeStack.Pop();
        }

        public void PushJsonArrayType()
        {
            _jsonScopeStack.Push(JsonScope.Array);
        }
    }
}