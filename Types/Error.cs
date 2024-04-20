namespace Payme.Types;

public record Error(int Code, Message Message, string Data);
public record Message(string Uz, string En, string Ru);