using NUnit.Framework;
using Moneybox.App;
using Moneybox.App.Features;
using Moneybox.App.DataAccess;
using Moneybox.App.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace MoneyBoxTester
{
    public class Tests
    {
        [Test]
        public void TestWithdrawMoneyRegular()
        {
            var usr = new User { Id = Guid.NewGuid(), Name = "Peter", Email = "test@email.com" };
            var acc = new Account { Id = Guid.NewGuid(), User = usr, Balance = 1000m, Withdrawn = 0m };
            var accounts = new List<Account> { acc };  
            var accRepo = new AccountRepo(accounts);


            var withdraw = new WithdrawMoney(accRepo, new NotificationSystem());
            withdraw.Execute(acc.Id, 200m);

            Assert.AreEqual(800m, accRepo.GetAccountById(acc.Id).Balance);
        }

        [Test]
        public void TestWithdrawMoneyLowFund()
        {
            var usr = new User { Id = Guid.NewGuid(), Name = "Peter", Email = "test@email.com" };
            var acc = new Account { Id = Guid.NewGuid(), User = usr, Balance = 1000m, Withdrawn = 0m };
            var accounts = new List<Account> { acc };
            var accRepo = new AccountRepo(accounts);


            var withdraw = new WithdrawMoney(accRepo, new NotificationSystem());
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                withdraw.Execute(acc.Id, 600m);

                Assert.AreEqual(400m, accRepo.GetAccountById(acc.Id).Balance);
                Assert.AreEqual("test@email.com has low funds\r\n", sw.ToString());
            }
        }

        [Test]
        public void TestWithdrawMoneyWithdrwalLimit()
        {
            var usr = new User { Id = Guid.NewGuid(), Name = "Peter", Email = "test@email.com" };
            var acc = new Account { Id = Guid.NewGuid(), User = usr, Balance = 1000m, Withdrawn = 0m };
            var accounts = new List<Account> { acc };
            var accRepo = new AccountRepo(accounts);


            var withdraw = new WithdrawMoney(accRepo, new NotificationSystem());
            Assert.Throws<InvalidOperationException>(() => withdraw.Execute(acc.Id, 1200m));
        }
    }

    public class AccountRepo : IAccountRepository
    {

        private Dictionary<Guid, Account> _accounts;

        public AccountRepo(List<Account> accounts)
        {
            _accounts = accounts.ToDictionary(x => x.Id);
        }

        public Account GetAccountById(Guid accountId)
        {
            return _accounts[accountId];
        }

        public void Update(Account account)
        {
            _accounts[account.Id] = account;
        }
    }

    public class NotificationSystem : INotificationService
    {
        public void NotifyApproachingPayInLimit(string emailAddress)
        {
            Console.WriteLine($"{emailAddress} is reaching pay limit");
        }

        public void NotifyFundsLow(string emailAddress)
        {
            Console.WriteLine($"{emailAddress} has low funds");
        }
    }
}