namespace JsonPrettyPrinterPlus.JsonPrettyPrinterInternals.JsonPPStrategies
{
    public class CloseSquareBracketStrategy : ICharacterStrategy
    {
        public void ExecutePrintyPrint(JsonPPStrategyContext context)
        {
            if (context.IsProcessingString)
            {
                context.AppendCurrentChar();
                return;
            }

            context.CloseCurrentScope();
            context.BuildContextIndents();
            context.AppendCurrentChar();
        }

        public char ForWhichCharacter
        {
            get { return ']'; }
        }
    }
}