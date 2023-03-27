using System;
using System.Collections.Generic;
using System.Linq;

namespace Toaster.Execution;

public class Interpreter
{
    public Interpreter(ExecutionConfig config)
    {
        ExecutionConfigValidator validator = new ExecutionConfigValidator(config);
        validator.Validate();

        if (validator.HasErrors)
            throw new ArgumentException($"validation of argument {config} shows errors", nameof(config));
    }
}