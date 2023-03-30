using System.IO;
using NUnit.Framework;
using Toaster.Definition;

namespace Toaster.Tests;

[SetUpFixture]
public class TestsSetup
{
    [OneTimeSetUp]
    public void LoadInstructions()
    {
        InstructionManager.LoadSignatures(File.ReadAllText("Definition\\instructions.json"));
    }
}