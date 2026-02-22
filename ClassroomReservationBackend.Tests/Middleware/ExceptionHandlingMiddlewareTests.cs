using System.Net;
using ClassroomReservationBackend.Middleware;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;

namespace ClassroomReservationBackend.Tests;

[TestFixture]
public class ExceptionHandlingMiddlewareTests
{
    private DefaultHttpContext _httpContext;

    [SetUp]
    public void Setup()
    {
        _httpContext = new DefaultHttpContext();
        _httpContext.Response.Body = new MemoryStream();
    }

    [Test]
    public async Task InvokeAsync_WhenKeyNotFoundException_Returns404()
    {
        var middleware = new ExceptionHandlingMiddleware(_ =>
            throw new KeyNotFoundException("Not found"));
        await middleware.InvokeAsync(_httpContext);
        Assert.That(_httpContext.Response.StatusCode, Is.EqualTo((int)HttpStatusCode.NotFound));
    }

    [Test]
    public async Task InvokeAsync_WhenUnauthorizedAccessException_Returns401()
    {
        var middleware = new ExceptionHandlingMiddleware(_ =>
            throw new UnauthorizedAccessException("Unauthorized"));
        await middleware.InvokeAsync(_httpContext);
        Assert.That(_httpContext.Response.StatusCode, Is.EqualTo((int)HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task InvokeAsync_WhenInvalidOperationException_Returns409()
    {
        var middleware = new ExceptionHandlingMiddleware(_ =>
            throw new InvalidOperationException("Conflict"));
        await middleware.InvokeAsync(_httpContext);
        Assert.That(_httpContext.Response.StatusCode, Is.EqualTo((int)HttpStatusCode.Conflict));
    }

    [Test]
    public async Task InvokeAsync_WhenUnhandledException_Returns500()
    {
        var middleware = new ExceptionHandlingMiddleware(_ =>
            throw new Exception("Server error"));
        await middleware.InvokeAsync(_httpContext);
        Assert.That(_httpContext.Response.StatusCode, Is.EqualTo((int)HttpStatusCode.InternalServerError));
    }

    [Test]
    public async Task InvokeAsync_WhenNoException_PassesThrough()
    {
        var middleware = new ExceptionHandlingMiddleware(_ => Task.CompletedTask);
        await middleware.InvokeAsync(_httpContext);
        Assert.That(_httpContext.Response.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
    }
}