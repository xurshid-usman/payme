using Payme.Types;

namespace Payme.Exceptions;

public class WrongAmountException() : DomainException(
    new Error(-31001, new Message(
        "Yaroqsiz summa."
        , "Invalid amount."
        , "Неверная сумма."), "order"));