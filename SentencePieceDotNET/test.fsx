
#r @"C:\Users\cybernetic\source\repos\SentencePieceDotNET\bin\x64\Release\netcoreapp3.1\SentencePieceDotNET.dll"
#r @"C:\Users\cybernetic\source\repos\Prelude\Prelude\bin\Release\net47\prelude.dll"

open Prelude.Common

let sp = new SentencePieceDotNET.SentencePieceDotNET()


sp.Load(@"D:\Downloads\NeuralNets\t5-base\spiece.model")

let res = sp.Encode("ö ")

"this is a test of who are you.".Length

let b2 = sp.Decode2 res

sp.Decode res 

let b1 = "ö" |> Strings.toUTF8Bytes

Array.zip b1 b2.[..b1.Length - 1]
b2.Length


let res = sp.Encode "Вагиф Сәмәдоғлу"
sp.Encode "μ α"
sp.Encode2 "kö"

"μ α .1 + τ α  Who is Kurt Gödel"