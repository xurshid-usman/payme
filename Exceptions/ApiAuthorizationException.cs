using Payme.Types;

namespace Payme.Exceptions;

public class ApiAuthorizationException(): DomainException(
        new Error(-32504
            , new Message("Ushbuni bajarish uchun imtiyozlar etarli emas."
                , "Insufficient privileges to execute method."
                , "Недостаточно привилегий для выполнения метода."), "authorization"));