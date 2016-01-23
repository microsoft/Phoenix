namespace CMP.Setup.Helpers
{
    class SetupValidationHelpers
    {
        /// <summary>
        /// Determines whether [is valid path for install] [the specified path to validate].
        /// </summary>
        /// <param name="pathToValidate">The path to validate.</param>
        /// <returns>
        ///     <c>true</c> if [is valid path for install] [the specified path to validate]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsValidPathForInstall(string pathToValidate)
        {
            char[] invalidCharacterArray = new char[] { ';', ',', '~', '=', '{', '}', '%' }; // Invalid characters

            return ValidateStringCharacters(pathToValidate, invalidCharacterArray);
        }

        /// <summary>
        /// Validates the path characters.
        /// </summary>
        /// <param name="valueToValidate">The value to validate.</param>
        /// <param name="invalidCharacters">The invalid characters.</param>
        /// <returns>true if valid, false otherwise</returns>
        public static bool ValidateStringCharacters(string valueToValidate, char[] invalidCharacters)
        {
            if (0 <= valueToValidate.IndexOfAny(invalidCharacters))
            {
                return false;
            }

            return true;
        }
    }
}
