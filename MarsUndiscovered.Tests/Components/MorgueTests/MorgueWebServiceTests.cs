﻿using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using FrigidRogue.TestInfrastructure;
using MarsUndiscovered.Components;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
#pragma warning disable CS4014

namespace MarsUndiscovered.Tests.Components.MorgueTests;

[TestClass]
public class MorgueWebServiceTests : BaseTest
{
    private MorgueWebService _morgueWebService;
    private IHttpClient _testHttpClient;

    [TestInitialize]
    public override void Setup()
    {
        base.Setup();
        
        _testHttpClient = Substitute.For<IHttpClient>();
        
        Environment.SetEnvironmentVariable("SENDMORGUE_BASEADDRESS", "localhost");
        Environment.SetEnvironmentVariable("SENDMORGUE_PORT", "9999");
        Environment.SetEnvironmentVariable("SENDMORGUE_ENDPOINT", "api/morgue");
        
        _morgueWebService = new MorgueWebService(_testHttpClient);
    }

    [TestMethod]
    public async Task Should_SendMorgue_Successfully()
    {
        // Arrange
        var morgueExportData = new MorgueExportData
        {
            Id = Guid.NewGuid()
        };
        
        var expectedUri = "https://localhost:9999/api/morgue";

        _testHttpClient
            .PostAsync(Arg.Is<Uri>(a => a.ToString().Equals(expectedUri)), Arg.Any<StringContent>())
            .Returns(new HttpResponseMessage(HttpStatusCode.OK));

        // Act
        await _morgueWebService.SendMorgue(morgueExportData);

        // Assert
        _testHttpClient
            .Received()
            .PostAsync(Arg.Is<Uri>(a => a.ToString().Equals(expectedUri)), Arg.Any<StringContent>());
    }
    
    [TestMethod]
    public async Task SendMorgue_Should_Throw_Exception_When_Returns_Unsuccessful_Result()
    {
        // Arrange
        var morgueExportData = new MorgueExportData
        {
            Id = Guid.NewGuid()
        };
        
        var expectedUri = "https://localhost:9999/api/morgue";

        _testHttpClient
            .PostAsync(Arg.Is<Uri>(a => a.ToString().Equals(expectedUri)), Arg.Any<StringContent>())
            .Returns(new HttpResponseMessage(HttpStatusCode.InternalServerError));

        // Act
        var exception = await Assert.ThrowsExceptionAsync<HttpRequestException>(async () => await _morgueWebService.SendMorgue(morgueExportData));

        // Assert
        Assert.AreEqual("Response status code does not indicate success: 500 (Internal Server Error).", exception.Message);
    }
}