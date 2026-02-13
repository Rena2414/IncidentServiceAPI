using Moq;
using Xunit;
using IncidentServiceAPI.Services;
using IncidentServiceAPI.Repositories.Interfaces;
using IncidentServiceAPI.Models.DTOs;
using IncidentServiceAPI.Models.Entities;
using System.Linq.Expressions;

namespace IncidentServiceAPI.Tests
{
    public class AccountServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IAccountRepository> _mockAccountRepo;
        private readonly Mock<IContactRepository> _mockContactRepo;
        private readonly AccountService _service;

        public AccountServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockAccountRepo = new Mock<IAccountRepository>();
            _mockContactRepo = new Mock<IContactRepository>();

            _mockUnitOfWork.Setup(u => u.Accounts).Returns(_mockAccountRepo.Object);
            _mockUnitOfWork.Setup(u => u.Contacts).Returns(_mockContactRepo.Object);

            _mockUnitOfWork.Setup(u => u.ExecuteInTransactionAsync(It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()))
                .Returns((Func<CancellationToken, Task> action, CancellationToken token) => action(token));

            _service = new AccountService(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task CreateAccount_ShouldThrowArgumentException_WhenAccountAlreadyExists()
        {
            // Arrange
            var request = new CreateAccountRequestDto { AccountName = "ExistingCorp" };
            _mockAccountRepo.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Account, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAccountAsync(request));
        }

        [Fact]
        public async Task CreateAccount_ShouldCreateAccountAndContact_WhenBothAreNew()
        {
            // Arrange
            var request = new CreateAccountRequestDto
            {
                AccountName = "NewCorp",
                ContactEmail = "ceo@newcorp.com",
                ContactFirstName = "Boss",
                ContactLastName = "Man"
            };

            _mockAccountRepo.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Account, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _mockContactRepo.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Contact, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Contact)null);

            // Act
            var result = await _service.CreateAccountAsync(request);

            // Assert
            Assert.Equal("NewCorp", result.AccountName);

            _mockAccountRepo.Verify(r => r.AddAsync(It.Is<Account>(a => a.Name == "NewCorp"), It.IsAny<CancellationToken>()), Times.Once);

            _mockContactRepo.Verify(r => r.AddAsync(It.Is<Contact>(c =>
                c.Email == request.ContactEmail &&
                c.Account.Name == "NewCorp"),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateAccount_ShouldRelinkContact_WhenContactAlreadyExists()
        {
            // Arrange
            var request = new CreateAccountRequestDto
            {
                AccountName = "NewCorp",
                ContactEmail = "freelancer@gmail.com",
                ContactFirstName = "Free",
                ContactLastName = "Lancer"
            };

            var existingContact = new Contact { Email = "freelancer@gmail.com", AccountName = "OldClient" };

            _mockAccountRepo.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Account, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _mockContactRepo.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Contact, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingContact);

            // Act
            await _service.CreateAccountAsync(request);

            // Assert
            Assert.Equal("NewCorp", existingContact.Account.Name);
            _mockContactRepo.Verify(r => r.Update(existingContact), Times.Once);
        }

        [Fact]
        public async Task CreateAccount_ShouldThrowArgumentNullException_WhenRequestIsNull()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.CreateAccountAsync(null));
        }
    }
}