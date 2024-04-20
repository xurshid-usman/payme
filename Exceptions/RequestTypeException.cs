using Payme.Features;
using Payme.Types;

namespace Payme.Exceptions;

public class RequestTypeException() : DomainException(
    new Error(-32600, new Message(
        "RPC so'rovida kerakli maydonlar yo'q yoki maydon turi spetsifikatsiyaga mos kelmaydi."
        , "The RPC request is missing required fields or the field type does not comply with the specification."
        , "В RPC-запросе отсутствуют обязательные поля или тип полей не соответствует спецификации."), ""));