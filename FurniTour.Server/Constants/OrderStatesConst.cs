namespace FurniTour.Server.Constants
{
    public class OrderStatesConst
    {
        public const int NewOrder = 1;
        public const int CancelledByUser = 2;
        public const int CancelledByAdmin = 3;
        public const int Confirmed = 4;
        public const int InDelivery = 5;
        public const int Delivered = 6;
        public const int DeliveryConfirmedByUser = 7;
        public static readonly int[] ValidStates =
        {
            NewOrder,
            CancelledByUser,
            CancelledByAdmin,
            Confirmed,
            InDelivery,
            Delivered,
            DeliveryConfirmedByUser
        };
    }
}
