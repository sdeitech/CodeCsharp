using App.Api.Controllers.Organization;
using App.Application.Dto.Organization;
using App.Application.Interfaces.Repositories.AuditLogs;
using App.Application.Interfaces.Repositories.Organization;
using App.Application.Interfaces.Services.AuthenticationModule;
using App.Application.Interfaces.Services.Images;
using App.Application.Interfaces.Services.Organization;
using App.Application.Service.Organization;
using App.Common.Constant;
using App.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;
using App.Application.Interfaces.Services.Images;
using App.Application.Interfaces.Services.AuthenticationModule;
using FluentAssertions;
using App.Application.Dto.Common;

namespace App.Services.Test.OrganizationModule
{
    public class OrganizationControllerTests
    {
        private readonly Mock<IOrganizationService> _mockOrganizationService;
        private readonly Mock<ICurrentUserClaimService> _mockCurrentUserClaimService;
        private readonly OrganizationController _controller;
        private readonly Mock<IHttpContextAccessor> _mockhttpContextAccessor;

        public OrganizationControllerTests()
        {
            _mockOrganizationService = new Mock<IOrganizationService>();
            _mockCurrentUserClaimService = new Mock<ICurrentUserClaimService>();
            _controller = new OrganizationController(_mockOrganizationService.Object, _mockCurrentUserClaimService.Object);
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _mockhttpContextAccessor = new Mock<IHttpContextAccessor>();
        }

        #region CreateOrganization

        [Fact]
        public async Task CreateOrganizationAsync_ShouldReturnOk_WhenOrganizationCreatedSuccessfully()
        {
            // Arrange
            var orgDto = new OrganizationDto
            {
                IsLocalStorage = true,
                LogoLocalPath = "logo.png",
                FavIconLocalPath = "favicon.png"
            };

            var mockImageService = new Mock<IImageService>();
            mockImageService.Setup(s => s.CreateFileNameWrtTime(It.IsAny<string>(), It.IsAny<string>())).Returns("logo_123.png");

            var mockRepo = new Mock<IOrganizationRepository>();
            mockRepo.Setup(r => r.CreateOrganizationAsync(It.IsAny<OrganizationDto>(), It.IsAny<int>()))
                    .ReturnsAsync(new JsonModel { Data = 101, Message = "Created" });

            var service = new OrganizationService(mockRepo.Object, mockImageService.Object, Mock.Of<IAuditLogRepository>(), _mockhttpContextAccessor.Object);

            // Act
            var result = await service.CreateOrganizationAsync(orgDto, 1);

            // Assert
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(StatusMessage.RecordSavedSuccessfully, result.Message);
            mockRepo.Verify(r => r.CreateOrganizationAsync(orgDto, 1), Times.Once);
        }

        [Fact]
        public async Task CreateOrganizationAsync_ShouldReturnInternalServerError_WhenRepositoryReturnsNull()
        {
            // Arrange
            var orgDto = new OrganizationDto();
            var mockRepo = new Mock<IOrganizationRepository>();
            mockRepo.Setup(r => r.CreateOrganizationAsync(It.IsAny<OrganizationDto>(), It.IsAny<int>()))
                    .ReturnsAsync((JsonModel)null);

            var service = new OrganizationService(mockRepo.Object, Mock.Of<IImageService>(), Mock.Of<IAuditLogRepository>(), _mockhttpContextAccessor.Object);

            // Act
            var result = await service.CreateOrganizationAsync(orgDto, 1);

            // Assert
            Assert.Equal((int)HttpStatusCode.InternalServerError, result.StatusCode);
            Assert.Equal(StatusMessage.InternalServerError, result.Message);
        }

        [Fact]
        public async Task CreateOrganizationAsync_ShouldReturnBadRequest_WhenRepositoryReturnsZeroId()
        {
            // Arrange
            var orgDto = new OrganizationDto();
            var mockRepo = new Mock<IOrganizationRepository>();
            mockRepo.Setup(r => r.CreateOrganizationAsync(It.IsAny<OrganizationDto>(), It.IsAny<int>()))
                    .ReturnsAsync(new JsonModel { Data = "0", Message = "Invalid Organization" });

            var service = new OrganizationService(mockRepo.Object, Mock.Of<IImageService>(), Mock.Of<IAuditLogRepository>(), _mockhttpContextAccessor.Object);

            // Act
            var result = await service.CreateOrganizationAsync(orgDto, 1);

            // Assert
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Equal("Invalid Organization", result.Message);
        }

        //[Fact]
        //public async Task CreateOrganizationAsync_Controller_ShouldReturn500_WhenExceptionThrown()
        //{
        //    // Arrange
        //    _mockCurrentUserClaimService.Setup(x => x.UserId).Returns(1);
        //    var orgDto = new OrganizationDto();
        //    var mockService = new Mock<IOrganizationService>();
        //    mockService.Setup(s => s.CreateOrganizationAsync(It.IsAny<OrganizationDto>(), It.IsAny<int>()))
        //               .ThrowsAsync(new Exception("Unexpected error"));

        //    var controller = new OrganizationController(mockService.Object, Mock.Of<ICurrentUserClaimService>());

        //    // Act
        //    var result = await controller.CreateOrganizationAsync(orgDto) as ObjectResult;

        //    // Assert
        //    Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);
        //}

        #endregion

        #region UpdateOrganization

        [Fact]
        public async Task UpdateOrganizationAsync_ShouldReturnOk_WhenStatusCodeIs200()
        {
            // Arrange
            var orgDto = new OrganizationDto();
            var response = new JsonModel { StatusCode = 200, Message = "Record Saved Successfully" };

            _mockOrganizationService.Setup(s => s.UpdateOrganizationAsync(orgDto, It.IsAny<int>()))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.UpdateOrganizationAsync(orgDto);

            // Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.Value.Should().BeEquivalentTo(response);
        }

        [Fact]
        public async Task UpdateOrganizationAsync_ShouldReturnBadRequest_WhenStatusCodeIs400()
        {
            // Arrange
            var orgDto = new OrganizationDto();
            var response = new JsonModel { StatusCode = 400, Message = "Invalid Request" };

            _mockOrganizationService.Setup(s => s.UpdateOrganizationAsync(orgDto, It.IsAny<int>()))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.UpdateOrganizationAsync(orgDto);

            // Assert
            var badRequest = result as BadRequestObjectResult;
            badRequest.Should().NotBeNull();
            badRequest.Value.Should().BeEquivalentTo(response);
        }

        [Fact]
        public async Task UpdateOrganizationAsync_ShouldReturn500_WhenStatusCodeIsNot200Or400()
        {
            // Arrange
            var orgDto = new OrganizationDto();
            var response = new JsonModel { StatusCode = 500, Message = "Internal Server Error" };

            _mockOrganizationService.Setup(s => s.UpdateOrganizationAsync(orgDto, It.IsAny<int>()))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.UpdateOrganizationAsync(orgDto);

            // Assert
            var objectResult = result as ObjectResult;
            objectResult.StatusCode.Should().Be(500);
            objectResult.Value.Should().BeEquivalentTo(response);
        }

        #endregion

        #region OrganizatonStatusUpdate

        [Fact]
        public async Task OrganizationStatusUpdateAsync_ShouldReturnOk_WhenStatusCodeIs200()
        {
            // Arrange
            var orgDto = new OrganizationStatusUpdateDto{ OrganizationID = 1, IsActive = true };
            var response = new JsonModel
            {
                StatusCode = 200,
                Message = "Record Saved Successfully"
            };

            _mockOrganizationService
                .Setup(s => s.OrganizationStatusUpdateAsync(orgDto, It.IsAny<int>()))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.OrganizationStatusUpdateAsync(orgDto);

            // Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().BeEquivalentTo(response);
        }

        [Fact]
        public async Task OrganizationStatusUpdateAsync_ShouldReturnBadRequest_WhenStatusCodeIs400()
        {
            // Arrange
            var orgDto = new OrganizationStatusUpdateDto{ OrganizationID = 1, IsActive = true };
            var response = new JsonModel
            {
                StatusCode = 400,
                Message = "Invalid Data"
            };

            _mockOrganizationService
                .Setup(s => s.OrganizationStatusUpdateAsync(orgDto, It.IsAny<int>()))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.OrganizationStatusUpdateAsync(orgDto);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult!.Value.Should().BeEquivalentTo(response);
        }

        [Fact]
        public async Task OrganizationStatusUpdateAsync_ShouldReturnInternalServerError_WhenUnexpectedStatusCode()
        {
            // Arrange
            var orgDto = new OrganizationStatusUpdateDto{ OrganizationID = 1, IsActive = true };
            var response = new JsonModel
            {
                StatusCode = 500,
                Message = "Internal Server Error"
            };

            _mockOrganizationService
                .Setup(s => s.OrganizationStatusUpdateAsync(orgDto, It.IsAny<int>()))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.OrganizationStatusUpdateAsync(orgDto);

            // Assert
            var objectResult = result as ObjectResult;
            objectResult.Should().NotBeNull();
            objectResult!.StatusCode.Should().Be(500);
            objectResult.Value.Should().BeEquivalentTo(response);
        }

        #endregion

        #region GetAllOrganization

        [Fact]
        public async Task GetAllOrganizationsAsync_ShouldReturnOk_WhenStatusCodeIs200()
        {
            // Arrange
            var filter = new FilterDto { PageNumber = 1, PageSize = 10 };
            var data = new List<OrganizationReponseDto>
            {
                new OrganizationReponseDto { OrganizationID = 1, OrganizationName = "Test Org" }
            };

            var response = new JsonModel { Data = data, StatusCode = (int)HttpStatusCode.OK };

            _mockOrganizationService
                .Setup(s => s.GetAllOrganizationsAsync(filter))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.GetAllOrganizationsAsync(filter);

            // Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().BeEquivalentTo(response);
        }

        [Fact]
        public async Task GetAllOrganizationsAsync_ShouldReturnBadRequest_WhenStatusCodeIs400()
        {
            // Arrange
            var filter = new FilterDto { PageNumber = 1, PageSize = 10 };
            var response = new JsonModel { Message = "Invalid Request", StatusCode = (int)HttpStatusCode.BadRequest };

            _mockOrganizationService
                .Setup(s => s.GetAllOrganizationsAsync(filter))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.GetAllOrganizationsAsync(filter);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult!.Value.Should().BeEquivalentTo(response);
        }

        [Fact]
        public async Task GetAllOrganizationsAsync_ShouldReturnInternalServerError_WhenStatusCodeIs500()
        {
            // Arrange
            var filter = new FilterDto { PageNumber = 1, PageSize = 10 };
            var response = new JsonModel { Message = "Internal Server Error", StatusCode = (int)HttpStatusCode.InternalServerError };

            _mockOrganizationService
                .Setup(s => s.GetAllOrganizationsAsync(filter))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.GetAllOrganizationsAsync(filter);

            // Assert
            var objectResult = result as ObjectResult;
            objectResult.Should().NotBeNull();
            objectResult!.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
            objectResult.Value.Should().BeEquivalentTo(response);
        }

        #endregion

        #region GetOrganizationById

        [Fact]
        public async Task GetOrganizationByIdAsync_ShouldReturnOk_WhenStatusCodeIs200()
        {
            // Arrange
            var organizationId = 1;
            var response = new JsonModel
            {
                Data = new OrganizationReponseDto { OrganizationID = 1, OrganizationName = "Test Org" },
                StatusCode = (int)HttpStatusCode.OK
            };

            _mockOrganizationService
                .Setup(s => s.GetOrganizationByIdAsync(
                    organizationId, It.IsAny<IHttpContextAccessor>()))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.GetOrganizationByIdAsync(organizationId);

            // Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().BeEquivalentTo(response);
        }

        [Fact]
        public async Task GetOrganizationByIdAsync_ShouldReturnBadRequest_WhenStatusCodeIs400()
        {
            // Arrange
            var organizationId = 1;
            var response = new JsonModel
            {
                Message = "Invalid Request",
                StatusCode = (int)HttpStatusCode.BadRequest
            };

            _mockOrganizationService
                .Setup(s => s.GetOrganizationByIdAsync(
                    organizationId, It.IsAny<IHttpContextAccessor>()))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.GetOrganizationByIdAsync(organizationId);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult!.Value.Should().BeEquivalentTo(response);
        }

        [Fact]
        public async Task GetOrganizationByIdAsync_ShouldReturnInternalServerError_WhenStatusCodeIs500()
        {
            // Arrange
            var organizationId = 1;
            var response = new JsonModel
            {
                Message = "Internal Server Error",
                StatusCode = (int)HttpStatusCode.InternalServerError
            };

            _mockOrganizationService
                .Setup(s => s.GetOrganizationByIdAsync(
                    organizationId, It.IsAny<IHttpContextAccessor>()))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.GetOrganizationByIdAsync(organizationId);

            // Assert
            var objectResult = result as ObjectResult;
            objectResult.Should().NotBeNull();
            objectResult!.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
            objectResult.Value.Should().BeEquivalentTo(response);
        }

        #endregion
    }
}
