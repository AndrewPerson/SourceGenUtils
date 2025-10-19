using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SourceGenUtils;

public class IndentedTextWriter(StreamWriter writer, bool ownsWriter = true) : TextWriter
{
    public override Encoding Encoding { get; } = Encoding.Default;

    public StreamWriter Writer { get; } = writer;
    
    private bool justWrittenNewline = true;
    private int numNewlinePartsWritten;
    
    private string indent = string.Empty;
    
    public override void Write(char value)
    {
        if (justWrittenNewline)
        {
            Writer.Write(indent);
            
            justWrittenNewline = false;
        }
        
        Writer.Write(value);

        if (value == NewLine[numNewlinePartsWritten]) numNewlinePartsWritten++;
        else numNewlinePartsWritten = 0;

        if (numNewlinePartsWritten == NewLine.Length)
        {
            justWrittenNewline = true;
            numNewlinePartsWritten = 0;
        }
    }

    public override async Task WriteAsync(char value)
    {
        if (justWrittenNewline)
        {
            await Writer.WriteAsync(indent);
            
            justWrittenNewline = false;
        }
        
        await Writer.WriteAsync(value);

        if (value == NewLine[numNewlinePartsWritten]) numNewlinePartsWritten++;
        else numNewlinePartsWritten = 0;

        if (numNewlinePartsWritten == NewLine.Length)
        {
            justWrittenNewline = true;
            numNewlinePartsWritten = 0;
        }
    }

    public override void Flush()
    {
        Writer.Flush();
    }

    public override async Task FlushAsync()
    {
        await Writer.FlushAsync();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (ownsWriter) Writer.Dispose();
        }
    }

    public void Indent(int amount = 1)
    {
        indent = new('\t', indent.Length + amount);
    }
    
    public void UnIndent(int amount = 1)
    {
        indent = new('\t', indent.Length - amount);
    }

    public Block WriteBlock(int indent = 1, string? leading = null, string? trailing = null)
    {
        return new Block(indent, leading ?? String.Empty, trailing ?? String.Empty, this);
    }

    public Block WriteBlock(string leading, string trailing, int indent = 1) => WriteBlock(indent, leading, trailing);

    public sealed class Block : IDisposable
    {
        public int Indent { get; }
        
        public string Leading { get; }
        public string Trailing { get; }

        private readonly IndentedTextWriter writer;

        private bool disposed;

        internal Block(int indent, string leading, string trailing, IndentedTextWriter writer)
        {
            Indent = indent;

            Leading = leading;
            Trailing = trailing;
            
            this.writer = writer;
            
            writer.WriteLine(Leading);
            writer.Indent(Indent);
        }

        public void Dispose()
        {
            if (disposed) return;
            
            writer.UnIndent(Indent);
            writer.WriteLine(Trailing);

            disposed = true;
        }
    }
}