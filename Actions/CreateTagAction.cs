using LibGit2Sharp;

namespace BigRepositoryBuilder.Actions
{
    public class CreateTagAction : IRepositoryBuilderAction
    {
        private readonly Random random;
        private readonly Repository repository;

        public CreateTagAction(Random random, Repository repository)
        {
            this.random = random;
            this.repository = repository;
        }

        public Task Execute()
        {
            var tagName = $"{random.GenerateName()}-tag";
            var commit = repository.Head.Tip;
            repository.Tags.Add(tagName, commit);

            return Task.CompletedTask;
        }
    }
}