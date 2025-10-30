using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace SourceGenUtils;

public readonly record struct StringyLocation(
    string FileName,
    TextSpan TextSpan,
    LinePositionSpan LineSpan
)
{
    public StringyLocation(SyntaxNode node) : this
    (
        node.SyntaxTree.FilePath,
        node.Span,
        node.SyntaxTree.GetLineSpan(node.Span).Span
    )
    {
    }
    
    public StringyLocation(SyntaxReference syntaxRef) : this
    (
        syntaxRef.SyntaxTree.FilePath,
        syntaxRef.Span,
        syntaxRef.SyntaxTree.GetLineSpan(syntaxRef.Span).Span
    )
    {
    }

    public Location AsLocation()
    {
        return Location.Create(FileName, TextSpan, LineSpan);
    }
}