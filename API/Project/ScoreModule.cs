using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Unity.Services.CloudCode.Apis;
using Unity.Services.CloudCode.Core;
using Unity.Services.CloudSave.Model;

namespace CommunityGoal;

public class ScoreModule
{
    [CloudCodeFunction("AddScore")]
    public async Task AddScore(IExecutionContext ctx, IScoreAggregator scoreAggregator, int score)
    {
        await scoreAggregator.Increment(ctx, score);
    }

    [CloudCodeFunction("InitializeCloudSave")]
    public async Task InitializeCloudSave(IExecutionContext ctx, IGameApiClient apiClient)
    {
        await apiClient.CloudSaveData.SetCustomItemAsync(
        ctx, ctx.ServiceToken, ctx.ProjectId, "global",
        new SetItemBody("event_score", 0));
    }
}