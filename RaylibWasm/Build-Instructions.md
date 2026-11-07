To build, start in solution root and run:   
`dotnet publish -c Release`

To test, run:  
`dotnet serve --mime .wasm=application/wasm --mime .js=text/javascript --mime .json=application/json --directory RaylibWasm/bin/Release/net10.0/browser-wasm/AppBundle`

For itch.io builds, zip the contents of \RaylibWasm\bin\Release\net10.0\browser-wasm\AppBundle\

If anything fails, try the proper instructions from https://github.com/Kiriller12/RaylibWasm/blob/main/README.md