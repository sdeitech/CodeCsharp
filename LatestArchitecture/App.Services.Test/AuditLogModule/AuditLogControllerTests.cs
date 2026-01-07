using App.Api.Controllers.AuditLog;
using App.Application.Dto.AuditLog;
using App.Application.Dto.Common;
using App.Application.Interfaces.Services.AuditLog;
using App.Common.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;

namespace App.Services.Test.AuditLogModule
{
    public class AuditLogControllerTests
    {
        private readonly Mock<IAuditLogService> _mockAuditLogService;
        private readonly AuditLogController _controller;

        public AuditLogControllerTests()
        {
            _mockAuditLogService = new Mock<IAuditLogService>();
            _controller = new AuditLogController(_mockAuditLogService.Object);
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
        }

        [Fact]
        public async Task GetAuditLogAsync_ShouldReturnOk_WhenStatusCodeIs200()
        {
            // Arrange
            var filter = new FilterDto { PageNumber = 1, PageSize = 10 };
            var data = new List<AuditLogResponseDto>
            {
                new AuditLogResponseDto { ActionID = 2/*update id = 2*/, OrganizationID = 1 }
            };

            var response = new JsonModel { Data = data, StatusCode = (int)HttpStatusCode.OK };

            _mockAuditLogService
                .Setup(s => s.GetAuditLogsAsync(filter))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.GetAuditLogAsync(filter);

            // Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().BeEquivalentTo(response);
        }

        [Fact]
        public async Task GetAuditLogAsync_ShouldReturnBadRequest_WhenStatusCodeIs400()
        {
            // Arrange
            var filter = new FilterDto { PageNumber = 1, PageSize = 10 };
            var response = new JsonModel
            {
                Message = "Invalid Request",
                StatusCode = (int)HttpStatusCode.BadRequest
            };

            _mockAuditLogService
                .Setup(s => s.GetAuditLogsAsync(filter))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.GetAuditLogAsync(filter);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult!.Value.Should().BeEquivalentTo(response);
        }

        [Fact]
        public async Task GetAuditLogAsync_ShouldReturnInternalServerError_WhenStatusCodeIs500()
        {
            // Arrange
            var filter = new FilterDto { PageNumber = 1, PageSize = 10 };
            var response = new JsonModel
            {
                Message = "Internal Server Error",
                StatusCode = (int)HttpStatusCode.InternalServerError
            };

            _mockAuditLogService
                .Setup(s => s.GetAuditLogsAsync(filter))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.GetAuditLogAsync(filter);

            // Assert
            var objectResult = result as ObjectResult;
            objectResult.Should().NotBeNull();
            objectResult!.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
            objectResult.Value.Should().BeEquivalentTo(response);
        }
    }
}
