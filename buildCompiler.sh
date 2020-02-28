pushd CSM_Mandatory > /dev/null 
../packages/fslexyacc/10.0.0/build/fslex/net46/fslex.exe --unicode Lexer.fsl
../packages/fslexyacc/10.0.0/build/fsyacc/net46/fsyacc.exe -v --module Parser Parser.fsy
popd > /dev/null
