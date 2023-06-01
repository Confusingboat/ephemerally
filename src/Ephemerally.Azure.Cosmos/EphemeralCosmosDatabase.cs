using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Fluent;

namespace Ephemerally.Azure.Cosmos;

public class EphemeralCosmosDatabase : Database, IAsyncDisposable
{
    private readonly IEphemeral<Database> _databaseEphemeral;

    private Database Database => _databaseEphemeral.Value;

    public EphemeralCosmosDatabase(IEphemeral<Database> databaseEphemeral)
    {
        _databaseEphemeral = databaseEphemeral;
    }

    public ValueTask DisposeAsync() => _databaseEphemeral.DisposeAsync();

    public override Task<DatabaseResponse> ReadAsync(RequestOptions requestOptions = null, CancellationToken cancellationToken = new CancellationToken()) => Database.ReadAsync(requestOptions, cancellationToken);

    public override Task<DatabaseResponse> DeleteAsync(RequestOptions requestOptions = null, CancellationToken cancellationToken = new CancellationToken()) => Database.DeleteAsync(requestOptions, cancellationToken);

    public override Task<int?> ReadThroughputAsync(CancellationToken cancellationToken = new CancellationToken()) => Database.ReadThroughputAsync(cancellationToken);

    public override Task<ThroughputResponse> ReadThroughputAsync(RequestOptions requestOptions, CancellationToken cancellationToken = new CancellationToken()) => Database.ReadThroughputAsync(requestOptions, cancellationToken);

    public override Task<ThroughputResponse> ReplaceThroughputAsync(ThroughputProperties throughputProperties, RequestOptions requestOptions = null,
        CancellationToken cancellationToken = new CancellationToken()) =>
        Database.ReplaceThroughputAsync(throughputProperties, requestOptions, cancellationToken);

    public override Task<ContainerResponse> CreateContainerAsync(ContainerProperties containerProperties, ThroughputProperties throughputProperties,
        RequestOptions requestOptions = null, CancellationToken cancellationToken = new CancellationToken()) =>
        Database.CreateContainerAsync(containerProperties, throughputProperties, requestOptions, cancellationToken);

    public override Task<ContainerResponse> CreateContainerIfNotExistsAsync(ContainerProperties containerProperties, ThroughputProperties throughputProperties,
        RequestOptions requestOptions = null, CancellationToken cancellationToken = new CancellationToken()) =>
        Database.CreateContainerIfNotExistsAsync(containerProperties, throughputProperties, requestOptions, cancellationToken);

    public override Task<ResponseMessage> CreateContainerStreamAsync(ContainerProperties containerProperties, ThroughputProperties throughputProperties,
        RequestOptions requestOptions = null, CancellationToken cancellationToken = new CancellationToken()) =>
        Database.CreateContainerStreamAsync(containerProperties, throughputProperties, requestOptions, cancellationToken);

    public override Task<ThroughputResponse> ReplaceThroughputAsync(int throughput, RequestOptions requestOptions = null,
        CancellationToken cancellationToken = new CancellationToken()) =>
        Database.ReplaceThroughputAsync(throughput, requestOptions, cancellationToken);

    public override Task<ResponseMessage> ReadStreamAsync(RequestOptions requestOptions = null,
        CancellationToken cancellationToken = new CancellationToken()) =>
        Database.ReadStreamAsync(requestOptions, cancellationToken);

    public override Task<ResponseMessage> DeleteStreamAsync(RequestOptions requestOptions = null,
        CancellationToken cancellationToken = new CancellationToken()) =>
        Database.DeleteStreamAsync(requestOptions, cancellationToken);

    public override Container GetContainer(string id) => Database.GetContainer(id);

    public override Task<ContainerResponse> CreateContainerAsync(ContainerProperties containerProperties, int? throughput = null,
        RequestOptions requestOptions = null, CancellationToken cancellationToken = new CancellationToken()) =>
        Database.CreateContainerAsync(containerProperties, throughput, requestOptions, cancellationToken);

    public override Task<ContainerResponse> CreateContainerAsync(string id, string partitionKeyPath, int? throughput = null,
        RequestOptions requestOptions = null, CancellationToken cancellationToken = new CancellationToken()) =>
        Database.CreateContainerAsync(id, partitionKeyPath, throughput, requestOptions, cancellationToken);

    public override Task<ContainerResponse> CreateContainerIfNotExistsAsync(ContainerProperties containerProperties, int? throughput = null,
        RequestOptions requestOptions = null, CancellationToken cancellationToken = new CancellationToken()) =>
        Database.CreateContainerIfNotExistsAsync(containerProperties, throughput, requestOptions, cancellationToken);

    public override Task<ContainerResponse> CreateContainerIfNotExistsAsync(string id, string partitionKeyPath, int? throughput = null,
        RequestOptions requestOptions = null, CancellationToken cancellationToken = new CancellationToken()) =>
        Database.CreateContainerIfNotExistsAsync(id, partitionKeyPath, throughput, requestOptions, cancellationToken);

    public override Task<ResponseMessage> CreateContainerStreamAsync(ContainerProperties containerProperties, int? throughput = null,
        RequestOptions requestOptions = null, CancellationToken cancellationToken = new CancellationToken()) =>
        Database.CreateContainerStreamAsync(containerProperties, throughput, requestOptions, cancellationToken);

    public override User GetUser(string id) => Database.GetUser(id);

    public override Task<UserResponse> CreateUserAsync(string id, RequestOptions requestOptions = null,
        CancellationToken cancellationToken = new CancellationToken()) =>
        Database.CreateUserAsync(id, requestOptions, cancellationToken);

    public override Task<UserResponse> UpsertUserAsync(string id, RequestOptions requestOptions = null,
        CancellationToken cancellationToken = new CancellationToken()) =>
        Database.UpsertUserAsync(id, requestOptions, cancellationToken);

    public override FeedIterator<T> GetContainerQueryIterator<T>(QueryDefinition queryDefinition, string continuationToken = null,
        QueryRequestOptions requestOptions = null) =>
        Database.GetContainerQueryIterator<T>(queryDefinition, continuationToken, requestOptions);

    public override FeedIterator GetContainerQueryStreamIterator(QueryDefinition queryDefinition, string continuationToken = null,
        QueryRequestOptions requestOptions = null) =>
        Database.GetContainerQueryStreamIterator(queryDefinition, continuationToken, requestOptions);

    public override FeedIterator<T> GetContainerQueryIterator<T>(string queryText = null, string continuationToken = null,
        QueryRequestOptions requestOptions = null) =>
        Database.GetContainerQueryIterator<T>(queryText, continuationToken, requestOptions);

    public override FeedIterator GetContainerQueryStreamIterator(string queryText = null, string continuationToken = null,
        QueryRequestOptions requestOptions = null) =>
        Database.GetContainerQueryStreamIterator(queryText, continuationToken, requestOptions);

    public override FeedIterator<T> GetUserQueryIterator<T>(string queryText = null, string continuationToken = null,
        QueryRequestOptions requestOptions = null) =>
        Database.GetUserQueryIterator<T>(queryText, continuationToken, requestOptions);

    public override FeedIterator<T> GetUserQueryIterator<T>(QueryDefinition queryDefinition, string continuationToken = null,
        QueryRequestOptions requestOptions = null) =>
        Database.GetUserQueryIterator<T>(queryDefinition, continuationToken, requestOptions);

    public override ContainerBuilder DefineContainer(string name, string partitionKeyPath) => Database.DefineContainer(name, partitionKeyPath);

    public override ClientEncryptionKey GetClientEncryptionKey(string id) => Database.GetClientEncryptionKey(id);

    public override FeedIterator<ClientEncryptionKeyProperties> GetClientEncryptionKeyQueryIterator(QueryDefinition queryDefinition, string continuationToken = null,
        QueryRequestOptions requestOptions = null) =>
        Database.GetClientEncryptionKeyQueryIterator(queryDefinition, continuationToken, requestOptions);

    public override Task<ClientEncryptionKeyResponse> CreateClientEncryptionKeyAsync(ClientEncryptionKeyProperties clientEncryptionKeyProperties,
        RequestOptions requestOptions = null, CancellationToken cancellationToken = new CancellationToken()) =>
        Database.CreateClientEncryptionKeyAsync(clientEncryptionKeyProperties, requestOptions, cancellationToken);

    public override string Id => Database.Id;

    public override CosmosClient Client => Database.Client;
}