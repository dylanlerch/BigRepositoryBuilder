using System.Text;

namespace BigRepositoryBuilder.Actions
{
    public class CreateFileAction : IRepositoryBuilderAction
    {
        static readonly byte[] characters = "|(!\"#$%&'()*+,-./0123456789:;<=>?C0ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_|0abcdefghijklmnopqrstuvwxyz{|}~(".Select(c => Encoding.ASCII.GetBytes(new[] { c }).Single()).ToArray();
        static readonly byte[] newline = Encoding.ASCII.GetBytes(Environment.NewLine);
        private readonly Random random;
        private readonly int sizeInKb;
        private readonly List<(bool Binary, string Name, byte[] Content)> fileBuffer;

        public CreateFileAction(Random random, int sizeInKb, List<(bool Binary, string Name, byte[] Content)> fileBuffer)
        {
            this.random = random;
            this.sizeInKb = sizeInKb;
            this.fileBuffer = fileBuffer;
        }
        public void Execute()
        {
            var fileName = $"{random.GeneratePath()}/{random.GenerateName()}-generated.txt";
            var fileContents = GenerateFile(sizeInKb);

            fileBuffer.Add((false, fileName, fileContents));
        }

        byte[] GenerateFile(int sizeInKb)
        {
            var sizeInBytes = sizeInKb * 1024;
            var fileBytes = new List<byte>();

            int totalBytes = 0;

            while (totalBytes < sizeInBytes)
            {
                // 1 in 20 chance of creating a new line
                var shouldCreateNewLine = random.Next(20) == 0;

                if (shouldCreateNewLine)
                {
                    fileBytes.AddRange(newline);
                    totalBytes += newline.Length;
                }
                else
                {
                    var characterByte = characters[random.Next(characters.Length)];
                    fileBytes.Add(characterByte);
                    totalBytes++;
                }
            }

            return fileBytes.ToArray();
        }
    }
}