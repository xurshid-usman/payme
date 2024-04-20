using Payme.Types;

namespace Payme.Exceptions;

public class ExistsActiveTransactionException() : DomainException(
    new Error(-31008, new Message(
    "Bu buyurtma uchun boshqa faol/tugallangan tranzaksiya mavjud."
    , "There is other active/completed transaction for this order."
    , "Для этого заказа есть другая активная/завершенная транзакция."), "transaction"));
