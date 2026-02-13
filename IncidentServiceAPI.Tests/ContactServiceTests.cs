using Moq;
using Xunit;
using IncidentServiceAPI.Services;
using IncidentServiceAPI.Repositories.Interfaces;
using IncidentServiceAPI.Models.DTOs;
using IncidentServiceAPI.Models.Entities;
using System.Linq.Expressions;

namespace IncidentServiceAPI.Tests
{
    public class ContactServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IContactRepository> _mockContactRepo;
        private readonly ContactService _service;

        public ContactServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockContactRepo = new Mock<IContactRepository>();

            _mockUnitOfWork.Setup(u => u.Contacts).Returns(_mockContactRepo.Object);

            _mockUnitOfWork.Setup(u => u.ExecuteInTransactionAsync(It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()))
                .Returns((Func<CancellationToken, Task> action, CancellationToken token) => action(token));

            _service = new ContactService(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task CreateOrUpdate_ShouldCreate_WhenContactDoesNotExist()
        {
            // Arrange
            var request = new CreateContactRequestDto
            {
                ContactEmail = "new@user.com",
                ContactFirstName = "John",
                ContactLastName = "Doe"
            };

            _mockContactRepo.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Contact, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Contact)null);

            // Act
            var result = await _service.CreateOrUpdateContactAsync(request);

            // Assert
            Assert.True(result.IsCreated);
            _mockContactRepo.Verify(r => r.AddAsync(It.Is<Contact>(c => c.Email == request.ContactEmail), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateOrUpdate_ShouldUpdate_WhenContactExists()
        {
            // Arrange
            var request = new CreateContactRequestDto
            {
                ContactEmail = "existing@user.com",
                ContactFirstName = "Jane",
                ContactLastName = "Doe"
            };

            var existingContact = new Contact { Email = "existing@user.com", FirstName = "OldName" };

            _mockContactRepo.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Contact, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingContact);

            // Act
            var result = await _service.CreateOrUpdateContactAsync(request);

            // Assert
            Assert.False(result.IsCreated);
            Assert.Equal("Jane", existingContact.FirstName);
            _mockContactRepo.Verify(r => r.Update(existingContact), Times.Once);
        }
    }
}