using System;

namespace JsonPrettyPrinterPlus.JsonPrettyPrinterInternals.JsonPPStrategies
{
    public class DefaultCharacterStrategy : ICharacterStrategy
    {
        public void ExecutePrintyPrint(JsonPPStrategyContext context)
        {
            context.AppendCurrentChar();
        }

        public char ForWhichCharacter
        {
            get
            {
                throw new InvalidOperationException(
                    "This strategy was not intended for any particular character, so it has no one character");
            }
        }
    }
}