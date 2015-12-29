namespace JsonPrettyPrinterPlus.JsonPrettyPrinterInternals.JsonPPStrategies
{
    public class CommaStrategy : ICharacterStrategy
    {
        public void ExecutePrintyPrint(JsonPPStrategyContext context)
        {
            context.AppendCurrentChar();

            if (context.IsProcessingString) return;

            context.BuildContextIndents();
            context.IsProcessingVariableAssignment = false;
        }

        public char ForWhichCharacter
        {
            get { return ','; }
        }
    }
}