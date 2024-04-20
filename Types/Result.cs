namespace Payme.Types;

public record SuccessResult<T>(T Result, int Id);
public record ErrorResult(Error Error, int Id);