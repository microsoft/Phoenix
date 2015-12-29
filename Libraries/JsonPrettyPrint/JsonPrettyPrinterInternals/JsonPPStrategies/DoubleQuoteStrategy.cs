namespace JsonPrettyPrinterPlus.JsonPrettyPrinterInternals.JsonPPStrategies
{
    public class DoubleQuoteStrategy : ICharacterStrategy
    {
        public void ExecutePrintyPrint(JsonPPStrategyContext context)
        {
            if (!context.IsProcessingSingleQuoteInitiatedString && !context.WasLastCharacterABackSlash)
                context.IsProcessingDoubleQuoteInitiatedString = !context.IsProcessingDoubleQuoteInitiatedString;

            context.AppendCurrentChar();
        }

        public char ForWhichCharacter
        {
            get { return '"'; }
        }
    }
}