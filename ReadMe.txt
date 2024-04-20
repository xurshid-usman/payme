cd payme

-- create psql db (or use in memory db)
dotnet ef database update

-- set merchant key to user secrets store
dotnet user-secrets init
dotnet user-secrets set "MerchantKey" "your_test_key"

-- for testing call back you can use ngrok
ngrok http https://localhost:7021
