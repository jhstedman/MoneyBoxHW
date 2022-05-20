using System;
using Moneybox.App.Domain.Services;

namespace Moneybox.App
{
    public class User
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public void Notify(INotificationService notif, bool low)
        {
            if (low)
            {
                notif.NotifyFundsLow(Email);
            }
            else
            {
                notif.NotifyApproachingPayInLimit(Email);
            }
        }
    }
}
