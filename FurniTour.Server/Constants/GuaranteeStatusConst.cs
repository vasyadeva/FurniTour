using System.Collections.Generic;

namespace FurniTour.Server.Constants
{
    public class GuaranteeStatusConst
    {
        public const string Pending = "Очікує розгляду";
        public const string InReview = "На розгляді";
        public const string Approved = "Підтверджено";
        public const string InProgress = "В обробці";
        public const string Completed = "Завершено";
        public const string Rejected = "Відхилено";
        public const string Cancelled = "Скасовано";

        public static readonly List<string> ValidStatuses = new List<string>
        {
            Pending,
            InReview,
            Approved,
            InProgress,
            Completed,
            Rejected,
            Cancelled
        };
    }
}
