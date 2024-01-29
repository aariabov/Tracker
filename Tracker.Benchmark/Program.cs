using AutoFixture;
using AutoFixture.AutoMoq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.InProcess.NoEmit;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Tracker.Instructions;
using Tracker.Instructions.Db;
using Tracker.Instructions.Interfaces;
using Tracker.Instructions.Repositories;

namespace Tracker.Benchmark;

public static class Program
{
    public static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run<ReCalcInstructionStatusesBenchmark>();

        // для дебага
        //BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, new FastAndDirtyConfig());

        // пример ручного измерения
        // var example = new ConcatStringExample();
        // example.Run();
    }
}

public class FastAndDirtyConfig : ManualConfig
{
    public FastAndDirtyConfig()
    {
        AddJob(new Job("FastAndDirtyJob")
        {
            Run =
                {
                    RunStrategy = RunStrategy.ColdStart,
                    LaunchCount = 1,
                    WarmupCount = 0,
                    IterationCount = 1
                }
        }
            .WithToolchain(InProcessNoEmitToolchain.Instance));
    }
}

[Config(typeof(FastAndDirtyConfig))]
[MemoryDiagnoser]
//[SimpleJob(RunStrategy.ColdStart, launchCount: 1, warmupCount: 0, iterationCount: 1, id: "FastAndDirtyJob")]
public class ReCalcInstructionStatusesBenchmark
{
    private ReCalcStatusService _reCalcStatusService;

    [GlobalSetup]
    public void Setup()
    {
        // самый понятный вариант - создать все руками, но может быть много зависимостей
        // var optionsBuilder = new DbContextOptionsBuilder<InstructionsDbContext>();
        // optionsBuilder.UseNpgsql(connectionString).UseSnakeCaseNamingConvention();
        // var db = new InstructionsDbContext(optionsBuilder.Options);
        // var repo = new InstructionsRepository(db, new InstructionsTreeRepositoryClosure(db));
        // reCalcStatusStatus = new ReCalcStatusStatus(repo, new InstructionStatusService());

        // можно так, но также может быть много зависимостей и можно что-нибудь нужное забыть
        // var fixture = new Fixture().Customize(new AutoMoqCustomization());
        // fixture.Inject(db);
        // fixture.Inject(repo);
        // reCalcStatusStatus = fixture.Create<ReCalcStatusStatus>();

        // самый правильный вариант - взять DI самого приложения, при этом не запускать его
        IConfiguration config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.Benchmark.json")
            .Build();
        var startUp = new Startup(config);
        var host = Host.CreateDefaultBuilder()
            .ConfigureServices(startUp.ConfigureServices)
            .Build();

        // чтобы полностью запустить веб приложение
        // var host = Host.CreateDefaultBuilder()
        //     .ConfigureAppConfiguration(builder =>
        //     {
        //         builder.Sources.Clear();
        //         builder.AddConfiguration(config);
        //     })
        //     .ConfigureWebHostDefaults(webBuilder =>
        //     {
        //         webBuilder.UseStartup<Startup>();
        //     })
        //     .Build();
        // host.Run();

        _reCalcStatusService = host.Services.GetService<ReCalcStatusService>();
    }

    [Benchmark]
    public async Task RecalculateStatusesSavePerIteration()
    {
        await _reCalcStatusService.RecalculateStatusesSavePerIteration();
    }

    [Benchmark]
    public async Task RecalculateStatusesSingleSave()
    {
        await _reCalcStatusService.RecalculateStatusesSingleSave();
    }

    [Benchmark]
    public async Task RecalculateStatusesForRoot()
    {
        await _reCalcStatusService.RecalculateStatusesForRoot();
    }

    [Benchmark]
    public async Task RecalculateStatusesForRootInWork()
    {
        await _reCalcStatusService.RecalculateStatusesForRootInWork();
    }

    [Benchmark]
    public async Task RecalculateStatusesForInWorkAndDeadlineLessNow()
    {
        await _reCalcStatusService.RecalculateStatusesForRootInWorkAndDeadlineLessNow();
    }
}
