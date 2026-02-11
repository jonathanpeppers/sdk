// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System;

namespace Microsoft.DotNet.HotReload;

/// <summary>
/// URL-safe Base64 encoding/decoding helpers.
/// WebSocket subprotocol tokens cannot contain +, /, or = characters,
/// so we use URL-safe Base64 (RFC 4648 Section 5).
/// </summary>
internal static class Base64Url
{
    /// <summary>
    /// Encodes binary data to URL-safe Base64.
    /// </summary>
    internal static string Encode(byte[] data)
        => Encode(Convert.ToBase64String(data));

#if NET
    /// <summary>
    /// Encodes binary data to URL-safe Base64.
    /// </summary>
    internal static string Encode(ReadOnlySpan<byte> data)
        => Encode(Convert.ToBase64String(data));
#endif

    /// <summary>
    /// Converts standard Base64 to URL-safe Base64.
    /// Replaces + with -, / with _, and removes padding =.
    /// </summary>
    internal static string Encode(string standardBase64)
    {
        return standardBase64
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');
    }

    /// <summary>
    /// Decodes URL-safe Base64 to binary data.
    /// </summary>
    internal static byte[] DecodeBytes(string urlSafeBase64)
        => Convert.FromBase64String(Decode(urlSafeBase64));

    /// <summary>
    /// Converts URL-safe Base64 back to standard Base64.
    /// Replaces - with +, _ with /, and adds padding if needed.
    /// </summary>
    internal static string Decode(string urlSafeBase64)
    {
        var standardBase64 = urlSafeBase64
            .Replace('-', '+')
            .Replace('_', '/');

        // Add padding if needed (Base64 length must be multiple of 4)
        var paddingNeeded = (4 - standardBase64.Length % 4) % 4;
        if (paddingNeeded > 0)
        {
            standardBase64 += new string('=', paddingNeeded);
        }

        return standardBase64;
    }
}
