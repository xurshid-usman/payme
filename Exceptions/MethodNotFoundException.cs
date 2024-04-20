using Payme.Features;
using Payme.Types;

namespace Payme.Exceptions;

public class MethodNotFoundException(string method): DomainException(
        new Error(-32601,
            new Message("Soʻralgan usul topilmadi."
                , "The requested method was not found."
                , "Запрашиваемый метод не найден."), method));