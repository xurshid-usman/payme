using Payme.Types;

namespace Payme.Exceptions;

public class TransactionNotFoundException() : DomainException(
    new Error(-31003, new Message(
        "Tranzaksiya topilmadi"
        , "Transaction not found"
        , "Транзакция не найдена"), ""));