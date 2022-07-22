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
            var outputPath = args.Length >= 3 ? args[2] : Path.Join(Environment.CurrentDirectory, "generated-repository", Guid.NewGuid().ToString());
            Directory.CreateDirectory(outputPath);

            var configuration = await ReadConfiguration(configurationFilePath);
            using var repository = CreateRepository(outputPath);
            random = new Random(configuration.Seed);




            var actions = new List<IRepositoryBuilderAction>();

            actions.AddRange(GenerateCreateFileActions(configuration.Files, outputPath));
            Repeat(configuration.Commits, () => actions.Add(new CommitAction(repository)));
            Repeat(configuration.Branches, () => actions.Add(new CreateBranchAction(random, repository)));
            Repeat(configuration.Tags, () => actions.Add(new CreateTagAction(random, repository)));

            Shuffle(actions);

            actions.Insert(0, new CommitAction(repository)); // Always have one commit at the start in case an action needs it
            actions.Add(new CommitAction(repository)); // Always commit at the end in case there are uncommited files

            foreach (var action in actions)
            {
                await action.Execute();
            }

            Console.WriteLine($"Done. Repository genereated at {outputPath}");
            Console.WriteLine($"Add a remote and push it all with `git push <remote-name> --all && git push <remote-name> --tags`");
        }

        static Repository CreateRepository(string repositoryWorkingDirectory)
        {
            var repositoryPath = Repository.Init(repositoryWorkingDirectory);
            return new Repository(repositoryPath);
        }

        static List<CreateFileAction> GenerateCreateFileActions(List<RepositoryConfigurationFile> fileConfigurations, string outputPath)
        {
            var createFileActions = new List<CreateFileAction>();

            foreach (var fileConfiguration in fileConfigurations)
            {
                for (int i = 0; i < fileConfiguration.Count; i++)
                {
                    createFileActions.Add(new CreateFileAction(random, fileConfiguration.Size, outputPath));
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