using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Toaster.Parsing;

namespace Toaster.Tests.Parsing;

[TestFixture]
public class ParserTests
{
    private string ReadSampleFile(string localPath)
    {
        const string localPathOffset = "..\\..\\..\\Toaster\\sample\\";
        string path = Path.Combine(localPathOffset, localPath);

        return File.ReadAllText(path);
    }

    [Test]
    public void LineSeparator()
    {
        string[] programTexts = new string [3]
        {
            "mov $reg1 $reg0\r\n\r\nmov $reg0 $reg1",
            "mov $reg1 $reg0\r\rmov $reg0 $reg1",
            "mov $reg1 $reg0\n\nmov $reg0 $reg1",
        };

        List<TokenProgram> results = new List<TokenProgram>();

        foreach (string programText in programTexts)
        {
            results.Add(new Parser().Tokenize(programText));
        }

        TokenProgram expected = results[0];

        for (int i = 1; i < results.Count; i++)
        {
            Assert.AreEqual(expected.Lines.Count, results[i].Lines.Count);
        }
    }

    [Test]
    public void TokenPosition_In_Program()
    {
        const string programText = "mov $reg1 $reg0\r\nmov $reg0 $reg1";

        Parser parser = new Parser();

        TokenProgram program = parser.Tokenize(programText);

        Assert.IsEmpty(parser.Errors.Errors);

        Assert.AreEqual(2, program.Lines.Count);

        TokenLine line = program.Lines[0];
        Assert.AreEqual(3, line.TokenCount);
        Assert.AreEqual(new TokenPosition(0, 0, 2), line.Tokens[0].Position);
        Assert.AreEqual(new TokenPosition(0, 4, 8), line.Tokens[1].Position);
        Assert.AreEqual(new TokenPosition(0, 10, 14), line.Tokens[2].Position);

        line = program.Lines[1];
        Assert.AreEqual(3, line.TokenCount);
        Assert.AreEqual(new TokenPosition(1, 0, 2), line.Tokens[0].Position);
        Assert.AreEqual(new TokenPosition(1, 4, 8), line.Tokens[1].Position);
        Assert.AreEqual(new TokenPosition(1, 10, 14), line.Tokens[2].Position);
    }

    [Test]
    public void TokenType_Zoo()
    {
        string programText = ReadSampleFile("token_zoo.tst");

        Parser parser = new Parser();
        TokenProgram tokenProgram = parser.Tokenize(programText);

        TokenType[][] types = new TokenType[tokenProgram.Lines.Count][];

        for (int i = 0; i < tokenProgram.Lines.Count; i++)
        {
            types[i] = tokenProgram.Lines[i].TokenTypes.ToArray();
        }

        Assert.AreEqual(6, types.Length);

        Assert.AreEqual(new [] {TokenType.LABEL}, types[0]);
        Assert.AreEqual(Array.Empty<TokenType>(), types[1]);
        Assert.AreEqual(new [] {TokenType.INSTRUCTION, TokenType.REGISTER, TokenType.REGISTER}, types[2]);
        Assert.AreEqual(new [] {TokenType.INSTRUCTION, TokenType.PIN, TokenType.PIN_RANGE, TokenType.PIN_RANGE_LENGTH}, types[3]);
        Assert.AreEqual(new [] {TokenType.INSTRUCTION, TokenType.NAME, TokenType.HEX, TokenType.INTEGER, TokenType.BINARY, TokenType.COMMENT}, types[4]);
        Assert.AreEqual(new [] {TokenType.COMMENT}, types[5]);
    }
}