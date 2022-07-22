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
    }
}