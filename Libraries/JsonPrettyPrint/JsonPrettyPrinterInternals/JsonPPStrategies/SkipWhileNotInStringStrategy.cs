namespace JsonPrettyPrinterPlus.JsonPrettyPrinterInternals.JsonPPStrategies
{
    public class SkipWhileNotInStringStrategy : ICharacterStrategy
    {
        private readonly char _selectionCharacter;

        public SkipWhileNotInStringStrategy(char selectionCharacter)
        {
            _selectionCharacter = selectionCharacter;
        }

        public void ExecutePrintyPrint(JsonPPStrategyContext context)
        {
            if (context.IsProcessingString)
                context.AppendCurrentChar();
        }

        public char ForWhichCharacter
        {
            get { return _selectionCharacter; }
        }
    }
}