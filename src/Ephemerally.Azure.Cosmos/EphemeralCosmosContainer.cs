using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Scripts;

namespace Ephemerally.Azure.Cosmos;

public class EphemeralCosmosContainer : Container, IAsyncDisposable
{
    private readonly IEphemeral<Container> _containerEphemeral;

    private Container Container => _containerEphemeral.Value;

    public EphemeralCosmosContainer(IEphemeral<Container> containerEphemeral)
    {
        _containerEphemeral = containerEphemeral;
    }

    public ValueTask DisposeAsync() => _containerEphemeral.DisposeAsync();

    public override Task<ContainerResponse> ReadContainerAsync(ContainerRequestOptions requestOptions = null,
        CancellationToken cancellationToken = new CancellationToken()) =>
        Container.ReadContainerAsync(requestOptions, cancellationToken);

    public override Task<ResponseMessage> ReadContainerStreamAsync(ContainerRequestOptions requestOptions = null,
        CancellationToken cancellationToken = new CancellationToken()) =>
        Container.ReadContainerStreamAsync(requestOptions, cancellationToken);

    public override Task<ContainerResponse> ReplaceContainerAsync(ContainerProperties containerProperties, ContainerRequestOptions requestOptions = null,
        CancellationToken cancellationToken = new CancellationToken()) =>
        Container.ReplaceContainerAsync(containerProperties, requestOptions, cancellationToken);

    public override Task<ResponseMessage> ReplaceContainerStreamAsync(ContainerProperties containerProperties, ContainerRequestOptions requestOptions = null,
        CancellationToken cancellationToken = new CancellationToken()) =>
        Container.ReplaceContainerStreamAsync(containerProperties, requestOptions, cancellationToken);

    public override Task<ContainerResponse> DeleteContainerAsync(ContainerRequestOptions requestOptions = null,
        CancellationToken cancellationToken = new CancellationToken()) =>
        Container.DeleteContainerAsync(requestOptions, cancellationToken);

    public override Task<ResponseMessage> DeleteContainerStreamAsync(ContainerRequestOptions requestOptions = null,
        CancellationToken cancellationToken = new CancellationToken()) =>
        Container.DeleteContainerStreamAsync(requestOptions, cancellationToken);

    public override Task<int?> ReadThroughputAsync(CancellationToken cancellationToken = new CancellationToken()) => Container.ReadThroughputAsync(cancellationToken);

    public override Task<ThroughputResponse> ReadThroughputAsync(RequestOptions requestOptions, CancellationToken cancellationToken = new CancellationToken()) => Container.ReadThroughputAsync(requestOptions, cancellationToken);

    public override Task<ThroughputResponse> ReplaceThroughputAsync(int throughput, RequestOptions requestOptions = null,
        CancellationToken cancellationToken = new CancellationToken()) =>
        Container.ReplaceThroughputAsync(throughput, requestOptions, cancellationToken);

    public override Task<ThroughputResponse> ReplaceThroughputAsync(ThroughputProperties throughputProperties, RequestOptions requestOptions = null,
        CancellationToken cancellationToken = new CancellationToken()) =>
        Container.ReplaceThroughputAsync(throughputProperties, requestOptions, cancellationToken);

    public override Task<ResponseMessage> CreateItemStreamAsync(Stream streamPayload, PartitionKey partitionKey, ItemRequestOptions requestOptions = null,
        CancellationToken cancellationToken = new CancellationToken()) =>
        Container.CreateItemStreamAsync(streamPayload, partitionKey, requestOptions, cancellationToken);

    public override Task<ItemResponse<T>> CreateItemAsync<T>(T item, PartitionKey? partitionKey = null, ItemRequestOptions requestOptions = null,
        CancellationToken cancellationToken = new CancellationToken()) =>
        Container.CreateItemAsync(item, partitionKey, requestOptions, cancellationToken);

    public override Task<ResponseMessage> ReadItemStreamAsync(string id, PartitionKey partitionKey, ItemRequestOptions requestOptions = null,
        CancellationToken cancellationToken = new CancellationToken()) =>
        Container.ReadItemStreamAsync(id, partitionKey, requestOptions, cancellationToken);

    public override Task<ItemResponse<T>> ReadItemAsync<T>(string id, PartitionKey partitionKey, ItemRequestOptions requestOptions = null,
        CancellationToken cancellationToken = new CancellationToken()) =>
        Container.ReadItemAsync<T>(id, partitionKey, requestOptions, cancellationToken);

    public override Task<ResponseMessage> UpsertItemStreamAsync(Stream streamPayload, PartitionKey partitionKey, ItemRequestOptions requestOptions = null,
        CancellationToken cancellationToken = new CancellationToken()) =>
        Container.UpsertItemStreamAsync(streamPayload, partitionKey, requestOptions, cancellationToken);

    public override Task<ItemResponse<T>> UpsertItemAsync<T>(T item, PartitionKey? partitionKey = null, ItemRequestOptions requestOptions = null,
        CancellationToken cancellationToken = new CancellationToken()) =>
        Container.UpsertItemAsync(item, partitionKey, requestOptions, cancellationToken);

    public override Task<ResponseMessage> ReplaceItemStreamAsync(Stream streamPayload, string id, PartitionKey partitionKey,
        ItemRequestOptions requestOptions = null, CancellationToken cancellationToken = new CancellationToken()) =>
        Container.ReplaceItemStreamAsync(streamPayload, id, partitionKey, requestOptions, cancellationToken);

    public override Task<ItemResponse<T>> ReplaceItemAsync<T>(T item, string id, PartitionKey? partitionKey = null, ItemRequestOptions requestOptions = null,
        CancellationToken cancellationToken = new CancellationToken()) =>
        Container.ReplaceItemAsync(item, id, partitionKey, requestOptions, cancellationToken);

    public override Task<ResponseMessage> ReadManyItemsStreamAsync(IReadOnlyList<(string id, PartitionKey partitionKey)> items, ReadManyRequestOptions readManyRequestOptions = null,
        CancellationToken cancellationToken = new CancellationToken()) =>
        Container.ReadManyItemsStreamAsync(items, readManyRequestOptions, cancellationToken);

    public override Task<FeedResponse<T>> ReadManyItemsAsync<T>(IReadOnlyList<(string id, PartitionKey partitionKey)> items, ReadManyRequestOptions readManyRequestOptions = null,
        CancellationToken cancellationToken = new CancellationToken()) =>
        Container.ReadManyItemsAsync<T>(items, readManyRequestOptions, cancellationToken);

    public override Task<ItemResponse<T>> PatchItemAsync<T>(string id, PartitionKey partitionKey, IReadOnlyList<PatchOperation> patchOperations,
        PatchItemRequestOptions requestOptions = null, CancellationToken cancellationToken = new CancellationToken()) =>
        Container.PatchItemAsync<T>(id, partitionKey, patchOperations, requestOptions, cancellationToken);

    public override Task<ResponseMessage> PatchItemStreamAsync(string id, PartitionKey partitionKey, IReadOnlyList<PatchOperation> patchOperations,
        PatchItemRequestOptions requestOptions = null, CancellationToken cancellationToken = new CancellationToken()) =>
        Container.PatchItemStreamAsync(id, partitionKey, patchOperations, requestOptions, cancellationToken);

    public override Task<ResponseMessage> DeleteItemStreamAsync(string id, PartitionKey partitionKey, ItemRequestOptions requestOptions = null,
        CancellationToken cancellationToken = new CancellationToken()) =>
        Container.DeleteItemStreamAsync(id, partitionKey, requestOptions, cancellationToken);

    public override Task<ItemResponse<T>> DeleteItemAsync<T>(string id, PartitionKey partitionKey, ItemRequestOptions requestOptions = null,
        CancellationToken cancellationToken = new CancellationToken()) =>
        Container.DeleteItemAsync<T>(id, partitionKey, requestOptions, cancellationToken);

    public override FeedIterator GetItemQueryStreamIterator(QueryDefinition queryDefinition, string continuationToken = null,
        QueryRequestOptions requestOptions = null) =>
        Container.GetItemQueryStreamIterator(queryDefinition, continuationToken, requestOptions);

    public override FeedIterator<T> GetItemQueryIterator<T>(QueryDefinition queryDefinition, string continuationToken = null,
        QueryRequestOptions requestOptions = null) =>
        Container.GetItemQueryIterator<T>(queryDefinition, continuationToken, requestOptions);

    public override FeedIterator GetItemQueryStreamIterator(string queryText = null, string continuationToken = null,
        QueryRequestOptions requestOptions = null) =>
        Container.GetItemQueryStreamIterator(queryText, continuationToken, requestOptions);

    public override FeedIterator<T> GetItemQueryIterator<T>(string queryText = null, string continuationToken = null,
        QueryRequestOptions requestOptions = null) =>
        Container.GetItemQueryIterator<T>(queryText, continuationToken, requestOptions);

    public override FeedIterator GetItemQueryStreamIterator(FeedRange feedRange, QueryDefinition queryDefinition, string continuationToken,
        QueryRequestOptions requestOptions = null) =>
        Container.GetItemQueryStreamIterator(feedRange, queryDefinition, continuationToken, requestOptions);

    public override FeedIterator<T> GetItemQueryIterator<T>(FeedRange feedRange, QueryDefinition queryDefinition,
        string continuationToken = null, QueryRequestOptions requestOptions = null) =>
        Container.GetItemQueryIterator<T>(feedRange, queryDefinition, continuationToken, requestOptions);

    public override IOrderedQueryable<T> GetItemLinqQueryable<T>(bool allowSynchronousQueryExecution = false, string continuationToken = null,
        QueryRequestOptions requestOptions = null, CosmosLinqSerializerOptions linqSerializerOptions = null) =>
        Container.GetItemLinqQueryable<T>(allowSynchronousQueryExecution, continuationToken, requestOptions, linqSerializerOptions);

    public override ChangeFeedProcessorBuilder GetChangeFeedProcessorBuilder<T>(string processorName, ChangesHandler<T> onChangesDelegate) => Container.GetChangeFeedProcessorBuilder(processorName, onChangesDelegate);

    public override ChangeFeedProcessorBuilder GetChangeFeedEstimatorBuilder(string processorName,
        ChangesEstimationHandler estimationDelegate, TimeSpan? estimationPeriod = null) =>
        Container.GetChangeFeedEstimatorBuilder(processorName, estimationDelegate, estimationPeriod);

    public override ChangeFeedEstimator GetChangeFeedEstimator(string processorName, Container leaseContainer) => Container.GetChangeFeedEstimator(processorName, leaseContainer);

    public override TransactionalBatch CreateTransactionalBatch(PartitionKey partitionKey) => Container.CreateTransactionalBatch(partitionKey);

    public override Task<IReadOnlyList<FeedRange>> GetFeedRangesAsync(CancellationToken cancellationToken = new CancellationToken()) => Container.GetFeedRangesAsync(cancellationToken);

    public override FeedIterator GetChangeFeedStreamIterator(ChangeFeedStartFrom changeFeedStartFrom, ChangeFeedMode changeFeedMode,
        ChangeFeedRequestOptions changeFeedRequestOptions = null) =>
        Container.GetChangeFeedStreamIterator(changeFeedStartFrom, changeFeedMode, changeFeedRequestOptions);

    public override FeedIterator<T> GetChangeFeedIterator<T>(ChangeFeedStartFrom changeFeedStartFrom, ChangeFeedMode changeFeedMode,
        ChangeFeedRequestOptions changeFeedRequestOptions = null) =>
        Container.GetChangeFeedIterator<T>(changeFeedStartFrom, changeFeedMode, changeFeedRequestOptions);

    public override ChangeFeedProcessorBuilder GetChangeFeedProcessorBuilder<T>(string processorName, ChangeFeedHandler<T> onChangesDelegate) => Container.GetChangeFeedProcessorBuilder(processorName, onChangesDelegate);

    public override ChangeFeedProcessorBuilder GetChangeFeedProcessorBuilderWithManualCheckpoint<T>(string processorName,
        ChangeFeedHandlerWithManualCheckpoint<T> onChangesDelegate) =>
        Container.GetChangeFeedProcessorBuilderWithManualCheckpoint(processorName, onChangesDelegate);

    public override ChangeFeedProcessorBuilder GetChangeFeedProcessorBuilder(string processorName,
        ChangeFeedStreamHandler onChangesDelegate) =>
        Container.GetChangeFeedProcessorBuilder(processorName, onChangesDelegate);

    public override ChangeFeedProcessorBuilder GetChangeFeedProcessorBuilderWithManualCheckpoint(string processorName,
        ChangeFeedStreamHandlerWithManualCheckpoint onChangesDelegate) =>
        Container.GetChangeFeedProcessorBuilderWithManualCheckpoint(processorName, onChangesDelegate);

    public override string Id => Container.Id;

    public override Database Database => Container.Database;

    public override Conflicts Conflicts => Container.Conflicts;

    public override Scripts Scripts => Container.Scripts;
}