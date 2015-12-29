using System;
using System.Text;
using JsonPrettyPrinterPlus.JsonPrettyPrinterInternals;
using JsonPrettyPrinterPlus.JsonPrettyPrinterInternals.JsonPPStrategies;

namespace JsonPrettyPrinterPlus
{
    public static class PrettyPrinterExtensions
    {
        public static string PrettyPrintJson(this string unprettyJson)
        {
            var pp = new JsonPrettyPrinter(new JsonPPStrategyContext());

            return pp.PrettyPrint(unprettyJson);
        }
    }

    public class JsonPrettyPrinter
    {
        private readonly JsonPPStrategyContext _context;

        public JsonPrettyPrinter(JsonPPStrategyContext context)
        {
            _context = context;

            _context.ClearStrategies();
            _context.AddCharacterStrategy(new OpenBracketStrategy());
            _context.AddCharacterStrategy(new CloseBracketStrategy());
            _context.AddCharacterStrategy(new OpenSquareBracketStrategy());
            _context.AddCharacterStrategy(new CloseSquareBracketStrategy());
            _context.AddCharacterStrategy(new SingleQuoteStrategy());
            _context.AddCharacterStrategy(new DoubleQuoteStrategy());
            _context.AddCharacterStrategy(new CommaStrategy());
            _context.AddCharacterStrategy(new ColonCharacterStrategy());
            _context.AddCharacterStrategy(new SkipWhileNotInStringStrategy('\n'));
            _context.AddCharacterStrategy(new SkipWhileNotInStringStrategy('\r'));
            _context.AddCharacterStrategy(new SkipWhileNotInStringStrategy('\t'));
            _context.AddCharacterStrategy(new SkipWhileNotInStringStrategy(' '));
        }

        public string PrettyPrint(string inputString)
        {
            if (inputString.Trim() == String.Empty)
                return "";

            var input = new StringBuilder(inputString);
            var output = new StringBuilder();

            PrettyPrintCharacter(input, output);

            return output.ToString();
        }

        private void PrettyPrintCharacter(StringBuilder input, StringBuilder output)
        {
            for (var i = 0; i < input.Length; i++)
                _context.PrettyPrintCharacter(input[i], output);
        }
    }
}