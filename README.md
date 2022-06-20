# CNFDotnet
Testing out different parser implementations

## Rationale
I've created this project to learn different parser implementations. It's a C# translation based on the code of [grammophone](https://github.com/mdaines/grammophone) by @mdaines.

## Tooling
The first tool is [grammophone](https://mdaines.github.io/grammophone/) itself, to veriy my translation.
This is also the first project I developed completely using [ViM](https://github.com/vim/vim) on [WSL1](https://docs.microsoft.com/en-us/windows/wsl/install).

Mandatory vim plugins are [OmniSharp-vim](https://github.com/OmniSharp/omnisharp-vim) and [Sharpen Up](https://github.com/nickspoons/vim-sharpenup). [OmniSharp-vim](https://github.com/OmniSharp/omnisharp-vim) relies on [OmniSharp-Roslyn](https://github.com/OmniSharp/omnisharp-roslyn) for C# support. [Sharpen Up](https://github.com/nickspoons/vim-sharpenup) - among other things - adds default mappings to [OmniSharp-vim](https://github.com/OmniSharp/omnisharp-vim).

Optionally you can get the debugger [Vimspector](https://github.com/puremourning/vimspector). It has .NET (core) support out of the box using [netcoredbg](https://github.com/Samsung/netcoredbg).

After some initial issues with the debugger (https://github.com/puremourning/vimspector/issues/448), it was a tolerable experience, but I had to figth ViM many times. It's *fast*, but can not compare to a fully fledged IDE.

## Usage
This library is solely made to run the [tests](test/CNFDotnet.Tests). The tests validate different types of grammars. As of now: LL(1), LR(0), SLR(1), LR(1) and LALR(1).

The code in [Parsing](src/Parsing) is well documented and should explain every single step in the grammar validation and parsing process.