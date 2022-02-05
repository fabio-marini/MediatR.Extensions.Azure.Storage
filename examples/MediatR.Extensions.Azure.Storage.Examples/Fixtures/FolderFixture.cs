using FluentAssertions;
using Polly;
using System;
using System.IO;
using System.Linq;
using Xunit.Abstractions;

namespace MediatR.Extensions.Azure.Storage.Examples
{
    public class FolderFixture
    {
        private readonly DirectoryInfo dir;
        private readonly ITestOutputHelper log;

        public FolderFixture(DirectoryInfo dir, ITestOutputHelper log)
        {
            this.dir = dir;
            this.log = log;
        }

        public void GivenFolderIsEmpty()
        {
            var allFiles = dir.GetFiles();

            if (allFiles.Any() == false)
            {
                return;
            }

            foreach (var f in allFiles)
            {
                File.Delete(f.FullName);
            }
        }

        public void ThenFolderHasFiles(int expectedCount)
        {
            var retryPolicy = Policy
                .HandleResult<int>(res => res != expectedCount)
                .WaitAndRetry(5, i => TimeSpan.FromSeconds(1));

            var actualCount = retryPolicy.Execute(() =>
            {
                var res = dir.GetFiles().Count();

                log.WriteLine($"Folder {dir.Name} has {res} files");

                return res;
            });

            actualCount.Should().Be(expectedCount);
        }

        public void ThenContainerIsEmpty() => ThenFolderHasFiles(0);

    }
}
