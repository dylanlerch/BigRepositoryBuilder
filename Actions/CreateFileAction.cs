namespace BigRepositoryBuilder.Actions
{
    public class CreateFileAction : IRepositoryBuilderAction
    {
        private readonly Random random;
        private readonly int sizeInKb;
        private readonly string outputPath;

        public CreateFileAction(Random random, int sizeInKb, string outputPath)
        {
            this.random = random;
            this.sizeInKb = sizeInKb;
            this.outputPath = outputPath;
        }

        public async Task Execute()
        {
            var fileName = $"{random.GenerateName()}-generated.bin";
            var fileContents = GenerateFile(sizeInKb);

            var filePath = Path.Join(outputPath, fileName);
            await File.WriteAllBytesAsync(filePath, fileContents.ToArray());
        }

        byte[] GenerateFile(int sizeInKb)
        {
            var fileBytes = new List<byte>();
            var byteBuffer = new byte[1024];

            for (int totalKiloBytes = 0; totalKiloBytes < sizeInKb; totalKiloBytes++)
            {
                random.NextBytes(byteBuffer);
                fileBytes.AddRange(byteBuffer);
            }

            return fileBytes.ToArray();
        }
    }
}