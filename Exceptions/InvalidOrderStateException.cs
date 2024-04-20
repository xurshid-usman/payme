using Payme.Features;
using Payme.Types;

namespace Payme.Exceptions;

public class InvalidOrderStateException() : DomainException(
    new Error(-31099, new Message(
        "Buyurtma holati yaroqsiz."
        , "Order state is invalid."
        , "Недопустимое состояние заказа."), "order"));