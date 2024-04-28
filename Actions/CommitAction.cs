using LibGit2Sharp;

namespace BigRepositoryBuilder.Actions
{
    public class CommitAction : IRepositoryBuilderAction
    {
        private readonly Repository repository;
        private readonly List<(bool Binary, string Name, byte[] Content)> fileBuffer;

        public CommitAction(Repository repository, List<(bool Binary, string Name, byte[] Content)> fileBuffer)
        {
            this.repository = repository;
            this.fileBuffer = fileBuffer;
        }

        public void Execute()
        {
            var branch = repository.Branches[repository.Head.CanonicalName];
            var parents = branch?.Tip is null ? Array.Empty<Commit>() : new[] { branch.Tip };

            var treeDefinition = branch?.Tip is null ? new TreeDefinition() : TreeDefinition.From(branch.Tip.Tree);

            foreach (var file in fileBuffer)
            {
                var blob = repository.ObjectDatabase.Write<Blob>(file.Content);
                treeDefinition.Add(file.Name, blob, file.Binary ? Mode.ExecutableFile : Mode.NonExecutableFile);
            }

            var tree = repository.ObjectDatabase.CreateTree(treeDefinition);

            var signature = new Signature("Big Repository Builder", "test@test.com", DateTimeOffset.UtcNow);
            var commit = repository.ObjectDatabase.CreateCommit(signature, signature, "Committed by Big Repository Builder", tree, parents, false);

            if (branch is null)
            {
                repository.Branches.Add(repository.Head.FriendlyName, commit);
            }
            else
            {
                repository.Refs.UpdateTarget(branch.Reference, commit.Sha);
            }

            fileBuffer.Clear();
        }
    }
}