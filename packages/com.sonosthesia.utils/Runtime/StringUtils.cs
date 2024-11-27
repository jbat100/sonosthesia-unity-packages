using System.Text;

namespace Sonosthesia.Utils
{
    public static class StringUtils
    {
        public static string PropertyNameToLabel(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input; // Return the original input if it's null or empty
            }

            // Remove leading underscores
            int index = 0;
            while (index < input.Length && input[index] == '_')
            {
                index++;
            }

            // Handle empty string case after removing underscores
            if (index >= input.Length)
            {
                return string.Empty; // Return empty string if all characters are underscores
            }

            // Capitalize the first letter
            char firstChar = char.ToUpper(input[index]);
            StringBuilder result = new StringBuilder();
            result.Append(firstChar);

            // Iterate through the rest of the characters
            for (int i = index + 1; i < input.Length; i++)
            {
                char currentChar = input[i];

                // Insert a space if the current character is uppercase
                if (char.IsUpper(currentChar))
                {
                    result.Append(' '); // Insert a space before the uppercase letter
                }

                result.Append(currentChar);
            }

            return result.ToString();
        }
    }
}