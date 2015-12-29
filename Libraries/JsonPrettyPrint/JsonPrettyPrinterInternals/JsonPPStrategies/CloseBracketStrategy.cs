namespace JsonPrettyPrinterPlus.JsonPrettyPrinterInternals.JsonPPStrategies
{
    public class CloseBracketStrategy : ICharacterStrategy
    {
        public void ExecutePrintyPrint(JsonPPStrategyContext context)
        {
            if (context.IsProcessingString)
            {
                context.AppendCurrentChar();
                return;
            }

            PeformNonStringPrint(context);
        }

        public char ForWhichCharacter
        {
            get { return '}'; }
        }

        private static void PeformNonStringPrint(JsonPPStrategyContext context)
        {
            context.CloseCurrentScope();
            context.BuildContextIndents();
            context.AppendCurrentChar();
        }
    }
}