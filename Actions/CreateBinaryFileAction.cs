namespace BigRepositoryBuilder.Actions
{
    public class CreateBinaryFileAction : IRepositoryBuilderAction
    {
        private readonly Random random;
        private readonly int sizeInKb;
        private readonly List<(bool Binary, string Name, byte[] Content)> fileBuffer;

        public CreateBinaryFileAction(Random random, int sizeInKb, List<(bool Binary, string Name, byte[] Content)> fileBuffer)
        {
            this.random = random;
            this.sizeInKb = sizeInKb;
            this.fileBuffer = fileBuffer;
        }

        public void Execute()
        {
            var fileName = $"{random.GeneratePath()}/{random.GenerateName()}-generated.bin";
            var fileContents = GenerateFile(sizeInKb);

            fileBuffer.Add((true, fileName, fileContents));
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