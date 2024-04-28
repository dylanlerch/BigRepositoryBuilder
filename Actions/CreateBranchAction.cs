using LibGit2Sharp;

namespace BigRepositoryBuilder.Actions
{
    public class CreateBranchAction : IRepositoryBuilderAction
    {
        private readonly Random random;
        private readonly Repository repository;

        public CreateBranchAction(Random random, Repository repository)
        {
            this.random = random;
            this.repository = repository;
        }

        public void Execute()
        {
            var branchName = $"{random.GenerateName()}-branch";
            var commit = repository.Head.Tip;
            repository.Branches.Add(branchName, commit);
        }
    }
}