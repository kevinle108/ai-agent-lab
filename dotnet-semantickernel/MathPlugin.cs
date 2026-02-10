using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Data;

namespace DotNetSemanticKernel;

/// <summary>
/// A plugin that provides mathematical expression evaluation capabilities.
/// </summary>
public class MathPlugin
{
    /// <summary>
    /// Evaluates a mathematical expression and returns the result.
    /// </summary>
    /// <param name="expression">A mathematical expression (e.g., "2 + 3 * 4", "sqrt(16)")</param>
    /// <returns>The result of the expression as a string</returns>
    [KernelFunction("Calculate")]
    [Description("Evaluates a mathematical expression and returns the result")]
    public string Calculate(
        [Description("The mathematical expression to evaluate (e.g., '2 + 3 * 4', 'sqrt(16)' )")]
        string expression)
    {
        try
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(expression))
            {
                return "Error: Expression cannot be empty";
            }

            // Use DataTable.Compute to safely evaluate the mathematical expression
            var dt = new DataTable();
            var result = dt.Compute(expression, null);

            // Convert result to string and handle potential null values
            if (result == null)
            {
                return "Error: Expression resulted in null";
            }

            return result.ToString() ?? "Error: Unable to convert result to string";
        }
        catch (SyntaxErrorException ex)
        {
            return $"Error: Syntax error in expression - {ex.Message}";
        }
        catch (EvaluateException ex)
        {
            return $"Error: Evaluation error - {ex.Message}";
        }
        catch (InvalidOperationException ex)
        {
            return $"Error: Invalid operation - {ex.Message}";
        }
        catch (DivideByZeroException ex)
        {
            return $"Error: Division by zero - {ex.Message}";
        }
        catch (Exception ex)
        {
            return $"Error: {ex.GetType().Name} - {ex.Message}";
        }
    }
}
