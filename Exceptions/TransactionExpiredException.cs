using Payme.Features;
using Payme.Types;

namespace Payme.Exceptions;

public class TransactionExpiredException() : DomainException(
    new Error(-31008, new Message(
        "Tranzaksiya muddati tugadi."
        , "Transaction is expired."
        , "Срок действия транзакции истек."), "transaction"));