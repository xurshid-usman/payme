using Payme.Features;
using Payme.Types;

namespace Payme.Exceptions;

public class AccountException() : DomainException(
    new Error(-31050, new Message(
        "Harid kodida xatolik."
        , "Incorrect order code."
        , "Неверный код заказа."), nameof(Account.OrderId)));