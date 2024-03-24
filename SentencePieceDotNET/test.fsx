
#r @"bin\Debug\net5.0\SentencePieceDotNET.dll"

let sp = new SentencePieceDotNET.SentencePieceDotNET()

sp.Load(@"D:\Downloads\NeuralNets\flan-t5-large\spiece.model")

sp.EncodeAsPieces("hello this is a test by fravilox of Elj; who are you?")

sp.Encode("hello this is a test of who are you.")
|> sp.Decode
 
sp.Encode "Who are Kurt Gödel and Вагиф Сәмәдоғлу"
|> sp.Decode

sp.Encode "μ α"
|> sp.Decode

sp.Encode "\\n ."
|> sp.Decode

open System.Text
 
for i in 0..32128 do
    printfn "%A" i
    try 
        sp.Decode [|i|] |> ignore
    with _ -> 
        printfn "%A" i