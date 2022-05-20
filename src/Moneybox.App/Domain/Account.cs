using System;
using Moneybox.App.Domain.Services;

namespace Moneybox.App
{
    public class Account
    {
        public const decimal PayInLimit = 4000m;

        public Guid Id { get; set; }

        public User User { get; set; }

        public decimal Balance { get; set; }

        public decimal Withdrawn { get; set; }

        public decimal PaidIn { get; set; }

        public void Withdraw(decimal amount, INotificationService notif)
        {
            var fromBalance = Balance - amount;
            if (fromBalance < 0m)
            {
                throw new InvalidOperationException("Insufficient funds to make withdrawl");
            }

            if (fromBalance < 500m)
            {
                User.Notify(notif, true);
            }

            Balance = Balance - amount;
            Withdrawn = Withdrawn - amount;
        }

        public void PayIn(decimal amount, INotificationService notif)
        {
            var paidIn = PaidIn + amount;
            if (paidIn > PayInLimit)
            {
                throw new InvalidOperationException("Account pay in limit reached");
            }

            if (Account.PayInLimit - paidIn < 500m)
            {
                User.Notify(notif, false);
            }

            Balance = Balance + amount;
            PaidIn = PaidIn + amount;
        }
    }
}
