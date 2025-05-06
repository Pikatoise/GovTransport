using System.Text;

namespace GovTransportSDK.Helpers
{
    internal static class GovNumberHelper
    {
        internal static string GenerateGovNumber(string regionCode)
        {
            char[] numbers = new char[10] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            char[] letters = new char[12] { 'A', 'B', 'C', 'E', 'H', 'K', 'M', 'O', 'P', 'T', 'X', 'Y' };

            Random rnd = new Random();
            StringBuilder govNumber = new StringBuilder();

            govNumber.Append(letters[rnd.Next(letters.Length)]);

            govNumber.Append(numbers[rnd.Next(numbers.Length)]);
            govNumber.Append(numbers[rnd.Next(numbers.Length)]);
            govNumber.Append(numbers[rnd.Next(numbers.Length)]);

            govNumber.Append(letters[rnd.Next(letters.Length)]);
            govNumber.Append(letters[rnd.Next(letters.Length)]);

            govNumber.Append(regionCode);

            return govNumber.ToString();
        }
    }
}
