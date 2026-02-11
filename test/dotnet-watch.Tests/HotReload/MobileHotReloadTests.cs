// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.DotNet.Watch.UnitTests;

public class MobileHotReloadTests(ITestOutputHelper logger) : DotNetWatchTestBase(logger)
{
    /// <summary>
    /// Tests that hot reload works for projects with the Android ProjectCapability.
    /// These projects use WebSocket transport instead of named pipes.
    /// </summary>
    [Fact]
    public async Task HotReload_WithAndroidCapability()
    {
        var testAsset = TestAssets.CopyTestAsset("WatchMobileApp")
            .WithSource();

        App.Start(testAsset, []);

        await App.WaitForOutputLineContaining("Started");
        await App.WaitForOutputLineContaining(MessageDescriptor.WaitingForChanges);

        // Verify the app is detected as mobile and uses WebSocket transport
        App.AssertOutputContains(MessageDescriptor.ApplicationKind_Mobile);
        App.AssertOutputContains("WebSocket server started at:");
        App.AssertOutputContains("WebSocket client connected");

        // Apply a hot reload change
        var programPath = Path.Combine(testAsset.Path, "Program.cs");
        UpdateSourceFile(programPath, src => src.Replace(
            """Console.WriteLine(".");""",
            """Console.WriteLine("Changed!");"""));

        await App.AssertOutputLineStartsWith("Changed!");
    }

    /// <summary>
    /// Tests that hot reload works for projects with the iOS ProjectCapability.
    /// </summary>
    [Fact]
    public async Task HotReload_WithiOSCapability()
    {
        var testAsset = TestAssets.CopyTestAsset("WatchMobileApp")
            .WithSource()
            .WithProjectChanges(project =>
            {
                // Change Android to iOS capability
                var capability = project.Root!.Descendants()
                    .First(e => e.Name.LocalName == "ProjectCapability" && e.Attribute("Include")?.Value == "Android");
                capability.SetAttributeValue("Include", "iOS");
            });

        App.Start(testAsset, []);

        await App.WaitForOutputLineContaining("Started");
        await App.WaitForOutputLineContaining(MessageDescriptor.WaitingForChanges);

        // Verify the app is detected as mobile and uses WebSocket transport
        App.AssertOutputContains(MessageDescriptor.ApplicationKind_Mobile);
        App.AssertOutputContains("WebSocket server started at:");
        App.AssertOutputContains("WebSocket client connected");

        // Apply a hot reload change
        var programPath = Path.Combine(testAsset.Path, "Program.cs");
        UpdateSourceFile(programPath, src => src.Replace(
            """Console.WriteLine(".");""",
            """Console.WriteLine("Changed!");"""));

        await App.AssertOutputLineStartsWith("Changed!");
    }
}
