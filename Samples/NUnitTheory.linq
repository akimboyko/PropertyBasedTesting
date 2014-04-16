<Query Kind="Program">
  <Reference>D:\tools\nunit\NUnit-2.6.3\bin\nunit.framework.dll</Reference>
  <Reference>D:\tools\nunit\NUnit-2.6.3\bin\lib\nunit-console-runner.dll</Reference>
  <Reference>D:\tools\nunit\NUnit-2.6.3\bin\nunit-console-x86.exe</Reference>
  <Namespace>NUnit.Framework</Namespace>
</Query>

void Main()
{
    // nunit runner
	NUnit.ConsoleRunner.Runner.Main(new string[]
   	{
    	Assembly.GetExecutingAssembly().Location, 
   	});
}

[TestFixture]
public class SqrtTests
{
    [Datapoints]
    public double[] values = new double[] { 0.0, 1.0, -1.0, 42.0 };
    
    [Datapoints]
    public static IEnumerable<double> squares = Enumerable.Range(0, 100).Select(n => 1.0 * n * n);
    
    [Datapoints]
    public static IEnumerable<double> from0to100
    {
        get
        {
            for(int n = 0; n <= 100; n++)
            {
                yield return n;
            }
            
        }
    } 

    // * If the assumptions are violated for all test cases, 
    //   then the Theory itself is marked as a failure.
    // * If any Assertion fails, the Theory itself fails.
    // * If at least some cases pass the stated assumptions, 
    //   and there are no assertion failures or exceptions, then the Theory passes.
    // http://nunit.com/index.php?p=theory&r=2.6.2
    [Theory]
    public void SquareRootDefinition(double num)
    {
        Console.WriteLine("Try {0} case", num);
    
        // If the assumption is not satisfied for a particular test case, 
        // that case returns an Inconclusive result, rather than a Success or Failure.
        Assume.That(num >= 0.0);

        Console.WriteLine("Try {0} case", num);

        double sqrt = Math.Sqrt(num);

        Assert.That(sqrt >= 0.0);
        Assert.That(sqrt * sqrt, Is.EqualTo(num).Within(0.000001));
    }
}
