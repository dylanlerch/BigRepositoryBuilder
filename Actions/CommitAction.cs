using LibGit2Sharp;

namespace BigRepositoryBuilder.Actions
{
    public class CommitAction : IRepositoryBuilderAction
    {
        private readonly Repository repository;

        public CommitAction(Repository repository)
        {
            this.repository = repository;
        }

        public Task Execute()
        {
            Commands.Stage(repository, "*");
            var signature = new Signature("Big Repository Builder", "test@test.com", DateTimeOffset.UtcNow);

            repository.Commit("Committed by Big Repository Builder", signature, signature, new CommitOptions { AllowEmptyCommit = true });

            return Task.CompletedTask;
        }
    }
}