namespace JsonPrettyPrinterPlus.JsonPrettyPrinterInternals
{
    public interface ICharacterStrategy
    {
        char ForWhichCharacter { get; }
        void ExecutePrintyPrint(JsonPPStrategyContext context);
    }
}