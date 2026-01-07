using App.Api.Controllers.AgencySubscription;
using App.Api.Models;
using App.Application.Dto;
using App.Application.Interfaces.Services.AgencySubscriptions;
using App.Common.Constant;
using App.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace App.Services.Test.subscriptionPlansTest
{
    public class AgencySubscriptionPlanTests
    {
        private readonly Mock<IAgencySubscriptionPlanService> _mockService;
        private readonly AgencySubscriptionPlan _controller;

        public AgencySubscriptionPlanTests()
        {
            _mockService = new Mock<IAgencySubscriptionPlanService>();
            _controller = new AgencySubscriptionPlan(_mockService.Object);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
        }

        #region GetCurrentSubscriptionPlan

        [Fact]
        public async Task GetCurrentSubscriptionPlan_ReturnsOk_WhenServiceReturns200()
        {
            // Arrange
            var jsonModel = new Common.Models.JsonModel(new object(), "Success", StatusCodes.Status200OK);
            _mockService.Setup(s => s.GetCurrentSubscriptionPlan(It.IsAny<int>()))
                .ReturnsAsync(jsonModel);

            // Act
            var result = await _controller.GetCurrentSubscriptionPlan(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        }

        [Fact]
        public async Task GetCurrentSubscriptionPlan_ReturnsNotFound_WhenServiceReturns404()
        {
            var jsonModel = new Common.Models.JsonModel(new object(), "Not Found", StatusCodes.Status404NotFound);
            _mockService.Setup(s => s.GetCurrentSubscriptionPlan(It.IsAny<int>()))
                .ReturnsAsync(jsonModel);

            var result = await _controller.GetCurrentSubscriptionPlan(1);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        #endregion

        #region GetAllSubscriptionPlan

        [Fact]
        public async Task GetAllSubscriptionPlan_ReturnsOk_WhenServiceReturns200()
        {
            var jsonModel = new Common.Models.JsonModel(new object(), "Success", StatusCodes.Status200OK);
            _mockService.Setup(s => s.GetAllSubscriptionPlan(It.IsAny<int>()))
                .ReturnsAsync(jsonModel);

            var result = await _controller.GetAllSubscriptionPlan(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        }

        #endregion

        #region GetSubscriptionPlanList

        [Fact]
        public async Task GetSubscriptionPlanList_ReturnsOk_WhenServiceReturns200()
        {
            var jsonModel = new Common.Models.JsonModel(new object(), "Plan List", StatusCodes.Status200OK);
            _mockService.Setup(s => s.GetSubscriptionAllPlanList(It.IsAny<int>()))
                .ReturnsAsync(jsonModel);

            var result = await _controller.GetSubscriptionPlanList(It.IsAny<int>());

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        }

        #endregion

        #region CancelSubscription

        [Fact]
        public async Task CancelSubscription_ReturnsOk_WhenServiceReturnsTrue()
        {
            _mockService.Setup(s => s.CancelSubscriptionAsync(It.IsAny<int>(), It.IsAny<int?>()))
                .ReturnsAsync(true);

            var result = await _controller.CancelSubscription(1, 2);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var json = Assert.IsType<Common.Models.JsonModel>(okResult.Value);
            Assert.Equal(StatusMessage.CancelSubscription, json.Message);
        }

        [Fact]
        public async Task CancelSubscription_ReturnsBadRequest_WhenServiceReturnsFalse()
        {
            _mockService.Setup(s => s.CancelSubscriptionAsync(It.IsAny<int>(), It.IsAny<int?>()))
              .ReturnsAsync(false);

            var result = await _controller.CancelSubscription(1,1);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var json = Assert.IsType<Common.Models.JsonModel>(badRequest.Value);
            Assert.Equal(StatusMessage.CancelSubscriptionError, json.Message);
        }

        #endregion

        #region BuyPlan

        [Fact]
        public async Task BuyPlan_ReturnsOk_WhenServiceReturns200()
        {
            var jsonModel = new Common.Models.JsonModel(new object(), "Plan Purchased", StatusCodes.Status200OK);
            _mockService.Setup(s => s.BuyPlan(It.IsAny<SessionMetadataModel>()))
                .ReturnsAsync(jsonModel);

            var result = await _controller.BuyPlan(new SessionMetadataModel());

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        }

        [Fact]
        public async Task BuyPlan_ReturnsBadRequest_WhenServiceReturns400()
        {
            var jsonModel = new Common.Models.JsonModel(new object(), "Invalid Request", StatusCodes.Status400BadRequest);
            _mockService.Setup(s => s.BuyPlan(It.IsAny<SessionMetadataModel>()))
                .ReturnsAsync(jsonModel);

            var result = await _controller.BuyPlan(new SessionMetadataModel());

            Assert.IsType<BadRequestObjectResult>(result);
        }

        #endregion
    }
}