using Discord.Interactions;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Zaoshi.Modules.Fun;

public class Calculate : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("calculate", "Write an expression and let the bot calculate it. Example: (cos(5*6)+abs(-8))/π+log(1000)%5")]
    public async Task CalculateCmd(string expression)
    {
        await RespondAsync(new PostfixCalculator().CalculatePostfix(new PostfixCalculator().InfixToPostfix(new StringBuilder(expression))).ToString(), ephemeral: true);
    }

    private class PostfixCalculator
    {
        private readonly Stack<double> _buffer = new Stack<double>();
        private string _exceptionMessage = "Invalid Syntax";
        private Dictionary<string, Func<double>> _operations = new Dictionary<string, Func<double>>();

        private void InitDict() => _operations = new Dictionary<string, Func<double>>{
            {"+", () => _buffer.Pop() + _buffer.Pop()},{
                "-", () =>
                {
                    double num2 = _buffer.Pop();
                    double num1 = _buffer.Pop();
                    return num1 - num2;
                }
            },
            {"*", () => _buffer.Pop() * _buffer.Pop()},{
                "/", () =>
                {
                    double num2 = _buffer.Pop();
                    double num1 = _buffer.Pop();
                    if (num2 == 0) ThrowException("Cannot divide by zero");
                    return num1 / num2;
                }
            },{
                "%", () =>
                {
                    double num2 = _buffer.Pop();
                    double num1 = _buffer.Pop();
                    return num1 % num2;
                }
            },
            {"sin", () => Math.Sin(_buffer.Pop())},
            {"cos", () => Math.Cos(_buffer.Pop())},
            {"tg", () => Math.Tan(_buffer.Pop())},{
                "cotg", () =>
                {
                    double num = _buffer.Pop();
                    if (num == 0) ThrowException("Cotg() cannot be zero");
                    return Math.Cos(num) / Math.Sin(num);
                }
            },
            {"pow", () => Math.Pow(_buffer.Pop(), 2)},{
                "sqrt", () =>
                {
                    double num = _buffer.Pop();
                    if (num < 0) ThrowException("Sqrt() cannot be < 0");
                    return Math.Sqrt(num);
                }
            },{
                "log", () =>
                {
                    double num = _buffer.Pop();
                    if (num <= 0) ThrowException("Log() cannot be ≤ 0");
                    return Math.Log10(num);
                }
            },{
                "ln", () =>
                {
                    double num = _buffer.Pop();
                    if (num <= 0) ThrowException("Ln() cannot be ≤ 0");
                    return Math.Log(num);
                }
            },
            {"abs", () => Math.Abs(_buffer.Pop())},
            {"π", () => Math.PI},
            {"e", () => Math.E}
        };

        public object CalculatePostfix(IEnumerable<dynamic> commands)
        {
            try
            {
                InitDict();
                foreach (dynamic item in commands)
                {
                    _buffer.Push(_operations.ContainsKey(Convert.ToString(item))
                        ? (double)_operations[Convert.ToString(item)]()
                        : (double)Convert.ToDouble(item));
                }

                if (_buffer.Count > 1) throw new Exception(); // more than one number in result
            }
            catch
            {
                return _exceptionMessage;
            }

            return _buffer.Pop();
        }

        private void ThrowException(string msg) //custom exceptions
        {
            _exceptionMessage = msg;
            throw new Exception();
        }

        public IEnumerable<dynamic> InfixToPostfix(StringBuilder infix)
        {
            InitDict();
            var postfix = new List<dynamic>();
            var stack = new Stack<string>();

            //replace constants
            infix = infix.Replace("π", Math.PI.ToString(CultureInfo.InvariantCulture));
            infix = infix.Replace("e", Math.E.ToString(CultureInfo.InvariantCulture));

            // fix for negative numbers -> adds 0 if '-' is after '(' or at the start of infix 
            if (infix[0] == '-')
                infix.Insert(0, '0');
            for (int i = 1; i < infix.Length; i++)
            {
                if (infix[i] == '-' && infix[i - 1] == '(')
                    infix.Insert(i, '0');
            }

            foreach (Match item in Regex.Matches(infix.ToString(), @"[+\%\-\*\/()]|\d*\.?\d+|[a-z]+"))
            {
                if (item.Value == "(") // start of stack region
                    stack.Push("(");
                else if (double.TryParse(item.Value,
                             NumberStyles.Any,
                             CultureInfo.InvariantCulture,
                             out double d)) // number
                    postfix.Add(d);
                else if (item.Value == ")") // pushes all of stack to postfix until '(' is met
                {
                    while (stack.Count > 0 && stack.Peek() != "(")
                        postfix.Add(stack.Pop());
                    if (stack.Count > 0) stack.Pop();
                }
                else // pushes operator/function to stack by precedence
                {
                    while (stack.Count > 0 && stack.Peek() != "(" &&
                           GetPrecedence(stack.Peek()) >= GetPrecedence(item.ToString()))
                        postfix.Add(stack.Pop());
                    stack.Push(item.Value);
                }
            }

            while (stack.Count > 0) // empty stack
                postfix.Add(stack.Pop());

            return postfix.ToArray();
        }

        private static int GetPrecedence(string op)
        {
            switch (op)
            {
                case "+":
                case "-":
                    return 1;
                case "*":
                case "/":
                case "%":
                    return 2;
                default:
                    return 3;
            }
        }
    }
}
