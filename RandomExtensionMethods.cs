namespace BigRepositoryBuilder
{
    public static class RandomExtensionMethods
    {
        public static string GenerateName(this Random random)
        {
            var stringCharacters = new char[40];
            var availableCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

            for (int i = 0; i < stringCharacters.Length; i++)
            {
                stringCharacters[i] = availableCharacters[random.Next(availableCharacters.Length)];
            }

            return new string(stringCharacters);
        }

        public static string GeneratePath(this Random random)
        {
            var availableCharacters = "0123456789abcdef";
            var pathDepth = random.Next(2, 4);
            var path = new List<string>();

            for (int i = 0; i < pathDepth; i++)
            {
                var folderName = availableCharacters[random.Next(availableCharacters.Length)];
                path.Add(folderName.ToString());
            }

            return string.Join('/', path);
        }
    }
}