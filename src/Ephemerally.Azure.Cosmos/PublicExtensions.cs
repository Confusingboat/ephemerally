﻿using Ephemerally.Azure.Cosmos;
using Microsoft.Azure.Cosmos;

// ReSharper disable once CheckNamespace
namespace Ephemerally;

public static class PublicExtensions
{
    private const string DefaultPartitionKeyPath = "/id";

    public static EphemeralCosmosDatabase ToEphemeral(this Database database, EphemeralOptions options = default) =>
        new(new CosmosDatabaseEphemeral(database, options));

    public static EphemeralCosmosContainer ToEphemeral(this Container container, EphemeralOptions options = default) =>
        new(new CosmosContainerEphemeral(container, options));

    public static async Task<EphemeralCosmosDatabase> CreateEphemeralDatabaseAsync(
        this CosmosClient client,
        EphemeralCreationOptions options = default)
    {
        var metadata = options.OrDefault().GetNewMetadata();
        var response = await client.CreateDatabaseIfNotExistsAsync(metadata.FullName).ConfigureAwait(false);
        return client.GetDatabase(response.Resource.Id).ToEphemeral(options);
    }

    public static async Task<EphemeralCosmosContainer> CreateEphemeralContainerAsync(
        this Database database,
        EphemeralCreationOptions options = default,
        ContainerProperties containerProperties = default,
        ThroughputProperties throughputProperties = default)
    {
        var metadata = options.OrDefault().GetNewMetadata();
        containerProperties ??= new();
        containerProperties.Id ??= metadata.FullName;
        containerProperties.PartitionKeyPath ??= DefaultPartitionKeyPath;
        var response = await database.CreateContainerIfNotExistsAsync(containerProperties, throughputProperties).ConfigureAwait(false);
        return database.GetContainer(response.Resource.Id).ToEphemeral(options);
    }

    public static IEphemeralMetadata GetEphemeralMetadata(this DatabaseProperties container) =>
        container.Id.GetContainerMetadata();
    
    public static IEphemeralMetadata GetEphemeralMetadata(this Database container) =>
        container.Id.GetContainerMetadata();

    public static IEphemeralMetadata GetEphemeralMetadata(this ContainerProperties container) =>
        container.Id.GetContainerMetadata();

    public static IEphemeralMetadata GetEphemeralMetadata(this Container container) =>
        container.Id.GetContainerMetadata();
}