using Payme.Features;
using Payme.Types;

namespace Payme.Exceptions;

public class RequestParseException() : DomainException(
    new Error(-32700, new Message(
        "JSON parsing xatosi."
        , "JSON parsing error."
        , "Ошибка парсинга JSON."), ""));