!/bin/bash

dotnet mpc -i "../../../unity/UnityProject/Sonosthesia.Pack.csproj" -o "./Generated/MessagePackGenerated.cs"

dotnet mpc -i "../../../unity/UnityProject/Sonosthesia.Pack.csproj" -o "../Runtime/MessagePackGenerated.cs"