using System.IO;
using System.Net;
using NUnit.Framework;
using Toaster.Definition;

namespace Toaster.Tests.Definition;

[TestFixture]
public class InstructionManagerTests
{
    public void LoadInstructions()
    {
        InstructionManager.LoadSignatures(File.ReadAllText("Definition\\instructions.json"));
    }

    [Test]
    public void LoadAllSignaturesWithoutErrors()
    {
        LoadInstructions();
    }
}