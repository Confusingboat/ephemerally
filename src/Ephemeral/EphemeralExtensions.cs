﻿namespace Ephemeral;

public static class EphemeralExtensions
{
    public static EphemeralOptions OrDefault(this EphemeralOptions options) =>
        options ?? EphemeralOptions.Default;

    public static bool IsExpired(this EphemeralMetadata metadata) =>
        IsExpired(metadata, DateTimeOffset.UtcNow);

    internal static bool IsExpired(this EphemeralMetadata metadata, DateTimeOffset now) =>
        metadata.Expiration.HasValue && metadata.Expiration.Value <= now;

    public static EphemeralMetadata GetContainerMetadata(this string fullName) =>
        EphemeralMetadata.New(fullName);
}