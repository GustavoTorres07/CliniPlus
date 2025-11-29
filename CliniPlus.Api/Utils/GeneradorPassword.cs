using System.Text;

namespace CliniPlus.Api.Utils
{
    public static class GeneradorPassword
    {
        private static readonly Random _random = new Random();

        private static int RandomNumber(int min, int max)
            => _random.Next(min, max);

        private static string RandomString(int size, bool lowerCase)
        {
            var builder = new StringBuilder();
            for (int i = 0; i < size; i++)
            {
                // Letras A-Z
                char ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * _random.NextDouble() + 65)));
                builder.Append(ch);
            }
            var result = builder.ToString();
            return lowerCase ? result.ToLower() : result;
        }

        // Password tipo: abc123 / xyz456
        public static string RandomPassword(int size = 0)
        {
            var builder = new StringBuilder();
            builder.Append(RandomString(3, true));      // letras minúsculas
            builder.Append(RandomNumber(100, 999));     // 3 dígitos
            return builder.ToString();
        }
    }
}
