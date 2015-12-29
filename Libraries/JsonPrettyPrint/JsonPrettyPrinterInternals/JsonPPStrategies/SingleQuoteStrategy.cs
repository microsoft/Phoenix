namespace JsonPrettyPrinterPlus.JsonPrettyPrinterInternals.JsonPPStrategies
{
    public class SingleQuoteStrategy : ICharacterStrategy
    {
        public void ExecutePrintyPrint(JsonPPStrategyContext context)
        {
            if (!context.IsProcessingDoubleQuoteInitiatedString && !context.WasLastCharacterABackSlash)
                context.IsProcessingSingleQuoteInitiatedString = !context.IsProcessingSingleQuoteInitiatedString;

            context.AppendCurrentChar();
        }

        public char ForWhichCharacter
        {
            get { return '\''; }
        }
    }
}