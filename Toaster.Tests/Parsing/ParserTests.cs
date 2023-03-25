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

    private void CompareTokenPositions(TokenPosition expected, TokenPosition actual, int expectedLength)
    {
        Assert.AreEqual(expected.Line, actual.Line);
        Assert.AreEqual(expected.StartColumn, actual.StartColumn);
        Assert.AreEqual(expected.EndColumn, actual.EndColumn);
        Assert.AreEqual(expectedLength, actual.Length);
        Assert.AreEqual(expectedLength, expected.Length);
    }

    [Test]
    public void Test_LineSeparators()
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
    public void Test_TokenPosition()
    {
        const string programText = "mov $reg1 $reg0\r\nmov $reg0 $reg1";

        Parser parser = new Parser();

        TokenProgram program = parser.Tokenize(programText);

        Assert.IsEmpty(parser.Errors.Errors);

        Assert.AreEqual(2, program.Lines.Count);

        TokenLine line = program.Lines[0];
        Assert.AreEqual(3, line.TokenCount);
        CompareTokenPositions(new TokenPosition(0, 0, 2), line.Tokens[0].Position, 3);
        CompareTokenPositions(new TokenPosition(0, 4, 8), line.Tokens[1].Position, 5);
        CompareTokenPositions(new TokenPosition(0, 10, 14), line.Tokens[2].Position, 5);

        line = program.Lines[1];
        Assert.AreEqual(3, line.TokenCount);
        CompareTokenPositions(new TokenPosition(1, 0, 2), line.Tokens[0].Position, 3);
        CompareTokenPositions(new TokenPosition(1, 4, 8), line.Tokens[1].Position, 5);
        CompareTokenPositions(new TokenPosition(1, 10, 14), line.Tokens[2].Position, 5);
    }

    [Test]
    public void Test_TokenType()
    {
        string programText = ReadSampleFile("token_zoo.tst");

        Parser parser = new Parser();
        TokenProgram tokenProgram = parser.Tokenize(programText);

        TokenId[][] types = new TokenId[tokenProgram.Lines.Count][];

        for (int i = 0; i < tokenProgram.Lines.Count; i++)
        {
            types[i] = tokenProgram.Lines[i].TokenIds.ToArray();
        }

        Assert.AreEqual(7, types.Length);

        Assert.AreEqual(new [] {TokenId.LABEL}, types[0]);
        Assert.AreEqual(Array.Empty<TokenId>(), types[1]);
        Assert.AreEqual(new [] {TokenId.INSTRUCTION, TokenId.REGISTER, TokenId.REGISTER}, types[2]);
        Assert.AreEqual(new [] {TokenId.INSTRUCTION, TokenId.PIN, TokenId.PIN_RANGE, TokenId.PIN_RANGE_LENGTH}, types[3]);
        Assert.AreEqual(new [] {TokenId.INSTRUCTION, TokenId.NAME, TokenId.HEX, TokenId.INTEGER, TokenId.BINARY, TokenId.COMMENT}, types[4]);
        Assert.AreEqual(Array.Empty<TokenId>(), types[5]);
        Assert.AreEqual(new [] {TokenId.COMMENT}, types[6]);
    }

    [Test]
    public void Test_InstructionLines()
    {
        string programText = ReadSampleFile("token_zoo.tst");

        Parser parser = new Parser();
        TokenProgram tokenProgram = parser.Tokenize(programText);

        Assert.AreEqual(3, tokenProgram.InstructionLines.Count);

        for (int index = 0; index < tokenProgram.InstructionLines.Count; index++)
        {
            TokenLine tokenLine = tokenProgram.InstructionLines[index];

            Assert.AreEqual(tokenProgram.Lines[tokenLine.LineIndex], tokenLine);
            Assert.AreEqual(index, tokenLine.OffsetLineIndex);
        }
    }

    [Test]
    public void Test_Error()
    {
        string programText = "test :incorrect ignored";

        Parser parser = new Parser();
        TokenProgram tokenProgram = parser.Tokenize(programText);

        Assert.That(parser.Errors.Errors.Count == 1);
        Assert.AreEqual(1, tokenProgram.Lines[0].TokenCount);
    }

    [Test]
    public void Test_TokenLine_FullLine()
    {
        string programText = ReadSampleFile("token_zoo.tst");
        string[] programLines = programText.Split(new[]
        {
            "\r\n", "\r", "\n",
        }, StringSplitOptions.None);

        Parser parser = new Parser();
        TokenProgram tokenProgram = parser.Tokenize(programText);

        for (int i = 0; i < programLines.Length; i++)
        {
            TokenLine line = tokenProgram.Lines[i];
            Assert.AreEqual(programLines[i], line.FullLine);
        }
    }

    [Test]
    public void Test_Token_Value()
    {
        const string programText = "mov $reg1 name .p0 .p0..p1 .p0:1 0xFF 0 0b0\r\n:label; comment £";

        Parser parser = new Parser();
        TokenProgram program = parser.Tokenize(programText);

        Assert.AreEqual("mov", program.Lines[0].Tokens[0].Value);
        Assert.AreEqual("$reg1", program.Lines[0].Tokens[1].Value);
        Assert.AreEqual("name", program.Lines[0].Tokens[2].Value);
        Assert.AreEqual(".p0", program.Lines[0].Tokens[3].Value);
        Assert.AreEqual(".p0..p1", program.Lines[0].Tokens[4].Value);
        Assert.AreEqual(".p0:1", program.Lines[0].Tokens[5].Value);
        Assert.AreEqual("0xFF", program.Lines[0].Tokens[6].Value);
        Assert.AreEqual("0", program.Lines[0].Tokens[7].Value);
        Assert.AreEqual("0b0", program.Lines[0].Tokens[8].Value);
        Assert.AreEqual(":label", program.Lines[1].Tokens[0].Value);
        Assert.AreEqual("; comment £", program.Lines[1].Tokens[1].Value);
    }

    [Test]
    public void Test_Is_Comment()
    {
        string programText = ReadSampleFile("token_zoo.tst");

        Parser parser = new Parser();
        TokenProgram program = parser.Tokenize(programText);

        Assert.IsTrue(program.Lines[4].Tokens[5].IsComment);
        Assert.IsTrue(program.Lines[6].Tokens[0].IsComment);
    }

    [Test]
    public void Test_Regex_Result()
    {
        string programText = ReadSampleFile("token_zoo.tst");

        Parser parser = new Parser();
        TokenProgram program = parser.Tokenize(programText);

        Assert.AreEqual("label", program.Lines[0].Tokens[0].RegexResult.Groups[1].Value);
        Assert.AreEqual("0", program.Lines[3].Tokens[2].RegexResult.Groups[1].Value);
        Assert.AreEqual("7", program.Lines[3].Tokens[2].RegexResult.Groups[2].Value);
    }
}