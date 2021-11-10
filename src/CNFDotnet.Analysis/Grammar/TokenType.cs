namespace CNFDotnet.Analysis.Grammar
{
    //The different token types found in a CNF Grammar
    public enum TokenType
    {
#pragma warning disable CA1720
        STRING,
#pragma warning restore CA1720
        ARROW,
        WHITESPACE,
        EOL,
        CHOICE,
        EOF,
        // Denotes a ε  
        EMPTY
    }
}
