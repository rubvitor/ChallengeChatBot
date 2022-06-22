using System.Reflection;

namespace Challenge.ChatBot.Util
{
    public static class CSVExtension
    {
        public static List<T> Transform<T>(this string input)
        {
            return input.Split(Environment.NewLine,
                            StringSplitOptions.RemoveEmptyEntries)
                                          .Skip(1)
                                          .Select(v => FromCsv<T>(v))
                                          .ToList();
        }

        private static T FromCsv<T>(string csvLine)
        {
            string[] values = csvLine.Split(',');

            var properties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                .Where(ø => ø.CanRead && ø.CanWrite)
                                .Where(ø => ø.PropertyType == typeof(string))?.ToList();

            T objectParsed = Activator.CreateInstance<T>();

            var findMin = new int?[] { values.Length, properties?.Count }.Min();

            if (!findMin.HasValue)
                return objectParsed;

            for (int i = 0; i < findMin.Value; i++)
                properties[i].SetValue(objectParsed, values[i]);

            return objectParsed;
        }
    }
}
