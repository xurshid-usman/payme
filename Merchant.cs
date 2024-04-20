using Payme.Data;
using Payme.Exceptions;
using Payme.Features;
using Payme.Types;

namespace Payme;

public class Merchant(ILogger<Merchant> logger, IConfiguration config)
{
    // from user-secrets
    private readonly string _key = config["MerchantKey"] ?? throw new ArgumentException("Key not found");

    private void Authorize(HttpRequest httpRequest)
    {
        if (httpRequest.Headers.Authorization.Count == 0)
            throw new ApiAuthorizationException();

        var value = httpRequest.Headers.Authorization[0];
        
        var token = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"Paycom:{_key}"));
        if(value != $"Basic {token}")
            throw new ApiAuthorizationException();
        
        logger.LogInformation("Authorize...ok");
    }

    public async Task Handle(AppDbContext db, HttpRequest httpRequest, HttpResponse response)
    {
        var req = await httpRequest.ReadFromJsonAsync<PaymeRequest>();
        if (req == null) return;

        var ct = new CancellationToken();
        
        try
        {
            Authorize(httpRequest);
            
            switch (req.Method)
            {
                case nameof(CheckPerformTransaction):
                    await response.WriteAsJsonAsync(
                        new SuccessResult<CheckPerformTransaction.Result>(
                            await new CheckPerformTransaction(db).Handle(req.GetParams<CheckPerformTransaction.Request>(), ct)
                            , req.Id), cancellationToken: ct);
                    break;
                
                case nameof(CreateTransaction):
                    await response.WriteAsJsonAsync(
                        new SuccessResult<CreateTransaction.Result>(
                            await new CreateTransaction(db).Handle(req.GetParams<CreateTransaction.Request>(), ct)
                            , req.Id), cancellationToken: ct);
                    break;
                
                case nameof(PerformTransaction):
                    await response.WriteAsJsonAsync(
                        new SuccessResult<PerformTransaction.Result>(
                            await new PerformTransaction(db).Handle(req.GetParams<PerformTransaction.Request>(), ct)
                            , req.Id), cancellationToken: ct);
                    break;
                
                case nameof(CheckTransaction):
                    await response.WriteAsJsonAsync(
                        new SuccessResult<CheckTransaction.Result>(
                            await new CheckTransaction(db).Handle(req.GetParams<CheckTransaction.Request>(), ct)
                            , req.Id), cancellationToken: ct);
                    break;
                
                case nameof(CancelTransaction):
                    await response.WriteAsJsonAsync(
                        new SuccessResult<CancelTransaction.Result>(
                            await new CancelTransaction(db).Handle(req.GetParams<CancelTransaction.Request>(), ct)
                            , req.Id), cancellationToken: ct);
                    break;
                
                case nameof(GetStatement):
                    await response.WriteAsJsonAsync(
                        new SuccessResult<GetStatement.Result>(
                            await new GetStatement(db).Handle(req.GetParams<GetStatement.Request>(), ct)
                            , req.Id), cancellationToken: ct);
                    break;
                
                case nameof(SetFiscalData):
                    await response.WriteAsJsonAsync(
                        new SuccessResult<CreateTransaction.Result>(
                            await new CreateTransaction(db).Handle(req.GetParams<CreateTransaction.Request>(), ct)
                            , req.Id), cancellationToken: ct);
                    break;
                
                default:
                    throw new MethodNotFoundException(req.Method);
            }
        }
        catch (DomainException domainException)
        {
            await response.WriteAsJsonAsync(new ErrorResult(domainException.Error, req.Id), cancellationToken: ct);
        }
        catch (Exception e)
        {
            logger.LogError(e, "");
            await response.WriteAsJsonAsync(new ErrorResult(new ApiInternalException().Error, req.Id), cancellationToken: ct);
        }
    }
}