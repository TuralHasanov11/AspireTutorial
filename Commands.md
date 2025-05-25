### Remove all Docker networks with a specific prefix

```sh
docker network ls --format "{{.Name}}" | Where-Object { $_ -like "default-aspire-network-*" } | ForEach-Object { docker network rm $_ }
```

```sh
dotnet run -- --publisher manifest --output-path ../aspire-manifest.json
```