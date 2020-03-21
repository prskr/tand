using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static _build.benchmarks.DotNetBenchmarkExtensions;

[GitHubActions(
        "dotnet",
        GitHubActionsImage.UbuntuLatest,
        AutoGenerate = false,
        On = new []{GitHubActionsTrigger.Push, GitHubActionsTrigger.PullRequest}
    )
]
[CheckBuildProjectConfigurations]
[UnsetVisualStudioEnvironmentVariables]
class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode
    public static int Main() => Execute<Build>(x => x.Pack);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution] readonly Solution Solution;
    [GitRepository] readonly GitRepository GitRepository;
    [GitVersion] readonly GitVersion GitVersion;

    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath TestsDirectory => RootDirectory / "test";
    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            TestsDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            EnsureCleanDirectory(ArtifactsDirectory);
        });

    Target Restore => _ => _
        .Executes(() =>
            DotNetRestore(s => s
                .SetProjectFile(Solution))
        );

    Target Compile => _ => _
        .DependsOn(Restore)
        .DependsOn(Clean)
        .Executes(() => DotNetBuild(s => s
            .SetProjectFile(Solution)
            .SetConfiguration(Configuration)
            .SetAssemblyVersion(GitVersion.AssemblySemVer)
            .SetFileVersion(GitVersion.AssemblySemFileVer)
            .SetInformationalVersion(GitVersion.InformationalVersion)
            .EnableNoRestore()));

    Target Test => _ => _
        .DependsOn(Compile)
        .Executes(() => DotNetTest(s => s
            .SetProjectFile(Solution)
            .SetConfiguration(Configuration)
            .EnableNoRestore()
            .EnableNoBuild()
            .EnableLogOutput()));

    Target Pack => _ => _
        .DependsOn(Test)
        .OnlyWhenStatic(() => GitRepository.IsOnMasterBranch())
        .Executes(() => SourceDirectory
            .GlobFiles("**/*.csproj")
            .ForEach(csproj => DotNetPack(s => s
                .SetProject(csproj)
                .SetConfiguration(Configuration)
                .SetOutputDirectory(ArtifactsDirectory)
                .SetVersion(GitVersion.FullSemVer)
                .EnableIncludeSource()
                .EnableIncludeSymbols()
                .EnableNoRestore()
                .EnableNoBuild()
                .EnableLogOutput()
            )));

    Target Benchmark => _ => _
        .DependsOn(Compile)
        .Requires(() => Configuration.Equals(Configuration.Release))
        .Executes(() => TestsDirectory
            .GlobFiles($"**/bin/{Configuration}/**/*Benchmark*.dll")
            .ForEach(benchmarkFile => DotNetBenchmark(
                s => s
                    .WithFilter("*")
                    .WithDllPath(benchmarkFile)))
        );
}