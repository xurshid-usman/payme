using Payme.Types;

namespace Payme.Exceptions;

public class ApiInternalException() : DomainException(
    new Error(-32400
        , new Message("Tizim (ichki xato)."
            , "System (internal error)."
            , "Системная (внутренняя ошибка)."), "error"));