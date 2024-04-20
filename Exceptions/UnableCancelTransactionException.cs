using Payme.Types;

namespace Payme.Exceptions;

public class UnableCancelTransactionException() : DomainException(
    new Error(31007, new Message(
        "Buyurtma tugallangan. Tranzaksiyani bekor qilib bo'lmaydi."
        , "The order is complete. It is not possible to cancel the transaction."
        , "Заказ выполнен. Невозможно отменить транзакцию."), ""));