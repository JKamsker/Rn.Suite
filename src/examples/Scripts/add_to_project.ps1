git submodule add --force https://github.com/JKamsker/Rn.Suite.git ".\external\Rn.Suite"

dotnet sln add -s "external\Rn.Suite" .\external\Rn.Suite\src\lib\Rnd.Lib\Rnd.Lib.csproj
dotnet sln add -s "external\Rn.Suite" .\external\Rn.Suite\src\lib\Rnd.IO\Rnd.IO.csproj
dotnet sln add -s "external\Rn.Suite" .\external\Rn.Suite\src\lib\Rnd.Logging\Rnd.Logging.csproj
dotnet sln add -s "external\Rn.Suite" .\external\Rn.Suite\src\lib\Rnd.MongoDb\Rnd.MongoDb.csproj