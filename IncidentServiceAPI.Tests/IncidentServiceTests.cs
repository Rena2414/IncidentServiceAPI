using Moq;
using Xunit;
using IncidentServiceAPI.Services;
using IncidentServiceAPI.Repositories.Interfaces;
using IncidentServiceAPI.Models.DTOs;
using IncidentServiceAPI.Models.Entities;
using System.Linq.Expressions;

namespace IncidentServiceAPI.Tests
{
    public class IncidentServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IAccountRepository> _mockAccountRepo;
        private readonly Mock<IContactRepository> _mockContactRepo;
        private readonly Mock<IIncidentRepository> _mockIncidentRepo;
        private readonly IncidentService _service;

        public IncidentServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockAccountRepo = new Mock<IAccountRepository>();
            _mockContactRepo = new Mock<IContactRepository>();
            _mockIncidentRepo = new Mock<IIncidentRepository>();

            _mockUnitOfWork.Setup(u => u.Accounts).Returns(_mockAccountRepo.Object);
            _mockUnitOfWork.Setup(u => u.Contacts).Returns(_mockContactRepo.Object);
            _mockUnitOfWork.Setup(u => u.Incidents).Returns(_mockIncidentRepo.Object);

            _mockUnitOfWork.Setup(u => u.ExecuteInTransactionAsync(It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()))
                .Returns((Func<CancellationToken, Task> action, CancellationToken token) => action(token));

            _service = new IncidentService(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task CreateIncident_ShouldThrowNotFound_WhenAccountDoesNotExist()
        {
            // Arrange
            var request = new CreateIncidentRequestDto { AccountName = "GhostAccount" };
            _mockAccountRepo.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Account, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Account)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.CreateIncidentAsync(request));
        }

        [Fact]
        public async Task CreateIncident_ShouldCreateNewContact_WhenContactDoesNotExist()
        {
            // Arrange
            var request = new CreateIncidentRequestDto
            {
                AccountName = "NiceCorp",
                ContactEmail = "new@tech.com",
                ContactFirstName = "New",
                ContactLastName = "User",
                IncidentDescription = "Server Down"
            };

            var existingAccount = new Account { Name = "NiceCorp" };

            _mockAccountRepo.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Account, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingAccount);

            _mockContactRepo.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Contact, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Contact)null);

            // Act
            var result = await _service.CreateIncidentAsync(request);

            // Assert
            _mockContactRepo.Verify(r => r.AddAsync(It.Is<Contact>(c =>
                c.Email == request.ContactEmail &&
                c.Account == existingAccount),
                It.IsAny<CancellationToken>()), Times.Once);

            _mockIncidentRepo.Verify(r => r.AddAsync(It.Is<Incident>(i =>
                i.Description == request.IncidentDescription &&
                i.Account == existingAccount),
                It.IsAny<CancellationToken>()), Times.Once);

            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateIncident_ShouldLinkExistingContact_WhenContactExistsButNotLinked()
        {
            // Arrange
            var request = new CreateIncidentRequestDto
            {
                AccountName = "NiceCorp",
                ContactEmail = "old@other.com",
                ContactFirstName = "UpdatedName",
                ContactLastName = "UpdatedLast",
                IncidentDescription = "Bug Report"
            };

            var existingAccount = new Account { Name = "TechCorp" };
            var existingContact = new Contact
            {
                Email = "old@other.com",
                FirstName = "OldName",
                AccountName = "OtherCorp"
            };

            _mockAccountRepo.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Account, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingAccount);

            _mockContactRepo.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Contact, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingContact);

            // Act
            await _service.CreateIncidentAsync(request);

            // Assert
            Assert.Equal("UpdatedName", existingContact.FirstName);
            Assert.Equal(existingAccount, existingContact.Account);

            _mockContactRepo.Verify(r => r.Update(existingContact), Times.Once);
        }


        [Fact]
        public async Task CreateIncident_ShouldLinkOrphanContact_WhenContactExistsButHasNoAccount()
        {
            // Arrange
            var request = new CreateIncidentRequestDto
            {
                AccountName = "NiceCorp",
                ContactEmail = "orphan@user.com",
                ContactFirstName = "Orphan",
                ContactLastName = "User",
                IncidentDescription = "Help"
            };

            var existingAccount = new Account { Name = "TechCorp" };

            var orphanContact = new Contact
            {
                Email = "orphan@user.com",
                Account = null,
                AccountName = null
            };

            _mockAccountRepo.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Account, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingAccount);

            _mockContactRepo.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Contact, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(orphanContact);

            // Act
            await _service.CreateIncidentAsync(request);

            // Assert
            Assert.Equal(existingAccount, orphanContact.Account);
            _mockContactRepo.Verify(r => r.Update(orphanContact), Times.Once);
        }

        [Fact]
        public async Task CreateIncident_ShouldNotRelink_WhenContactIsAlreadyLinkedCorrectly()
        {
            // Arrange
            var request = new CreateIncidentRequestDto
            {
                AccountName = "NiceCorp",
                ContactEmail = "loyal@tech.com",
                ContactFirstName = "Loyal",
                ContactLastName = "Employee",
                IncidentDescription = "Routine Check"
            };

            var existingAccount = new Account { Name = "NiceCorp" };

            var loyalContact = new Contact
            {
                Email = "loyal@tech.com",
                Account = existingAccount,
                AccountName = "NiceCorp"
            };

            _mockAccountRepo.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Account, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingAccount);

            _mockContactRepo.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Contact, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(loyalContact);

            // Act
            await _service.CreateIncidentAsync(request);

            // Assert
            Assert.Equal(existingAccount, loyalContact.Account);

            Assert.NotNull(loyalContact.Account);
        }

        [Fact]
        public async Task CreateIncident_ShouldThrowArgumentNullException_WhenRequestIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.CreateIncidentAsync(null));
        }

        [Fact]
        public async Task CreateIncident_ShouldPropagateException_WhenDatabaseFails()
        {
            // Arrange
            var request = new CreateIncidentRequestDto
            {
                AccountName = "TechCorp",
                ContactEmail = "user@tech.com",
                ContactFirstName = "Test",
                ContactLastName = "User",
                IncidentDescription = "Db Crash Test"
            };

            var existingAccount = new Account { Name = "TechCorp" };

            _mockAccountRepo.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Account, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingAccount);

            _mockIncidentRepo.Setup(r => r.AddAsync(It.IsAny<Incident>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database Connection Lost"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _service.CreateIncidentAsync(request));
            Assert.Equal("Database Connection Lost", exception.Message);

            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}