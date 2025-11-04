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

namespace PlayerScoreApiModule
{
    public class PlayerData
    {
        public int Score { get; set; }
        public int LastUsedCharacterIndex { get; set; }
        public string lastUsedCharacterName { get; set; } = "Rangga";
    }
    public class CloudSave
    {
        private readonly ILogger<CloudSave> _logger;

        private const string ScoreKey = "playerScore";
        private const string CharacterKey = "lastUsedCharacterIndex";
        private const string CharacterNameKey = "lastUsedCharacter";

        public CloudSave(ILogger<CloudSave> logger)
        {
            _logger = logger;
        }

        [CloudCodeFunction("PUT_PlayerScore")]
        public async Task PutPlayerScore(IExecutionContext context, IGameApiClient gameApiClient, int score)
        {
            _logger.LogInformation("PUT_PlayerScore called for player {PlayerId}", context.PlayerId);
            try
            {
                var itemToSave = new SetItemBody(ScoreKey, score);

                await gameApiClient.CloudSaveData.SetItemBatchAsync(
                    context,
                    context.AccessToken,
                    context.ProjectId,
                    context.PlayerId,
                    new SetItemBatchBody(new List<SetItemBody> { itemToSave })
                );
            }
            catch (ApiException ex)
            {
                _logger.LogError(ex, "Failed to PUT player score for player {PlayerId}", context.PlayerId);
                throw;
            }
        }

        [CloudCodeFunction("PUT_PlayerCharacter")]
        public async Task PutPlayerCharacter(IExecutionContext context, IGameApiClient gameApiClient, int characterIndex)
        {
            _logger.LogInformation("PUT_PlayerCharacter called for player {PlayerId}", context.PlayerId);
            try
            {
                var itemToSave = new SetItemBody(CharacterKey, characterIndex);

                await gameApiClient.CloudSaveData.SetItemBatchAsync(
                    context,
                    context.AccessToken,
                    context.ProjectId,
                    context.PlayerId,
                    new SetItemBatchBody(new List<SetItemBody> { itemToSave })
                );
            }
            catch (ApiException ex)
            {
                _logger.LogError(ex, "Failed to PUT player character for player {PlayerId}", context.PlayerId);
                throw;
            }
        }

        [CloudCodeFunction("PUT_Character")]
        public async Task PutPlayerCharacter(IExecutionContext context, IGameApiClient gameApiClient, string characterName)
        {
            _logger.LogInformation("PUT_Character called for player {PlayerId}", context.PlayerId);
            try
            {
                var itemToSave = new SetItemBody(CharacterNameKey, characterName);

                await gameApiClient.CloudSaveData.SetItemBatchAsync(
                    context,
                    context.AccessToken,
                    context.ProjectId,
                    context.PlayerId,
                    new SetItemBatchBody(new List<SetItemBody> { itemToSave}) 
                    );
            }
            catch(ApiException ex)
            {
                _logger.LogError(ex, "Failed to PUT player character for player {PlayerId}", context.PlayerId);
                throw;
            }
        }

        [CloudCodeFunction("PUT_PlayerData")]
        public async Task PutPlayerData(IExecutionContext context, IGameApiClient gameApiClient, int score, int characterIndex, string characterName)
        {
            _logger.LogInformation("PUT_PlayerData called for player {PlayerId}", context.PlayerId);
            try
            {
                var itemsToSave = new List<SetItemBody>
                {
                    new SetItemBody(ScoreKey, score),
                    new SetItemBody(CharacterKey, characterIndex),
                    new SetItemBody(CharacterNameKey, characterName)
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
                var keysToGet = new List<string> { ScoreKey, CharacterKey, CharacterNameKey };

                var result = await gameApiClient.CloudSaveData.GetItemsAsync(
                    context,
                    context.AccessToken,
                    context.ProjectId,
                    context.PlayerId,
                    keysToGet
                );

                //Default Value
                var data = new PlayerData
                {
                    Score = 0,
                    LastUsedCharacterIndex = 0,
                    lastUsedCharacterName = "Rangga"
                };

                foreach (var item in result.Data.Results)
                {
                    try
                    {
                        switch (item.Key)
                        {
                            case ScoreKey:
                                if (item.Value != null)
                                {
                                    // Convert to string first then parse to avoid casting issues
                                    var scoreStr = item.Value.ToString();
                                    if (int.TryParse(scoreStr, out int scoreValue))
                                    {
                                        data.Score = scoreValue;
                                    }
                                }
                                break;

                            case CharacterKey:
                                if (item.Value != null)
                                {
                                    var charIndexStr = item.Value.ToString();
                                    if (int.TryParse(charIndexStr, out int charValue))
                                    {
                                        data.LastUsedCharacterIndex = charValue;
                                    }
                                }
                                break;

                            case CharacterNameKey:
                                if (item.Value != null)
                                {
                                    data.lastUsedCharacterName = item.Value.ToString();
                                }
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Error parsing value for key {Key}", item.Key);
                        // Continue with next item if one fails
                    }
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