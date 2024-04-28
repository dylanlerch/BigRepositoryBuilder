namespace BigRepositoryBuilder
{
    public class RepositoryConfiguration
    {
        public int Seed { get; set; } = new Random().Next();
        public int Commits { get; set; }
        public int Branches { get; set; }
        public int Tags { get; set; }
        public List<RepositoryConfigurationFile> Files { get; set; } = new();
    }

    public class RepositoryConfigurationFile
    {
        public int Size { get; set; }
        public int Count { get; set; }
        public bool Binary { get; set; }
    }
}
