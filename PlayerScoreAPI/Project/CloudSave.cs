using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Unity.Services.CloudCode.Apis;
using Unity.Services.CloudCode.Core;
using Unity.Services.CloudCode.Shared;
using Unity.Services.CloudSave.Model;
using System.Threading;
using System.Dynamic;

namespace PlayerScoreApiModule
{
    public class PlayerData
    {
        public int Score { get; set; }
        public int LastUsedCharacterIndex { get; set; }
    }
    public class CloudSave
    {
        private readonly ILogger<CloudSave> _logger;

        private const string ScoreKey = "playerScore";
        private const string CharacterKey = "lastUsedCharacterIndex";

        public CloudSave(ILogger<CloudSave> logger)
        {
            _logger = logger;
        }

        [CloudCodeFunction("PUT_PlayerData")]
        public async Task PutPlayerData(IExecutionContext context, IGameApiClient gameApiClient, int score, int characterIndex)
        {
            _logger.LogInformation("PUT_PlayerData called for player {PlayerId}", context.PlayerId);
            try
            {
                var itemsToSave = new List<SetItemBody>
                {
                    new SetItemBody(ScoreKey, score),
                    new SetItemBody(CharacterKey, characterIndex)
                };

                await gameApiClient.CloudSaveData.SetItemBatchAsync(
                    context,
                    context.AccessToken,
                    context.ProjectId,
                    context.PlayerId,
                    new SetItemBatchBody(itemsToSave)
                );
            }
            catch (ApiException ex)
            {
                _logger.LogError(ex, "Failed to PUT player data for player {PlayerId}", context.PlayerId);
                throw;
            }
        }

        [CloudCodeFunction("GET_PlayerData")]
        public async Task<PlayerData> GetPlayerData(IExecutionContext context, IGameApiClient gameApiClient)
        {
            _logger.LogInformation("GET_PlayerData called for player {PlayerId}", context.PlayerId);
            try
            {
                // Request both keys at the same time
                var keysToGet = new List<string> { ScoreKey, CharacterKey };

                var result = await gameApiClient.CloudSaveData.GetItemsAsync(
                    context,
                    context.AccessToken,
                    context.ProjectId,
                    context.PlayerId,
                    keysToGet
                );

                var data = new PlayerData
                {
                    Score = 0, // Default value
                    LastUsedCharacterIndex = 0 // Default value
                };

                // Find and parse the score
                var scoreItem = result.Data.Results.FirstOrDefault(item => item.Key == ScoreKey);
                if (scoreItem?.Value != null && (JsonElement)scoreItem.Value is var scoreValue && scoreValue.ValueKind == JsonValueKind.Number)
                {
                    data.Score = scoreValue.GetInt32();
                }

                // Find and parse the character index
                var charItem = result.Data.Results.FirstOrDefault(item => item.Key == CharacterKey);
                if (charItem?.Value != null && (JsonElement)charItem.Value is var charValue && charValue.ValueKind == JsonValueKind.Number)
                {
                    data.LastUsedCharacterIndex = charValue.GetInt32();
                }

                return data;
            }
            catch (ApiException ex)
            {
                _logger.LogError(ex, "Failed to GET player data for player {PlayerId}", context.PlayerId);
                throw;
            }
        }

        [CloudCodeFunction("DELETE_PlayerData")]
        public async Task DeletePlayerData(IExecutionContext context, IGameApiClient gameApiClient)
        {
            _logger.LogInformation("DELETE_PlayerData called for player {PlayerId}", context.PlayerId);
            try
            {
                // A list of all keys to delete
                var keysToDelete = new List<string> { ScoreKey, CharacterKey };

                foreach (var key in keysToDelete)
                {

                    await gameApiClient.CloudSaveData.DeleteItemAsync(
                        context,
                        context.AccessToken,
                        context.ProjectId,
                        context.PlayerId,
                        key
                    );
                }
            }
            catch (ApiException ex)
            {
                _logger.LogError(ex, "Failed to DELETE player data for player {PlayerId}", context.PlayerId);
                throw;
            }
        }
    }
}