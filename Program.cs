using System.Text.Json;
using BigRepositoryBuilder.Actions;
using LibGit2Sharp;

namespace BigRepositoryBuilder
{
    internal class Program
    {
        static Random random;

        private static async Task Main(string[] args)
        {
            // Arguments and defaults
            var configurationFilePath = args[1];
            var outputPath = args.Length >= 3 ? args[2] : Path.Join(Environment.CurrentDirectory, "generated-repository", $"{Guid.NewGuid()}.git");
            Directory.CreateDirectory(outputPath);

            var configuration = await ReadConfiguration(configurationFilePath);
            WriteRepositorySummary(configuration);

            using var repository = CreateRepository(outputPath);
            random = new Random(configuration.Seed);

            var actions = new List<IRepositoryBuilderAction>();
            var fileBuffer = new List<(bool Binary, string Name, byte[] Content)>();

            actions.AddRange(GenerateCreateFileActions(configuration.Files, fileBuffer));
            Repeat(configuration.Commits, () => actions.Add(new CommitAction(repository, fileBuffer)));
            Repeat(configuration.Branches, () => actions.Add(new CreateBranchAction(random, repository)));
            Repeat(configuration.Tags, () => actions.Add(new CreateTagAction(random, repository)));

            Shuffle(actions);

            actions.Insert(0, new CommitAction(repository, fileBuffer)); // Always have one commit at the start in case an action needs it
            actions.Add(new CommitAction(repository, fileBuffer)); // Always commit at the end in case there are uncommited files

            int lastFullPercent = 0;
            for (int i = 0; i < actions.Count; i++)
            {
                int currentPercent = i * 100 / actions.Count;
                if (currentPercent > lastFullPercent)
                {
                    lastFullPercent = currentPercent;
                    Console.WriteLine($"{currentPercent}%");
                }

                var action = actions[i];
                action.Execute();
            }

            Console.WriteLine($"Done. Repository generated at {outputPath}");
            Console.WriteLine($"Add a remote and push it all with `git push <remote-name> --all && git push <remote-name> --tags`");
        }

        static Repository CreateRepository(string repositoryWorkingDirectory)
        {
            var repositoryPath = Repository.Init(repositoryWorkingDirectory, true);
            return new Repository(repositoryPath);
        }

        static void WriteRepositorySummary(RepositoryConfiguration configuration)
        {
            var totalSize = configuration.Files.Sum(f => f.Count * f.Size);
            var totalFiles = configuration.Files.Sum(f => f.Count);

            Console.WriteLine($"Generating ~{totalSize}KB (~{totalSize / 1024}MB) repository with {totalFiles} files, {configuration.Commits} commits, {configuration.Branches} branches, and {configuration.Tags} tags (seed: {configuration.Seed}).");
        }

        static List<IRepositoryBuilderAction> GenerateCreateFileActions(List<RepositoryConfigurationFile> fileConfigurations, List<(bool Binary, string Name, byte[] Content)> fileBuffer)
        {
            var createFileActions = new List<IRepositoryBuilderAction>();

            foreach (var fileConfiguration in fileConfigurations)
            {
                for (int i = 0; i < fileConfiguration.Count; i++)
                {
                    if (fileConfiguration.Binary)
                    {
                        createFileActions.Add(new CreateBinaryFileAction(random, fileConfiguration.Size, fileBuffer));
                    }
                    else
                    {
                        createFileActions.Add(new CreateFileAction(random, fileConfiguration.Size, fileBuffer));
                    }
                }
            }

            return createFileActions;
        }

        static void Repeat(int count, Action action)
        {
            for (int i = 0; i < count; i++)
            {
                action();
            }
        }

        static async Task<RepositoryConfiguration> ReadConfiguration(string configurationFilePath)
        {
            var configurationFileContents = await File.ReadAllTextAsync(configurationFilePath);

            var serializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var configuration = JsonSerializer.Deserialize<RepositoryConfiguration>(configurationFileContents, serializerOptions);

            return configuration;
        }

        static void Shuffle<T>(List<T> list)
        {
            // https://en.wikipedia.org/wiki/Fisher–Yates_shuffle
            for (int n = list.Count - 1; n > 0; n--)
            {
                int k = random.Next(n + 1);
                (list[n], list[k]) = (list[k], list[n]);
            }
        }
    }
}