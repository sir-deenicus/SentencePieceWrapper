
#r @"bin\x64\Release\netcoreapp3.1\SentencePieceDotNET.dll"

let sp = new SentencePieceDotNET.SentencePieceDotNET()

sp.Load(@"D:\Downloads\NeuralNets\t5-base\spiece.model")

sp.Encode("hello this is a test of who are you.")
|> sp.Decode
 
sp.Encode "Who are Kurt Gödel and Вагиф Сәмәдоғлу"

sp.Encode "μ α"
  