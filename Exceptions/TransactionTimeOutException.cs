using Payme.Entities;
using Payme.Types;

namespace Payme.Exceptions;

public class TransactionTimeOutException() : DomainException(
    new Error(-31008, new Message(
        $"Tranzaksiya yaratilgan sanadan {OrderTransaction.Timeout} ms o`tgan"
        , $"Since create time of the transaction passed {OrderTransaction.Timeout} ms"
        , $"С даты создания транзакции прошло {OrderTransaction.Timeout} мс"), "time"));