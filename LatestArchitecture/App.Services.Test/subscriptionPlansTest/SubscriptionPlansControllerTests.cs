     
using App.Api.Controllers;
using App.Application.Dto.Common;
using App.Application.Dto.SubscriptionPlan;
using App.Application.Interfaces.Services.SubscriptionPlan;
using App.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace App.Services.Test.subscriptionPlansTest
{
    public class SubscriptionPlansControllerTests
    {
        private readonly Mock<ISubscriptionPlansService> _mockSubscriptionPlansService;
        private readonly SubscriptionPlansController _controller;

        public SubscriptionPlansControllerTests()
        {
            _mockSubscriptionPlansService = new Mock<ISubscriptionPlansService>();
            _controller = new SubscriptionPlansController(_mockSubscriptionPlansService.Object);
        }

        #region SaveSubscriptionPlan Tests

        [Fact]
        public async Task SaveSubscriptionPlan_ReturnsOkResult_WhenPlanIsValid()
        {
            // Arrange
            var subscriptionPlansDTO = new SubscriptionPlansDTO
            {
                PlanId = 1,
                PlanName = "Basic",
                PlatFormRate = 9.99m,
                IsLicensed = true
            };
                
            var expectedResult = new JsonModel { StatusCode = 200, Message = "Plan saved successfully" };

            _mockSubscriptionPlansService
                .Setup(service => service.SaveSubscriptionPlan(It.IsAny<SubscriptionPlansDTO>()))
                .ReturnsAsync(expectedResult); // Use ReturnsAsync for Task<JsonModel>

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            // Act
            var result = await _controller.SaveSubscriptionPlan(subscriptionPlansDTO); // Use await for async method

            // Assert
            var okResult = Assert.IsAssignableFrom<OkObjectResult>(result);
            Assert.Equal(expectedResult, okResult.Value);
        }



        [Fact]
        public async Task SaveSubscriptionPlan_Returns500_WhenServiceThrowsException()
        {
            var subscriptionPlansDTO = new SubscriptionPlansDTO();

            _mockSubscriptionPlansService
                .Setup(s => s.SaveSubscriptionPlan(It.IsAny<SubscriptionPlansDTO>()))
                .ThrowsAsync(new Exception("Database error"));

            await Assert.ThrowsAnyAsync<Exception>(async () =>
                await _controller.SaveSubscriptionPlan(subscriptionPlansDTO));
        }
        [Fact]
        public async Task DeleteSubscriptionPlan_ReturnsOkResult_WhenDeleteIsSuccessful()
        {
            var id = 1;
            var expectedJson = new JsonModel { StatusCode = 200, Message = "Deleted" };

            _mockSubscriptionPlansService
                .Setup(service => service.DeleteSubscriptionPlan(It.IsAny<int>()))
                .ReturnsAsync(expectedJson); // Async return

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            var result = await _controller.DeleteSubscriptionPlan(id); // Await async call

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedJson, okResult.Value);
        }
        [Fact]
        public async Task DeleteSubscriptionPlan_Returns500_WhenServiceThrowsException()
        {
            var id = 999;

            _mockSubscriptionPlansService
                .Setup(s => s.DeleteSubscriptionPlan(id))
                .ThrowsAsync(new NullReferenceException("Object reference not set to an instance of an object."));

            await Assert.ThrowsAnyAsync<Exception>(async () =>
                await _controller.DeleteSubscriptionPlan(id));
        }
        #endregion

        #region GetSubscriptionPlanId Tests

        [Fact]
        public async Task GetSubscriptionPlanId_ReturnsOkResult_WhenPlanExists()
        {
            var id = 1;
            var expectedResult = new JsonModel { StatusCode = 200, Data = new SubscriptionPlansDTO { PlanId = 1, PlanName = "Basic" } };

            _mockSubscriptionPlansService
                .Setup(service => service.GetSubscriptionPlanId(id))
                .ReturnsAsync(expectedResult); // Async return

            var result = await _controller.GetSubscriptionPlanId(id);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedResult, okResult.Value);
        }
        [Fact]
        public async Task GetSubscriptionPlanId_Returns500_WhenServiceThrowsException()
        {
            var id = 999;

            _mockSubscriptionPlansService
                .Setup(s => s.GetSubscriptionPlanId(id))
                .ThrowsAsync(new Exception("Service error"));

            await Assert.ThrowsAnyAsync<Exception>(async () =>
                await _controller.GetSubscriptionPlanId(id));
        }
        #endregion

        #region GetAllSubscriptionPlan Tests

        [Fact]
        public async Task GetAllSubscriptionPlan_ReturnsOkResult_WhenPlansExist()
        {
            var listingFiltterDTO = new ListingFiltterDTO();
            var expectedResult = new JsonModel { StatusCode = 200, Data = new List<SubscriptionPlansDTO>() };

            _mockSubscriptionPlansService
                .Setup(service => service.GetAllSubscriptionPlan(listingFiltterDTO))
                .ReturnsAsync(expectedResult); // Async return

            var result = await _controller.GetAllSubscriptionPlan(listingFiltterDTO);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedResult, okResult.Value);
        }
        [Fact]
        public async Task GetAllSubscriptionPlan_Returns500_WhenServiceThrowsException()
        {
            var listingFiltterDTO = new ListingFiltterDTO();

            _mockSubscriptionPlansService
                .Setup(s => s.GetAllSubscriptionPlan(listingFiltterDTO))
                .ThrowsAsync(new Exception("Service error"));

            await Assert.ThrowsAnyAsync<Exception>(async () =>
                await _controller.GetAllSubscriptionPlan(listingFiltterDTO));
        }
        #endregion

        #region GetAllSubscriptionPlans Tests

        [Fact]
        public async Task GetAllSubscriptionPlans_ReturnsOkResult_WhenPlansExist()
        {
            var expectedResult = new JsonModel { StatusCode = 200, Data = new List<SubscriptionPlansDTO>() };

            _mockSubscriptionPlansService
                .Setup(service => service.GetAllSubscriptionPlans())
                .ReturnsAsync(expectedResult); // Async return

            var result = await _controller.GetAllSubscriptionPlans();

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedResult, okResult.Value);
        }
        [Fact]
        public async Task GetAllSubscriptionPlans_Returns500_WhenServiceThrowsException()
        {
            _mockSubscriptionPlansService
                .Setup(s => s.GetAllSubscriptionPlans())
                .ThrowsAsync(new Exception("Service error"));

            await Assert.ThrowsAnyAsync<Exception>(async () =>
                await _controller.GetAllSubscriptionPlans());
        }
        #endregion

        #region GetAllModuleList Tests



        [Fact]
        public async Task GetAllModuleList_Returns500_WhenServiceThrowsException()
        {
            _mockSubscriptionPlansService
                .Setup(s => s.GetAllModuleList())
                .ThrowsAsync(new Exception("Service error"));

            await Assert.ThrowsAnyAsync<Exception>(async () =>
                await _controller.GetAllModuleList());
        }
        #endregion

        #region GetSubscriptionPlans Tests

        [Fact]
        public async Task GetSubscriptionPlans_ReturnsOkResult_WhenPlansExist()
        {
            // Arrange
            var filterDto = new FilterDto
            {
                SearchTerm = "",
                SortColumn = "",
                SortOrder = "",
                PageNumber = 1,
                PageSize = 10
            };

            var expectedResult = new JsonModel
            {
                StatusCode = 200,
                Data = new List<SubscriptionPlansDTO>()
            };

            _mockSubscriptionPlansService
                .Setup(service => service.GetSubscriptionPlans(filterDto))
                .ReturnsAsync(expectedResult); // Use ReturnsAsync for async methods

            // Act
            var result = await _controller.GetSubscriptionPlans(filterDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualResult = Assert.IsType<JsonModel>(okResult.Value);
            Assert.Equal(expectedResult.StatusCode, actualResult.StatusCode);
            Assert.Equal(expectedResult.Data, actualResult.Data);
        }

        [Fact]
        public async Task GetSubscriptionPlans_Returns500_WhenServiceThrowsException()
        {
            var filterDto = new FilterDto
            {
                SearchTerm = "",
                SortColumn = "",
                SortOrder = "",
                PageNumber = 1,
                PageSize = 10
            };

            _mockSubscriptionPlansService
                .Setup(s => s.GetSubscriptionPlans(filterDto))
                .ThrowsAsync(new Exception("Service error"));

            await Assert.ThrowsAnyAsync<Exception>(async () =>
                await _controller.GetSubscriptionPlans(filterDto));
        }
        #endregion

        #region SetActiveInActive Tests

        [Fact]
        public async Task SetActiveInActive_ReturnsOk_WhenUpdateIsSuccessful()
        {
            // Arrange
            var planId = 1;
            var value = true;

            var expectedResult = new JsonModel
            {
                StatusCode = StatusCodes.Status200OK,
                Data = true
            };

            _mockSubscriptionPlansService
                .Setup(service => service.SetActiveInActive(planId, value))
                .ReturnsAsync(expectedResult); // Returns Task<JsonModel>

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            // Act
            var result = await _controller.SetActiveInActive(planId, value);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualResult = Assert.IsType<JsonModel>(okResult.Value);
            Assert.Equal(expectedResult.StatusCode, actualResult.StatusCode);
            Assert.Equal(expectedResult.Data, actualResult.Data);
        }
        [Fact]
        public async Task SetActiveInActive_Returns500_WhenServiceThrowsException()
        {
            var planId = 1;
            var value = true;

            _mockSubscriptionPlansService
                .Setup(s => s.SetActiveInActive(planId, value))
                .ThrowsAsync(new Exception("Service error"));

            await Assert.ThrowsAnyAsync<Exception>(async () =>
                await _controller.SetActiveInActive(planId, value));
        }
        #endregion
    }
}
