#!/bin/bash

# if it doesn't work as expected check:
# - have you compile the package with the correct UnityProject (in this repo)
# - is the UnityProject linking to local packages or npm versions
# - did you mark the classes with MessagePackObject
# - are the classes public

dotnet mpc -i "../../../unity/UnityProject/Sonosthesia.Pack.csproj" -o "../Runtime/MessagePackGenerated.cs" -n "Sonosthesia.Pack"