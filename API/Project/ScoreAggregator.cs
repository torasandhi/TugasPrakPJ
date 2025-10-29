#region Libraries
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Unity.Services.CloudCode.Apis;
using Unity.Services.CloudCode.Core;
using Unity.Services.CloudCode.Shared;
using Unity.Services.CloudSave.Api;
using Unity.Services.CloudSave.Model;
#endregion

public interface IScoreAggregator
{
    Task Increment(IExecutionContext ctx, long score);
}

public class ScoreAggregator : IScoreAggregator
{
    readonly Lockable<long> runningCount = new(0);
    readonly Lockable<DateTime> completedCount = new(DateTime.UtcNow);

    readonly ICloudSaveDataApi cloudSave;
    readonly ILogger<ScoreAggregator> logger;

    public ScoreAggregator(IGameApiClient gameApiClient, ILogger<ScoreAggregator> logger)
    {
        this.logger = logger;
        this.cloudSave = gameApiClient.CloudSaveData;
        logger.LogWarning("ScoreAggregator constructed");
    }

    public async Task Increment(IExecutionContext ctx, long score)
    {
        lock (runningCount)
        {
            runningCount.Value += score;
        }

        long scoreToAdd;
        lock (runningCount)
        {
            scoreToAdd = runningCount.Value;
            runningCount.Value = 0;
        }

        try
        {
            ApiResponse<GetItemsResponse> current = await cloudSave.GetCustomItemsAsync(
                ctx, ctx.ServiceToken, ctx.ProjectId,
                "global", new List<string> { "event_score" });

            Item item = current.Data.Results[0];
            long currentScore = long.TryParse(item.Value?.ToString(), out var val) ? val : 0;
            long newScore = currentScore + scoreToAdd;

            logger.LogInformation("Flushing score : { scoreToAdd}, new total: {newScore}", scoreToAdd, newScore);

            ApiResponse<SetItemResponse> result = await cloudSave.SetCustomItemAsync(
                ctx, ctx.ServiceToken, ctx.ProjectId, "global",
                new SetItemBody("event_score", newScore, item.WriteLock));

            if (result.StatusCode != HttpStatusCode.OK)
            {
                logger.LogWarning("Flush failed (status {status}), re adding score", result.StatusCode);
                lock (runningCount)
                {
                    runningCount.Value += scoreToAdd;
                }
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Flush failed - re-adding score");
            lock (runningCount)
            {
                runningCount.Value += scoreToAdd;
            }
        }

    }
}

class Lockable<T>
{
    public T Value;
    public Lockable(T value) { Value = value; }
}
