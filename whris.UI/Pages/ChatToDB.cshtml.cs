using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using whris.Data.Data;
using whris.UI.Services;

namespace whris.UI.Pages
{
    public class SaveConversationRequest
    {
        public int? Id { get; set; }
        public List<ChatMessage> History { get; set; }
    }

    public class UpdateTitleRequest
    {
        public int Id { get; set; }
        public string NewTitle { get; set; }
    }

    public class SummarizeDataRequest
    {
        public string UserQuestion { get; set; }
        public List<object> TableData { get; set; }
    }

    public class ChatRequest
    {
        public string Question { get; set; }
        public List<ChatMessage> History { get; set; }
        public string LastSqlQuery { get; set; }
        public bool EnableInsights { get; set; }
    }

    public class GeocodingResult
    {
        public bool Success { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class QueryExecutionResult
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("chartData")]
        public List<object> ChartData { get; set; }

        [JsonPropertyName("tableData")]
        public List<object> TableData { get; set; }

        [JsonPropertyName("chartType")]
        public string ChartType { get; set; }

        [JsonPropertyName("lastSuccessfulQuery")]
        public string LastSuccessfulQuery { get; set; }

        [JsonPropertyName("mapQuery")]
        public string MapQuery { get; set; }

        [JsonPropertyName("mapLatitude")]
        public string MapLatitude { get; set; }

        [JsonPropertyName("mapLongitude")]
        public string MapLongitude { get; set; }

        [JsonPropertyName("mapZoom")]
        public int MapZoom { get; set; }
        [JsonPropertyName("suggestedQuestions")]
        public List<string> SuggestedQuestions { get; set; } = new List<string>();

        [JsonPropertyName("executedQueries")]
        public List<SqlExecutionRecord> ExecutedQueries { get; set; } = new List<SqlExecutionRecord>();

        [JsonPropertyName("insights")]
        public string Insights { get; set; }

        [JsonPropertyName("suggestedVisualizations")]
        public List<string> SuggestedVisualizations { get; set; } = new List<string>();

        [JsonPropertyName("clarificationQuestions")]
        public List<string> ClarificationQuestions { get; set; } = new List<string>();

        [JsonPropertyName("sql")]
        public string Sql { get; set; }

        [JsonPropertyName("isCrosstab")]
        public bool IsCrosstab { get; set; } = false;

        [JsonPropertyName("isPivot")]
        public bool IsPivot { get; set; } = false;

        [JsonPropertyName("mermaidDiagramSyntax")]
        public string MermaidDiagramSyntax { get; set; }

        [JsonPropertyName("isTree")]
        public bool IsTree { get; set; } = false;

        [JsonPropertyName("treeData")]
        public object TreeData { get; set; }

        [JsonPropertyName("aiTreeConfig")]
        public AiTreeConfig AiTreeConfig { get; set; }
    }

    [Authorize]
    public class ChatToDBModel : PageModel
    {
        private readonly GeminiService _geminiService;
        private readonly HRISContext _context;
        private readonly IMemoryCache _cache;
        private readonly IConversationService _conversationService;
        private readonly ILogger<ChatToDBModel> _logger;

        private string GetCurrentUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);
        private readonly JsonSerializerOptions _jsonSerializerOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        private static readonly HttpClient _geocodingHttpClient = new HttpClient();

        private const string IntentGeneralKnowledge = "GENERAL_KNOWLEDGE";
        private const string IntentChat = "CHAT";
        private const string IntentMap = "MAP";
        private const string IntentSql = "SQL";
        private const string IntentModifySql = "MODIFY_SQL";
        private const string IntentChart = "CHART";
        private const string IntentCrosstab = "CROSSTAB";
        private const string IntentPivotTable = "PIVOT_TABLE";
        private const string IntentDrawDiagram = "DRAW_DIAGRAM";
        private const string IntentTreeView = "TREE_VIEW";

        private const int MaxSqlExecutionRetries = 1;

        private static readonly Dictionary<string, string> ChartTypeNormalizationMap = new Dictionary<string, string>
        {
            { "barchart", "bar" },
            { "linechart", "line" },
            { "piechart", "pie" },
            { "areachart", "area" },
            { "scatterchart", "scatter" },
            { "scatterplot", "scatter" },
            { "donutchart", "doughnut" },
            { "doughnutchart", "doughnut" }
        };

        private static readonly Regex ChartFromPreviousRegex = new Regex(@"Generate an? (?<charttype>\w+(?:\s+\w+)?) chart (for|from|using) the previous data", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public ChatToDBModel(
            GeminiService geminiService,
            HRISContext context,
            IMemoryCache cache,
            IConversationService conversationService,
            ILogger<ChatToDBModel> logger)
        {
            _geminiService = geminiService;
            _context = context;
            _cache = cache;
            _conversationService = conversationService;
            _logger = logger;

            if (_geocodingHttpClient.DefaultRequestHeaders.UserAgent.Count == 0)
            {
                _geocodingHttpClient.DefaultRequestHeaders.UserAgent.ParseAdd("WHRIS-Chat-Map-Feature/1.0 (erwinwilsonceniza@gmail.com)");
            }
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostSendMessageAsync([FromBody] ChatRequest request)
        {
            _geminiService.EnableInsights = request.EnableInsights;

            _logger.LogInformation("OnPostSendMessageAsync received question: '{Question}'", request.Question);

            if (string.IsNullOrWhiteSpace(request.Question))
            {
                _logger.LogWarning("Question is null or whitespace.");
                return new JsonResult(new { reply = "Please ask a question." }, _jsonSerializerOptions);
            }

            request.History ??= new List<ChatMessage>();
            object responseData;

            var matchChartFromPrevious = ChartFromPreviousRegex.Match(request.Question);
            if (matchChartFromPrevious.Success)
            {
                _logger.LogInformation("Handling 'Chart from Previous Data' intent directly.");
                responseData = await HandleChartFromPreviousDataIntentAsync(request, matchChartFromPrevious);
                return new JsonResult(responseData, _jsonSerializerOptions);
            }

            string intent = await _geminiService.ClassifyIntentAsync(request.Question, request.History);
            _logger.LogInformation("Classified Intent: {Intent} for question: '{Question}'", intent, request.Question);

            switch (intent)
            {
                case IntentGeneralKnowledge:
                case IntentChat:
                    responseData = await HandleGeneralChatIntentAsync(request);
                    break;

                case IntentMap:
                    responseData = await HandleMapDisplayIntentAsync(request);
                    break;
                case IntentDrawDiagram:
                    responseData = await HandleDrawDiagramIntentAsync(request);
                    break;
                case IntentSql:
                case IntentChart:
                case IntentModifySql:
                case IntentCrosstab:
                case IntentPivotTable:

                    if (intent == IntentChart) intent = IntentSql;

                    var dbResponseObject = await HandleDatabaseQueryIntentAsync(request, intent);
                    if (dbResponseObject is QueryExecutionResult queryExecResult &&
                        !queryExecResult.Success &&
                        queryExecResult.ClarificationQuestions.Any())
                    {
                        _logger.LogInformation("Ambiguity detected for intent '{Intent}'. Returning clarification questions.", intent);
                        return new JsonResult(queryExecResult, _jsonSerializerOptions);
                    }
                    responseData = dbResponseObject;
                    break;
                case IntentTreeView:
                    responseData = await HandleTreeViewIntentAsync(request);
                    break;
                default:
                    _logger.LogWarning("Unknown intent classified: '{Intent}'. Defaulting to general chat response.", intent);
                    responseData = await HandleGeneralChatIntentAsync(request);
                    break;
            }

            return new JsonResult(responseData, _jsonSerializerOptions);
        }

        public async Task<object> HandleChartFromPreviousDataIntentAsync(ChatRequest request, Match chartMatch)
        {
            _logger.LogInformation("Handling 'Chart from Previous Data' intent. Matched chart type (raw): {ChartTypeRaw}", chartMatch.Groups["charttype"].Value);

            string rawChartTypeInput = chartMatch.Groups["charttype"].Value.ToLowerInvariant().Replace(" ", string.Empty);
            string normalizedChartType = ChartTypeNormalizationMap.TryGetValue(rawChartTypeInput, out var mappedType) ? mappedType : rawChartTypeInput;
            _logger.LogDebug("Normalized chart type for previous data: {NormalizedChartType}", normalizedChartType);

            var lastBotMessageWithData = request.History
                .Where(m => m.Role == "model")
                .LastOrDefault(m =>
                    HasJsonArrayData(m.TableData) || HasJsonArrayData(m.ChartData)
                );

            if (lastBotMessageWithData == null)
            {
                _logger.LogWarning("No previous bot message with suitable data found for re-visualization.");
                return new { reply = "I couldn't find any previous data to make a new chart from." };
            }

            string? originalSql = lastBotMessageWithData.Sql;
            List<SqlExecutionRecord> originalExecutedQueries = lastBotMessageWithData.ExecutedQueries ?? new List<SqlExecutionRecord>();

            List<object>? dataSource = TryDeserializeData(lastBotMessageWithData.TableData, "TableData")
                                     ?? TryDeserializeData(lastBotMessageWithData.ChartData, "ChartData");

            if (dataSource == null || !dataSource.Any())
            {
                _logger.LogWarning("Previous data source for re-visualization was empty or could not be deserialized.");
                return new { reply = "The previous data appears to be empty, or I had trouble reading it." };
            }

            _logger.LogInformation("Successfully obtained previous data source with {Count} items for re-visualization.", dataSource.Count);

            QueryExecutionResult chartSpecificResult = FormatExistingDataForChart(dataSource, normalizedChartType, request.Question ?? "previous data");
            chartSpecificResult.Sql = originalSql;
            chartSpecificResult.ExecutedQueries = originalExecutedQueries;

            if (chartSpecificResult.Success)
            {
                var dataForAnalysis = chartSpecificResult.ChartData?.Any() == true ? chartSpecificResult.ChartData :
                                        chartSpecificResult.TableData?.Any() == true ? chartSpecificResult.TableData :
                                        null;

                if (dataForAnalysis != null)
                {
                    var jsonDataForAnalysis = JsonSerializer.Serialize(dataForAnalysis, _jsonSerializerOptions);
                    try
                    {
                        _logger.LogInformation("Generating insights for re-visualized data for question: {Question}", request.Question);
                        chartSpecificResult.Insights = await _geminiService.GenerateInsightsAsync(jsonDataForAnalysis, request.Question ?? "related to the chart");
                        _logger.LogInformation("Generating follow-up questions for re-visualized data for question: {Question}", request.Question);
                        chartSpecificResult.SuggestedQuestions = await _geminiService.GetFollowUpQuestionsAsync(request.Question ?? "related to the chart", jsonDataForAnalysis);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error generating insights or follow-up questions from Gemini service for re-visualized data.");
                        chartSpecificResult.Insights = "Failed to generate insights due to an error.";
                    }
                }
                else
                {
                    _logger.LogWarning("No suitable data (ChartData or TableData) found in chartSpecificResult for generating insights, despite overall success.");
                }
            }

            return new
            {
                reply = chartSpecificResult.Message,
                tableData = chartSpecificResult.TableData,
                chartData = chartSpecificResult.ChartData,
                chartType = chartSpecificResult.ChartType,
                sql = chartSpecificResult.Sql,
                insights = chartSpecificResult.Insights,
                suggestedVisualizations = chartSpecificResult.SuggestedVisualizations,
                executedQueries = chartSpecificResult.ExecutedQueries,
                suggestedQuestions = chartSpecificResult.SuggestedQuestions
            };
        }

        public async Task<IActionResult> OnPostSummarizeDataAsync([FromBody] SummarizeDataRequest request)
        {
            if (request.TableData == null || !request.TableData.Any())
            {
                _logger.LogWarning("OnPostSummarizeDataAsync: No data to summarize.");
                return new JsonResult(new { reply = "There's no data to summarize." }, _jsonSerializerOptions);
            }
            try
            {
                var jsonData = JsonSerializer.Serialize(request.TableData, _jsonSerializerOptions);
                var summary = await _geminiService.SummarizeDataAsync(jsonData, request.UserQuestion);
                return new JsonResult(new { reply = summary }, _jsonSerializerOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OnPostSummarizeDataAsync: Error summarizing for question '{UserQuestion}'", request.UserQuestion);
                return new JsonResult(new { reply = "Sorry, I couldn't summarize the data." }, _jsonSerializerOptions);
            }
        }

        public async Task<JsonResult> OnGetConversationsAsync()
        {
            var userId = GetCurrentUserId();
            _logger.LogInformation("OnGetConversationsAsync for UserId: {UserId}", userId ?? "NULL");
            if (string.IsNullOrEmpty(userId)) return new JsonResult(new List<Conversation>(), _jsonSerializerOptions);
            var conversations = await _conversationService.GetAllAsync(userId);
            _logger.LogDebug("OnGetConversationsAsync: Found {Count} conversations for UserId {UserId}", conversations?.Count ?? 0, userId);
            return new JsonResult(conversations);
        }

        public async Task<JsonResult> OnGetConversationAsync(int id)
        {
            var userId = GetCurrentUserId();
            _logger.LogInformation("OnGetConversationAsync for Id: {ConvId}, UserId: {UserId}", id, userId ?? "NULL");
            if (string.IsNullOrEmpty(userId)) return new JsonResult(NotFound());
            var conversation = await _conversationService.GetByIdAsync(id, userId);
            if (conversation == null)
            {
                _logger.LogWarning("OnGetConversationAsync: Conversation {ConvId} not found for UserId {UserId}", id, userId);
                return new JsonResult(NotFound());
            }
            List<ChatMessage> historyList = null;
            try
            {
                historyList = JsonSerializer.Deserialize<List<ChatMessage>>(conversation.History, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "OnGetConversationAsync: Failed to deserialize history for {ConvId}. Snippet: {Snippet}", id, conversation.History?.Substring(0, Math.Min(200, conversation.History.Length)));
            }
            var lastSuccessfulQuery = historyList?.LastOrDefault(m => m.Role == "model" && !string.IsNullOrEmpty(m.Sql))?.Sql;
            return new JsonResult(new
            {
                Id = conversation.Id,
                Title = conversation.Title,
                History = historyList,
                LastSqlQuery = lastSuccessfulQuery
            }, _jsonSerializerOptions);
        }

        public async Task<IActionResult> OnPostDeleteConversationAsync(int id)
        {
            var userId = GetCurrentUserId();
            _logger.LogInformation("OnPostDeleteConversationAsync: User {UserId} deleting {ConvId}", userId, id);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();
            await _conversationService.DeleteAsync(id, userId);
            return new OkResult();
        }

        public async Task<IActionResult> OnPostTogglePinAsync(int id)
        {
            var userId = GetCurrentUserId();
            _logger.LogInformation("OnPostTogglePinAsync: User {UserId} toggling pin for {ConvId}", userId, id);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();
            var success = await _conversationService.TogglePinAsync(id, userId);
            if (!success)
            {
                _logger.LogWarning("OnPostTogglePinAsync: Toggle pin FAILED for {ConvId}, User {UserId}", id, userId);
                return NotFound();
            }
            return new OkResult();
        }

        public async Task<IActionResult> OnPostUpdateTitleAsync([FromBody] UpdateTitleRequest request)
        {
            _logger.LogInformation("OnPostUpdateTitleAsync for Id {ConvId} to '{NewTitle}'", request?.Id, request?.NewTitle);
            if (request == null || string.IsNullOrWhiteSpace(request.NewTitle))
            {
                _logger.LogWarning("OnPostUpdateTitleAsync: Invalid request.");
                return BadRequest("New title cannot be empty.");
            }
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId)) return Unauthorized();
            var conversation = await _conversationService.GetByIdAsync(request.Id, userId);
            if (conversation == null)
            {
                _logger.LogWarning("OnPostUpdateTitleAsync: Conversation {ConvId} not found for User {UserId}", request.Id, userId);
                return NotFound();
            }
            conversation.Title = request.NewTitle.Trim();
            conversation.LastModified = DateTime.UtcNow;
            await _conversationService.UpdateAsync(conversation);
            _logger.LogInformation("OnPostUpdateTitleAsync: Title updated for {ConvId}", request.Id);
            return new OkObjectResult(new { Id = conversation.Id, NewTitle = conversation.Title });
        }

        public async Task<IActionResult> OnPostSaveConversationAsync([FromBody] SaveConversationRequest request)
        {
            _logger.LogInformation("[SAVE_CONV_SERVER] OnPostSaveConversationAsync. Request Id: {ReqId}", request?.Id?.ToString() ?? "null");
            if (request.History == null || !request.History.Any())
            {
                if (request.Id.HasValue && request.Id > 0)
                {
                    _logger.LogInformation("[SAVE_CONV_SERVER] Updating existing conv {ConvId} with empty history", request.Id.Value);
                }
                else
                {
                    _logger.LogWarning("[SAVE_CONV_SERVER] BadRequest: History null/empty for new conv.");
                    return new BadRequestObjectResult("Not enough history to save for a new conversation.");
                }
            }
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("[SAVE_CONV_SERVER] Unauthorized: UserId null/empty.");
                return Unauthorized();
            }
            string historyJson = JsonSerializer.Serialize(request.History ?? new List<ChatMessage>(), _jsonSerializerOptions);
            Conversation conversation;
            if (request.Id.HasValue && request.Id > 0)
            {
                _logger.LogInformation("[SAVE_CONV_SERVER] Attempting to UPDATE conv Id: {ConvId}", request.Id.Value);
                conversation = await _conversationService.GetByIdAsync(request.Id.Value, userId);
                if (conversation == null)
                {
                    _logger.LogWarning("[SAVE_CONV_SERVER] NotFound: Conv {ConvId} for user {UserId}.", request.Id.Value, userId);
                    return NotFound();
                }
                conversation.History = historyJson;
                conversation.LastModified = DateTime.UtcNow;
                await _conversationService.UpdateAsync(conversation);
                _logger.LogInformation("[SAVE_CONV_SERVER] Conv Id: {ConvId} UPDATED successfully.", conversation.Id);
            }
            else
            {
                _logger.LogInformation("[SAVE_CONV_SERVER] Attempting to CREATE NEW conv (request.Id was null or 0).");
                var firstUserMessage = request.History?.FirstOrDefault(m => m.Role == "user")?.Content ?? "New Chat";
                const int titleMaxLength = 100;
                var title = firstUserMessage.Length > titleMaxLength ? firstUserMessage.Substring(0, titleMaxLength) : firstUserMessage;
                conversation = new Conversation
                {
                    UserId = userId,
                    Title = title,
                    History = historyJson,
                    LastModified = DateTime.UtcNow,
                    Pin = false
                };
                await _conversationService.AddAsync(conversation);
                _logger.LogInformation("[SAVE_CONV_SERVER] NEW conv CREATED with Id: {ConvId}", conversation.Id);
            }
            return new JsonResult(conversation, _jsonSerializerOptions);
        }

        private bool HasJsonArrayData(JsonElement? jsonElement)
        {
            return jsonElement?.ValueKind == JsonValueKind.Array && jsonElement.Value.GetArrayLength() > 0;
        }

        private List<object>? TryDeserializeData(JsonElement? jsonElement, string dataTypeForLog)
        {
            if (HasJsonArrayData(jsonElement))
            {
                try
                {
                    return JsonSerializer.Deserialize<List<object>>(jsonElement.Value.GetRawText(), _jsonSerializerOptions);
                }
                catch (JsonException ex)
                {
                    _logger.LogError(ex, "[RE-VISUALIZE] JSON deserialization error for previous {DataType}.", dataTypeForLog);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[RE-VISUALIZE] Generic error deserializing previous {DataType}.", dataTypeForLog);
                }
            }
            return null;
        }

        private async Task<object> HandleGeneralChatIntentAsync(ChatRequest request)
        {
            _logger.LogInformation("Handling GENERAL_KNOWLEDGE or CHAT intent.");
            var botReply = await _geminiService.GenerateChatResponseAsync(request.Question, request.History);
            return new { reply = botReply };
        }

        private async Task<object> HandleMapDisplayIntentAsync(ChatRequest request)
        {
            _logger.LogInformation("Handling MAP intent for question: '{Question}'", request.Question);
            var result = await HandleMapIntent(request);
            return new
            {
                reply = result.Message,
                mapQuery = result.MapQuery,
                mapLatitude = result.MapLatitude,
                mapLongitude = result.MapLongitude,
                mapZoom = result.MapZoom,
                executedQueries = result.ExecutedQueries,
                sql = result.Sql
            };
        }

        private async Task<object> HandleDatabaseQueryIntentAsync(ChatRequest request, string intentInput)
        {
            _logger.LogInformation("Handling intent: {IntentValue}", intentInput);
            string currentIntent = intentInput;
            if (currentIntent == IntentModifySql && string.IsNullOrWhiteSpace(request.LastSqlQuery))
                if (currentIntent == IntentModifySql && string.IsNullOrWhiteSpace(request.LastSqlQuery))
                {
                    _logger.LogInformation("Intent was MODIFY_SQL but LastSqlQuery is empty. Switching to SQL intent.");
                    currentIntent = IntentSql;
                }

            var allTables = GetTablesAndColumnsFromContext();
            var tablesFromQuestion = await _geminiService.GetRelevantTablesAsync(request.Question, allTables);
            var tablesFromLastQuery = ExtractTablesFromQuery(request.LastSqlQuery);
            var relevantTables = tablesFromQuestion.Union(tablesFromLastQuery, StringComparer.OrdinalIgnoreCase).Distinct().ToList();
            _logger.LogDebug("Relevant tables for DB Query: {Count} - {List}", relevantTables.Count, string.Join(", ", relevantTables));

            var dbSchema = GetDatabaseSchema(relevantTables);
            _logger.LogTrace("DB Schema for AI (length {Length}):\n{Snippet}", dbSchema.Length, dbSchema.Substring(0, Math.Min(500, dbSchema.Length)));

            var clarificationQuestions = await _geminiService.CheckForAmbiguityAsync(request.Question, dbSchema, request.History);

            if (clarificationQuestions != null && clarificationQuestions.Any())
            {
                _logger.LogInformation("Question is ambiguous. Returning QueryExecutionResult object.");
                return new QueryExecutionResult { Success = false, Message = "Your question seems a bit ambiguous. Could you please clarify?", ClarificationQuestions = clarificationQuestions, IsCrosstab = (currentIntent == "CROSSTAB" || currentIntent == "PIVOT_TABLE") };
            }

            var (queryExecResult, _) = await HandleSqlQueryIntent(request, currentIntent, dbSchema, relevantTables);

            queryExecResult.IsCrosstab = (currentIntent == IntentCrosstab || currentIntent == IntentPivotTable);
            queryExecResult.IsPivot = (currentIntent == IntentPivotTable);

            if (queryExecResult.Success && queryExecResult.TableData?.Any() == true)
            {
                var jsonDataForAnalysis = JsonSerializer.Serialize(queryExecResult.TableData, _jsonSerializerOptions);
                queryExecResult.Insights = await _geminiService.GenerateInsightsAsync(jsonDataForAnalysis, request.Question);
                if (!queryExecResult.IsCrosstab)
                {
                    queryExecResult.SuggestedVisualizations = await _geminiService.SuggestVisualizationsAsync(jsonDataForAnalysis);
                }
                queryExecResult.SuggestedQuestions = await _geminiService.GetFollowUpQuestionsAsync(request.Question, jsonDataForAnalysis);
            }
            return new
            {
                reply = queryExecResult.Message,
                tableData = queryExecResult.TableData,
                chartData = queryExecResult.IsCrosstab ? null : queryExecResult.ChartData,
                chartType = queryExecResult.IsCrosstab ? null : queryExecResult.ChartType,
                sql = queryExecResult.Sql,
                insights = queryExecResult.Insights,
                suggestedVisualizations = queryExecResult.SuggestedVisualizations,
                executedQueries = queryExecResult.ExecutedQueries,
                suggestedQuestions = queryExecResult.SuggestedQuestions,
                mapLatitude = queryExecResult.MapLatitude,
                mapLongitude = queryExecResult.MapLongitude,
                mapZoom = queryExecResult.MapZoom,
                mapQuery = queryExecResult.MapQuery,
                isCrosstab = queryExecResult.IsCrosstab,
                isPivot = queryExecResult.IsPivot
            };
        }

        private async Task<(QueryExecutionResult Result, List<ChatMessage> History)> HandleSqlQueryIntent(ChatRequest request, string intent, string dbSchema, List<string> primaryRelevantTableNames)
        {
            var currentHistory = new List<ChatMessage>(request.History);
            var executedQueries = new List<SqlExecutionRecord>();
            
            var stopwatch = new Stopwatch();
            bool isCrosstabOrPivot = (intent == IntentCrosstab || intent == IntentPivotTable);

            if (primaryRelevantTableNames == null || !primaryRelevantTableNames.Any())
            {
                if (intent == IntentSql || isCrosstabOrPivot)
                {
                    _logger.LogWarning("HandleSqlQueryIntent: No primary relevant tables for {IntentValue} intent.", intent);
                    return (new QueryExecutionResult { Success = false, Message = "I understand you're asking about data, but I couldn't identify which specific tables would answer your question. Could you be more specific?", ExecutedQueries = executedQueries, IsCrosstab = isCrosstabOrPivot }, currentHistory);
                }
            }

            string chartType = isCrosstabOrPivot ? null : DetectChartType(request.Question);
            bool isChartRequest = chartType != null;
            string sqlQuery = string.Empty;
            QueryExecutionResult executionResult = null;

            for (int attempt = 0; attempt <= MaxSqlExecutionRetries; attempt++)
            {
                if (attempt == 0)
                {
                    _logger.LogInformation("HandleSqlQueryIntent: Attempt {AttemptNr} to get SQL. Intent: {IntentValue}", attempt + 1, intent);
                    if (intent == IntentModifySql)
                    {
                        sqlQuery = await _geminiService.GetModifiedSqlAsync(request.Question, request.LastSqlQuery, dbSchema, currentHistory);
                    }
                    else if (isCrosstabOrPivot)
                    {
                        sqlQuery = await _geminiService.GetSqlForCrosstabAsync(request.Question, dbSchema, currentHistory);
                    }
                    else
                    {
                        sqlQuery = await _geminiService.GetSqlFromQuestionAsync(request.Question, dbSchema, isChartRequest, false, currentHistory);
                    }
                }
                string cleanedQuery = CleanAiResponse(sqlQuery);
                _logger.LogDebug("HandleSqlQueryIntent: Attempt {AttemptNr}. Raw SQL: '{RawSql}'. Cleaned SQL: '{CleanSql}'", attempt + 1, sqlQuery, cleanedQuery);

                if (string.IsNullOrEmpty(cleanedQuery) || cleanedQuery.Equals("ERROR", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogWarning("HandleSqlQueryIntent: AI returned ERROR or empty query. Raw: '{RawSql}'", sqlQuery);
                    return (new QueryExecutionResult { Success = false, Message = "I'm sorry, I wasn't able to construct a valid query. Could you try rephrasing?", ExecutedQueries = executedQueries, Sql = sqlQuery, IsCrosstab = isCrosstabOrPivot }, currentHistory);
                }

                if (!IsQuerySafe(cleanedQuery))
                {
                    _logger.LogWarning("HandleSqlQueryIntent: Query deemed unsafe: '{CleanedQuery}'", cleanedQuery);
                    return (new QueryExecutionResult { Success = false, Message = "For security reasons, I can only execute SELECT queries.", ExecutedQueries = executedQueries, Sql = cleanedQuery, IsCrosstab = isCrosstabOrPivot }, currentHistory);
                }

                _logger.LogInformation("HandleSqlQueryIntent: Executing query (Attempt {AttemptNr}): {Query}", attempt + 1, cleanedQuery);
                stopwatch.Restart();
                executionResult = await ExecuteQueryAsync(cleanedQuery, (isCrosstabOrPivot ? false : isChartRequest), chartType);
                stopwatch.Stop();
                _logger.LogInformation("HandleSqlQueryIntent: Query execution time: {ElapsedMs}ms. Success: {SuccessState}", stopwatch.ElapsedMilliseconds, executionResult.Success);

                executionResult.IsCrosstab = isCrosstabOrPivot;

                executedQueries.Add(new SqlExecutionRecord
                {
                    Query = cleanedQuery,
                    Success = executionResult.Success,
                    Error = executionResult.Success ? null : executionResult.Message,
                    ExecutionTimeMs = stopwatch.ElapsedMilliseconds
                });
                executionResult.ExecutedQueries = new List<SqlExecutionRecord>(executedQueries);
                executionResult.Sql = cleanedQuery;

                if (executionResult.Success)
                {
                    executionResult.LastSuccessfulQuery = cleanedQuery;
                    _logger.LogInformation("HandleSqlQueryIntent: Query successful.");
                    return (executionResult, currentHistory);
                }

                if (attempt <= MaxSqlExecutionRetries)
                {
                    _logger.LogWarning("HandleSqlQueryIntent: Query failed on attempt {AttemptNr}. Error: {ErrorMsg}. Attempting correction.", attempt + 1, executionResult.Message);
                    sqlQuery = await _geminiService.GetCorrectedSqlAsync(request.Question, dbSchema, cleanedQuery, executionResult.Message, (isCrosstabOrPivot ? false : isChartRequest), currentHistory);
                }
                else
                {
                    _logger.LogError("HandleSqlQueryIntent: Query failed after max retries. Final Error: {ErrorMsg} for Query: {Query}", executionResult.Message, cleanedQuery);
                    executionResult.Message = $"I tried to run a query, but it failed with the error: {executionResult.Message}. Please try rephrasing your request.";
                    return (executionResult, currentHistory);
                }
            }
            _logger.LogError("HandleSqlQueryIntent: Unexpected exit from query loop.");
            return (new QueryExecutionResult { Success = false, Message = "An unexpected error occurred while generating the SQL query.", ExecutedQueries = executedQueries, IsCrosstab = isCrosstabOrPivot }, currentHistory);
        }

        private async Task<QueryExecutionResult> HandleMapIntent(ChatRequest request)
        {
            _logger.LogInformation("Original HandleMapIntent called for question: '{Question}'", request.Question);
            var executedQueries = new List<SqlExecutionRecord>();
            var stopwatch = new Stopwatch();
            string addressSqlUsed = null;
            string addressString = null;
            string rawAddressSqlFromAI = null;

            try
            {
                var allTablesAndColumns = GetTablesAndColumnsFromContext();
                var relevantTableNames = await _geminiService.GetRelevantTablesAsync(request.Question, allTablesAndColumns);
                if (!relevantTableNames.Any())
                {
                    _logger.LogWarning("HandleMapIntent: No relevant tables found.");
                    return new QueryExecutionResult { Success = false, Message = "I couldn't figure out which table might contain the address information.", ExecutedQueries = executedQueries };
                }
                var dbSchema = GetDatabaseSchema(relevantTableNames);
                rawAddressSqlFromAI = await _geminiService.GetSqlForMapAddressAsync(request.Question, dbSchema, request.History);
                addressSqlUsed = CleanAiResponse(rawAddressSqlFromAI);
                _logger.LogDebug("HandleMapIntent: Raw SQL for map: '{RawSql}', Cleaned: '{CleanSql}'", rawAddressSqlFromAI, addressSqlUsed);

                if (string.IsNullOrEmpty(addressSqlUsed) || addressSqlUsed.Equals("ERROR", StringComparison.OrdinalIgnoreCase) || !IsQuerySafe(addressSqlUsed))
                {
                    _logger.LogWarning("HandleMapIntent: Invalid or unsafe SQL for map: '{AddressSql}'", addressSqlUsed);
                    return new QueryExecutionResult { Success = false, Message = "I was unable to create a valid query to find that address.", ExecutedQueries = executedQueries, Sql = rawAddressSqlFromAI };
                }

                using var connection = _context.Database.GetDbConnection();
                _logger.LogInformation("HandleMapIntent: Executing map address query: {Query}", addressSqlUsed);
                stopwatch.Restart();
                var result = (await connection.QueryAsync<dynamic>(addressSqlUsed)).FirstOrDefault();
                stopwatch.Stop();
                _logger.LogInformation("HandleMapIntent: Map address query time: {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
                executedQueries.Add(new SqlExecutionRecord { Query = addressSqlUsed, Success = true, ExecutionTimeMs = stopwatch.ElapsedMilliseconds });

                if (result == null)
                {
                    _logger.LogInformation("HandleMapIntent: Map address query returned no result for: {Query}", addressSqlUsed);
                    return new QueryExecutionResult { Success = false, Message = "I searched the database, but couldn't find a record for that address request.", ExecutedQueries = executedQueries, Sql = addressSqlUsed };
                }
                var resultDict = result as IDictionary<string, object>;
                addressString = resultDict?.Values.FirstOrDefault()?.ToString();

                if (string.IsNullOrWhiteSpace(addressString))
                {
                    _logger.LogWarning("HandleMapIntent: Found record, but address field is empty. SQL: {Query}", addressSqlUsed);
                    return new QueryExecutionResult { Success = false, Message = "I found a matching record, but its address field is empty.", ExecutedQueries = executedQueries, Sql = addressSqlUsed };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "HandleMapIntent: Error retrieving address. SQL used: '{Sql}'", addressSqlUsed);
                return new QueryExecutionResult { Success = false, Message = "Error retrieving address: " + ex.Message, ExecutedQueries = executedQueries, Sql = addressSqlUsed ?? rawAddressSqlFromAI };
            }

            _logger.LogInformation("[MAP INTENT] Address from DB to geocode: '{AddressString}' (original log style)", addressString);
            GeocodingResult coordinates = await GeocodeAddressAsync(addressString);

            string message;
            int defaultZoom = 17;

            if (coordinates.Success)
            {
                message = "Certainly, here is the map location you requested.";
                _logger.LogInformation("[MAP INTENT] SUCCESSFUL Geocoding. Lat: '{Latitude}', Lon: '{Longitude}'. (original log style)", coordinates.Latitude, coordinates.Longitude);
                return new QueryExecutionResult
                {
                    Success = true,
                    Message = message,
                    MapLatitude = coordinates.Latitude,
                    MapLongitude = coordinates.Longitude,
                    MapZoom = defaultZoom,
                    MapQuery = addressSqlUsed,
                    ExecutedQueries = executedQueries,
                    Sql = addressSqlUsed
                };
            }
            else
            {
                message = $"I found the address '{addressString}', but had trouble pinpointing its exact coordinates. (Geocoding note: {coordinates.ErrorMessage})";
                _logger.LogWarning("[MAP INTENT] Geocoding FAILED. Error: {ErrorMessage}. Address: '{Address}' (original log style)", coordinates.ErrorMessage, addressString);
                return new QueryExecutionResult
                {
                    Success = false,
                    Message = message,
                    MapLatitude = null,
                    MapLongitude = null,
                    MapZoom = defaultZoom,
                    MapQuery = addressSqlUsed,
                    ExecutedQueries = executedQueries,
                    Sql = addressSqlUsed
                };
            }
        }

        private async Task<object> HandleDrawDiagramIntentAsync(ChatRequest request)
        {
            _logger.LogInformation("Handling DRAW_DIAGRAM intent for question: '{Question}'", request.Question);
            string mermaidSyntax = await _geminiService.GetMermaidDiagramSyntaxAsync(request.Question, request.History);
            bool success = !string.IsNullOrWhiteSpace(mermaidSyntax) && !mermaidSyntax.Contains("ERROR") && mermaidSyntax.Length > 5;

            return new QueryExecutionResult
            {
                Success = success,
                Message = success ? "Here's the diagram you requested:" : "I had trouble creating that diagram. Could you try rephrasing?",
                MermaidDiagramSyntax = success ? mermaidSyntax : null,
            };
        }

        private async Task<object> HandleTreeViewIntentAsync(ChatRequest request)
        {
            _logger.LogInformation("Handling TREE_VIEW intent for question: '{Question}'", request.Question);
            var executedQueries = new List<SqlExecutionRecord>();
            var stopwatch = new Stopwatch();
            var finalResult = new QueryExecutionResult { Success = false };

            var allTables = GetTablesAndColumnsFromContext();
            var tablesFromQuestion = await _geminiService.GetRelevantTablesAsync(request.Question, allTables);

            if (tablesFromQuestion == null || !tablesFromQuestion.Any())
            {
                finalResult.Message = "I couldn't identify the relevant tables for your tree view request.";
                _logger.LogWarning("HandleTreeViewIntentAsync: No relevant tables found by AI for question: {Question}", request.Question);
                return finalResult;
            }
            var dbSchema = GetDatabaseSchema(tablesFromQuestion);

            string sqlQueryFromAI = await _geminiService.GetSqlFromQuestionAsync(request.Question, dbSchema, false, true, request.History);
            string cleanedQuery = CleanAiResponse(sqlQueryFromAI);
            finalResult.Sql = cleanedQuery;

            if (string.IsNullOrEmpty(cleanedQuery) || cleanedQuery.Equals("ERROR", StringComparison.OrdinalIgnoreCase))
            {
                finalResult.Message = "I had trouble generating the SQL needed for the tree view. Can you try rephrasing?";
                _logger.LogWarning("HandleTreeViewIntentAsync: AI returned ERROR or empty SQL: '{RawSql}'", sqlQueryFromAI);
                return finalResult;
            }
            if (!IsQuerySafe(cleanedQuery))
            {
                finalResult.Message = "The generated SQL for the tree view was not safe to execute.";
                _logger.LogWarning("HandleTreeViewIntentAsync: Unsafe SQL query: '{CleanedQuery}'", cleanedQuery);
                return finalResult;
            }

            List<Dictionary<string, object>> flatData = null;
            try
            {
                using var connection = _context.Database.GetDbConnection();
                stopwatch.Restart();
                var dynamicResult = (await connection.QueryAsync<dynamic>(cleanedQuery)).ToList();
                stopwatch.Stop();
                var execRecord = new SqlExecutionRecord { Query = cleanedQuery, Success = true, ExecutionTimeMs = stopwatch.ElapsedMilliseconds };
                executedQueries.Add(execRecord);
                finalResult.ExecutedQueries = new List<SqlExecutionRecord>(executedQueries);

                if (!dynamicResult.Any())
                {
                    finalResult.Success = true;
                    finalResult.Message = "The query for the tree view ran successfully but returned no data.";
                    _logger.LogInformation("HandleTreeViewIntentAsync: SQL query returned no data for: {Query}", cleanedQuery);
                    return finalResult;
                }
                flatData = dynamicResult
                    .Select(row => ((IDictionary<string, object>)row)
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value))
                    .ToList();
                finalResult.TableData = flatData.Cast<object>().ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "HandleTreeViewIntentAsync: Error executing SQL for TreeView: {Query}", cleanedQuery);
                stopwatch.Stop();
                executedQueries.Add(new SqlExecutionRecord { Query = cleanedQuery, Success = false, Error = ex.Message, ExecutionTimeMs = stopwatch.ElapsedMilliseconds });
                finalResult.ExecutedQueries = new List<SqlExecutionRecord>(executedQueries);
                finalResult.Message = $"Error executing SQL for tree view: {ex.Message}";
                return finalResult;
            }

            if (flatData == null || !flatData.Any())
            {
                finalResult.Success = true;
                finalResult.Message = "The query for the tree view ran successfully but returned no data.";
                _logger.LogInformation("HandleTreeViewIntentAsync: SQL query returned no data for: {Query}", cleanedQuery);
                return finalResult;
            }

            var actualColumns = flatData.First().Keys.ToList();
            _logger.LogInformation("HandleTreeViewIntentAsync: Actual columns passed to GetTreeViewConfigurationAsync: [{ActualColumnsFromQuery}]", string.Join(", ", actualColumns));

            AiTreeConfig treeConfig = await _geminiService.GetTreeViewConfigurationAsync(request.Question, actualColumns, request.History);
            finalResult.AiTreeConfig = treeConfig;

            if (treeConfig == null)
            {
                finalResult.Success = true;
                finalResult.Message = "I got the data, but I had trouble understanding how to structure it as a tree. Displaying as a flat table instead.";
                finalResult.IsTree = false;
                _logger.LogWarning("HandleTreeViewIntentAsync: GetTreeViewConfigurationAsync returned null. Fallback to flat table. Actual columns seen by config AI: [{ActualCols}]", string.Join(", ", actualColumns));
                return finalResult;
            }

            if (!ValidateAiTreeConfigColumns(treeConfig, actualColumns, out string validationError))
            {
                _logger.LogWarning("HandleTreeViewIntentAsync: AI Tree Config validation failed: {ValidationError}. Config: {@TreeConfig}, ActualCols: [{ActualColumns}]", validationError, treeConfig, string.Join(",", actualColumns));
                finalResult.Success = true;
                finalResult.Message = $"I got the data, but there was an issue with the tree structure plan: {validationError}. Displaying as a flat table.";
                finalResult.IsTree = false;
                return finalResult;
            }

            try
            {
                List<TreeNode> treeNodes = BuildDynamicTree(flatData, treeConfig);
                if (treeNodes == null || !treeNodes.Any())
                {
                    _logger.LogWarning("HandleTreeViewIntentAsync: BuildDynamicTree returned null or empty. Config: {@TreeConfig}", treeConfig);
                    finalResult.Success = true;
                    finalResult.Message = "Data was fetched, but the tree structure resulted in no displayable nodes. Displaying as flat table.";
                    finalResult.IsTree = false;
                }
                else
                {
                    finalResult.TreeData = treeNodes;
                    finalResult.IsTree = true;
                    finalResult.Success = true;
                    finalResult.Message = "Here's the hierarchical data you requested.";
                    finalResult.TableData = null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "HandleTreeViewIntentAsync: Error transforming data to tree structure. Config: {@TreeConfig}", treeConfig);
                finalResult.Success = true;
                finalResult.Message = $"Error building the tree: {ex.Message}. Displaying as a flat table.";
                finalResult.IsTree = false;
            }

            if (finalResult.IsTree && request.EnableInsights && finalResult.TreeData != null)
            {
                try
                {
                    var jsonDataForInsights = JsonSerializer.Serialize(flatData, _jsonSerializerOptions);
                    finalResult.Insights = await _geminiService.GenerateInsightsAsync(jsonDataForInsights, request.Question);
                    finalResult.SuggestedQuestions = await _geminiService.GetFollowUpQuestionsAsync(request.Question, jsonDataForInsights);
                }
                catch (Exception insightsEx)
                {
                    _logger.LogError(insightsEx, "HandleTreeViewIntentAsync: Error generating insights/follow-up for tree view.");
                }
            }
            return finalResult;
        }

        private async Task<QueryExecutionResult> ExecuteQueryAsync(string sqlQuery, bool isChartRequest, string chartType)
        {
            var cacheKey = $"Query-{sqlQuery}";
            if (_cache.TryGetValue(cacheKey, out QueryExecutionResult cachedResult))
            {
                _logger.LogInformation("ExecuteQueryAsync: Returning CACHED result for: {Query}", sqlQuery);
                return cachedResult;
            }
            _logger.LogInformation("ExecuteQueryAsync: Executing DB query (not cached): {Query}", sqlQuery);

            var queryResult = new QueryExecutionResult { Success = true, Sql = sqlQuery };

            try
            {
                using var connection = _context.Database.GetDbConnection();
                var result = (await connection.QueryAsync<dynamic>(sqlQuery)).ToList();
                _logger.LogDebug("ExecuteQueryAsync: Query returned {RowCount} rows.", result.Count);

                if (!result.Any())
                {
                    queryResult.Message = "The query ran successfully but returned no results.";
                }
                else if (isChartRequest)
                {
                    var firstRow = result.First() as IDictionary<string, object>;
                    if (firstRow == null)
                    {
                        queryResult.Message = "Chart data structure error. Displaying as table.";
                        queryResult.TableData = result.Cast<object>().ToList();
                        queryResult.ChartType = null;
                    }
                    else
                    {
                        var columnCount = firstRow.Keys.Count;
                        if (columnCount >= 3 &&
                            (chartType == ChartTypeNormalizationMap["linechart"] ||
                             chartType == ChartTypeNormalizationMap["barchart"] ||
                             chartType == ChartTypeNormalizationMap["areachart"] ||
                             chartType == ChartTypeNormalizationMap["scatterchart"]))
                        {
                            var seriesColumnName = firstRow.Keys.ElementAt(1);
                            queryResult.ChartData = result.GroupBy(row => (row as IDictionary<string, object>)[seriesColumnName]?.ToString())
                                .Select(g => new
                                {
                                    series = g.Key,
                                    values = g.Select(row =>
                                    {
                                        var rowDict = row as IDictionary<string, object>;
                                        decimal.TryParse(rowDict.ElementAt(2).Value?.ToString(), out decimal value);
                                        return new { label = rowDict.ElementAt(0).Value?.ToString(), value };
                                    }).ToList()
                                }).ToList<object>();
                            queryResult.Message = $"Here is the multi-series {chartType} chart you requested.";
                        }
                        else if (columnCount == 2)
                        {
                            queryResult.ChartData = result.Select(row =>
                            {
                                var rowDict = row as IDictionary<string, object>;
                                if (decimal.TryParse(rowDict.ElementAt(1).Value?.ToString(), out decimal value))
                                {
                                    return new { label = rowDict.ElementAt(0).Value?.ToString(), value };
                                }
                                return null;
                            }).Where(item => item != null).ToList<object>();
                            if (!queryResult.ChartData.Any())
                            {
                                queryResult.Message = "I found data, but the second column wasn't numeric for the chart. Displaying as table.";
                                queryResult.ChartData = null;
                            }
                            else
                            {
                                queryResult.Message = $"Here is the {chartType} chart you requested.";
                            }
                        }
                        else
                        {
                            queryResult.Message = "The query did not return enough columns for a chart. Displaying as a table instead.";
                            queryResult.ChartData = null;
                        }
                        queryResult.ChartType = queryResult.ChartData != null ? chartType : null;
                        queryResult.TableData = result.Cast<object>().ToList();
                    }
                }
                else
                {
                    queryResult.TableData = result.Cast<object>().ToList();
                    queryResult.Message = "Here is the data you requested.";
                }
                _cache.Set(cacheKey, queryResult, TimeSpan.FromMinutes(5));
                return queryResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ExecuteQueryAsync: Query FAILED: {Query}", sqlQuery);
                return new QueryExecutionResult { Success = false, Message = ex.Message, Sql = sqlQuery };
            }
        }

        private Dictionary<string, List<string>> GetTablesAndColumnsFromContext()
        {
            var result = _context.Model.GetEntityTypes()
                .Where(e => e.GetTableName() != null)
                .ToDictionary(
                    e => e.GetTableName()!,
                    e => e.GetProperties().Select(p => p.GetColumnName(StoreObjectIdentifier.Table(e.GetTableName()!, null))).Where(c => c != null).ToList()!
                );
            _logger.LogTrace("GetTablesAndColumnsFromContext: Found {Count} tables.", result.Count);
            return result;
        }

        private string DetectChartType(string question)
        {
            if (string.IsNullOrWhiteSpace(question)) return null;
            var qLower = question.ToLowerInvariant();
            if (qLower.Contains("bar chart")) return ChartTypeNormalizationMap["barchart"];
            if (qLower.Contains("line chart")) return ChartTypeNormalizationMap["linechart"];
            if (qLower.Contains("pie chart")) return ChartTypeNormalizationMap["piechart"];
            if (qLower.Contains("area chart")) return ChartTypeNormalizationMap["areachart"];
            if (qLower.Contains("scatter chart")) return ChartTypeNormalizationMap["scatterchart"];
            if (qLower.Contains("scatter plot")) return ChartTypeNormalizationMap["scatterplot"];
            if (qLower.Contains("doughnut chart")) return ChartTypeNormalizationMap["doughnutchart"];
            if (qLower.Contains("donut chart")) return ChartTypeNormalizationMap["donutchart"];
            return null;
        }

        private List<string> ExtractTablesFromQuery(string sqlQuery)
        {
            if (string.IsNullOrWhiteSpace(sqlQuery)) return new List<string>();
            var matches = Regex.Matches(sqlQuery, @"\b(?:FROM|JOIN)\s+([a-zA-Z0-9_\[\]\.]+)", RegexOptions.IgnoreCase);
            var tables = matches.Cast<Match>()
                .Select(m => m.Groups[1].Value.Trim('[', ']').Split('.').Last())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
            _logger.LogTrace("ExtractTablesFromQuery from '{Sql}' yielded: {Tables}", sqlQuery.Substring(0, Math.Min(sqlQuery.Length, 100)), string.Join(", ", tables));
            return tables;
        }

        private bool IsQuerySafe(string sqlQuery)
        {
            if (string.IsNullOrWhiteSpace(sqlQuery))
            {
                _logger.LogWarning("[SECURITY] IsQuerySafe: Query is null/whitespace.");
                return false;
            }
            var trimmedQuery = sqlQuery.Trim();
            if (!trimmedQuery.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase) && !trimmedQuery.StartsWith("WITH", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("[SECURITY] IsQuerySafe: Failed. Query: '{QuerySnippet}'", trimmedQuery.Substring(0, Math.Min(100, trimmedQuery.Length)));
                return false;
            }
            string[] forbiddenKeywords = { "INSERT", "UPDATE", "DELETE", "DROP", "TRUNCATE", "ALTER", "CREATE", "EXEC", "MERGE" };
            if (Regex.IsMatch(trimmedQuery, @"\b(" + string.Join("|", forbiddenKeywords) + @")\b", RegexOptions.IgnoreCase) ||
                trimmedQuery.Contains("--") || trimmedQuery.Contains("/*") || trimmedQuery.Contains("*/") ||
                Regex.IsMatch(trimmedQuery, @";.*\w+", RegexOptions.IgnoreCase))
            {
                _logger.LogWarning("[SECURITY] IsQuerySafe: Failed with keywords/comments. Query: '{QuerySnippet}'", trimmedQuery.Substring(0, Math.Min(100, trimmedQuery.Length)));
                return false;
            }
            _logger.LogInformation("[SECURITY] IsQuerySafe: Passed. Query: '{QuerySnippet}'", trimmedQuery.Substring(0, Math.Min(100, trimmedQuery.Length)));
            return true;
        }

        private string GetSimplifiedType(string dbType)
        {
            if (string.IsNullOrWhiteSpace(dbType)) return "string";
            var lowerDbType = dbType.ToLowerInvariant();
            if (lowerDbType.Contains("int") || lowerDbType.Contains("decimal") || lowerDbType.Contains("numeric") ||
                lowerDbType.Contains("money") || lowerDbType.Contains("float") || lowerDbType.Contains("real") ||
                lowerDbType.Contains("double") || lowerDbType.Contains("byte")) return "numeric";
            if (lowerDbType.Contains("date") || lowerDbType.Contains("time")) return "datetime";
            if (lowerDbType.Contains("bit")) return "boolean";
            return "string";
        }

        private string GetDatabaseSchema(List<string> primaryRelevantTableNames)
        {
            _logger.LogDebug("GetDatabaseSchema (Original Logic) called with {Count} tables: {Tables}", primaryRelevantTableNames?.Count ?? 0, string.Join(", ", primaryRelevantTableNames ?? new List<string>()));
            if (primaryRelevantTableNames == null || !primaryRelevantTableNames.Any()) return string.Empty;

            var schemaBuilder = new StringBuilder();
            var allEntityTypes = _context.Model.GetEntityTypes();
            var definedTables = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var allRelevantEntityTypes = new List<IEntityType>();
            var queue = new Queue<string>(primaryRelevantTableNames.Distinct(StringComparer.OrdinalIgnoreCase));
            var visitedTableNames = new HashSet<string>(primaryRelevantTableNames, StringComparer.OrdinalIgnoreCase);

            while (queue.Any())
            {
                var tableName = queue.Dequeue();
                var entityType = allEntityTypes.FirstOrDefault(e => string.Equals(e.GetTableName(), tableName, StringComparison.OrdinalIgnoreCase));
                if (entityType == null) continue;
                allRelevantEntityTypes.Add(entityType);
                foreach (var fk in entityType.GetForeignKeys())
                {
                    var principalTableName = fk.PrincipalEntityType.GetTableName();
                    if (principalTableName != null && !visitedTableNames.Contains(principalTableName))
                    {
                        visitedTableNames.Add(principalTableName);
                        queue.Enqueue(principalTableName);
                    }
                }
            }
            _logger.LogDebug("GetDatabaseSchema (Original Logic): Total {Count} relevant entity types after FK traversal.", allRelevantEntityTypes.Distinct().Count());

            foreach (var entityType in allRelevantEntityTypes.Distinct().OrderBy(e => e.GetTableName()))
            {
                var tableName = entityType.GetTableName();
                if (tableName == null || definedTables.Contains(tableName)) continue;
                var columnsToInclude = new List<IProperty>();
                var primaryKey = entityType.FindPrimaryKey();
                if (primaryKey != null) columnsToInclude.AddRange(primaryKey.Properties);
                var descriptiveColumn = FindDescriptiveColumn(entityType);
                if (descriptiveColumn != null && !columnsToInclude.Contains(descriptiveColumn)) columnsToInclude.Add(descriptiveColumn);
                var remainingProperties = entityType.GetProperties().Where(p => !columnsToInclude.Contains(p));
                columnsToInclude.AddRange(remainingProperties);
                var storeId = StoreObjectIdentifier.Table(tableName, null);
                var columnDefinitions = columnsToInclude.Distinct()
                    .Select(p => $"{p.GetColumnName(storeId)}({GetSimplifiedType(p.GetColumnType())})");
                schemaBuilder.AppendLine($"TABLE {tableName}: {string.Join(", ", columnDefinitions)}");
                definedTables.Add(tableName);
            }
            schemaBuilder.AppendLine("--");
            foreach (var entityType in allRelevantEntityTypes.Distinct().OrderBy(e => e.GetTableName()))
            {
                var tableName = entityType.GetTableName();
                if (tableName == null) continue;
                var tableId = StoreObjectIdentifier.Table(tableName, null);
                foreach (var fk in entityType.GetForeignKeys())
                {
                    if (!allRelevantEntityTypes.Contains(fk.PrincipalEntityType)) continue;
                    var principalTableName = fk.PrincipalEntityType.GetTableName();
                    if (principalTableName == null) continue;
                    var principalTableId = StoreObjectIdentifier.Table(principalTableName, null);
                    var foreignKeyColumn = fk.Properties.First().GetColumnName(tableId);
                    var principalColumn = fk.PrincipalKey.Properties.First().GetColumnName(principalTableId);
                    schemaBuilder.AppendLine($"FOREIGN KEY: {tableName}({foreignKeyColumn}) REFERENCES {principalTableName}({principalColumn})");
                }
            }
            var finalSchema = schemaBuilder.ToString();
            return finalSchema;
        }

        private string CleanAiResponse(string response)
        {
            if (string.IsNullOrWhiteSpace(response)) return string.Empty;
            var cleanedResponse = Regex.Replace(response, @"^```sql\s*|\s*```$", string.Empty, RegexOptions.Multiline | RegexOptions.IgnoreCase).Trim();
            cleanedResponse = Regex.Replace(cleanedResponse, @"--.*$", string.Empty, RegexOptions.Multiline);
            cleanedResponse = Regex.Replace(cleanedResponse, @"/\*[\s\S]*?\*/", string.Empty, RegexOptions.Singleline).Trim();
            if (cleanedResponse.EndsWith(";"))
            {
                cleanedResponse = cleanedResponse.Substring(0, cleanedResponse.Length - 1).TrimEnd();
            }
            return cleanedResponse;
        }

        private IProperty FindDescriptiveColumn(IEntityType entityType)
        {
            var commonNames = new[] { "Name", "Title", "Description", "Label" };
            var entityTableName = entityType.GetTableName()?.ToLowerInvariant() ?? string.Empty;
            IProperty bestCandidate = null;
            foreach (var prop in entityType.GetProperties())
            {
                var propNameLower = prop.Name.ToLowerInvariant();
                if (commonNames.Contains(prop.Name, StringComparer.OrdinalIgnoreCase)) return prop;
                if (propNameLower == entityTableName && prop.ClrType == typeof(string)) bestCandidate = prop;
                if (propNameLower.Contains("name") && prop.ClrType == typeof(string) && bestCandidate == null) bestCandidate = prop;
            }
            return bestCandidate ?? entityType.GetProperties()
                .FirstOrDefault(p => p.ClrType == typeof(string) && !p.IsPrimaryKey() && !p.IsForeignKey());
        }

        private async Task<GeocodingResult> GeocodeAddressAsync(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
            {
                _logger.LogWarning("[GEOCODING] Address to geocode is empty.");
                return new GeocodingResult { Success = false, ErrorMessage = "Address to geocode cannot be empty." };
            }
            try
            {
                string encodedAddress = Uri.EscapeDataString(address);
                string url = $"https://nominatim.openstreetmap.org/search?q={encodedAddress}&format=json&limit=1&countrycodes=PH";
                _logger.LogInformation("[GEOCODING] Requesting URL: {Url}", url);
                HttpResponseMessage response = await _geocodingHttpClient.GetAsync(url);
                string jsonResponse = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("[GEOCODING] Response Status: {StatusCode}", response.StatusCode);
                _logger.LogTrace("[GEOCODING] RAW JSON Response from Nominatim: {JsonResponse}", jsonResponse);
                if (response.IsSuccessStatusCode)
                {
                    if (string.IsNullOrWhiteSpace(jsonResponse) || jsonResponse == "[]")
                    {
                        _logger.LogWarning("[GEOCODING] Nominatim returned empty or '[]' for: {Address}", address);
                        return new GeocodingResult { Success = false, ErrorMessage = "Nominatim returned an empty response." };
                    }
                    JsonDocument doc = JsonDocument.Parse(jsonResponse);
                    if (doc.RootElement.ValueKind == JsonValueKind.Array && doc.RootElement.GetArrayLength() > 0)
                    {
                        JsonElement location = doc.RootElement[0];
                        string lat = location.TryGetProperty("lat", out JsonElement latEl) ? latEl.GetString() : null;
                        string lon = location.TryGetProperty("lon", out JsonElement lonEl) ? lonEl.GetString() : null;
                        _logger.LogDebug("[GEOCODING] Extracted Lat: '{Lat}', Lon: '{Lon}'", lat, lon);
                        if (!string.IsNullOrEmpty(lat) && !string.IsNullOrEmpty(lon))
                        {
                            _logger.LogInformation("[GEOCODING] Success for '{Address}': Lat={Lat}, Lon={Lon}", address, lat, lon);
                            return new GeocodingResult { Success = true, Latitude = lat, Longitude = lon };
                        }
                        else
                        {
                            _logger.LogWarning("[GEOCODING] Lat/Lon not found in Nominatim response for: {Address}", address);
                            return new GeocodingResult { Success = false, ErrorMessage = "Latitude or Longitude not found in Nominatim's response structure (possibly null or empty)." };
                        }
                    }
                    else
                    {
                        _logger.LogWarning("[GEOCODING] Nominatim response not an array or empty for: {Address}", address);
                        return new GeocodingResult { Success = false, ErrorMessage = $"Address '{address}' not found by Nominatim, or an empty array was returned." };
                    }
                }
                else
                {
                    _logger.LogError("[GEOCODING] API request failed: {StatusCode}. Details: {Details}", response.StatusCode, jsonResponse.Substring(0, Math.Min(jsonResponse.Length, 500)));
                    return new GeocodingResult { Success = false, ErrorMessage = $"Nominatim API request failed: {response.StatusCode}. Details: {jsonResponse.Substring(0, Math.Min(jsonResponse.Length, 500))}" };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[GEOCODING] Exception geocoding address '{Address}'", address);
                return new GeocodingResult { Success = false, ErrorMessage = $"An exception occurred during geocoding: {ex.Message}" };
            }
        }

        private QueryExecutionResult FormatExistingDataForChart(List<object> sourceData, string newChartType, string userQuestionContext)
        {
            _logger.LogInformation("FormatExistingDataForChart for type '{NewChartType}', {Count} items.", newChartType, sourceData?.Count ?? 0);
            var queryResult = new QueryExecutionResult { Success = true };
            List<Dictionary<string, object>> standardizedSourceData;
            try
            {
                var tempJson = JsonSerializer.Serialize(sourceData, _jsonSerializerOptions);
                standardizedSourceData = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(tempJson, _jsonSerializerOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[RE-VISUALIZE] Error standardizing source data.");
                queryResult.Success = false; queryResult.Message = "The structure of the previous data couldn't be processed for charting.";
                queryResult.TableData = sourceData; return queryResult;
            }
            if (standardizedSourceData == null || !standardizedSourceData.Any())
            {
                _logger.LogWarning("[RE-VISUALIZE] Standardized data is null/empty.");
                queryResult.Success = false; queryResult.Message = "The previous data was empty, so I can't generate a new chart.";
                queryResult.TableData = sourceData; return queryResult;
            }
            queryResult.TableData = standardizedSourceData.Cast<object>().ToList();
            var firstRow = standardizedSourceData.First();
            var columnNames = firstRow.Keys.ToList();
            var columnCount = columnNames.Count;
            try
            {
                if (columnCount >= 3 &&
                    (newChartType == ChartTypeNormalizationMap["linechart"] ||
                     newChartType == ChartTypeNormalizationMap["barchart"] ||
                     newChartType == ChartTypeNormalizationMap["areachart"] ||
                     newChartType == ChartTypeNormalizationMap["scatterchart"]))
                {
                    var labelCol = columnNames[0]; var seriesCol = columnNames[1]; var valCol = columnNames[2];
                    queryResult.ChartData = standardizedSourceData.GroupBy(r => r[seriesCol]?.ToString())
                        .Select(g => new { series = g.Key, values = g.Select(r => { decimal.TryParse(r[valCol]?.ToString(), out decimal v); return new { label = r[labelCol]?.ToString(), value = v }; }).ToList() })
                        .ToList<object>();
                    queryResult.Message = $"Okay, here's the multi-series {newChartType} chart using the previous data.";
                }
                else if (columnCount == 2)
                {
                    var labelCol = columnNames[0]; var valCol = columnNames[1];
                    var items = standardizedSourceData.Select(r => { if (decimal.TryParse(r[valCol]?.ToString(), out decimal v)) return new { label = r[labelCol]?.ToString(), value = v }; return null; })
                        .Where(i => i != null).ToList<object>();
                    if (!items.Any()) { queryResult.Message = "The previous data's second column wasn't numeric. I'll show the original data table instead."; queryResult.ChartData = null; }
                    else { queryResult.ChartData = items; queryResult.Message = $"Sure, here's the {newChartType} chart from the previous data."; }
                }
                else if (columnCount == 1 &&
                         (newChartType == ChartTypeNormalizationMap["piechart"] ||
                          newChartType == ChartTypeNormalizationMap["donutchart"] ||
                          newChartType == ChartTypeNormalizationMap["barchart"]))
                {
                    var labelCol = columnNames[0];
                    queryResult.ChartData = standardizedSourceData.GroupBy(r => r[labelCol]?.ToString())
                        .Select(g => new { label = g.Key ?? "N/A", value = (decimal)g.Count() }).ToList<object>();
                    queryResult.Message = $"Here's a {newChartType} chart showing counts from the previous data's '{labelCol}' column.";
                }
                else
                {
                    queryResult.Message = $"The previous data (with {columnCount} columns) isn't suitable for a {newChartType} chart. Displaying the data as a table."; queryResult.ChartData = null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[RE-VISUALIZE] Error transforming data for chart.");
                queryResult.Message = $"I ran into a little trouble preparing that {newChartType} chart. Here's the original data as a table."; queryResult.ChartData = null;
            }
            queryResult.ChartType = queryResult.ChartData != null ? newChartType : null;
            return queryResult;
        }

        private bool ValidateAiTreeConfigColumns(AiTreeConfig config, List<string> actualColumns, out string validationErrorMessage)
        {
            validationErrorMessage = string.Empty;

            if (config == null)
            {
                validationErrorMessage = "AI Tree Configuration is null.";
                return false;
            }

            var actualColsHashSet = new HashSet<string>(actualColumns, StringComparer.OrdinalIgnoreCase);

            string CheckColumn(string colName, string fieldDescription, bool allowNullOrEmpty = false)
            {
                if (allowNullOrEmpty && string.IsNullOrWhiteSpace(colName)) return null;

                if (string.IsNullOrWhiteSpace(colName))
                {
                    return $"{fieldDescription} column name is missing or empty in AI configuration.";
                }
                if (!actualColsHashSet.Contains(colName))
                {
                    return $"{fieldDescription} column '{colName}' provided by AI not found in dataset columns: [{string.Join(", ", actualColumns)}].";
                }
                return null;
            }

            string CheckColumnList(List<string> colList, string fieldDescription)
            {
                if (colList == null) return null;
                foreach (var colNameInList in colList)
                {
                    string itemError = CheckColumn(colNameInList, $"{fieldDescription} (item)");
                    if (itemError != null) return itemError;
                }
                return null;
            }

            string currentError;

            currentError = CheckColumn(config.RootGroupingColumn, "RootGroupingColumn", allowNullOrEmpty: true);
            if (currentError != null) { validationErrorMessage = currentError; return false; }

            currentError = CheckColumn(config.LeafIdentifierColumn, "LeafIdentifierColumn");
            if (currentError != null) { validationErrorMessage = currentError; return false; }
            if (string.IsNullOrWhiteSpace(config.LeafIdentifierColumn))
            {
                validationErrorMessage = "LeafIdentifierColumn cannot be empty in AiTreeConfig.";
                return false;
            }

            currentError = CheckColumnList(config.CoreLeafAttributeColumns, "CoreLeafAttributeColumns");
            if (currentError != null) { validationErrorMessage = currentError; return false; }

            if (config.Paths == null || !config.Paths.Any())
            {
                validationErrorMessage = "AiTreeConfig must have at least one Path defined.";
                return false;
            }

            foreach (var path in config.Paths)
            {
                if (path == null)
                {
                    validationErrorMessage = "A Path object within the Paths array is null.";
                    return false;
                }
                if (string.IsNullOrWhiteSpace(path.PathStemName))
                {
                    validationErrorMessage = "PathStemName cannot be empty in a Path definition.";
                    return false;
                }
                if (path.GroupingSubColumns == null || !path.GroupingSubColumns.Any())
                {
                    validationErrorMessage = $"Path '{path.PathStemName}' must have at least one GroupingSubColumn.";
                    return false;
                }
                currentError = CheckColumnList(path.GroupingSubColumns, $"Path '{path.PathStemName}' GroupingSubColumns");
                if (currentError != null) { validationErrorMessage = currentError; return false; }

                currentError = CheckColumnList(path.ContextualLeafColumns, $"Path '{path.PathStemName}' ContextualLeafColumns");
                if (currentError != null) { validationErrorMessage = currentError; return false; }
            }

            return true;
        }

        public List<TreeNode> BuildDynamicTree(List<Dictionary<string, object>> flatData, AiTreeConfig config)
        {
            if (flatData == null || !flatData.Any() || config == null || string.IsNullOrWhiteSpace(config.LeafIdentifierColumn) || config.Paths == null || !config.Paths.Any())
            {
                _logger.LogWarning("BuildDynamicTree: Invalid input data or configuration.");
                return new List<TreeNode>();
            }

            if (!string.IsNullOrWhiteSpace(config.RootGroupingColumn))
            {
                var groupedByRoot = flatData
                    .GroupBy(row => GetValueOrDefault(row, config.RootGroupingColumn, "Unspecified " + config.RootGroupingColumn))
                    .OrderBy(g => g.Key?.ToString());

                List<TreeNode> rootNodes = new List<TreeNode>();
                foreach (var rootGroup in groupedByRoot)
                {
                    var rootNode = new TreeNode { Name = rootGroup.Key.ToString(), Expandable = true };
                    foreach (var pathConfig in config.Paths)
                    {
                        var pathStemNode = new TreeNode { Name = pathConfig.PathStemName, Expandable = true };
                        ProcessPathRecursive(rootGroup.ToList(), pathConfig, 0, pathStemNode, config);
                        if (pathStemNode.Children.Any()) rootNode.Children.Add(pathStemNode);
                    }
                    if (rootNode.Children.Any()) rootNodes.Add(rootNode);
                }
                return rootNodes;
            }
            else
            {
                List<TreeNode> stemNodes = new List<TreeNode>();
                foreach (var pathConfig in config.Paths)
                {
                    var pathStemNode = new TreeNode { Name = pathConfig.PathStemName, Expandable = true };
                    ProcessPathRecursive(flatData, pathConfig, 0, pathStemNode, config);
                    if (pathStemNode.Children.Any()) stemNodes.Add(pathStemNode);
                }
                return stemNodes;
            }
        }

        private void ProcessPathRecursive(
            List<Dictionary<string, object>> currentLevelData,
            AiTreePathConfig pathConfig,
            int groupColumnIndex,
            TreeNode parentNode,
            AiTreeConfig overallConfig)
        {
            if (groupColumnIndex >= pathConfig.GroupingSubColumns.Count)
            {
                var actualLeafRows = currentLevelData
                    .Where(row => GetValueOrDefault(row, overallConfig.LeafIdentifierColumn, null) != null)
                    .OrderBy(r => GetValueOrDefault(r, overallConfig.LeafIdentifierColumn, "Unknown Leaf")?.ToString())
                    .ToList();

                _logger.LogInformation(
                    "Path '{PathName}', Parent '{ParentName}': Leaf Level (GroupColumnIndex {Idx}). Processing {LeafCount} actual leaf rows out of {TotalCurrentLevelDataCount} items in currentLevelData for this group.",
                    pathConfig.PathStemName,
                    parentNode.Name,
                    groupColumnIndex,
                    actualLeafRows.Count,
                    currentLevelData.Count);

                foreach (var rowData in actualLeafRows)
                {
                    var rowDataContentForLog = string.Join(", ", rowData.Select(kv => $"{kv.Key}:'{kv.Value?.ToString() ?? "NULL"}'"));
                    _logger.LogDebug("  Leaf Node Creation for Parent '{ParentName}': Full rowData: [{RowDataContent}]", parentNode.Name, rowDataContentForLog);

                    var leafNode = new TreeNode
                    {
                        Name = GetValueOrDefault(rowData, overallConfig.LeafIdentifierColumn, "Unknown Leaf").ToString(),
                        Expandable = false,
                        AdditionalData = new Dictionary<string, object>()
                    };

                    if (overallConfig.CoreLeafAttributeColumns != null)
                    {
                        foreach (var attrCol in overallConfig.CoreLeafAttributeColumns)
                        {
                            if (!string.IsNullOrWhiteSpace(attrCol))
                            {
                                var sanitizedKey = SanitizeKeyForJson(attrCol);
                                var value = GetValueOrDefault(rowData, attrCol, null);
                                leafNode.AdditionalData[sanitizedKey] = value;
                                _logger.LogDebug("    Leaf '{LeafName}': Added Core Attribute '{AttrCol}' (as key: '{SanitizedKey}') = '{Value}'", leafNode.Name, attrCol, sanitizedKey, value?.ToString() ?? "NULL");
                            }
                        }
                    }

                    if (pathConfig.ContextualLeafColumns != null)
                    {
                        foreach (var ctxCol in pathConfig.ContextualLeafColumns)
                        {
                            if (!string.IsNullOrWhiteSpace(ctxCol) &&
                                ctxCol != overallConfig.LeafIdentifierColumn &&
                                !(overallConfig.CoreLeafAttributeColumns?.Contains(ctxCol) == true))
                            {
                                var sanitizedKey = SanitizeKeyForJson(ctxCol);
                                var value = GetValueOrDefault(rowData, ctxCol, null);
                                leafNode.AdditionalData[sanitizedKey] = value;
                                _logger.LogDebug("    Leaf '{LeafName}': Added Contextual Attribute for path '{PathName}' -> '{CtxCol}' (as key: '{SanitizedKey}') = '{Value}'", leafNode.Name, pathConfig.PathStemName, ctxCol, sanitizedKey, value?.ToString() ?? "NULL");
                            }
                        }
                    }

                    _logger.LogInformation("  Leaf '{LeafName}': Final AdditionalData before adding to parent: {AdditionalDataJson}", leafNode.Name, JsonSerializer.Serialize(leafNode.AdditionalData));

                    parentNode.Children.Add(leafNode);
                }
                parentNode.Expandable = parentNode.Children.Any();
                return;
            }

            string currentGroupingColumn = pathConfig.GroupingSubColumns[groupColumnIndex];

            if (string.IsNullOrWhiteSpace(currentGroupingColumn))
            {
                _logger.LogWarning("ProcessPathRecursive: Path '{PathName}', Parent '{ParentName}': Encountered null or empty grouping column at index {GroupColumnIndex}. Attempting to process next level or leaves with current data.",
                    pathConfig.PathStemName, parentNode.Name, groupColumnIndex);
                ProcessPathRecursive(currentLevelData, pathConfig, groupColumnIndex + 1, parentNode, overallConfig);
                return;
            }

            var groupedData = currentLevelData
                .GroupBy(row => GetValueOrDefault(row, currentGroupingColumn, "Unspecified " + currentGroupingColumn))
                .OrderBy(g => g.Key?.ToString());

            _logger.LogInformation(
                "Path '{PathName}', Parent '{ParentName}': Grouping by '{CurrentGroupingColumn}' (Index {Idx}). Found {GroupCount} distinct groups.",
                pathConfig.PathStemName,
                parentNode.Name,
                currentGroupingColumn,
                groupColumnIndex,
                groupedData.Count());

            if (!groupedData.Any() && groupColumnIndex < pathConfig.GroupingSubColumns.Count - 1)
            {
                _logger.LogWarning("Path '{PathName}', Parent '{ParentName}': No groups found for '{CurrentGroupingColumn}'. No further children will be added to this branch of the parent.", pathConfig.PathStemName, parentNode.Name, currentGroupingColumn);
            }

            foreach (var group in groupedData)
            {
                var groupNode = new TreeNode
                {
                    Name = group.Key.ToString(),
                    Expandable = true,
                    AdditionalData = new Dictionary<string, object>()
                };

                _logger.LogDebug(
                    "  Path '{PathName}', Parent '{ParentName}': Created groupNode '{GroupName}' for column '{CurrentGroupingColumn}'. Will recursively process {ItemCount} items in this group.",
                    pathConfig.PathStemName,
                    parentNode.Name,
                    groupNode.Name,
                    currentGroupingColumn,
                    group.Count());

                ProcessPathRecursive(new List<Dictionary<string, object>>(group.ToList()), pathConfig, groupColumnIndex + 1, groupNode, overallConfig);

                if (groupNode.Children.Any())
                {
                    parentNode.Children.Add(groupNode);
                }
                else
                {
                    _logger.LogDebug("  Path '{PathName}', GroupNode '{GroupName}' (under parent '{ParentNodeName}') had no children after recursion, so it will not be added to the tree.",
                        pathConfig.PathStemName, groupNode.Name, parentNode.Name);
                }
            }
            parentNode.Expandable = parentNode.Children.Any();
        }

        private string SanitizeKeyForJson(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) return "unknown_key";
            return key.Replace(" ", "_").Replace(".", "_").Replace("[", string.Empty).Replace("]", string.Empty);
        }


        private object GetValueOrDefault(Dictionary<string, object> row, string columnName, object defaultValue = null)
        {
            if (string.IsNullOrWhiteSpace(columnName)) return defaultValue;
            return row.TryGetValue(columnName, out var value) ? (value ?? defaultValue) : defaultValue;
        }
    }
}