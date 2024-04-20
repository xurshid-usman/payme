using Payme.Features;
using Payme.Types;

namespace Payme.Exceptions;

public class OrderNotExistsException() : DomainException(
    new Error(-31050, new Message(
        "Buyurtma topilmadi"
        , "Order not found"
        , "Заказ не найден"), "order"));