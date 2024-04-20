using Payme.Types;

namespace Payme.Exceptions;

public class TransactionFoundButCancelledException() : DomainException(
    new Error(-31008, new Message(
        $"Tranzaksiya topildi, lekin faol emas."
        , $"Transaction found, but is not active."
        , $"Транзакция найдена, но не активна."), "transaction"));